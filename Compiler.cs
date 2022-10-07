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
        public static readonly string[] difficulty = {"Easy", "Basic", "Advanced", "Expert", "Master", "Remaster", "Utage" };

        /// <summary>
        /// Stores chart collections
        /// </summary>
        private List<Chart> charts;

        /// <summary>
        /// Stores global information
        /// </summary>
        private Dictionary<string, string> information;

        /// <summary>
        /// Stores read in music XML file
        /// </summary>
        private XmlInformation musicXml;

        /// <summary>
        /// Stores the path separator
        /// </summary>
        private string globalSep;

        /// <summary>
        /// Access the path separator
        /// </summary>
        public string GlobalSep
        {
            get { return this.globalSep; }
        }

        /// <summary>
        /// Stores the information of Compiled Chart
        /// </summary>
        private List<string> compiledChart;

        /// <summary>
        /// Stores the information of Compiled Chart
        /// </summary>
        public List<string> CompiledChart
        {
            get { return this.compiledChart; }
            set { this.compiledChart = value; }
        }

        /// <summary>
        /// Access stored charts
        /// </summary>
        public List<Chart> Charts
        {
            get { return this.charts; }
            set { this.charts = value; }
        }

        /// <summary>
        /// Access global information
        /// </summary>
        public Dictionary<string, string> Information
        {
            get { return this.information; }
            set { this.information = value; }
        }

        /// <summary>
        /// Access read in music XML file
        /// </summary>
        public XmlInformation MusicXml
        {
            get { return this.musicXml; }
            set { this.musicXml = value; }
        }
        
        /// <summary>
        /// Access difficulty;
        /// </summary>
        public string[] Difficulty
        {
            get { return difficulty; }
        }



        /// <summary>
        /// Construct compiler of a single song.
        /// </summary>
        /// <param name="location">Folder</param>
        /// <param name="targetLocation">Output folder</param>
        public Compiler(string location, string targetLocation)
        {
            compiledChart = new();
            charts = new List<Chart>();
            this.musicXml = new XmlInformation(location);
            musicXml.Update();
            this.information = musicXml.Information;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                globalSep = "\\";
            }
            else
            {
                globalSep = "/";
            }
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public Compiler()
        {
            compiledChart = new();
            charts = new List<Chart>();
            information = new Dictionary<string, string>();
            this.musicXml = new XmlInformation();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                globalSep = "\\";
            }
            else
            {
                globalSep = "/";
            }
        }

        public bool CheckValidity()
        {
            bool result = true;
            foreach (Chart x in charts)
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
            this.information = information;
        }

        /// <summary>
        /// Return the chart bpm change table of MaiCompiler
        /// </summary>
        /// <returns>First BPM change table of this.charts</returns>
        public BPMChanges SymbolicBPMTable()
        {
            BPMChanges bpmTable = new BPMChanges();
            bool foundTable = false;
            for (int i = 0; i < this.charts.Count && !foundTable; i++)
            {
                if (this.charts[i] != null)
                {
                    bpmTable = this.charts[i].BPMChanges;
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
                return this.charts[4].FirstNote ?? throw new NullReferenceException("Null first note: master chart is invalid");
            }
            else if (isUtage)
            {
                return this.charts[0].FirstNote ?? throw new NullReferenceException("Null first note: utage chart is invalid");
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
            if (this.charts.Equals(null) || !this.CheckValidity())
            {
                throw new NullReferenceException("This compiler has empty chat list!");
            }
            result += "(" + this.information["Music ID"] + ")" + this.information["Name"] + ", " + this.information["Genre"] + ", ";

            return result;
        }
    }
}
