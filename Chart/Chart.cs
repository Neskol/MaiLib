namespace MaiLib;

using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;

/// <summary>
///     A class holding notes and information to form a chart
/// </summary>
public abstract class Chart : IChart
{
    private const double Tolerance = 0.0001;

    //Theoretical Rating = (Difference in 100-down and Max score)/100-down

    /// <summary>
    ///     Empty constructor
    /// </summary>
    public Chart()
    {
        Notes = [];
        BPMChanges = new BPMChanges();
        MeasureChanges = new MeasureChanges();
        StoredChart = [];
        Information = [];
        // IsDxChart = false;
        Definition = 384;
        UnitScore = [500, 1000, 1500, 2000, 2500];
        ChartVersion = ChartVersion.Ma2_103;
    }

    #region Fields

    /// <summary>
    ///     Defines the chart type by enums
    /// </summary>
    public ChartType ChartType => Notes.Any(p =>
        p.NoteSpecialState is SpecialState.EX or SpecialState.BreakEX or SpecialState.ConnectingSlide)
        ? ChartType.DX
        : ChartType.Standard;

    /// <summary>
    ///     Defines the chart version by enums
    /// </summary>
    public ChartVersion ChartVersion { get; set; }

    /// <summary>
    ///     Stores all notes
    /// </summary>
    public List<Note> Notes { get; set; }

    /// <summary>
    ///     Stores definitions of BPM Changes
    /// </summary>
    public BPMChanges BPMChanges { get; set; }

    /// <summary>
    ///     Stores definitions of Measure Changes
    /// </summary>
    public MeasureChanges MeasureChanges { get; protected set; }

    public int NormalTapNum =>
        Notes.Count(p => p.NoteType is NoteType.TAP && p.NoteSpecialState is SpecialState.Normal);

    public int BreakTapNum => Notes.Count(p => p.NoteType is NoteType.TAP && p.NoteSpecialState is SpecialState.Break);
    public int ExTapNum => Notes.Count(p => p.NoteType is NoteType.TAP && p.NoteSpecialState is SpecialState.EX);

    public int BreakExTapNum =>
        Notes.Count(p => p.NoteType is NoteType.TAP && p.NoteSpecialState is SpecialState.BreakEX);

    public int NormalHoldNum =>
        Notes.Count(p => p.NoteType is NoteType.HLD && p.NoteSpecialState is SpecialState.Normal);

    public int ExHoldNum => Notes.Count(p => p.NoteType is NoteType.HLD && p.NoteSpecialState is SpecialState.EX);
    public int BreakHoldNum => Notes.Count(p => p.NoteType is NoteType.HLD && p.NoteSpecialState is SpecialState.Break);

    public int BreakExHoldNum =>
        Notes.Count(p => p.NoteType is NoteType.HLD && p.NoteSpecialState is SpecialState.BreakEX);

    public int NormalSlideStartNum =>
        Notes.Count(p => p.NoteType is NoteType.STR && p.NoteSpecialState is SpecialState.Normal);

    public int BreakSlideStartNum =>
        Notes.Count(p => p.NoteType is NoteType.STR && p.NoteSpecialState is SpecialState.Break);

    public int ExSlideStartNum => Notes.Count(p => p.NoteType is NoteType.STR && p.NoteSpecialState is SpecialState.EX);

    public int BreakExSlideStartNum =>
        Notes.Count(p => p.NoteType is NoteType.STR && p.NoteSpecialState is SpecialState.BreakEX);

    public int TouchTapNum => Notes.Count(p => p.NoteType is NoteType.TTP);

    public int TouchHoldNum =>
        Notes.Count(p => p.NoteType is NoteType.THO);

    public int NormalSlideNum =>
        Notes.Count(p => p.NoteGenre is NoteGenre.SLIDE && p.NoteSpecialState is SpecialState.Normal);

    public int BreakSlideNum =>
        Notes.Count(p => p.NoteGenre is NoteGenre.SLIDE && p.NoteSpecialState is SpecialState.Break);

    public int AllNoteRecNum => Notes.Count(p => p.NoteSpecialState is not SpecialState.ConnectingSlide);
    public int TapNum => NormalTapNum + ExTapNum + NormalSlideStartNum + ExSlideStartNum + TouchTapNum;

    public int BreakNum => BreakTapNum + BreakExTapNum + BreakHoldNum + BreakExHoldNum + BreakSlideStartNum +
                           BreakExSlideStartNum + BreakSlideNum;

    public int HoldNum => NormalHoldNum + ExHoldNum + TouchHoldNum;
    public int SlideNum => NormalSlideNum;
    public int AllNoteNum => TapNum + BreakNum + HoldNum + SlideNum;
    public int TapJudgeNum => TapNum + BreakTapNum + BreakExTapNum + BreakSlideStartNum + BreakExSlideStartNum;
    public int HoldJudgeNum => (HoldNum + BreakHoldNum + BreakExHoldNum) * 2;
    public int SlideJudgeNum => NormalSlideNum + BreakSlideNum;
    public int AllJudgeNum => TapJudgeNum + HoldJudgeNum + SlideJudgeNum;

    public int TapScore => TapNum * 500;
    public int BreakScore => BreakNum * 2600;
    public int HoldScore => HoldNum * 1000;
    public int SlideScore => SlideNum * 1500;
    public int AllScore => TapScore + BreakScore + HoldScore + SlideScore;
    public int ScoreS => (int)double.Round(AllScore * 0.97);
    public int ScoreSs => (int)double.Round(AllScore * 0.99);
    public int RatedAchievement => (int)double.Round((1 + (double)(BreakNum * 100) / AllScore) * 10000);

    public int EachPairsNum
    {
        get
        {
            Dictionary<int, int> eachPairDictionary = [];
            foreach (Note note in this.Notes.Where(p => p.NoteGenre is NoteGenre.TAP or NoteGenre.HOLD))
            {
                if (!eachPairDictionary.Keys.Contains(note.TickStamp)) eachPairDictionary.Add(note.TickStamp, 1);
                else eachPairDictionary[note.TickStamp]++;
            }

            return eachPairDictionary.Values.Count(p => p > 1);
        }
    }

    /// <summary>
    ///     Defines if the chart is DX chart
    /// </summary>
    public bool IsDxChart => Notes.Any(note =>
        note.NoteSpecialState is SpecialState.EX or SpecialState.BreakEX or SpecialState.BreakEX
            or SpecialState.ConnectingSlide || note.NoteType is NoteType.TTP or NoteType.THO);

    /// <summary>
    ///     The first note of the chart
    /// </summary>
    public Note? FirstNote => Notes.Where(note => note.NoteGenre is not NoteGenre.BPM or NoteGenre.MEASURE)
        .MinBy(note => note.TickStamp);

    /// <summary>
    ///     The definition of this chart
    /// </summary>
    public int Definition { get; set; }

    /// <summary>
    ///     The basic unit of maimai chart
    /// </summary>
    /// <value>Unit score</value>
    public int[] UnitScore { get; set; }

    /// <summary>
    ///     The total achievement of this chart{get; set;} to be updated
    /// </summary>
    public int Achievement { get; set; }

    /// <summary>
    ///     The total delay of holds behind final note{get; set;} to be updated
    /// </summary>
    public int TotalDelay { get; set; }

    /// <summary>
    ///     Stored chart in structure of bar of notes
    /// </summary>
    public List<List<Note>> StoredChart { get; protected set; }

    /// <summary>
    ///     Stored the information of this chart, if any
    /// </summary>
    public Dictionary<string, string> Information { get; set; }

    /// <summary>
    ///     Defines whether this chart is Utage chart
    /// </summary>
    public bool IsUtage { get; protected set; }

    #endregion

    public abstract bool CheckValidity();

    /// <summary>
    ///     Update properties in Good Brother for exporting
    /// </summary>
    public virtual void Update()
    {
        StoredChart = [];
        int maxBar = Notes.Count > 0 ? Notes.Max(p => p.Bar) : 0;
        double timeStamp = 0.0;

        foreach (Note x in Notes)
        {
            x.BPMChangeNotes = BPMChanges.ChangeNotes;
            x.Update();
        }

        //Iterate over bar
        for (int i = 0; i <= maxBar; i++)
        {
            List<Note>? bar = [];
            BPMChange? noteChange = new BPMChange();
            double currentBPM = BPMChanges.ChangeNotes[0].BPM;
            Note lastNote = new Rest();
            Note realLastNote = new Rest();
            foreach (BPMChange? x in BPMChanges.ChangeNotes)
                if (x.Bar == i)
                    bar.Add(x); //Extract the first BPM change in bar to the beginning of the bar
            foreach (Note? x in Notes)
            {
                // if (FirstNote == null && !(x.NoteType is NoteType.BPM or NoteType.MEASURE)) FirstNote = x;
                // Console.WriteLine(x.Compose(0));
                //x.TickTimeStamp = this.GetTimeStamp(x.TickStamp);
                //x.WaitTimeStamp = this.GetTimeStamp(x.WaitTickStamp);
                // x.LastTimeStamp = this.GetTimeStamp(x.LastTickStamp);
                if (x.Bar == i)
                {
                    //x.ReplaceBPMChanges(this.bpmChanges);
                    //x.BPMChangeNotes = this.bpmChanges.ChangeNotes;
                    //x.Update();
                    // Console.WriteLine("This note contains "+x.BPMChangeNotes.Count+" BPM notes");
                    //Console.WriteLine(GetNoteDetail(this.bpmChanges, x));
                    int delay = x.Bar * Definition + x.Tick + x.WaitLength + x.LastLength;
                    switch (x.NoteSpecificGenre)
                    {
                        case NoteSpecificGenre.BPM:
                            currentBPM = x.BPM;
                            break;
                        case NoteSpecificGenre.MEASURE:
                            break;
                        case NoteSpecificGenre.REST:
                            break;
                        case NoteSpecificGenre.TAP:
                            // if (x.NoteSpecialState is SpecialState.EX) IsDxChart = false;
                            // if (x.NoteType is NoteType.TTP)
                            // {
                            //     IsDxChart = false;
                            // }
                            // else if (x.NoteSpecialState is SpecialState.Break or SpecialState.BreakEX)
                            // {
                            // }

                            break;
                        case NoteSpecificGenre.HOLD:
                            // x.TickBPMDisagree = Math.Abs(GetBPMByTick(x.TickStamp) - GetBPMByTick(x.LastTickStamp)) > Tolerance ||
                            //                     HasBPMChangeInBetween(x.TickStamp, x.LastTickStamp);
                            // x.Update();
                            x.TickTimeStamp = GetTimeStamp(x.TickStamp);
                            if (x.CalculatedLastTime == 0)
                            {
                                x.LastTimeStamp = GetTimeStamp(x.LastTickStamp);
                                x.CalculatedLastTime = x.LastTimeStamp - x.TickTimeStamp;
                                x.FixedLastLength =
                                    (int)double.Round(x.CalculatedLastTime /
                                                      GetBPMTimeUnit(GetBPMByTick(x.TickStamp), Definition));
                            }

                            if (delay > TotalDelay) TotalDelay = delay;
                            //Console.WriteLine("New delay: " + delay);
                            //Console.WriteLine(x.Compose(1));
                            // if (x.NoteType is NoteType.THO)
                            // {
                            //     IsDxChart = false;
                            // }

                            break;
                        case NoteSpecificGenre.SLIDE_START:
                            break;
                        case NoteSpecificGenre.SLIDE:
                            // x.TickBPMDisagree = Math.Abs(GetBPMByTick(x.TickStamp) - GetBPMByTick(x.WaitTickStamp)) > Tolerance ||
                            //                     Math.Abs(GetBPMByTick(x.WaitTickStamp) - GetBPMByTick(x.LastTickStamp)) > Tolerance ||
                            //                     Math.Abs(GetBPMByTick(x.TickStamp) - GetBPMByTick(x.LastTickStamp)) > Tolerance ||
                            //                     HasBPMChangeInBetween(x.TickStamp, x.WaitTickStamp);
                            // x.Update();
                            x.TickTimeStamp = GetTimeStamp(x.TickStamp);
                            if (x.CalculatedWaitTime == 0)
                            {
                                x.WaitTimeStamp = GetTimeStamp(x.WaitTickStamp);
                                x.CalculatedWaitTime = x.WaitTimeStamp - x.TickTimeStamp;
                            }

                            if (x.CalculatedLastTime == 0)
                            {
                                x.LastTimeStamp = GetTimeStamp(x.LastTickStamp);
                                x.CalculatedLastTime = x.LastTimeStamp - x.TickTimeStamp;
                                x.FixedLastLength =
                                    (int)double.Round(x.CalculatedLastTime /
                                                      GetBPMTimeUnit(GetBPMByTick(x.TickStamp), Definition));
                            }

                            // if (lastNote.NoteSpecificType.Equals("SLIDE_START") && (lastNote.Bar == x.Bar && lastNote.Tick == x.Tick && lastNote.Key.Equals(x.Key)))
                            // {
                            //     x.SlideStart = lastNote;
                            //     lastNote.ConsecutiveSlide = x;
                            // }
                            if (delay > TotalDelay) TotalDelay = delay;
                            //Console.WriteLine("New delay: "+delay);
                            //Console.WriteLine(x.Compose(1));
                            // if (x.SlideStart == null)
                            // {
                            //     Console.WriteLine("A SLIDE WITHOUT START WAS FOUND");
                            //     Console.WriteLine(x.Compose(1));
                            //     Console.WriteLine("This slide has start: " + (x.SlideStart == null));
                            //     throw new NullReferenceException("A SLIDE WITHOUT START WAS FOUND");
                            // }
                            break;
                    }

                    // x.BPM = currentBPM;
                    bar.Add(x);
                    if (x.NoteGenre is not NoteGenre.SLIDE) lastNote = x;
                    realLastNote = x;
                    timeStamp += x.TickTimeStamp;
                }
            }

            List<Note>? afterBar =
            [
                new MeasureChange(i, 0, CalculateQuaver(CalculateLeastMeasure(bar, Definition), Definition)),
                //Console.WriteLine();
                //Console.WriteLine("In bar "+i+", LeastMeasure is "+ CalculateLeastMeasure(bar)+", so quaver will be "+ CalculateQuaver(CalculateLeastMeasure(bar)));
                .. bar,
            ];
            StoredChart.Add(FinishBar(afterBar, i,
                CalculateQuaver(CalculateLeastMeasure(bar, Definition), Definition), Definition));
        }

        //Console.WriteLine("TOTAL DELAY: "+this.TotalDelay);
        //Console.WriteLine("TOTAL COUNT: "+ this.chart.Count * 384);
        if (TotalDelay < StoredChart.Count * Definition)
            TotalDelay = 0;
        else
            TotalDelay -= StoredChart.Count * Definition;
    }

    public virtual void UpdateTest()
    {
        int maxBar = Notes.Count > 0 ? Notes.Max(p => p.Bar) + 1 : 1;
        List<Note>[] chartCandidate = new List<Note>[maxBar];
        for (int i = 0; i < chartCandidate.Length; i++) chartCandidate[i] = [];

        foreach (BPMChange bpmChangeNote in BPMChanges.ChangeNotes)
        {
            chartCandidate[bpmChangeNote.Bar].Insert(0, bpmChangeNote);
            // Well I know this is not the best practice but we assume bar may be empty here
        }

        foreach (Note x in Notes)
        {
            x.BPMChangeNotes = BPMChanges.ChangeNotes;
            x.Update();
            int delay = x.Bar * Definition + x.Tick + x.WaitLength + x.LastLength;
            switch (x.NoteSpecificGenre)
            {
                case NoteSpecificGenre.BPM:
                    break;
                case NoteSpecificGenre.MEASURE:
                    break;
                case NoteSpecificGenre.REST:
                    break;
                case NoteSpecificGenre.TAP:

                    break;
                case NoteSpecificGenre.HOLD:
                    // x.TickBPMDisagree = Math.Abs(GetBPMByTick(x.TickStamp) - GetBPMByTick(x.LastTickStamp)) > Tolerance ||
                    //                     HasBPMChangeInBetween(x.TickStamp, x.LastTickStamp);

                    if (delay > TotalDelay) TotalDelay = delay;
                    break;
                case NoteSpecificGenre.SLIDE_START:
                    break;
                case NoteSpecificGenre.SLIDE:
                    // x.TickBPMDisagree = Math.Abs(GetBPMByTick(x.TickStamp) - GetBPMByTick(x.WaitTickStamp)) > Tolerance ||
                    //                     Math.Abs(GetBPMByTick(x.WaitTickStamp) - GetBPMByTick(x.LastTickStamp)) > Tolerance ||
                    //                     Math.Abs(GetBPMByTick(x.TickStamp) - GetBPMByTick(x.LastTickStamp)) > Tolerance ||
                    //                     HasBPMChangeInBetween(x.TickStamp, x.WaitTickStamp);
                    if (delay > TotalDelay) TotalDelay = delay;
                    break;
            }

            chartCandidate[x.Bar].Add(x);
        }

        StoredChart = [];
        for (int i = 0; i < chartCandidate.Length; i++)
        {
            List<Note>? afterBar =
            [
                new MeasureChange(i, 0,
                    CalculateQuaver(CalculateLeastMeasure(chartCandidate[i], Definition), Definition)),
                .. chartCandidate[i],
            ];
            StoredChart.Add(FinishBar(afterBar, i,
                CalculateQuaver(CalculateLeastMeasure(chartCandidate[i], Definition), Definition), Definition));
        }
    }

    public virtual string Compose()
    {
        return Compose(ChartVersion);
    }

    public virtual string Compose(ChartVersion chartVersion)
    {
        switch (chartVersion)
        {
            case ChartVersion.Simai:
                return new Simai(this) { ChartVersion = ChartVersion.Simai }.Compose();
            case ChartVersion.SimaiFes:
                return new Simai(this) { ChartVersion = ChartVersion.SimaiFes }.Compose();
            case ChartVersion.Ma2_103:
                return new Ma2(this) { ChartVersion = ChartVersion.Ma2_103 }.Compose();
            case ChartVersion.Ma2_104:
                return new Ma2(this) { ChartVersion = ChartVersion.Ma2_104 }.Compose();
            case ChartVersion.Ma2_105:
                return new Ma2(this) { ChartVersion = ChartVersion.Ma2_105 }.Compose();
            case ChartVersion.Debug:
            default:
                return new Ma2(this) { ChartVersion = ChartVersion.Ma2_104 }.Compose();
        }
    }


    public double GetTimeStamp(int bar, int tick)
    {
        double result = 0.0;
        int overallTick = bar * Definition + tick;
        if (overallTick != 0)
        {
            int maximumBPMIndex = 0;
            for (int i = 0; i < BPMChanges.ChangeNotes.Count; i++)
                if (BPMChanges.ChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
            if (maximumBPMIndex == 0)
            {
                result = 60 / BPMChanges.ChangeNotes[0].BPM * 4 / Definition;
            }
            else
            {
                for (int i = 1; i <= maximumBPMIndex; i++)
                {
                    double previousTickTimeUnit = 60 / BPMChanges.ChangeNotes[i - 1].BPM * 4 / Definition;
                    result += (BPMChanges.ChangeNotes[i].TickStamp - BPMChanges.ChangeNotes[i - 1].TickStamp) *
                              previousTickTimeUnit;
                }

                double tickTimeUnit = 60 / BPMChanges.ChangeNotes[maximumBPMIndex].BPM * 4 / Definition;
                result += (overallTick - BPMChanges.ChangeNotes[maximumBPMIndex].TickStamp) * tickTimeUnit;
            }
        }

        return result;
    }

    private Note CopyAndShiftNote(Note x, int overallTick)
    {
        Note copy;
        switch (x)
        {
            case Tap:
                copy = new Tap(x);
                break;
            case Hold:
                copy = new Hold(x);
                break;
            case SlideEachSet set:
                SlideEachSet copySet = new SlideEachSet(set);
                copy = copySet;
                copySet.InternalSlides = [];
                foreach (Slide slide in set.InternalSlides)
                {
                    copySet.InternalSlides.Add((Slide)CopyAndShiftNote(slide, overallTick));
                }

                if (set.SlideStart is not null)
                {
                    copySet.SlideStart = CopyAndShiftNote(set.SlideStart, overallTick);
                }

                break;
            case SlideGroup group:
                copy = new SlideGroup(x);
                ((SlideGroup)copy).InternalSlides.Clear();
                foreach (Slide slide in group.InternalSlides)
                {
                    ((SlideGroup)copy).InternalSlides.Add((Slide)CopyAndShiftNote(slide, overallTick));
                }

                break;
            case Slide:
                copy = new Slide(x);
                break;
            case BPMChange:
                copy = new BPMChange(x);
                break;
            case MeasureChange change:
                copy = new MeasureChange(change);
                break;
            default:
                return new Rest();
        }

        copy.Bar += overallTick / Definition;
        copy.Tick += overallTick % Definition;
        // Deal with negative shifts
        // Assuming copy.Bar = 2, copy.Tick = 0, overallTick = -192
        // copy.Tick += overallTick % Definition leads copy.Tick to be negative
        if (copy.Tick < 0)
        {
            copy.Bar -= 1;
            copy.Tick += Definition;
        }

        copy.Update();
        return copy;
    }

    public void ShiftByOffset(int overallTick)
    {
        List<Note>? updatedNotes = [];
        foreach (Note? x in Notes)
            if (x.NoteType is not NoteType.BPM || x.NoteGenre is not NoteGenre.MEASURE ||
                (x.NoteType is NoteType.BPM && x.Bar != 0 && x.Tick != 0) ||
                (x.NoteGenre is NoteGenre.MEASURE && x.Bar != 0 && x.Tick != 0))
            {
                updatedNotes.Add(CopyAndShiftNote(x, overallTick));
            } //! This method is not designed with detecting overallTickOverflow!
            else
            {
                Console.WriteLine(x.Compose(ChartVersion.Ma2_104));
                updatedNotes.Add(x);
            }

        Notes = new List<Note>(updatedNotes);
        Update();
    }

    public void ShiftByOffset(int bar, int tick)
    {
        int overallTick = bar * Definition + tick;
        ShiftByOffset(overallTick);
    }

    public void RotateNotes(FlipMethod method)
    {
        foreach (Note? x in Notes) x.Flip(method);
        Update();
    }


    /// <summary>
    ///     Override and compose with given arrays
    /// </summary>
    /// <param name="bpm">Override BPM array</param>
    /// <param name="measure">Override Measure array</param>
    /// <returns>Good Brother with override array</returns>
    public abstract string Compose(BPMChanges bpm, MeasureChanges measure);

    /// <summary>
    ///     Return the least none 0 measure of bar.
    /// </summary>
    /// <param name="bar">bar to take in</param>
    /// <returns>List none 0 measure</returns>
    public static int CalculateLeastMeasure(List<Note> bar, int definition)
    {
        List<int>? startTimeList = [0];
        foreach (Note? x in bar)
        {
            if (!startTimeList.Contains(x.Tick)) startTimeList.Add(x.Tick);
            // if (x.NoteType is NoteType.BPM)
            // {
            //     Console.WriteLine(x.Compose(0));
            // }
        }

        if (startTimeList[startTimeList.Count - 1] != definition) startTimeList.Add(definition);
        List<int>? intervalCandidates = [];
        int minimalInterval = GCD(startTimeList[0], startTimeList[1]);
        for (int i = 1; i < startTimeList.Count; i++) minimalInterval = GCD(minimalInterval, startTimeList[i]);
        return minimalInterval;
    }

    /// <summary>
    ///     Return note number except Rest, BPM and Measure.
    /// </summary>
    /// <param name="Bar">bar of note to take in</param>
    /// <returns>Number</returns>
    public static int RealNoteNumber(List<Note> Bar)
    {
        int result = 0;
        foreach (Note? x in Bar)
            if (x.IsNote)
                result++;
        return result;
    }

    /// <summary>
    ///     Judges if this bar contains notes
    /// </summary>
    /// <param name="Bar">Bar to analyze on</param>
    /// <returns>True if contains, false otherwise</returns>
    public static bool ContainNotes(List<Note> Bar)
    {
        bool result = false;
        foreach (Note? x in Bar) result = result || x.IsNote;
        return result;
    }

    /// <summary>
    ///     Generate appropriate length for hold and slide.
    /// </summary>
    /// <param name="length">Last Time</param>
    /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
    public static int CalculateQuaver(int length, int definition)
    {
        int result = 0;
        int divisor = GCD(definition, length);
        int quaver = definition / divisor, beat = length / divisor;
        result = quaver;
        return result;
    }

    /// <summary>
    ///     Finish Bar writing byu adding specific rest note in between.
    /// </summary>
    /// <param name="bar">Bar to finish with</param>
    /// <param name="barNumber">Bar number of Bar</param>
    /// <param name="minimalQuaver">Minimal interval calculated from bar</param>
    /// <param name="definition">Definition of chart, usually 384</param>
    /// <exception cref="InvalidOperationException">Returns exception if number of notes does not match after modification</exception>
    /// <returns>Finished bar</returns>
    public static List<Note> FinishBar(List<Note> bar, int barNumber, int minimalQuaver, int definition)
    {
        List<Note>? result = [];
        bool writeRest = true;
        result.Add(bar[0]);
        if (minimalQuaver < 0)
        {
            throw new InvalidDataException("NEGATIVE VALUE: minimalQuaver");
        }

        for (int i = 0; i < definition; i += definition / minimalQuaver)
        {
            //Separate Touch and others to prevent ordering issue
            Note bpm = new Rest();
            List<Note>? eachSet = [];
            List<Note>? touchEachSet = [];

            //Set condition to write rest if appropriate
            writeRest = true;
            //Add Appropriate note into each set
            Note lastNote = new Rest();
            foreach (Note? x in bar)
            {
                if (x.Tick == i && x.IsNote && !(x.NoteType is NoteType.TTP or NoteType.THO))
                {
                    if (x.NoteSpecificGenre is NoteSpecificGenre.BPM)
                    {
                        bpm = x;
                    }
                    else
                    {
                        eachSet.Add(x);
                        //Console.WriteLine("A note was found at tick " + i + " of bar " + barNumber + ", it is "+x.NoteType);
                        writeRest = false;
                        x.Prev = lastNote;
                        lastNote.Next = x;
                    }
                }
                else if (x.Tick == i && x is { IsNote: true, NoteType: NoteType.TTP or NoteType.THO })
                {
                    if (x.NoteSpecificGenre is NoteSpecificGenre.BPM)
                    {
                        bpm = x;
                        //Console.WriteLine("A note was found at tick " + i + " of bar " + barNumber + ", it is "+x.NoteType);
                    }
                    else
                    {
                        touchEachSet.Add(x);
                        //Console.WriteLine("A note was found at tick " + i + " of bar " + barNumber + ", it is "+x.NoteType);
                        writeRest = false;
                        x.Prev = lastNote;
                        lastNote.Next = x;
                    }
                }

                if (x.NoteSpecificGenre is not NoteSpecificGenre.BPM &&
                    x.NoteSpecificGenre is not NoteSpecificGenre.SLIDE_START)
                    lastNote = x.NewInstance();
            }

            //Searching for BPM change. If find one, get into front.
            if (bpm.BPM != 0)
            {
                List<Note>? adjusted = [bpm, .. touchEachSet, .. eachSet];
                eachSet = adjusted;
            }
            else
            {
                List<Note>? adjusted = [.. touchEachSet, .. eachSet];
                eachSet = adjusted;
            }

            if (writeRest)
                //Console.WriteLine("There is no note at tick " + i + " of bar " + barNumber + ", Adding one");
                eachSet.Add(new Rest(barNumber, i));
            result.AddRange(eachSet);
        }

        if (RealNoteNumber(result) != RealNoteNumber(bar))
        {
            string? error = "";
            error += "Bar notes not match in bar: " + barNumber + "\n";
            error += "Expected: " + RealNoteNumber(bar) + "\n";
            foreach (Note? x in bar) error += x.Compose(ChartVersion.Debug) + "\n";
            error += "\nActual: " + RealNoteNumber(result) + "\n";
            foreach (Note? y in result) error += y.Compose(ChartVersion.Debug) + "\n";
            Console.WriteLine(error);
            throw new InvalidOperationException($"NOTE NUMBER IS NOT MATCHING\nError Details:\n{error}");
        }

        bool hasFirstBPMChange = false;
        List<Note>? changedResult = [];
        Note potentialFirstChange = new Rest();
        {
            for (int i = 0; !hasFirstBPMChange && i < result.Count(); i++)
                if (result[i].NoteGenre is NoteGenre.BPM && result[i].Tick == 0)
                {
                    changedResult.Add(result[i]);
                    potentialFirstChange = result[i];
                    hasFirstBPMChange = true;
                }

            if (hasFirstBPMChange)
            {
                result.Remove(potentialFirstChange);
                changedResult.AddRange(result);
                result = changedResult;
            }
        }

        return result;
    }

    /// <summary>
    ///     Return GCD of A and B.
    /// </summary>
    /// <param name="a">A</param>
    /// <param name="b">B</param>
    /// <returns>GCD of A and B</returns>
    public static int GCD(int a, int b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    /// <summary>
    ///     Return if this is a prime (1 counts as prime)
    /// </summary>
    /// <param name="number">Number to inspect</param>
    /// <returns>True if is prime, false otherwise</returns>
    public static bool IsPrime(int number)
    {
        if (number < 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        int boundary = (int)Math.Floor(Math.Sqrt(number));

        for (int i = 3; i <= boundary; i += 2)
            if (number % i == 0)
                return false;

        return true;
    }

    /// <summary>
    ///     Take in and replace the current information.
    /// </summary>
    /// <param name="information">Dictionary containing information needed</param>
    public void TakeInformation(Dictionary<string, string> information)
    {
        foreach (KeyValuePair<string, string> x in information) Information.Add(x.Key, x.Value);
    }

    /// <summary>
    ///     Give time stamp given overall tick
    /// </summary>
    /// <param name="overallTick">Note.Bar*384+Note.Tick</param>
    /// <returns>Appropriate time stamp in seconds</returns>
    public double GetTimeStamp(int overallTick)
    {
        double result = 0.0;
        if (overallTick != 0)
        {
            bool foundMax = false;
            int maximumBPMIndex = 0;
            for (int i = 0; i < BPMChanges.ChangeNotes.Count && !foundMax; i++)
                if (BPMChanges.ChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
                else
                    foundMax = true;
            if (maximumBPMIndex == 0)
            {
                result = GetBPMTimeUnit(BPMChanges.ChangeNotes[0].BPM, Definition) * overallTick;
            }
            else
            {
                for (int i = 1; i <= maximumBPMIndex; i++)
                {
                    double previousTickTimeUnit = GetBPMTimeUnit(BPMChanges.ChangeNotes[i - 1].BPM, Definition);
                    result += (BPMChanges.ChangeNotes[i].TickStamp - BPMChanges.ChangeNotes[i - 1].TickStamp) *
                              previousTickTimeUnit;
                }

                double tickTimeUnit = GetBPMTimeUnit(BPMChanges.ChangeNotes[maximumBPMIndex].BPM, Definition);
                result += (overallTick - BPMChanges.ChangeNotes[maximumBPMIndex].TickStamp) * tickTimeUnit;
            }
        }

        return result;
    }

    /// <summary>
    ///     Give time stamp given overall tick
    /// </summary>
    /// <param name="overallTick">Note.Bar*384+Note.Tick</param>
    /// <returns>Appropriate time stamp in seconds</returns>
    public static double GetTimeStamp(BPMChanges bpmChanges, int overallTick, int definition)
    {
        double result = 0.0;
        if (overallTick != 0)
        {
            int maximumBPMIndex = 0;
            for (int i = 0; i < bpmChanges.ChangeNotes.Count; i++)
                if (bpmChanges.ChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
            if (maximumBPMIndex == 0)
            {
                result = GetBPMTimeUnit(bpmChanges.ChangeNotes[0].BPM, definition) * overallTick;
            }
            else
            {
                for (int i = 1; i <= maximumBPMIndex; i++)
                {
                    double previousTickTimeUnit = GetBPMTimeUnit(bpmChanges.ChangeNotes[i - 1].BPM, definition);
                    result += (bpmChanges.ChangeNotes[i].TickStamp - bpmChanges.ChangeNotes[i - 1].TickStamp) *
                              previousTickTimeUnit;
                }

                double tickTimeUnit = GetBPMTimeUnit(bpmChanges.ChangeNotes[maximumBPMIndex].BPM, definition);
                result += (overallTick - bpmChanges.ChangeNotes[maximumBPMIndex].TickStamp) * tickTimeUnit;
            }
        }

        return result;
    }

    /// <summary>
    ///     Return BPM tick unit of given bpm
    /// </summary>
    /// <param name="bpm">BPM to calculate</param>
    /// <returns>Tick Unit of BPM</returns>
    public static double GetBPMTimeUnit(double bpm, int definition) => 60 / bpm * 4 / definition;

    /// <summary>
    ///     For debug use: print out the note's time stamp in given bpm changes
    /// </summary>
    /// <param name="bpmChanges">The list of BPMChanges</param>
    /// <param name="inTake">The Note to test</param>
    /// <returns>String of result, consists tick time stamp, wait time stamp and last time stamp</returns>
    public static string GetNoteDetail(BPMChanges bpmChanges, Note inTake, int definition)
    {
        string? result = "";
        result += inTake.Compose(ChartVersion.Debug) + "\n";
        result += "This is a " + inTake.NoteSpecificGenre + " note,\n";
        result += "This note has overall tick of " + inTake.TickStamp +
                  ", and therefor, the tick time stamp shall be " +
                  GetTimeStamp(bpmChanges, inTake.TickStamp, definition) + "\n";
        if (inTake.NoteGenre is NoteGenre.SLIDE)
        {
            result += "This note has wait length of " + inTake.WaitLength + ", and therefor, its wait tick stamp is " +
                      inTake.WaitTickStamp + " with wait time stamp of " +
                      GetTimeStamp(bpmChanges, inTake.WaitTickStamp, definition) + "\n";
            result += "This note has last length of " + inTake.LastLength + ", and therefor, its last tick stamp is " +
                      inTake.LastTickStamp + " with last time stamp of " +
                      GetTimeStamp(bpmChanges, inTake.LastTickStamp, definition) + "\n";
        }

        return result;
    }

    /// <summary>
    ///     Return the BPM at certain tick
    /// </summary>
    /// <param name="overallTick">Tick to specify</param>
    /// <returns>BPM at that tick</returns>
    public double GetBPMByTick(int overallTick)
    {
        double result = BPMChanges.ChangeNotes[0].BPM;
        if (overallTick > 0)
        {
            int maximumBPMIndex = 0;
            for (int i = 0; i < BPMChanges.ChangeNotes.Count; i++)
                if (BPMChanges.ChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
            result = BPMChanges.ChangeNotes[maximumBPMIndex].BPM;
        }

        return result;
    }

    /// <summary>
    ///     Determine if there are BPM change in between ticks
    /// </summary>
    /// <param name="startTick">Tick to start with</param>
    /// <param name="endTick">Tick to end with</param>
    /// <returns></returns>
    public bool HasBPMChangeInBetween(int startTick, int endTick)
    {
        bool result = false;

        for (int i = 0; i < BPMChanges.ChangeNotes.Count && !result; i++)
            if (BPMChanges.ChangeNotes[i].TickStamp > startTick && BPMChanges.ChangeNotes[i].TickStamp < endTick)
                result = true;

        return result;
    }

    public void ExtractSlideEachGroup()
    {
        List<Note> adjusted = [];
        List<Slide> slideCandidates = [];
        foreach (Note? x in Notes)
        {
            switch (x.NoteSpecificGenre)
            {
                case NoteEnum.NoteSpecificGenre.SLIDE_EACH:
                    SlideEachSet? candidate = x as SlideEachSet ??
                                              throw new InvalidOperationException("THIS IS NOT A SLIDE EACH");
                    if (candidate.SlideStart != null) adjusted.Add(candidate.SlideStart);
                    if (candidate.InternalSlides.Count > 0) slideCandidates.AddRange(candidate.InternalSlides);
                    break;
                case NoteEnum.NoteSpecificGenre.SLIDE_GROUP:
                    x.Update();
                    SlideGroup? groupCandidate = x as SlideGroup ??
                                                 throw new InvalidOperationException("THIS IS NOT A SLIDE GROUP");
                    if (groupCandidate.InternalSlides.Count > 0) adjusted.AddRange(groupCandidate.InternalSlides);
                    break;
                default:
                    adjusted.Add(x);
                    break;
            }
        }

        foreach (Slide? x in slideCandidates)
        {
            switch (x.NoteSpecificGenre)
            {
                case NoteSpecificGenre.SLIDE_GROUP:
                    SlideGroup? groupCandidate = x as SlideGroup ??
                                                 throw new InvalidOperationException("THIS IS NOT A SLIDE GROUP");
                    if (groupCandidate.InternalSlides.Count > 0) adjusted.AddRange(groupCandidate.InternalSlides);
                    break;
                default:
                    adjusted.Add(x);
                    break;
            }
        }

        Notes = adjusted;
    }

    public void ComposeSlideGroup()
    {
        List<Note> adjusted = [];
        List<Slide> connectedSlides = [];
        List<Slide> slideNotesOfChart = [];
        List<Slide> processedSlideOfChart = [];
        Dictionary<Slide, bool> processedSlideDic = [];

        int maximumBar = 0;
        foreach (Note? candidate in Notes)
        {
            maximumBar = candidate.Bar > maximumBar ? candidate.Bar : maximumBar;
            if (candidate.NoteSpecificGenre is NoteSpecificGenre.SLIDE or NoteSpecificGenre.SLIDE_GROUP)
            {
                // Slide slideCandidate = candidate as Slide ?? throw new InvalidCastException("Candidate is not a SLIDE. It is: "+candidate.Compose(ChartVersion.Debug));
                // slideCandidate.NoteSpecialState = candidate.NoteSpecialState;
                slideNotesOfChart.Add((Slide)candidate);
                processedSlideDic.Add((Slide)candidate, false);
            }
            else adjusted.Add(candidate);
        }

        // If this chart only have one slide, it cannot be connecting slide; otherwise this chart is invalid.

        int processedSlidesCount = 0;

        foreach (KeyValuePair<Slide, bool> parentPair in processedSlideDic)
        {
            Slide? parentSlide = parentPair.Key;
            maximumBar = parentSlide.Bar > maximumBar ? parentSlide.Bar : maximumBar;
            if (!parentPair.Value && parentSlide.NoteSpecialState != SpecialState.ConnectingSlide)
            {
                SlideGroup currentGroup = new();
                currentGroup.AddConnectingSlide(parentSlide);
                foreach (KeyValuePair<Slide, bool> candidatePair in processedSlideDic)
                {
                    Slide? candidate = candidatePair.Key;
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
                    errorMsg += "This slide is also a Slide Group with last slide as " +
                                (x as SlideGroup ?? throw new NullReferenceException(
                                    "This note cannot be casted to SlideGroup: " + x.Compose(ChartVersion.Debug)))
                                .LastSlide.Compose(ChartVersion.Debug) + "\n";
                }
            }

            throw new InvalidOperationException("SLIDE NUMBER MISMATCH - Expected: " + slideNotesOfChart.Count +
                                                ", Actual:" + processedSlidesCount + ", Skipped: " +
                                                processedSlideDic.Count(p => !p.Value) + "\n" + errorMsg);
        }

        Notes = new List<Note>(adjusted);
    }

    public void ComposeSlideEachGroup()
    {
        List<SlideEachSet> composedCandidates = [];
        List<Note> adjusted = [];
        int processedNotes = 0;
        foreach (Note? x in Notes)
        {
            bool eachCandidateCombined = false;
            if (!(x.NoteSpecificGenre is NoteSpecificGenre.SLIDE or NoteSpecificGenre.SLIDE_START
                    or NoteSpecificGenre.SLIDE_GROUP))
            {
                adjusted.Add(x);
                processedNotes++;
            }
            else if (composedCandidates.Count > 0 && x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START)
            {
                foreach (SlideEachSet? parent in composedCandidates)
                {
                    Tap? slideStartCandidate =
                        x as Tap ?? throw new InvalidOperationException("THIS IS NOT A SLIDE START");
                    eachCandidateCombined = eachCandidateCombined || parent.TryAddCandidateNote(slideStartCandidate);
                    if (eachCandidateCombined) processedNotes++;
                }
            }
            else if (composedCandidates.Count > 0 &&
                     x.NoteSpecificGenre is NoteSpecificGenre.SLIDE or NoteSpecificGenre.SLIDE_GROUP)
            {
                foreach (SlideEachSet? parent in composedCandidates)
                {
                    Slide? slideStartCandidate =
                        x as Slide ?? throw new InvalidOperationException("THIS IS NOT A SLIDE");
                    eachCandidateCombined = eachCandidateCombined || parent.TryAddCandidateNote(slideStartCandidate);
                    if (eachCandidateCombined) processedNotes++;
                }
            }

            if (!eachCandidateCombined && x.NoteSpecificGenre is NoteSpecificGenre.SLIDE
                    or NoteSpecificGenre.SLIDE_START or NoteSpecificGenre.SLIDE_GROUP)
            {
                composedCandidates.Add(new SlideEachSet(x));
                processedNotes++;
            }
        }

        // if (processedNotes != this.Notes.Count) throw new InvalidOperationException("PROCESSED NOTES MISMATCH: Expected "+this.Notes.Count+", Actual "+processedNotes);
        adjusted.AddRange(composedCandidates);
        Notes = adjusted;
    }
}