using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MaiLib
{
    public abstract class Compiler : ICompiler
    {/// <summary>
     /// Stores difficulty keywords
     /// </summary>
     /// <value>Difficulty</value>
        public static readonly string[] Difficulty = { "Easy", "Basic", "Advanced", "Expert", "Master", "Remaster", "Utage" };

        /// <summary>
        /// Stores chart collections
        /// </summary>
        public List<Chart> Charts { get; set; }

        /// <summary>
        /// Stores global information
        /// </summary>
        public Dictionary<string, string> Information { get; set; }

        /// <summary>
        /// Stores read in music XML file
        /// </summary>
        public XmlInformation MusicXML { get; set; }

        /// <summary>
        /// Stores the path separator
        /// </summary>
        public string GlobalSep { get; set; }

        /// <summary>
        /// Stores the rotate dictionary
        /// </summary>
        public Dictionary<string, string> RotateDictionary = new Dictionary<string, string> { { "17", "UpSideDown" }, { "305", "LeftToRight" }, { "417", "Clockwise90" } };

        /// <summary>
        /// Stores the information of Compiled Chart
        /// </summary>
        public List<string> CompiledChart { get; set; }

        /// <summary>
        /// Construct compiler of a single song.
        /// </summary>
        /// <param name="location">Folder</param>
        /// <param name="targetLocation">Output folder</param>
        public Compiler(string location, string targetLocation)
        {
            this.CompiledChart = new();
            Charts = new List<Chart>();
            this.MusicXML = new XmlInformation(location);
            MusicXML.Update();
            this.Information = MusicXML.Information;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GlobalSep = "\\";
            }
            else
            {
                GlobalSep = "/";
            }
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Compiler()
        {
            this.CompiledChart = new();
            Charts = new List<Chart>();
            Information = new Dictionary<string, string>();
            this.MusicXML = new XmlInformation();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GlobalSep = "\\";
            }
            else
            {
                GlobalSep = "/";
            }
        }

        public bool CheckValidity()
        {
            bool result = true;
            foreach (Chart x in Charts)
            {
                result = result && x.CheckValidity();
            }
            return result;
        }

        public abstract string Compose();

        /// <summary>
        /// Return compose of specified chart.
        /// </summary>
        /// <param name="chart">Chart to compose</param>
        /// <returns>Maidata of specified chart WITHOUT headers</returns>
        public abstract string Compose(Chart chart);

        /// <summary>
        /// Compose utage charts
        /// </summary>
        /// <param name="isUtage">switch to produce utage</param>
        /// <returns>Corresponding utage chart</returns>
        public abstract string Compose(bool isUtage, List<string> ma2files);

        public void TakeInformation(Dictionary<string, string> information)
        {
            this.Information = information;
        }

        /// <summary>
        /// Return the chart bpm change table of MaiCompiler
        /// </summary>
        /// <returns>First BPM change table of this.charts</returns>
        public BPMChanges SymbolicBPMTable()
        {
            BPMChanges bpmTable = new BPMChanges();
            bool foundTable = false;
            for (int i = 0; i < this.Charts.Count && !foundTable; i++)
            {
                if (this.Charts[i] != null)
                {
                    bpmTable = this.Charts[i].BPMChanges;
                    foundTable = true;
                }
            }
            return bpmTable;
        }

        /// <summary>
        /// Return the first note of master chart
        /// </summary>
        /// <returns>The first note of the master chart, or first note of the Utage chart if isUtage is turned true</returns>
        /// <exception cref="System.NullReferenceException">Throws null reference exception if the chart does not exist</exception>
        public Note SymbolicFirstNote(bool isUtage)
        {
            if (!isUtage)
            {
                return this.Charts[4].FirstNote ?? throw new NullReferenceException("Null first note: master chart is invalid");
            }
            else if (isUtage)
            {
                Note? firstNote;
                bool foundFirstNote = false;
                for (int i = this.Charts.Count; i >= 0 && !foundFirstNote; i++)
                {
                    if (this.Charts[i] != null)
                    {
                        firstNote = this.Charts[i].FirstNote;
                        foundFirstNote = true;
                    }
                }
                return this.Charts[0].FirstNote ?? throw new NullReferenceException("Null first note: utage chart is invalid");
            }
            else throw new NullReferenceException("This compiler contains invalid Master Chart and is not Utage Chart: no first note is returned");
        }

        /// <summary>
        /// Generate one line summary of this track with ID, name, genre and difficulty
        /// </summary>
        /// <returns></returns>
        public string GenerateOneLineSummary()
        {
            string result = "";
            if (this.Charts.Equals(null))
            {
                throw new NullReferenceException("This compiler has empty chat list!");
            }
            result += "(" + this.Information["Music ID"] + ")" + this.Information["Name"] + ", " + this.Information["Genre"] + ", ";
            if (!this.Information["Easy"].Equals(""))
            {
                result += this.Information["Easy"] + "/";
            }
            if (!this.Information["Basic"].Equals(""))
            {
                result += this.Information["Basic"];
            }
            else result += "-";
            if (!this.Information["Advanced"].Equals(""))
            {
                result += "/" + this.Information["Advanced"];
            }
            else result += "-";
            if (!this.Information["Expert"].Equals(""))
            {
                result += "/" + this.Information["Expert"];
            }
            else result += "-";
            if (!this.Information["Master"].Equals(""))
            {
                result += "/" + this.Information["Master"];
            }
            else result += "-";
            if (!this.Information["Remaster"].Equals(""))
            {
                result += "/" + this.Information["Remaster"];
            }
            else result += "-";
            if (!this.Information["Utage"].Equals(""))
            {
                result += "\\" + this.Information["Utage"];
            }

            return result;
        }
    }
}
