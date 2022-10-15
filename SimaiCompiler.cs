using System.Collections.Immutable;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Xml;

namespace MaiLib
{
    /// <summary>
    /// Compile various Ma2 Charts
    /// </summary>
    public class SimaiCompiler : Compiler
    {
        /// <summary>
        /// Construct compiler of a single song.
        /// </summary>
        /// <param name="location">Folder</param>
        /// <param name="targetLocation">Output folder</param>
        public SimaiCompiler(string location, string targetLocation)
        {
            for (int i = 0; i < 7; i++)
            {
                this.Charts.Add(new Simai());
            }
            this.MusicXml = new XmlInformation(location);
            this.Information = MusicXml.Information;
            //Construct Charts
            {
                if (!this.Information["Easy"].Equals(""))
                {
                    Charts[0] = new Ma2(location + this.Information.GetValueOrDefault("Advanced Chart Path"));
                }
                if (!this.Information["Basic"].Equals(""))
                {
                    //Console.WriteLine("Have basic: "+ location + this.Information.GetValueOrDefault("Basic Chart Path"));
                    Charts[1] = new Ma2(location + this.Information.GetValueOrDefault("Basic Chart Path"));
                }
                if (!this.Information["Advanced"].Equals(""))
                {
                    Charts[2] = new Ma2(location + this.Information.GetValueOrDefault("Advanced Chart Path"));
                }
                if (!this.Information["Expert"].Equals(""))
                {
                    Charts[3] = new Ma2(location + this.Information.GetValueOrDefault("Expert Chart Path"));
                }
                if (!this.Information["Master"].Equals(""))
                {
                    Charts[4] = new Ma2(location + this.Information.GetValueOrDefault("Master Chart Path"));
                }
                if (!this.Information["Remaster"].Equals(""))
                {
                    Charts[5] = new Ma2(location + this.Information.GetValueOrDefault("Remaster Chart Path"));
                }
                if (!this.Information["Utage"].Equals(""))
                {
                    Charts[6] = new Ma2(location + this.Information.GetValueOrDefault("Advanced Chart Path"));
                }
            }

            string result = this.Compose();
            //Console.WriteLine(result);
            StreamWriter sw = new StreamWriter(targetLocation + GlobalSep + "maidata.txt", false);
            {
                sw.WriteLine(result);
            }
            sw.Close();
        }

        /// <summary>
        /// Construct compiler of a single song.
        /// </summary>
        /// <param name="location">Folder</param>
        /// <param name="targetLocation">Output folder</param>
        /// <param name="forUtage">True if for utage</param>
        public SimaiCompiler(string location, string targetLocation, bool forUtage)
        {
            string[] ma2files = Directory.GetFiles(location, "*.ma2");
            Charts = new List<Chart>();
            this.MusicXml = new XmlInformation(location);
            this.Information = MusicXml.Information;
            foreach (string ma2file in ma2files)
            {
                Charts.Add(new Ma2(ma2file));
            }

            List<string> ma2List = new List<string>();
            ma2List.AddRange(ma2files);

            string result = this.Compose(true, ma2List);
            //Console.WriteLine(result);
            StreamWriter sw = new StreamWriter(targetLocation + GlobalSep + "maidata.txt", false);
            {
                sw.WriteLine(result);
            }
            sw.Close();
        }

        /// <summary>
        /// Empty constructor.
        /// </summary>
        public SimaiCompiler()
        {
            for (int i = 0; i < 7; i++)
            {
                this.Charts.Add(new Simai());
            }
            Charts = new List<Chart>();
            Information = new Dictionary<string, string>();
            this.MusicXml = new XmlInformation();
        }

        public override string Compose()
        {
            string result = "";
            //Add Information
            {
                string beginning = "";
                beginning += "&title=" + this.Information.GetValueOrDefault("Name") + this.Information.GetValueOrDefault("SDDX Suffix") + "\n";
                beginning += "&wholebpm=" + this.Information.GetValueOrDefault("BPM") + "\n";
                beginning += "&artist=" + this.Information.GetValueOrDefault("Composer") + "\n";
                beginning += "&des=" + this.Information.GetValueOrDefault("Master Chart Maker") + "\n";
                beginning += "&shortid=" + this.Information.GetValueOrDefault("Music ID") + "\n";
                beginning += "&genre=" + this.Information.GetValueOrDefault("Genre") + "\n";
                beginning += "&cabinet=";
                if (this.MusicXml.IsDXChart)
                {
                    beginning += "DX\n";
                }
                else
                {
                    beginning += "SD\n";
                }
                beginning += "&version=" + this.MusicXml.TrackVersion + "\n";
                beginning += "&chartconverter=Neskol\n";
                beginning += "&chartconverttool=MaichartConverter\n";
                beginning += "&chartconverttoolversion=1.0.3.0\n";
                beginning += "&smsg=See https://github.com/Neskol/MaichartConverter for updates\n";
                beginning += "\n";

                if (this.Information.TryGetValue("Easy", out string? easy) && this.Information.TryGetValue("Easy Chart Maker", out string? easyMaker))
                {
                    beginning += "&lv_1=" + easy + "\n";
                    beginning += "&des_1=" + easyMaker + "\n";
                    beginning += "\n";
                }

                if (this.Information.TryGetValue("Basic", out string? basic) && this.Information.TryGetValue("Basic Chart Maker", out string? basicMaker))
                {
                    beginning += "&lv_2=" + basic + "\n";
                    beginning += "&des_2=" + basicMaker + "\n";
                    beginning += "\n";
                }


                if (this.Information.TryGetValue("Advanced", out string? advance) && this.Information.TryGetValue("Advanced Chart Maker", out string? advanceMaker))
                {
                    beginning += "&lv_3=" + advance + "\n";
                    beginning += "&des_3=" + advanceMaker + "\n";
                    beginning += "\n";
                }


                if (this.Information.TryGetValue("Expert", out string? expert) && this.Information.TryGetValue("Expert Chart Maker", out string? expertMaker))
                {
                    beginning += "&lv_4=" + expert + "\n";
                    beginning += "&des_4=" + expertMaker + "\n";
                    beginning += "\n";
                }


                if (this.Information.TryGetValue("Master", out string? master) && this.Information.TryGetValue("Master Chart Maker", out string? masterMaker))
                {
                    beginning += "&lv_5=" + master + "\n";
                    beginning += "&des_5=" + masterMaker + "\n";
                    beginning += "\n";
                }


                if (this.Information.TryGetValue("Remaster", out string? remaster) && this.Information.TryGetValue("Remaster Chart Maker", out string? remasterMaker))
                {
                    beginning += "&lv_6=" + remaster + "\n";
                    beginning += "&des_6=" + remasterMaker; beginning += "\n";
                    beginning += "\n";
                }
                result += beginning;
            }
            Console.WriteLine("Finished writing header of " + this.Information.GetValueOrDefault("Name"));

            //Compose Charts
            {
                for (int i = 0; i < this.Charts.Count; i++)
                {
                    // Console.WriteLine("Processing chart: " + i);
                    if (!this.Information[this.Difficulty[i]].Equals(""))
                    {
                        string? isDxChart = this.Information.GetValueOrDefault("SDDX Suffix");
                        if (!Charts[i].IsDXChart)
                        {
                            isDxChart = "";
                        }
                        result += "&inote_" + (i + 1) + "=\n";
                        result += this.Compose(Charts[i]);
                        this.CompiledChart.Add(this.Information.GetValueOrDefault("Name") + isDxChart + " [" + this.Difficulty[i] + "]");
                    }
                    result += "\n";
                }
            }
            Console.WriteLine("Finished composing.");
            return result;
        }

        /// <summary>
        /// Return compose of specified chart.
        /// </summary>
        /// <param name="chart">Chart to compose</param>
        /// <returns>Maidata of specified chart WITHOUT headers</returns>
        public override string Compose(Chart chart)
        {
            return new Simai(chart).Compose();
        }

        /// <summary>
        /// Compose utage Charts
        /// </summary>
        /// <param name="isUtage">switch to produce utage</param>
        /// <returns>Corresponding utage chart</returns>
        public override string Compose(bool isUtage, List<string> ma2files)
        {
            string result = "";
            //Add Information

            string beginning = "";
            beginning += "&title=" + this.Information.GetValueOrDefault("Name") + "[宴]" + "\n";
            beginning += "&wholebpm=" + this.Information.GetValueOrDefault("BPM") + "\n";
            beginning += "&artist=" + this.Information.GetValueOrDefault("Composer") + "\n";
            beginning += "&des=" + this.Information.GetValueOrDefault("Master Chart Maker") + "\n";
            beginning += "&shortid=" + this.Information.GetValueOrDefault("Music ID") + "\n";
            beginning += "&genre=" + this.Information.GetValueOrDefault("Genre") + "\n";
            beginning += "&cabinet=SD";
            beginning += "&version=" + this.MusicXml.TrackVersion + "\n";
            beginning += "&chartconverter=Neskol\n";
            beginning += "\n";

            int defaultChartIndex = 7;
            if (ma2files.Count > 1)
            {
                defaultChartIndex = 2;
                foreach (string ma2file in ma2files)
                {
                    beginning += "&lv_" + defaultChartIndex + "=" + "宴" + "\n";
                    beginning += "\n";
                    defaultChartIndex++;
                }
            }
            else
            {
                beginning += "&lv_" + defaultChartIndex + "=" + "宴" + "\n";
                beginning += "\n";
            }



            result += beginning;
            Console.WriteLine("Finished writing header of " + this.Information.GetValueOrDefault("Name"));

            //Compose Charts

            if (defaultChartIndex < 7)
            {
                for (int i = 0; i < this.Charts.Count; i++)
                {
                    // Console.WriteLine("Processing chart: " + i);
                    if (!this.Information[this.Difficulty[i]].Equals(""))
                    {
                        string? isDxChart = "Utage";
                        result += "&inote_" + (i + 2) + "=\n";
                        result += this.Compose(Charts[i]);
                        this.CompiledChart.Add(this.Information.GetValueOrDefault("Name") + isDxChart + " [宴]");
                    }
                    result += "\n";
                }
            }
            else
            {
                result += "&inote_7=\n";
                result += this.Compose(Charts[0]);
                this.CompiledChart.Add(this.Information.GetValueOrDefault("Name") + "Utage" + " [宴]");
            }

            Console.WriteLine("Finished composing.");
            return result;
        }
    }
}