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
            this.MusicXml = new XmlInformation(location);
            this.Information = MusicXml.Information;
            //Construct Charts
            {
                if (!this.Information["Basic"].Equals(""))
                {
                    //Console.WriteLine("Have basic: "+ location + this.Information.GetValueOrDefault("Basic Chart Path"));
                    Charts[0] = new Ma2(location + this.Information.GetValueOrDefault("Basic Chart Path"));
                }
                if (!this.Information["Advanced"].Equals(""))
                {
                    Charts[1] = new Ma2(location + this.Information.GetValueOrDefault("Advanced Chart Path"));
                }
                if (!this.Information["Expert"].Equals(""))
                {
                    Charts[2] = new Ma2(location + this.Information.GetValueOrDefault("Expert Chart Path"));
                }
                if (!this.Information["Master"].Equals(""))
                {
                    Charts[3] = new Ma2(location + this.Information.GetValueOrDefault("Master Chart Path"));
                }
                if (!this.Information["Remaster"].Equals(""))
                {
                    Charts[4] = new Ma2(location + this.Information.GetValueOrDefault("Remaster Chart Path"));
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
                beginning += "\n";


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
                    if (!this.Information[difficulty[i]].Equals(""))
                    {
                        string? isDxChart = this.Information.GetValueOrDefault("SDDX Suffix");
                        if (!Charts[i].IsDXChart)
                        {
                            isDxChart = "";
                        }
                        result += "&inote_" + (i + 2) + "=\n";
                        result += this.Compose(Charts[i]);
                        this.CompiledChart.Add(this.Information.GetValueOrDefault("Name") + isDxChart + " [" + difficulty[i] + "]");
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
            string result = "";
            int delayBar = (chart.TotalDelay) / 384 + 2;
            //Console.WriteLine(chart.Compose());
            //foreach (BPMChange x in chart.BPMChanges.ChangeNotes)
            //{
            //    Console.WriteLine("BPM Change verified in " + x.Bar + " " + x.Tick + " of BPM" + x.BPM);
            //}
            List<Note> firstBpm = new List<Note>();
            foreach (Note bpm in chart.Notes)
            {
                if (bpm.NoteSpecificType.Equals("BPM"))
                {
                    firstBpm.Add(bpm);
                }
            }
            // if (firstBpm.Count > 1)
            // {
            //     chart.Chart[0][0] = firstBpm[1];
            // }
            foreach (List<Note> bar in chart.StoredChart)
            {
                Note lastNote = new MeasureChange();
                //result += bar[1].Bar;
                foreach (Note x in bar)
                {
                    switch (lastNote.NoteSpecificType)
                    {
                        case "MEASURE":
                            break;
                        case "BPM":
                            break;
                        case "TAP":
                            if (x.IsNote && ((!x.NoteSpecificType.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM")))
                            {
                                result += "/";
                            }
                            else result += ",";
                            break;
                        case "HOLD":
                            if (x.IsNote && (!x.NoteSpecificType.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM"))
                            {
                                result += "/";
                            }
                            else result += ",";
                            break;
                        case "SLIDE_START":
                            //if (x.IsNote() && x.NoteSpecificType().Equals("SLIDE"))
                            //{

                            //}
                            break;
                        case "SLIDE":
                            if (x.IsNote && (!x.NoteSpecificType.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM"))
                            {
                                result += "/";
                            }
                            else if (x.IsNote && x.NoteSpecificType.Equals("SLIDE") && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM"))
                            {
                                result += "*";
                            }
                            else result += ",";
                            break;
                        default:
                            result += ",";
                            break;
                    }
                    //if (x.Prev!=null&&x.Prev.NoteType.Equals("NST"))
                    //if (x.NoteGenre.Equals("SLIDE")&&x.SlideStart== null)
                    //{
                    //    result += Int32.Parse(x.Key) + 1;
                    //    result += "!";
                    //}
                    result += x.Compose(0);
                    lastNote = x;
                    //if (x.NoteGenre().Equals("BPM"))
                    //{
                    //    result+="("+ x.Bar + "_" + x.Tick + ")";
                    //}
                }
                result += ",\n";
            }
            //if (delayBar>0)
            //{
            //    Console.WriteLine("TOTAL DELAYED BAR: "+delayBar);
            //}
            for (int i = 0; i < delayBar + 1; i++)
            {
                result += "{1},\n";
            }
            result += "E\n";
            return result;
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
            beginning += "&cabinate=SD";
            beginning += "&version=" + this.MusicXml.TrackVersion + "\n";
            beginning += "&chartconverter=Neskol\n";
            beginning += "\n";

            int defaultChartIndex = 7;
            if (ma2files.Count > 1)
            {
                defaultChartIndex = 0;
            }

            foreach (string ma2file in ma2files)
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
                    if (!this.Information[difficulty[i]].Equals(""))
                    {
                        string? isDxChart = "Utage";
                        result += "&inote_" + (i + 2) + "=\n";
                        result += this.Compose(Charts[i]);
                        this.CompiledChart.Add(this.Information.GetValueOrDefault("Name") + isDxChart + " [" + difficulty[i] + "]");
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