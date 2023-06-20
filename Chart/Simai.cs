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

        public void ComposeSlideGroup()
        {
            List<Note> adjusted = new();
            List<Slide> connectedSlides = new();
            List<Slide> slideNotesOfChart = new();

            int maximumBar = 0;
            foreach (Note candidate in this.Notes)
            {
                maximumBar = candidate.Bar > maximumBar ? candidate.Bar : maximumBar;
                if (candidate.NoteSpecificGenre.Equals("SLIDE")||candidate.NoteSpecificGenre.Equals("SLIDE_GROUP")) slideNotesOfChart.Add((Slide)candidate);
                else adjusted.Add(candidate);
            }

            /// If this chart only have one slide, it cannot be connecting slide; otherwise this chart is invalid.

            int processedSlides = 0;
            
            for (int i = 0; i < slideNotesOfChart.Count; i++)
            {              
                Slide parentSlide = slideNotesOfChart[i];
                maximumBar = parentSlide.Bar > maximumBar ? parentSlide.Bar : maximumBar;        
                if (parentSlide.NoteSpecialState != Note.SpecialState.ConnectingSlide)
                {
                    SlideGroup currentGroup = new();
                    currentGroup.AddConnectingSlide(parentSlide);
                    for (int j = i == 0 ? 1 : 0; j < slideNotesOfChart.Count; j += j + 1 == i ? 2 : 1)
                    {
                        Slide candidate = slideNotesOfChart[j];
                        if (candidate.NoteSpecialState == Note.SpecialState.ConnectingSlide && candidate.TickStamp == currentGroup.LastSlide.LastTickStamp && candidate.Key.Equals(currentGroup.LastSlide.EndKey) && !connectedSlides.Contains(candidate))
                        {
                            currentGroup.AddConnectingSlide(candidate);
                            connectedSlides.Add(candidate);
                            processedSlides++;
                        }
                    }
                    if (currentGroup.SlideCount>1) adjusted.Add(currentGroup);
                    else adjusted.Add(parentSlide);
                    processedSlides++;
                }
            }

            //For verification only: check if slide count is correct
            if (processedSlides!=slideNotesOfChart.Count) throw new InvalidOperationException("SLIDE NUMBER MISMATCH - Expected: " + slideNotesOfChart.Count + ", Actual:" + processedSlides);
            this.Notes = new(adjusted);
            
        }

        public void ComposeSlideEachGroup()
        {
            List<SlideEachSet> composedCandidates = new();
            List<Note> adjusted = new();
            int processedNotes = 0;
            foreach (Note x in this.Notes)
            {
                bool eachCandidateCombined = false;
                if (!(x.NoteSpecificGenre.Equals("SLIDE") || x.NoteSpecificGenre.Equals("SLIDE_START")||x.NoteSpecificGenre.Equals("SLIDE_GROUP")))
                {
                    adjusted.Add(x);
                    processedNotes++;
                }
                else if (composedCandidates.Count>0 && x.NoteSpecificGenre.Equals("SLIDE_START")) foreach (SlideEachSet parent in composedCandidates)
                {
                    Tap slideStartCandidate = x as Tap ?? throw new InvalidOperationException("THIS IS NOT A SLIDE START");
                    eachCandidateCombined = eachCandidateCombined || parent.TryAddCandidateNote(slideStartCandidate);
                    if (eachCandidateCombined) processedNotes++;
                }
                else if (composedCandidates.Count > 0 && (x.NoteSpecificGenre.Equals("SLIDE")||x.NoteSpecificGenre.Equals("SLIDE_GROUP"))) foreach (SlideEachSet parent in composedCandidates)
                {
                    Slide slideStartCandidate = x as Slide ?? throw new InvalidOperationException("THIS IS NOT A SLIDE");
                    eachCandidateCombined = eachCandidateCombined || parent.TryAddCandidateNote(slideStartCandidate);
                    if (eachCandidateCombined) processedNotes++;
                }
                if (!eachCandidateCombined && (x.NoteSpecificGenre.Equals("SLIDE") || x.NoteSpecificGenre.Equals("SLIDE_START") ||x.NoteSpecificGenre.Equals("SLIDE_GROUP")))
                {
                    composedCandidates.Add(new SlideEachSet(x));
                    processedNotes++;
                }
            }
            // if (processedNotes != this.Notes.Count) throw new InvalidOperationException("PROCESSED NOTES MISMATCH: Expected "+this.Notes.Count+", Actual "+processedNotes);
            adjusted.AddRange(composedCandidates);
            this.Notes = adjusted;
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
                        default:
                            if (x.IsOfSameTime(lastNote)) result += "/";
                            else 
                            {
                                result += ",";
                                commaCompiled++;
                            }
                            break;
                    }
                    result += x.Compose(0);
                    lastNote = x;
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
            for (int i = 0; i < delayBar + 1; i++)
            {
                result += "{1},\n";
            }
            result += "E\n";
            return result;
        }

        public override bool CheckValidity()
        {
            bool result = this == null;
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

        public override void Update()
        {
            this.ComposeSlideGroup();
            this.ComposeSlideEachGroup();
            base.Update();
        }
    }
}