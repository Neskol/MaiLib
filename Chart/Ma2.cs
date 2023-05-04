using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace MaiLib
{
    /// <summary>
    /// Implementation of chart in ma2 format.
    /// </summary>
    public class Ma2 : Chart, ICompiler
    {
        /// <summary>
        /// Default Constructor.
        /// </summary>
        public Ma2() : base()
        {
        }

        public Ma2(List<Note> notes):base()
        {
            this.Notes = notes;
            this.Update();
        }

        /// <summary>
        /// Construct Ma2 with given notes, bpm change definitions and measure change definitions.
        /// </summary>
        /// <param name="notes">Notes in Ma2</param>
        /// <param name="bpmChanges">BPM Changes: Initial BPM is NEEDED!</param>
        /// <param name="measureChanges">Measure Changes: Initial Measure is NEEDED!</param>
        // public Ma2(List<Note> notes, BPMChanges bpmChanges, MeasureChanges measureChanges)
        // {
        //     this.Notes = new List<Note>(notes);
        //     this.BPMChanges = new BPMChanges(bpmChanges);
        //     this.MeasureChanges = new MeasureChanges(measureChanges);
        //     this.StoredChart = new List<List<Note>>();
        //     this.Information = new Dictionary<string, string>();
        //     this.Update();
        // }

        /// <summary>
        /// Construct GoodBrother from location specified
        /// </summary>
        /// <param name="location">MA2 location</param>
        public Ma2(string location)
        {
            string[] tokens = new Ma2Tokenizer().Tokens(location);
            Chart takenIn = new Ma2Parser().ChartOfToken(tokens);
            this.Notes = new List<Note>(takenIn.Notes);
            // this.BPMChanges = new BPMChanges(takenIn.BPMChanges);
            // this.MeasureChanges = new MeasureChanges(takenIn.MeasureChanges);
            this.StoredChart = new List<List<Note>>(takenIn.StoredChart);
            this.Information = new Dictionary<string, string>(takenIn.Information);
            this.Update();
        }

        /// <summary>
        /// Construct Ma2 with tokens given
        /// </summary>
        /// <param name="tokens">Tokens given</param>
        public Ma2(string[] tokens)
        {
            Chart takenIn = new Ma2Parser().ChartOfToken(tokens);
            this.Notes = takenIn.Notes;
            // this.BPMChanges = takenIn.BPMChanges;
            // this.MeasureChanges = takenIn.MeasureChanges;
            this.StoredChart = new List<List<Note>>(takenIn.StoredChart);
            this.Information = new Dictionary<string, string>(takenIn.Information);
            this.Update();
        }

        /// <summary>
        /// Construct Ma2 with existing values
        /// </summary>
        /// <param name="takenIn">Existing good brother</param>
        public Ma2(Chart takenIn)
        {
            this.Notes = new List<Note>(takenIn.Notes);
            // this.BPMChanges = new BPMChanges(takenIn.BPMChanges);
            // this.MeasureChanges = new MeasureChanges(takenIn.MeasureChanges);
            this.StoredChart = new List<List<Note>>(takenIn.StoredChart);
            this.Information = new Dictionary<string, string>(takenIn.Information);
            this.Update();
        }

        public override bool CheckValidity()
        {
            bool result = this == null;
            // Not yet implemented
            return result;
        }

        public override string Compose()
        {
            string result = "";
            const string header1 = "VERSION\t0.00.00\t1.03.00\nFES_MODE\t0\n";
            const string header2 = "RESOLUTION\t384\nCLK_DEF\t384\nCOMPATIBLE_CODE\tMA2\n";
            result += header1;
            if (this.BPMChangeNotes.Count > 4)
            {
                result += "BPM_DEF" + "\t";
                for (int x = 0; x < 4; x++)
                {
                    result = result + String.Format("{0:F3}", this.BPMChangeNotes[x].BPM);
                    result += "\t";
                }
                return result + "\n";
            }
            else
            {
                int times = 0;
                result += "BPM_DEF" + "\t";
                foreach (BPMChange x in this.BPMChangeNotes)
                {
                    result += String.Format("{0:F3}", x.BPM);
                    result += "\t";
                    times++;
                }
                while (times < 4)
                {
                    result += String.Format("{0:F3}", this.BPMChangeNotes[0].BPM);
                    result += "\t";
                    times++;
                }
            }
            result += "MET_DEF" + "\t" + this.MeasureChangeNotes[0] + "\t" + this.MeasureChangeNotes[0] + "\n";
            result += header2;
            result += "\n";

            for (int i = 0; i < BPMChangeNotes.Count; i++)
            {
                result += "BPM" + "\t" + this.BPMChangeNotes[i].Bar + "\t" + this.BPMChangeNotes[i].Tick + "\t" + this.BPMChangeNotes[i].BPM + "\n";
                //result += "BPM" + "\t" + bar[i] + "\t" + tick[i] + "\t" + String.Format("{0:F3}", bpm[i])+"\n";
            }

            if (this.StoredChart.Count == 0)
            {
                result += "MET" + "\t" + 0 + "\t" + 0 + "\t" + 4 + "\t" + 4 + "\n";
            }
            else
            {
                for (int i = 0; i < MeasureChangeNotes.Count; i++)
                {
                    result += "BPM" + "\t" + this.MeasureChangeNotes[i].Bar + "\t" + this.MeasureChangeNotes[i].Tick + "\t" + this.MeasureChangeNotes[i].BPM + "\n";
                    //result += "BPM" + "\t" + bar[i] + "\t" + tick[i] + "\t" + String.Format("{0:F3}", bpm[i])+"\n";
                }
            }
            result += "\n";

            //foreach (Note x in this.Notes)
            //{
            //    if (!x.Compose(1).Equals(""))
            //    {
            //        result += x.Compose(1) + "\n";
            //    }
            //}
            foreach (List<Note> bar in this.StoredChart)
            {
                foreach (Note x in bar)
                {
                    if (!x.Compose(1).Equals(""))
                    {
                        result += x.Compose(1) + "\n";
                    }
                }
            }
            result += "\n";
            return result;
        }

        /// <summary>
        /// Extracts the special slide containers created by Simai
        /// </summary>
        /// <exception cref="InvalidOperationException">If slide container is casted wrongly, this exception will be raised</exception>
        public void ExtractSlideEachGroup()
        {
            List<Note> adjusted = new();
            List<Slide> slideCandidates = new();
            foreach (Note x in this.Notes)
            {
                switch (x.NoteSpecificGenre)
                {
                    case "SLIDE_EACH":
                        SlideEachSet candidate = x as SlideEachSet ?? throw new InvalidOperationException("THIS IS NOT A SLIDE EACH");
                        if (candidate.SlideStart != null) adjusted.Add(candidate.SlideStart);
                        if (candidate.InternalSlides.Count > 0) slideCandidates.AddRange(candidate.InternalSlides);
                        break;
                    case "SLIDE_GROUP":
                        SlideGroup groupCandidate = x as SlideGroup ?? throw new InvalidOperationException("THIS IS NOT A SLIDE GROUP");
                        if (groupCandidate.InternalSlides.Count > 0) adjusted.AddRange(groupCandidate.InternalSlides);
                        break;
                    default:
                        adjusted.Add(x);
                        break;
                }
            }

            foreach (Slide x in slideCandidates)
            {
                switch (x.NoteSpecificGenre)
                {
                    case "SLIDE_GROUP":
                        SlideGroup groupCandidate = x as SlideGroup ?? throw new InvalidOperationException("THIS IS NOT A SLIDE GROUP");
                        if (groupCandidate.InternalSlides.Count > 0) adjusted.AddRange(groupCandidate.InternalSlides);
                        break;
                    default:
                        adjusted.Add(x);
                        break;
                }
            }
            this.Notes = adjusted;
        }

        // /// <summary>
        // /// Override and compose with given arrays
        // /// </summary>
        // /// <param name="bpm">Override BPM array</param>
        // /// <param name="measure">Override Measure array</param>
        // /// <returns>Good Brother with override array</returns>
        // public override string Compose(BPMChanges bpm, MeasureChanges measure)
        // {
        //     string result = "";
        //     const string header1 = "VERSION\t0.00.00\t1.03.00\nFES_MODE\t0\n";
        //     const string header2 = "RESOLUTION\t384\nCLK_DEF\t384\nCOMPATIBLE_CODE\tMA2\n";
        //     result += header1;
        //     result += bpm.InitialChange;
        //     result += measure.InitialChange;
        //     result += header2;
        //     result += "\n";

        //     result += bpm.Compose();
        //     result += measure.Compose();
        //     result += "\n";

        //     foreach (Note y in this.Notes)
        //     {
        //         result += y.Compose(1) + "\n";
        //     }
        //     result += "\n";
        //     return result;
        // }

        public override void Update()
        {
            this.ExtractSlideEachGroup();
            base.Update();
        }
    }
}
