namespace MaiLib;
using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;
using System.Text;

public class Simai : Chart
{
    #region Constructors
    /// <summary>
    ///     Empty constructor
    /// </summary>
    public Simai()
    {
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
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
        Information = new Dictionary<string, string>(chart.Information);
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
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
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
        Update();
    }

    public Simai(Chart takenIn)
    {
        Notes = takenIn.Notes;
        BPMChanges = takenIn.BPMChanges;
        MeasureChanges = takenIn.MeasureChanges;
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
        Update();
    }
    #endregion

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
            if (candidate.NoteSpecificGenre is NoteSpecificGenre.SLIDE || candidate.NoteSpecificGenre is NoteSpecificGenre.SLIDE_GROUP)
            {
                // Slide slideCandidate = candidate as Slide ?? throw new InvalidCastException("Candidate is not a SLIDE. It is: "+candidate.Compose(ChartVersion.Debug));
                // slideCandidate.NoteSpecialState = candidate.NoteSpecialState;
                slideNotesOfChart.Add((Slide)candidate);
                processedSlideDic.Add((Slide)candidate, false);
            }
            else adjusted.Add(candidate);
        }

        // If this chart only have one slide, it cannot be connecting slide; otherwise this chart is invalid.

        var processedSlidesCount = 0;

        foreach (KeyValuePair<Slide, bool> parentPair in processedSlideDic)
        {
            var parentSlide = parentPair.Key;
            maximumBar = parentSlide.Bar > maximumBar ? parentSlide.Bar : maximumBar;
            if (!parentPair.Value && parentSlide.NoteSpecialState != SpecialState.ConnectingSlide)
            {
                SlideGroup currentGroup = new();
                currentGroup.AddConnectingSlide(parentSlide);
                foreach (KeyValuePair<Slide, bool> candidatePair in processedSlideDic)
                {
                    var candidate = candidatePair.Key;
                    if (candidate != parentSlide && candidate.NoteSpecialState == SpecialState.ConnectingSlide &&
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
        // foreach (KeyValuePair<Slide, bool> x in processedSlideDic)
        // {
        //     if (!x.Value)
        //     {
        //         Slide normalSlide = new Slide(x.Key);
        //         normalSlide.NoteSpecialState = SpecialState.Normal;
        //         adjusted.Add(normalSlide);
        //         processedSlidesCount++;
        //     }
        // }

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
                    errorMsg += x.Key.Compose(ChartVersion.Ma2_104) + ", " + x.Key.TickStamp;
                    if (x.Key.NoteSpecialState is SpecialState.ConnectingSlide)
                    {
                        errorMsg += ", and it is a connecting slide\n";
                    }
                }
            }

            errorMsg += "\n------------\nComposedSlides: \n";
            foreach (Slide x in processedSlideOfChart)
            {
                errorMsg += x.Compose(ChartVersion.Ma2_104) + "\n";
                if (x is SlideGroup)
                {
                    errorMsg += "This slide is also a Slide Group with last slide as " + (x as SlideGroup ?? throw new NullReferenceException("This note cannot be casted to SlideGroup: "+x.Compose(ChartVersion.Debug))).LastSlide.Compose(ChartVersion.Debug) + "\n";
                }
            }
            throw new InvalidOperationException("SLIDE NUMBER MISMATCH - Expected: " + slideNotesOfChart.Count +
                                                ", Actual:" + processedSlidesCount + ", Skipped: " + processedSlideDic.Count(p => !p.Value) + "\n" + errorMsg);
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
            if (!(x.NoteSpecificGenre is NoteSpecificGenre.SLIDE || x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START ||
                  x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_GROUP))
            {
                adjusted.Add(x);
                processedNotes++;
            }
            else if (composedCandidates.Count > 0 && x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START)
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
                     (x.NoteSpecificGenre is NoteSpecificGenre.SLIDE || x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_GROUP))
            {
                foreach (var parent in composedCandidates)
                {
                    var slideStartCandidate = x as Slide ?? throw new InvalidOperationException("THIS IS NOT A SLIDE");
                    eachCandidateCombined = eachCandidateCombined || parent.TryAddCandidateNote(slideStartCandidate);
                    if (eachCandidateCombined) processedNotes++;
                }
            }

            if (!eachCandidateCombined && (x.NoteSpecificGenre is NoteSpecificGenre.SLIDE ||
                                           x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START ||
                                           x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_GROUP))
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
        StringBuilder result = new StringBuilder();
        var delayBar = TotalDelay / 384 + 2;
        //Console.WriteLine(chart.Compose());
        //foreach (BPMChange x in chart.BPMChanges.ChangeNotes)
        //{
        //    Console.WriteLine("BPM Change verified in " + x.Bar + " " + x.Tick + " of BPM" + x.BPM);
        //}
        var firstBpm = new List<Note>();
        foreach (var bpm in Notes)
            if (bpm.NoteSpecificGenre is NoteSpecificGenre.BPM)
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
                //if (x.Bar == 6)
                //{
                //    Console.WriteLine("This is bar 6");
                //}
                switch (lastNote.NoteSpecificGenre)
                {
                    case NoteSpecificGenre.MEASURE:
                        currentQuaver = (lastNote as MeasureChange ??
                                         throw new Exception("This note is not measure change")).Quaver;
                        break;
                    case NoteSpecificGenre.BPM:
                        break;
                    default:
                        if (x.IsOfSameTime(lastNote) && x.IsNote && lastNote.IsNote)
                        {
                            result.Append("/");
                        }
                        else
                        {
                            result.Append(",");
                            commaCompiled++;
                        }

                        break;
                }

                result.Append(x.Compose(ChartVersion));
                lastNote = x;
            }

            result.Append(",\n");
            commaCompiled++;
            if (commaCompiled != currentQuaver)
            {
                Console.WriteLine("Notes in bar: " + bar[0].Bar);
                foreach (var x in bar) Console.WriteLine(x.Compose(ChartVersion.Debug));
                Console.WriteLine(result);
                Console.WriteLine("Expected comma number: " + currentQuaver);
                Console.WriteLine("Actual comma number: " + commaCompiled);
                throw new NullReferenceException("COMMA COMPILED MISMATCH IN BAR " + bar[0].Bar);
            }
        }

        for (var i = 0; i < delayBar + 1; i++) result.Append("{1},\n");
        result.Append("E\n");
        return result.ToString();
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
