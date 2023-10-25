namespace MaiLib;
using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;

/// <summary>
///     A class holding notes and information to form a chart
/// </summary>
public abstract class Chart : IChart
{
    //Theoretical Rating = (Difference in 100-down and Max score)/100-down

    /// <summary>
    ///     Empty constructor
    /// </summary>
    public Chart()
    {
        Notes = new List<Note>();
        BPMChanges = new BPMChanges();
        MeasureChanges = new MeasureChanges();
        StoredChart = new List<List<Note>>();
        Information = new Dictionary<string, string>();
        IsDxChart = false;
        Definition = 384;
        UnitScore = new[] { 500, 1000, 1500, 2000, 2500 };
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Ma2_103;
    }

    /// <summary>
    /// Defines the chart type by enums
    /// </summary>
    public ChartType ChartType { get; protected set; }

    /// <summary>
    /// Defines the chart version by enums
    /// </summary>
    public ChartVersion ChartVersion { get; protected set; }

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
    public MeasureChanges MeasureChanges { get; set; }

    /// <summary>
    ///     Stores total note number
    /// </summary>
    public int TotalNoteNumber { get; set; }

    /// <summary>
    ///     Counts number of Tap
    /// </summary>
    public int TapNumber { get; set; }

    /// <summary>
    ///     Counts number of Break
    /// </summary>
    public int BreakNumber { get; set; }

    /// <summary>
    ///     Counts number of Hold
    /// </summary>
    public int HoldNumber { get; set; }

    /// <summary>
    ///     Counts number of Slide
    /// </summary>
    public int SlideNumber { get; set; }

    /// <summary>
    ///     Counts number of Touch
    /// </summary>
    public int TouchNumber { get; set; }

    /// <summary>
    ///     Counts number of Touch Hold
    /// </summary>
    public int ThoNumber { get; set; }

    /// <summary>
    ///     Defines if the chart is DX chart
    /// </summary>
    public bool IsDxChart { get; set; }

    /// <summary>
    ///     The first note of the chart
    /// </summary>
    public Note? FirstNote { get; set; }

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
    public List<List<Note>> StoredChart { get; set; }

    /// <summary>
    ///     Stored the information of this chart, if any
    /// </summary>
    public Dictionary<string, string> Information { get; set; }

    public abstract bool CheckValidity();

    /// <summary>
    ///     Update properties in Good Brother for exporting
    /// </summary>
    public virtual void Update()
    {
        StoredChart = new List<List<Note>>();
        var maxBar = 0;
        var timeStamp = 0.0;
        if (Notes.Count > 0)
            foreach (var x in Notes)
                if (x.Bar > maxBar)
                    maxBar = x.Bar;

        //Iterate over bar
        for (var i = 0; i <= maxBar; i++)
        {
            var bar = new List<Note>();
            var noteChange = new BPMChange();
            var currentBPM = BPMChanges.ChangeNotes[0].BPM;
            Note lastNote = new Rest();
            Note realLastNote = new Rest();
            foreach (var x in BPMChanges.ChangeNotes)
                if (x.Bar == i)
                    bar.Add(x); //Extract the first BPM change in bar to the beginning of the bar
            foreach (var x in Notes)
            {
                if (FirstNote == null && !(x.NoteType is NoteType.BPM || x.NoteType is NoteType.MEASURE)) FirstNote = x;
                // Console.WriteLine(x.Compose(0));
                //x.BPMChangeNotes = this.bpmChanges.ChangeNotes;
                //x.Update();
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
                    var delay = x.Bar * Definition + x.Tick + x.WaitLength + x.LastLength;
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
                            TapNumber++;
                            if (x.NoteSpecialState is SpecialState.EX) IsDxChart = false;
                            if (x.NoteType.Equals("TTP"))
                            {
                                TouchNumber++;
                                IsDxChart = false;
                            }
                            else if (x.NoteType.Equals("BRK") || x.NoteType.Equals("BST"))
                            {
                                BreakNumber++;
                            }

                            break;
                        case NoteSpecificGenre.HOLD:
                            HoldNumber++;
                            x.TickBPMDisagree = GetBPMByTick(x.TickStamp) != GetBPMByTick(x.LastTickStamp) ||
                                                HasBPMChangeInBetween(x.TickStamp, x.LastTickStamp);
                            x.Update();
                            if (x.TickTimeStamp == 0) x.TickTimeStamp = GetTimeStamp(x.TickStamp);
                            if (x.CalculatedLastTime == 0)
                            {
                                x.LastTimeStamp = GetTimeStamp(x.LastTickStamp);
                                x.CalculatedLastTime = x.LastTimeStamp - x.TickTimeStamp;
                                x.FixedLastLength =
                                    (int)(x.CalculatedLastTime / GetBPMTimeUnit(GetBPMByTick(x.TickStamp)));
                            }

                            if (delay > TotalDelay) TotalDelay = delay;
                            //Console.WriteLine("New delay: " + delay);
                            //Console.WriteLine(x.Compose(1));
                            if (x.NoteType.Equals("THO"))
                            {
                                ThoNumber++;
                                IsDxChart = false;
                            }

                            break;
                        case NoteSpecificGenre.SLIDE_START:
                            TapNumber++;
                            break;
                        case NoteSpecificGenre.SLIDE:
                            SlideNumber++;
                            x.TickBPMDisagree = GetBPMByTick(x.TickStamp) != GetBPMByTick(x.WaitTickStamp) ||
                                                GetBPMByTick(x.WaitTickStamp) != GetBPMByTick(x.LastTickStamp) ||
                                                GetBPMByTick(x.TickStamp) != GetBPMByTick(x.LastTickStamp) ||
                                                HasBPMChangeInBetween(x.TickStamp, x.WaitTickStamp);
                            x.Update();
                            if (x.TickTimeStamp == 0) x.TickTimeStamp = GetTimeStamp(x.TickStamp);
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
                                    (int)(x.CalculatedLastTime / GetBPMTimeUnit(GetBPMByTick(x.TickStamp)));
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

                    x.BPM = currentBPM;
                    //if (x.NoteGenre is NoteGenre.SLIDE && !lastNote.NoteSpecificType.Equals("SLIDE_START"))
                    //{
                    //    x.Prev = new Tap("NST", x.Bar, x.Tick, x.Key);
                    //    lastNote.Next = x.Prev;
                    //}
                    //else
                    // // lastNote.Next = x;
                    // // x.Prev = lastNote;
                    // // x.Prev.Next = x;
                    //if ((!x.NoteGenre is NoteGenre.SLIDE) && x.Prev.NoteType.Equals("STR")&&x.Prev.ConsecutiveSlide == null)
                    //{
                    //    Console.WriteLine("Found NSS");
                    //    Console.WriteLine("This note's note type: " + x.NoteType);
                    //    Console.WriteLine(x.Compose(1));
                    //    Console.WriteLine("Prev note's note type: " + x.Prev.NoteType);
                    //    Console.WriteLine(x.Prev.Compose(1));
                    //    lastNote.NoteType = "NSS";
                    //    x.Prev.NoteType = "NSS";
                    //}
                    bar.Add(x);
                    if (x.NoteGenre is not NoteGenre.SLIDE) lastNote = x;
                    realLastNote = x;
                    timeStamp += x.TickTimeStamp;
                }
            }

            var afterBar = new List<Note>();
            afterBar.Add(new MeasureChange(i, 0, CalculateQuaver(CalculateLeastMeasure(bar))));
            //Console.WriteLine();
            //Console.WriteLine("In bar "+i+", LeastMeasure is "+ CalculateLeastMeasure(bar)+", so quaver will be "+ CalculateQuaver(CalculateLeastMeasure(bar)));
            afterBar.AddRange(bar);
            StoredChart.Add(FinishBar(afterBar, BPMChanges.ChangeNotes, i,
                CalculateQuaver(CalculateLeastMeasure(bar))));
        }

        //Console.WriteLine("TOTAL DELAY: "+this.TotalDelay);
        //Console.WriteLine("TOTAL COUNT: "+ this.chart.Count * 384);
        if (TotalDelay < StoredChart.Count * Definition)
            TotalDelay = 0;
        else
            TotalDelay -= StoredChart.Count * Definition;
        TotalNoteNumber += TapNumber + HoldNumber + SlideNumber;
    }

    /// <summary>
    ///     Compose chart in appropriate result.
    /// </summary>
    /// <returns>String of chart compiled</returns>
    public abstract string Compose();

    public double GetTimeStamp(int bar, int tick)
    {
        var result = 0.0;
        var overallTick = bar * 384 + tick;
        if (overallTick != 0)
        {
            var maximumBPMIndex = 0;
            for (var i = 0; i < BPMChanges.ChangeNotes.Count; i++)
                if (BPMChanges.ChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
            if (maximumBPMIndex == 0)
            {
                result = 60 / BPMChanges.ChangeNotes[0].BPM * 4 / 384;
            }
            else
            {
                for (var i = 1; i <= maximumBPMIndex; i++)
                {
                    var previousTickTimeUnit = 60 / BPMChanges.ChangeNotes[i - 1].BPM * 4 / 384;
                    result += (BPMChanges.ChangeNotes[i].TickStamp - BPMChanges.ChangeNotes[i - 1].TickStamp) *
                              previousTickTimeUnit;
                }

                var tickTimeUnit = 60 / BPMChanges.ChangeNotes[maximumBPMIndex].BPM * 4 / 384;
                result += (overallTick - BPMChanges.ChangeNotes[maximumBPMIndex].TickStamp) * tickTimeUnit;
            }
        }

        return result;
    }

    public void ShiftByOffset(int overallTick)
    {
        var updatedNotes = new List<Note>();
        foreach (var x in Notes)
            if (x.NoteType is not NoteType.BPM || x.NoteGenre is not NoteGenre.MEASURE ||
                (x.NoteType is NoteType.BPM && x.Bar != 0 && x.Tick != 0) ||
                (x.NoteGenre is NoteGenre.MEASURE && x.Bar != 0 && x.Tick != 0))
            {
                Note copy;
                switch (x.NoteGenre)
                {
                    case NoteGenre.TAP:
                        copy = new Tap(x);
                        copy.Bar += overallTick / 384;
                        copy.Tick += overallTick % 384;
                        copy.Update();
                        break;
                    case NoteGenre.HOLD:
                        copy = new Hold(x);
                        copy.Bar += overallTick / 384;
                        copy.Tick += overallTick % 384;
                        copy.Update();
                        break;
                    case NoteGenre.SLIDE:
                        copy = new Slide(x);
                        copy.Bar += overallTick / 384;
                        copy.Tick += overallTick % 384;
                        copy.Update();
                        break;
                    case NoteGenre.BPM:
                        copy = new BPMChange(x);
                        copy.Bar += overallTick / 384;
                        copy.Tick += overallTick % 384;
                        copy.Update();
                        break;
                    case NoteGenre.MEASURE:
                        copy = new MeasureChange((MeasureChange)x);
                        copy.Bar += overallTick / 384;
                        copy.Tick += overallTick % 384;
                        copy.Update();
                        break;
                    default:
                        copy = new Rest();
                        break;
                }

                updatedNotes.Add(copy);
            } //! This method is not designed with detecting overallTickOverflow!
            else
            {
                updatedNotes.Add(x);
            }

        Notes = new List<Note>(updatedNotes);
        Update();
    }

    public void ShiftByOffset(int bar, int tick)
    {
        var overallTick = bar * 384 + tick;
        ShiftByOffset(overallTick);
    }

    public void RotateNotes(FlipMethod method)
    {
        foreach (var x in Notes) x.Flip(method);
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
    public static int CalculateLeastMeasure(List<Note> bar)
    {
        var startTimeList = new List<int>();
        startTimeList.Add(0);
        foreach (var x in bar)
        {
            if (!startTimeList.Contains(x.Tick)) startTimeList.Add(x.Tick);
            if (x.NoteType is NoteType.BPM)
            {
                //Console.WriteLine(x.Compose(0));
            }
        }

        if (startTimeList[startTimeList.Count - 1] != 384) startTimeList.Add(384);
        var intervalCandidates = new List<int>();
        var minimalInterval = GCD(startTimeList[0], startTimeList[1]);
        for (var i = 1; i < startTimeList.Count; i++) minimalInterval = GCD(minimalInterval, startTimeList[i]);
        return minimalInterval;
    }

    /// <summary>
    ///     Return note number except Rest, BPM and Measure.
    /// </summary>
    /// <param name="Bar">bar of note to take in</param>
    /// <returns>Number</returns>
    public static int RealNoteNumber(List<Note> Bar)
    {
        var result = 0;
        foreach (var x in Bar)
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
        var result = false;
        foreach (var x in Bar) result = result || x.IsNote;
        return result;
    }

    /// <summary>
    ///     Generate appropriate length for hold and slide.
    /// </summary>
    /// <param name="length">Last Time</param>
    /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
    public static int CalculateQuaver(int length)
    {
        var result = 0;
        const int definition = 384;
        var divisor = GCD(definition, length);
        int quaver = definition / divisor, beat = length / divisor;
        result = quaver;
        return result;
    }

    /// <summary>
    ///     Finish Bar writing byu adding specific rest note in between.
    /// </summary>
    /// <param name="bar">Bar to finish with</param>
    /// <param name="bpmChanges">BPMChange Notes</param>
    /// <param name="barNumber">Bar number of Bar</param>
    /// <param name="minimalQuaver">Minimal interval calculated from bar</param>
    /// <returns>Finished bar</returns>
    public static List<Note> FinishBar(List<Note> bar, List<BPMChange> bpmChanges, int barNumber, int minimalQuaver)
    {
        var result = new List<Note>();
        var writeRest = true;
        result.Add(bar[0]);
        for (var i = 0; i < 384; i += 384 / minimalQuaver)
        {
            //Separate Touch and others to prevent ordering issue
            Note bpm = new Rest();
            var eachSet = new List<Note>();
            var touchEachSet = new List<Note>();

            //Set condition to write rest if appropriate
            writeRest = true;
            //Add Appropriate note into each set
            Note lastNote = new Rest();
            foreach (var x in bar)
            {
                if (x.Tick == i && x.IsNote && !(x.NoteType.Equals("TTP") || x.NoteType.Equals("THO")))
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
                else if (x.Tick == i && x.IsNote && (x.NoteType.Equals("TTP") || x.NoteType.Equals("THO")))
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

                if (x.NoteSpecificGenre is not NoteSpecificGenre.BPM && x.NoteSpecificGenre is not NoteSpecificGenre.SLIDE_START)
                    lastNote = x.NewInstance();
            }

            //Searching for BPM change. If find one, get into front.
            if (bpm.BPM != 0)
            {
                var adjusted = new List<Note>();
                adjusted.Add(bpm);
                adjusted.AddRange(touchEachSet);
                adjusted.AddRange(eachSet);
                eachSet = adjusted;
            }
            else
            {
                var adjusted = new List<Note>();
                adjusted.AddRange(touchEachSet);
                adjusted.AddRange(eachSet);
                eachSet = adjusted;
            }

            if (writeRest)
                //Console.WriteLine("There is no note at tick " + i + " of bar " + barNumber + ", Adding one");
                eachSet.Add(new Rest(barNumber, i));
            result.AddRange(eachSet);
        }

        if (RealNoteNumber(result) != RealNoteNumber(bar))
        {
            var error = "";
            error += "Bar notes not match in bar: " + barNumber + "\n";
            error += "Expected: " + RealNoteNumber(bar) + "\n";
            foreach (var x in bar) error += x.Compose(ChartVersion.Debug) + "\n";
            error += "\nActual: " + RealNoteNumber(result) + "\n";
            foreach (var y in result) error += y.Compose(ChartVersion.Debug) + "\n";
            Console.WriteLine(error);
            throw new Exception("NOTE NUMBER IS NOT MATCHING");
        }

        var hasFirstBPMChange = false;
        var changedResult = new List<Note>();
        Note potentialFirstChange = new Rest();
        {
            for (var i = 0; !hasFirstBPMChange && i < result.Count(); i++)
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
    ///     Return if this is a prime (1 counts)
    /// </summary>
    /// <param name="number">Number to inspect</param>
    /// <returns>True if is prime, false otherwise</returns>
    public static bool IsPrime(int number)
    {
        if (number < 1) return false;
        if (number == 2) return true;
        if (number % 2 == 0) return false;

        var boundary = (int)Math.Floor(Math.Sqrt(number));

        for (var i = 3; i <= boundary; i += 2)
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
        foreach (var x in information) Information.Add(x.Key, x.Value);
    }

    /// <summary>
    ///     Give time stamp given overall tick
    /// </summary>
    /// <param name="overallTick">Note.Bar*384+Note.Tick</param>
    /// <returns>Appropriate time stamp in seconds</returns>
    public double GetTimeStamp(int overallTick)
    {
        var result = 0.0;
        if (overallTick != 0)
        {
            var foundMax = false;
            var maximumBPMIndex = 0;
            for (var i = 0; i < BPMChanges.ChangeNotes.Count && !foundMax; i++)
                if (BPMChanges.ChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
                else
                    foundMax = true;
            if (maximumBPMIndex == 0)
            {
                result = GetBPMTimeUnit(BPMChanges.ChangeNotes[0].BPM) * overallTick;
            }
            else
            {
                for (var i = 1; i <= maximumBPMIndex; i++)
                {
                    var previousTickTimeUnit = GetBPMTimeUnit(BPMChanges.ChangeNotes[i - 1].BPM);
                    result += (BPMChanges.ChangeNotes[i].TickStamp - BPMChanges.ChangeNotes[i - 1].TickStamp) *
                              previousTickTimeUnit;
                }

                var tickTimeUnit = GetBPMTimeUnit(BPMChanges.ChangeNotes[maximumBPMIndex].BPM);
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
    public static double GetTimeStamp(BPMChanges bpmChanges, int overallTick)
    {
        var result = 0.0;
        if (overallTick != 0)
        {
            var maximumBPMIndex = 0;
            for (var i = 0; i < bpmChanges.ChangeNotes.Count; i++)
                if (bpmChanges.ChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
            if (maximumBPMIndex == 0)
            {
                result = GetBPMTimeUnit(bpmChanges.ChangeNotes[0].BPM) * overallTick;
            }
            else
            {
                for (var i = 1; i <= maximumBPMIndex; i++)
                {
                    var previousTickTimeUnit = GetBPMTimeUnit(bpmChanges.ChangeNotes[i - 1].BPM);
                    result += (bpmChanges.ChangeNotes[i].TickStamp - bpmChanges.ChangeNotes[i - 1].TickStamp) *
                              previousTickTimeUnit;
                }

                var tickTimeUnit = GetBPMTimeUnit(bpmChanges.ChangeNotes[maximumBPMIndex].BPM);
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
    public static double GetBPMTimeUnit(double bpm)
    {
        var result = 60 / bpm * 4 / 384;
        return result;
    }

    /// <summary>
    ///     For debug use: print out the note's time stamp in given bpm changes
    /// </summary>
    /// <param name="bpmChanges">The list of BPMChanges</param>
    /// <param name="inTake">The Note to test</param>
    /// <returns>String of result, consists tick time stamp, wait time stamp and last time stamp</returns>
    public static string GetNoteDetail(BPMChanges bpmChanges, Note inTake)
    {
        var result = "";
        result += inTake.Compose(ChartVersion.Debug) + "\n";
        result += "This is a " + inTake.NoteSpecificGenre + " note,\n";
        result += "This note has overall tick of " + inTake.TickStamp +
                  ", and therefor, the tick time stamp shall be " + GetTimeStamp(bpmChanges, inTake.TickStamp) + "\n";
        if (inTake.NoteGenre is NoteGenre.SLIDE)
        {
            result += "This note has wait length of " + inTake.WaitLength + ", and therefor, its wait tick stamp is " +
                      inTake.WaitTickStamp + " with wait time stamp of " +
                      GetTimeStamp(bpmChanges, inTake.WaitTickStamp) + "\n";
            result += "This note has last length of " + inTake.LastLength + ", and therefor, its last tick stamp is " +
                      inTake.LastTickStamp + " with last time stamp of " +
                      GetTimeStamp(bpmChanges, inTake.LastTickStamp) + "\n";
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
        var result = BPMChanges.ChangeNotes[0].BPM;
        if (overallTick > 0)
        {
            var maximumBPMIndex = 0;
            for (var i = 0; i < BPMChanges.ChangeNotes.Count; i++)
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
        var result = false;

        for (var i = 0; i < BPMChanges.ChangeNotes.Count && !result; i++)
            if (BPMChanges.ChangeNotes[i].TickStamp > startTick && BPMChanges.ChangeNotes[i].TickStamp < endTick)
                result = true;

        return result;
    }
}
