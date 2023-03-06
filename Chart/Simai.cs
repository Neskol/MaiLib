using System.Net.Http.Headers;

namespace MaiLib
{
    public class Simai : Chart
    {
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Simai()
        {
            this.Notes = new List<Note>();
            this.BPMChanges = new BPMChanges();
            this.MeasureChanges = new MeasureChanges();
            this.StoredChart = new List<List<Note>>();
            this.Information = new Dictionary<string, string>();
        }

        /// <summary>
        /// Constructs Simai chart directly from path specified
        /// </summary>
        /// <param name="location"></param>
        public Simai(string location)
        {
            string[] tokens = new SimaiTokenizer().Tokens(location);
            Chart chart = new SimaiParser().ChartOfToken(tokens);
            this.Notes = new List<Note>(chart.Notes);
            this.BPMChanges = new BPMChanges(chart.BPMChanges);
            this.MeasureChanges = new MeasureChanges(chart.MeasureChanges);
            this.StoredChart = new List<List<Note>>();
            this.Information = new Dictionary<string, string>(chart.Information);
            this.Update();
        }

        /// <summary>
        /// Construct Simai from given parameters
        /// </summary>
        /// <param name="notes">Notes to take in</param>
        /// <param name="bpmChanges">BPM change to take in</param>
        /// <param name="measureChanges">Measure change to take in</param>
        public Simai(List<Note> notes, BPMChanges bpmChanges, MeasureChanges measureChanges)
        {
            this.Notes = notes;
            this.BPMChanges = bpmChanges;
            this.MeasureChanges = measureChanges;
            this.StoredChart = new List<List<Note>>();
            this.Information = new Dictionary<string, string>();
            this.Update();
        }

        public Simai(Chart takenIn)
        {
            this.Notes = takenIn.Notes;
            this.BPMChanges = takenIn.BPMChanges;
            this.MeasureChanges = takenIn.MeasureChanges;
            this.StoredChart = new List<List<Note>>();
            this.Information = new Dictionary<string, string>();
            this.Update();
        }

        public override string Compose()
        {
            string result = "";
            int delayBar = (this.TotalDelay) / 384 + 2;
            //Console.WriteLine(chart.Compose());
            //foreach (BPMChange x in chart.BPMChanges.ChangeNotes)
            //{
            //    Console.WriteLine("BPM Change verified in " + x.Bar + " " + x.Tick + " of BPM" + x.BPM);
            //}
            List<Note> firstBpm = new List<Note>();
            foreach (Note bpm in this.Notes)
            {
                if (bpm.NoteSpecificGenre.Equals("BPM"))
                {
                    firstBpm.Add(bpm);
                }
            }
            // if (firstBpm.Count > 1)
            // {
            //     chart.Chart[0][0] = firstBpm[1];
            // }
            foreach (List<Note> bar in this.StoredChart)
            {
                Note lastNote = new MeasureChange();
                int currentQuaver = 0;
                int commaCompiled = 0;
                //result += bar[1].Bar;
                foreach (Note x in bar)
                {
                    switch (lastNote.NoteSpecificGenre)
                    {
                        case "MEASURE":
                            currentQuaver = (lastNote as MeasureChange ?? throw new Exception("This note is not measure change")).Quaver;
                            break;
                        case "BPM":
                            break;
                        case "TAP":
                            if (x.IsNote && ((!x.NoteSpecificGenre.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM")))
                            {
                                result += "/";
                            }
                            else
                            {
                                result += ",";
                                commaCompiled++;
                            }
                            break;
                        case "HOLD":
                            if (x.IsNote && (!x.NoteSpecificGenre.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM"))
                            {
                                result += "/";
                            }
                            else
                            {
                                result += ",";
                                commaCompiled++;
                            }
                            break;
                        case "SLIDE_START":
                            // if (lastNote.ConsecutiveSlide == null)
                            // {
                            //     result += "$";
                            // }
                            // if (x.IsNote && (!x.NoteGenre.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM"))
                            // {
                            //     result += "/";
                            // }
                            // else if (x.NoteGenre != "SLIDE"||lastNote.Bar!=x.Bar || lastNote.Tick!=x.Tick)
                            // {
                            //     result += ",";
                            // }
                            if (x.IsNote && ((!x.NoteSpecificGenre.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM")))
                            {
                                result += "/";
                            }
                            else if (x.IsNote && !x.NoteSpecificGenre.Equals("SLIDE") && !x.NoteGenre.Equals("BPM"))
                            {
                                result += ",";
                                commaCompiled++;
                            }
                            else if (x.NoteGenre.Equals("REST"))
                            {
                                result += ",";
                                commaCompiled++;
                            }
                            break;
                        case "SLIDE":
                            if (x.IsNote && (!x.NoteSpecificGenre.Equals("SLIDE")) && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM"))
                            {
                                result += "/";
                            }
                            else if (x.IsNote && x.NoteSpecificGenre.Equals("SLIDE") && x.Tick == lastNote.Tick && !x.NoteGenre.Equals("BPM"))
                            {
                                result += "*";
                            }
                            // else if (x.IsNote && !lastNote.NoteSpecificType.Equals("SLIDE_START")&& x.Bar!=lastNote.Bar && x.Tick!=lastNote.Tick&& !x.NoteGenre.Equals("BPM"))
                            // {
                            //     result += ",";
                            // }
                            else
                            {
                                result += ",";
                                commaCompiled++;
                            }
                            break;
                        default:
                            result += ",";
                            commaCompiled++;
                            break;
                    }
                    // if (x.NoteGenre.Equals("SLIDE"))
                    // {
                    //     if (x.SlideStart==null)
                    //     {
                    //         x.SlideStart = new Tap("NST",x.Bar,x.Tick,x.Key);
                    //     }
                    // }
                    // if (x.SlideStart!=null&&x.SlideStart.NoteType.Equals("NST")&&(!lastNote.NoteGenre.Equals("SLIDE")||lastNote.NoteGenre.Equals("SLIDE")&&lastNote.TickStamp!=x.TickStamp&&!lastNote.Key.Equals(x)))
                    // {
                    //     result += x.SlideStart.Compose(0);
                    // }
                    result += x.Compose(0);
                    lastNote = x;
                    //if (x.NoteGenre().Equals("BPM"))
                    //{
                    //    result+="("+ x.Bar + "_" + x.Tick + ")";
                    //}
                }
                result += ",\n";
                commaCompiled++;
                if (commaCompiled != currentQuaver)
                {
                    Console.WriteLine("Notes in bar: " + bar[0].Bar);
                    foreach (Note x in bar)
                    {
                        Console.WriteLine(x.Compose(1));
                    }
                    Console.WriteLine(result);
                    Console.WriteLine("Expected comma number: " + currentQuaver);
                    Console.WriteLine("Actual comma number: " + commaCompiled);
                    throw new NullReferenceException("COMMA COMPILED MISMATCH IN BAR " + bar[0].Bar);
                }
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

        public override bool CheckValidity()
        {
            bool result = this != null;
            // Not yet implemented
            return result;
        }

        /// <summary>
        /// Reconstruct the chart with given arrays
        /// </summary>
        /// <param name="bpm">New BPM Changes</param>
        /// <param name="measure">New Measure Changes</param>
        /// <returns>New Composed Chart</returns>
        public override string Compose(BPMChanges bpm, MeasureChanges measure)
        {
            BPMChanges sourceBPM = this.BPMChanges;
            MeasureChanges sourceMeasures = this.MeasureChanges;
            this.BPMChanges = bpm;
            this.MeasureChanges = measure;
            this.Update();

            string result = this.Compose();
            this.BPMChanges = sourceBPM;
            this.MeasureChanges = sourceMeasures;
            this.Update();
            return result;
        }
    }
}