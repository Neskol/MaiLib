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

        var maximumBar = 0;
        foreach (var candidate in Notes)
        {
            maximumBar = candidate.Bar > maximumBar ? candidate.Bar : maximumBar;
            if (candidate.NoteSpecificGenre.Equals("SLIDE") || candidate.NoteSpecificGenre.Equals("SLIDE_GROUP"))
                slideNotesOfChart.Add((Slide)candidate);
            else adjusted.Add(candidate);
        }

        /// If this chart only have one slide, it cannot be connecting slide; otherwise this chart is invalid.

        var processedSlides = 0;

        for (var i = 0; i < slideNotesOfChart.Count; i++)
        {
            var parentSlide = slideNotesOfChart[i];
            maximumBar = parentSlide.Bar > maximumBar ? parentSlide.Bar : maximumBar;
            if (parentSlide.NoteSpecialState != Note.SpecialState.ConnectingSlide)
            {
                SlideGroup currentGroup = new();
                currentGroup.AddConnectingSlide(parentSlide);
                for (var j = i == 0 ? 1 : 0; j < slideNotesOfChart.Count; j += j + 1 == i ? 2 : 1)
                {
                    var candidate = slideNotesOfChart[j];
                    if (candidate.NoteSpecialState == Note.SpecialState.ConnectingSlide &&
                        candidate.TickStamp == currentGroup.LastSlide.LastTickStamp &&
                        candidate.Key.Equals(currentGroup.LastSlide.EndKey) && !connectedSlides.Contains(candidate))
                    {
                        currentGroup.AddConnectingSlide(candidate);
                        connectedSlides.Add(candidate);
                        processedSlides++;
                    }
                }

                if (currentGroup.SlideCount > 1) adjusted.Add(currentGroup);
                else adjusted.Add(parentSlide);
                processedSlides++;
            }
        }

        //For verification only: check if slide count is correct
        if (processedSlides != slideNotesOfChart.Count)
            throw new InvalidOperationException("SLIDE NUMBER MISMATCH - Expected: " + slideNotesOfChart.Count +
                                                ", Actual:" + processedSlides);
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