namespace MaiLib;

public class Simai : Chart
{
    /// <summary>
    ///     Empty constructor
    /// </summary>
    public Simai()
    {
        Notes = new List<Note>();
        BPMChanges = new BPMChanges();
        MeasureChanges = new MeasureChanges();
        StoredChart = new List<List<Note>>();
        Information = new Dictionary<string, string>();
    }

    /// <summary>
    ///     Constructs Simai chart directly from path specified
    /// </summary>
    /// <param name="location"></param>
    public Simai(string location)
    {
        var tokens = new SimaiTokenizer().Tokens(location);
        var chart = new SimaiParser().ChartOfToken(tokens);
        Notes = new List<Note>(chart.Notes);
        BPMChanges = new BPMChanges(chart.BPMChanges);
        MeasureChanges = new MeasureChanges(chart.MeasureChanges);
        StoredChart = new List<List<Note>>();
        Information = new Dictionary<string, string>(chart.Information);
        Update();
    }

    /// <summary>
    ///     Construct Simai from given parameters
    /// </summary>
    /// <param name="notes">Notes to take in</param>
    /// <param name="bpmChanges">BPM change to take in</param>
    /// <param name="measureChanges">Measure change to take in</param>
    public Simai(List<Note> notes, BPMChanges bpmChanges, MeasureChanges measureChanges)
    {
        Notes = notes;
        BPMChanges = bpmChanges;
        MeasureChanges = measureChanges;
        StoredChart = new List<List<Note>>();
        Information = new Dictionary<string, string>();
        Update();
    }

    public Simai(Chart takenIn)
    {
        Notes = takenIn.Notes;
        BPMChanges = takenIn.BPMChanges;
        MeasureChanges = takenIn.MeasureChanges;
        StoredChart = new List<List<Note>>();
        Information = new Dictionary<string, string>();
        Update();
    }

    public void ComposeSlideGroup()
    {
        List<Note> adjusted = new();
        List<Slide> connectedSlides = new();
        List<Slide> slideNotesOfChart = new();
        List<Slide> processedSlideOfChart = new();
        Dictionary<Slide, bool> processedSlideDic = new();

        var maximumBar = 0;
        foreach (var candidate in Notes)
        {
            maximumBar = candidate.Bar > maximumBar ? candidate.Bar : maximumBar;
            if (candidate.NoteSpecificGenre.Equals("SLIDE") || candidate.NoteSpecificGenre.Equals("SLIDE_GROUP"))
            {
                slideNotesOfChart.Add((Slide)candidate);
                processedSlideDic.Add((Slide)candidate, false);
            }
            else adjusted.Add(candidate);
        }

        // If this chart only have one slide, it cannot be connecting slide; otherwise this chart is invalid.

        var processedSlidesCount = 0;

        foreach (KeyValuePair<Slide,bool> parentPair in processedSlideDic)
        {
            var parentSlide = parentPair.Key;
            maximumBar = parentSlide.Bar > maximumBar ? parentSlide.Bar : maximumBar;
            if (!parentPair.Value && parentSlide.NoteSpecialState != Note.SpecialState.ConnectingSlide)
            {
                SlideGroup currentGroup = new();
                currentGroup.AddConnectingSlide(parentSlide);
                foreach (KeyValuePair<Slide,bool> candidatePair in processedSlideDic)
                {
                    var candidate = candidatePair.Key;
                    if (candidate != parentSlide && candidate.NoteSpecialState == Note.SpecialState.ConnectingSlide &&
                        candidate.TickStamp == currentGroup.LastSlide.LastTickStamp &&
                        candidate.Key.Equals(currentGroup.LastSlide.EndKey) && !candidatePair.Value)
                    {
                        currentGroup.AddConnectingSlide(candidate);
                        connectedSlides.Add(candidate);
                        processedSlidesCount++;
                        processedSlideDic[candidate] = true;
                    }
                }

                if (currentGroup.SlideCount > 1)
                {
                    adjusted.Add(currentGroup);
                    processedSlideOfChart.Add(currentGroup);
                    processedSlideDic[parentSlide] = true;
                }
                else
                {
                    adjusted.Add(parentSlide);
                    processedSlideOfChart.Add(parentSlide);
                    processedSlideDic[parentSlide] = true;
                }
                processedSlidesCount++;
            }
        }

        // This for loop shouldn't be here: compromise of each connecting slide
        foreach (KeyValuePair<Slide, bool> x in processedSlideDic)
        {
            if (!x.Value)
            {
                Slide normalSlide = new Slide(x.Key);
                normalSlide.NoteSpecialState = Note.SpecialState.Normal;
                adjusted.Add(normalSlide);
                processedSlidesCount++;
            }
        }

        //For verification only: check if slide count is correct
        if (processedSlidesCount != slideNotesOfChart.Count)
        {
            slideNotesOfChart.Sort((p, q) => p.TickStamp.CompareTo(q.TickStamp));
            processedSlideOfChart.Sort((p, q) => p.TickStamp.CompareTo(q.TickStamp));
            string errorMsg = "Slide(s) were skipped during processing: \n";
            foreach (KeyValuePair<Slide, bool> x in processedSlideDic)
            {
                if (!x.Value)
                {
                    errorMsg += x.Key.Compose(1) + ", " + x.Key.TickStamp;
                    if (x.Key.NoteSpecialState is Note.SpecialState.ConnectingSlide)
                    {
                        errorMsg += ", and it is a connecting slide";
                    }
                }
            }

            errorMsg += "\n------------\nComposedSlides: \n";
            foreach (Slide x in processedSlideOfChart)
            {
                errorMsg += x.Compose(0) + "\n";
                if (x is SlideGroup)
                {
                    errorMsg += "This slide is also a Slide Group with last slide as " + (x as SlideGroup).LastSlide.Compose(1)+"\n";
                }

            }
            throw new InvalidOperationException("SLIDE NUMBER MISMATCH - Expected: " + slideNotesOfChart.Count +
                                                ", Actual:" + processedSlidesCount +", Skipped: "+ processedSlideDic.Count(p => !p.Value) + "\n" + errorMsg);
        }
        Notes = new List<Note>(adjusted);
    }

    public void ComposeSlideEachGroup()
    {
        List<SlideEachSet> composedCandidates = new();
        List<Note> adjusted = new();
        var processedNotes = 0;
        foreach (var x in Notes)
        {
            var eachCandidateCombined = false;
            if (!(x.NoteSpecificGenre.Equals("SLIDE") || x.NoteSpecificGenre.Equals("SLIDE_START") ||
                  x.NoteSpecificGenre.Equals("SLIDE_GROUP")))
            {
                adjusted.Add(x);
                processedNotes++;
            }
            else if (composedCandidates.Count > 0 && x.NoteSpecificGenre.Equals("SLIDE_START"))
            {
                foreach (var parent in composedCandidates)
                {
                    var slideStartCandidate =
                        x as Tap ?? throw new InvalidOperationException("THIS IS NOT A SLIDE START");
                    eachCandidateCombined = eachCandidateCombined || parent.TryAddCandidateNote(slideStartCandidate);
                    if (eachCandidateCombined) processedNotes++;
                }
            }
            else if (composedCandidates.Count > 0 &&
                     (x.NoteSpecificGenre.Equals("SLIDE") || x.NoteSpecificGenre.Equals("SLIDE_GROUP")))
            {
                foreach (var parent in composedCandidates)
                {
                    var slideStartCandidate = x as Slide ?? throw new InvalidOperationException("THIS IS NOT A SLIDE");
                    eachCandidateCombined = eachCandidateCombined || parent.TryAddCandidateNote(slideStartCandidate);
                    if (eachCandidateCombined) processedNotes++;
                }
            }

            if (!eachCandidateCombined && (x.NoteSpecificGenre.Equals("SLIDE") ||
                                           x.NoteSpecificGenre.Equals("SLIDE_START") ||
                                           x.NoteSpecificGenre.Equals("SLIDE_GROUP")))
            {
                composedCandidates.Add(new SlideEachSet(x));
                processedNotes++;
            }
        }

        // if (processedNotes != this.Notes.Count) throw new InvalidOperationException("PROCESSED NOTES MISMATCH: Expected "+this.Notes.Count+", Actual "+processedNotes);
        adjusted.AddRange(composedCandidates);
        Notes = adjusted;
    }

    public override string Compose()
    {
        var result = "";
        var delayBar = TotalDelay / 384 + 2;
        //Console.WriteLine(chart.Compose());
        //foreach (BPMChange x in chart.BPMChanges.ChangeNotes)
        //{
        //    Console.WriteLine("BPM Change verified in " + x.Bar + " " + x.Tick + " of BPM" + x.BPM);
        //}
        var firstBpm = new List<Note>();
        foreach (var bpm in Notes)
            if (bpm.NoteSpecificGenre.Equals("BPM"))
                firstBpm.Add(bpm);
        // if (firstBpm.Count > 1)
        // {
        //     chart.Chart[0][0] = firstBpm[1];
        // }
        foreach (var bar in StoredChart)
        {
            Note lastNote = new MeasureChange();
            var currentQuaver = 0;
            var commaCompiled = 0;
            //result += bar[1].Bar;
            foreach (var x in bar)
            {
                switch (lastNote.NoteSpecificGenre)
                {
                    case "MEASURE":
                        currentQuaver = (lastNote as MeasureChange ??
                                         throw new Exception("This note is not measure change")).Quaver;
                        break;
                    case "BPM":
                        break;
                    default:
                        if (x.IsOfSameTime(lastNote))
                        {
                            result += "/";
                        }
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
                foreach (var x in bar) Console.WriteLine(x.Compose(1));
                Console.WriteLine(result);
                Console.WriteLine("Expected comma number: " + currentQuaver);
                Console.WriteLine("Actual comma number: " + commaCompiled);
                throw new NullReferenceException("COMMA COMPILED MISMATCH IN BAR " + bar[0].Bar);
            }
        }

        for (var i = 0; i < delayBar + 1; i++) result += "{1},\n";
        result += "E\n";
        return result;
    }

    public override bool CheckValidity()
    {
        var result = this == null;
        // Not yet implemented
        return result;
    }

    /// <summary>
    ///     Reconstruct the chart with given arrays
    /// </summary>
    /// <param name="bpm">New BPM Changes</param>
    /// <param name="measure">New Measure Changes</param>
    /// <returns>New Composed Chart</returns>
    public override string Compose(BPMChanges bpm, MeasureChanges measure)
    {
        var sourceBPM = BPMChanges;
        var sourceMeasures = MeasureChanges;
        BPMChanges = bpm;
        MeasureChanges = measure;
        Update();

        var result = Compose();
        BPMChanges = sourceBPM;
        MeasureChanges = sourceMeasures;
        Update();
        return result;
    }

    public override void Update()
    {
        ComposeSlideGroup();
        ComposeSlideEachGroup();
        base.Update();
    }
}