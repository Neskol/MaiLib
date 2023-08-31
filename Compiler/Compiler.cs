using System.Runtime.InteropServices;

namespace MaiLib;

public abstract class Compiler : ICompiler
{
    /// <summary>
    ///     Stores difficulty keywords
    /// </summary>
    /// <value>Difficulty</value>
    public static readonly string[] Difficulty =
        { "Easy", "Basic", "Advanced", "Expert", "Master", "Remaster", "Utage" };

    /// <summary>
    ///     Stores the rotate dictionary
    /// </summary>
    public Dictionary<string, string> RotateDictionary = new()
        { { "17", "UpSideDown" }, { "305", "LeftToRight" }, { "417", "Clockwise90" } };

#region Constructors
    /// <summary>
    ///     Construct compiler of a single song.
    /// </summary>
    /// <param name="location">Folder</param>
    /// <param name="targetLocation">Output folder</param>
    public Compiler(string location, string targetLocation)
    {
        CompiledChart = new List<string>();
        Charts = new List<Chart>();
        MusicXML = new XmlInformation(location);
        MusicXML.Update();
        Information = MusicXML.Information;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            GlobalSep = "\\";
        else
            GlobalSep = "/";
    }

    /// <summary>
    ///     Empty constructor.
    /// </summary>
    public Compiler()
    {
        CompiledChart = new List<string>();
        Charts = new List<Chart>();
        Information = new Dictionary<string, string>();
        MusicXML = new XmlInformation();
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            GlobalSep = "\\";
        else
            GlobalSep = "/";
    }
    #endregion

    /// <summary>
    ///     Stores chart collections
    /// </summary>
    public List<Chart> Charts { get; set; }

    /// <summary>
    ///     Stores global information
    /// </summary>
    public Dictionary<string, string> Information { get; set; }

    /// <summary>
    ///     Stores read in music XML file
    /// </summary>
    public XmlInformation MusicXML { get; set; }

    /// <summary>
    ///     Stores the path separator
    /// </summary>
    public string GlobalSep { get; set; }

    /// <summary>
    ///     Stores the information of Compiled Chart
    /// </summary>
    public List<string> CompiledChart { get; set; }

    public bool CheckValidity()
    {
        var result = true;
        foreach (var x in Charts) result = result && x.CheckValidity();
        return result;
    }

    public abstract string Compose();

    public void TakeInformation(Dictionary<string, string> information)
    {
        Information = information;
    }

    /// <summary>
    ///     Return compose of specified chart.
    /// </summary>
    /// <param name="chart">Chart to compose</param>
    /// <returns>Maidata of specified chart WITHOUT headers</returns>
    public abstract string Compose(Chart chart);

    /// <summary>
    ///     Compose utage charts
    /// </summary>
    /// <param name="isUtage">switch to produce utage</param>
    /// <returns>Corresponding utage chart</returns>
    public abstract string Compose(bool isUtage, List<string> ma2files);

    /// <summary>
    ///     Return the chart bpm change table of MaiCompiler
    /// </summary>
    /// <returns>First BPM change table of this.charts</returns>
    public BPMChanges SymbolicBPMTable()
    {
        var bpmTable = new BPMChanges();
        var foundTable = false;
        for (var i = 0; i < Charts.Count && !foundTable; i++)
            if (Charts[i] != null)
            {
                bpmTable = Charts[i].BPMChanges;
                foundTable = true;
            }

        return bpmTable;
    }

    /// <summary>
    ///     Return the first note of master chart
    /// </summary>
    /// <returns>The first note of the master chart, or first note of the Utage chart if isUtage is turned true</returns>
    /// <exception cref="System.NullReferenceException">Throws null reference exception if the chart does not exist</exception>
    public Note SymbolicFirstNote(bool isUtage)
    {
        if (!isUtage)
            return Charts[4].FirstNote ?? throw new NullReferenceException("Null first note: master chart is invalid");

        if (isUtage)
        {
            Note? firstNote;
            var foundFirstNote = false;
            for (var i = Charts.Count; i >= 0 && !foundFirstNote; i++)
                if (Charts[i] != null)
                {
                    firstNote = Charts[i].FirstNote;
                    foundFirstNote = true;
                }

            return Charts[0].FirstNote ?? throw new NullReferenceException("Null first note: utage chart is invalid");
        }

        throw new NullReferenceException(
            "This compiler contains invalid Master Chart and is not Utage Chart: no first note is returned");
    }

    /// <summary>
    ///     Generate one line summary of this track with ID, name, genre and difficulty
    /// </summary>
    /// <returns></returns>
    public string GenerateOneLineSummary()
    {
        var result = "";
        if (Charts.Equals(null)) throw new NullReferenceException("This compiler has empty chat list!");
        result += "(" + Information["Music ID"] + ")" + Information["Name"] + ", " + Information["Genre"] + ", ";
        if (!Information["Easy"].Equals("")) result += Information["Easy"] + "/";
        if (!Information["Basic"].Equals(""))
            result += Information["Basic"];
        else result += "-";
        if (!Information["Advanced"].Equals(""))
            result += "/" + Information["Advanced"];
        else result += "-";
        if (!Information["Expert"].Equals(""))
            result += "/" + Information["Expert"];
        else result += "-";
        if (!Information["Master"].Equals(""))
            result += "/" + Information["Master"];
        else result += "-";
        if (!Information["Remaster"].Equals(""))
            result += "/" + Information["Remaster"];
        else result += "-";
        if (!Information["Utage"].Equals("")) result += "\\" + Information["Utage"];

        return result;
    }
}