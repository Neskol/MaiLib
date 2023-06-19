using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Xml;

namespace MaiLib
{

    /// <summary>
    /// A class holding notes and information to form a chart
    /// </summary>
    public abstract class Chart : IChart
    {
        //Stores all notes
        private List<Note> notes;

        //Stores definitions of BPM Changes
        private BPMChanges bpmChanges;

        //Stores definitions of Measure Changes
        private MeasureChanges measureChanges;

        private int totalNoteNumber;

        //Counts number of Tap
        private int tapNumber;

        //Counts number of Break
        private int breakNumber;

        //Counts number of Hold
        private int holdNumber;

        //Counts number of Slide
        private int slideNumber;

        //Counts number of Touch
        private int touchNumber;

        //Counts number of Touch Hold
        private int thoNumber;

        //Defines if the chart is DX chart
        private bool isDxChart;

        /// <summary>
        /// The first note of the chart
        /// </summary>
        private Note? firstNote;

        /// <summary>
        /// The definition of this chart
        /// </summary>
        private int definition = 384;

        //Defines 
        /// <summary>
        /// The basic unit of maimai chart
        /// </summary>
        /// <value>Unit score</value>
        private int[] unitScore = { 500, 1000, 1500, 2500 };

        /// <summary>
        /// The total achievement of this chart; to be updated
        /// </summary>
        private int achievement = 0;

        /// <summary>
        /// The total delay of holds behind final note; to be updated
        /// </summary>
        private int totalDelay = 0;

        /// <summary>
        /// Stored chart in structure of bar of notes
        /// </summary>
        private List<List<Note>> chart;

        /// <summary>
        /// Stored the information of this chart, if any
        /// </summary>
        private Dictionary<string, string> information;

        /// <summary>
        /// Allowed TAP notes
        /// </summary>
        /// <value>TAP defs</value>
        private readonly string[] TapTypes = { "TAP", "STR", "TTP", "XTP", "XST" };

        /// <summary>
        /// Allowed HOLD notes
        /// </summary>
        /// <value>HOLD defs</value>
        private readonly string[] HoldTypes = { "HLD", "THO", "XHO" };

        /// <summary>
        /// Allowed SLIDE notes
        /// </summary>
        /// <value>SLIDE defs</value>
        private readonly string[] SlideTypes = { "SI_", "SV_", "SF_", "SCL", "SCR", "SUL", "SUR", "SLL", "SLR", "SXL", "SXR", "SSL", "SSR" };

        //Theoretical Rating = (Difference in 100-down and Max score)/100-down

        /// <summary>
        /// Access to Notes
        /// </summary>
        public List<Note> Notes
        {
            get
            {
                return this.notes;
            }
            set
            {
                this.notes = value;
            }
        }

        /// <summary>
        /// Returns this.Chart. aka List of bars
        /// </summary>
        /// <value>this.Chart</value>
        public List<List<Note>> StoredChart
        {
            get
            {
                return this.chart;
            }
            set
            {
                this.chart = value;
            }
        }

        /// <summary>
        /// Access to BPM Changes
        /// </summary>
        public BPMChanges BPMChanges
        {
            get
            {
                return this.bpmChanges;
            }
            set
            {
                this.bpmChanges = value;
            }
        }

        /// <summary>
        /// Access to Measure Changes
        /// </summary>
        public MeasureChanges MeasureChanges
        {
            get
            {
                return this.measureChanges;
            }
            set
            {
                this.measureChanges = value;
            }
        }

        /// <summary>
        /// Access to Tap Number
        /// </summary>
        public int TapNumber
        {
            get
            {
                return this.tapNumber;
            }
            set
            {
                this.tapNumber = value;
            }
        }

        /// <summary>
        /// Access to Break Number
        /// </summary>
        public int BreakNumber
        {
            get
            {
                return this.breakNumber;
            }
            set
            {
                this.breakNumber = value;
            }
        }

        /// <summary>
        /// Access to Hold Number
        /// </summary>
        public int HoldNumber
        {
            get
            {
                return this.holdNumber;
            }
            set
            {
                this.holdNumber = value;
            }
        }

        /// <summary>
        /// Access to Slide Number
        /// </summary>
        public int SlideNumber
        {
            get
            {
                return this.slideNumber;
            }
            set
            {
                this.slideNumber = value;
            }
        }

        /// <summary>
        /// Access to Touch Number
        /// </summary>
        public int TouchNumber
        {
            get
            {
                return this.touchNumber;
            }
            set
            {
                this.touchNumber = value;
            }
        }

        /// <summary>
        /// Access to Touch Hold Number
        /// </summary>
        public int ThoNumber
        {
            get
            {
                return this.thoNumber;
            }
            set
            {
                this.thoNumber = value;
            }
        }

        /// <summary>
        /// Access to the total note number
        /// </summary>
        public int TotalNoteNumber
        {
            get { return this.totalNoteNumber; }
        }

        /// <summary>
        /// Access to Unit Score
        /// </summary>
        public int[] UnitScore
        {
            get
            {
                return this.unitScore;
            }
        }

        /// <summary>
        /// Access to theoretical Achievement
        /// </summary>
        public int Achievement
        {
            get
            {
                return this.achievement;
            }
            set
            {
                this.achievement = value;
            }
        }

        /// <summary>
        /// Return the total delayed value of this Chart.
        /// </summary>
        /// <value>this.TotalDelayedValue</value>
        public int TotalDelay
        {
            get
            {
                return this.totalDelay;
            }
            set
            {
                this.totalDelay = value;
            }
        }

        /// <summary>
        /// Return Information
        /// </summary>
        /// <value>this.Information</value>
        public Dictionary<string, string> Information
        {
            get
            {
                return this.information;
            }
            set
            {
                this.information = value;
            }
        }

        public bool IsDXChart
        {
            get => this.isDxChart;
            set => this.isDxChart = value;
        }

        public Note? FirstNote
        {
            get => this.firstNote;
            set => this.firstNote = value;
        }

        /// <summary>
        /// Access the definition of this chart, default by 384
        /// </summary>
        /// <value>this.chartDefinition</value>
        public int Definition
        {get; set; }

    /// <summary>
    /// Empty constructor
    /// </summary>
    public Chart()
        {
            this.notes = new List<Note>();
            this.bpmChanges = new BPMChanges();
            this.measureChanges = new MeasureChanges();
            this.chart = new List<List<Note>>();
            this.information = new Dictionary<string, string>();
            this.isDxChart = false;
            this.definition = 384;
        }

        public abstract bool CheckValidity();

        /// <summary>
        /// Update properties in Good Brother for exporting
        /// </summary>
        public virtual void Update()
        {
            this.StoredChart = new List<List<Note>>();
            int maxBar = 0;
            double timeStamp = 0.0;
            if (notes.Count > 0) foreach (Note x in this.Notes)
            {
                if (x.Bar>maxBar) maxBar = x.Bar;
            }

            //Iterate over bar
            for (int i = 0; i <= maxBar; i++)
            {
                List<Note> bar = new List<Note>();
                BPMChange noteChange = new BPMChange();
                double currentBPM = this.BPMChanges.ChangeNotes[0].BPM;
                Note lastNote = new Rest();
                Note realLastNote = new Rest();
                foreach (BPMChange x in this.BPMChanges.ChangeNotes)
                {
                    if (x.Bar == i)
                    {
                        bar.Add(x); //Extract the first BPM change in bar to the beginning of the bar
                    }
                }
                foreach (Note x in this.Notes)
                {
                    if (this.FirstNote == null && !(x.NoteType.Equals("BPM") || x.NoteType.Equals("MEASURE")))
                    {
                        this.FirstNote = x;
                        // Console.WriteLine(x.Compose(0));
                    }
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
                        int delay = x.Bar * definition + x.Tick + x.WaitLength + x.LastLength;
                        switch (x.NoteSpecificGenre)
                        {
                            case "BPM":
                                currentBPM = x.BPM;
                                break;
                            case "MEASURE":
                                break;
                            case "REST":
                                break;
                            case "TAP":
                                this.tapNumber++;
                                if (x.NoteSpecificGenre.Equals("XTP"))
                                {
                                    this.isDxChart = false;
                                }
                                if (x.NoteType.Equals("TTP"))
                                {
                                    this.touchNumber++;
                                    this.isDxChart = false;
                                }
                                else if (x.NoteType.Equals("BRK") || x.NoteType.Equals("BST"))
                                {
                                    this.breakNumber++;
                                }
                                break;
                            case "HOLD":
                                this.holdNumber++;
                                x.TickBPMDisagree = (GetBPMByTick(x.TickStamp) != GetBPMByTick(x.LastTickStamp) || HasBPMChangeInBetween(x.TickStamp, x.LastTickStamp));
                                x.Update();
                                if (x.TickTimeStamp == 0)
                                {
                                    x.TickTimeStamp = this.GetTimeStamp(x.TickStamp);
                                }
                                if (x.CalculatedLastTime == 0)
                                {
                                    x.LastTimeStamp = this.GetTimeStamp(x.LastTickStamp);
                                    x.CalculatedLastTime = x.LastTimeStamp - x.TickTimeStamp;
                                    x.FixedLastLength = (int)(x.CalculatedLastTime / GetBPMTimeUnit(GetBPMByTick(x.TickStamp)));
                                }
                                if (delay > this.TotalDelay)
                                {
                                    this.totalDelay = delay;
                                    //Console.WriteLine("New delay: " + delay);
                                    //Console.WriteLine(x.Compose(1));
                                }
                                if (x.NoteType.Equals("THO"))
                                {
                                    this.thoNumber++;
                                    this.isDxChart = false;
                                }
                                break;
                            case "SLIDE_START":
                                this.tapNumber++;
                                break;
                            case "SLIDE":
                                this.slideNumber++;
                                x.TickBPMDisagree = (GetBPMByTick(x.TickStamp) != GetBPMByTick(x.WaitTickStamp) ||
                                    GetBPMByTick(x.WaitTickStamp) != GetBPMByTick(x.LastTickStamp) ||
                                    GetBPMByTick(x.TickStamp) != GetBPMByTick(x.LastTickStamp) ||
                                    HasBPMChangeInBetween(x.TickStamp, x.WaitTickStamp));
                                x.Update();
                                if (x.TickTimeStamp == 0)
                                {
                                    x.TickTimeStamp = this.GetTimeStamp(x.TickStamp);
                                }
                                if (x.CalculatedWaitTime == 0)
                                {
                                    x.WaitTimeStamp = this.GetTimeStamp(x.WaitTickStamp);
                                    x.CalculatedWaitTime = x.WaitTimeStamp - x.TickTimeStamp;
                                }
                                if (x.CalculatedLastTime == 0)
                                {
                                    x.LastTimeStamp = this.GetTimeStamp(x.LastTickStamp);
                                    x.CalculatedLastTime = x.LastTimeStamp - x.TickTimeStamp;
                                    x.FixedLastLength = (int)(x.CalculatedLastTime / GetBPMTimeUnit(GetBPMByTick(x.TickStamp)));
                                }
                                // if (lastNote.NoteSpecificType.Equals("SLIDE_START") && (lastNote.Bar == x.Bar && lastNote.Tick == x.Tick && lastNote.Key.Equals(x.Key)))
                                // {
                                //     x.SlideStart = lastNote;
                                //     lastNote.ConsecutiveSlide = x;
                                // }
                                if (delay > this.TotalDelay)
                                {
                                    this.totalDelay = delay;
                                    //Console.WriteLine("New delay: "+delay);
                                    //Console.WriteLine(x.Compose(1));
                                }
                                // if (x.SlideStart == null)
                                // {
                                //     Console.WriteLine("A SLIDE WITHOUT START WAS FOUND");
                                //     Console.WriteLine(x.Compose(1));
                                //     Console.WriteLine("This slide has start: " + (x.SlideStart == null));
                                //     throw new NullReferenceException("A SLIDE WITHOUT START WAS FOUND");
                                // }
                                break;
                            default:
                                break;
                        }
                        x.BPM = currentBPM;
                        //if (x.NoteGenre.Equals("SLIDE") && !lastNote.NoteSpecificType.Equals("SLIDE_START"))
                        //{
                        //    x.Prev = new Tap("NST", x.Bar, x.Tick, x.Key);
                        //    lastNote.Next = x.Prev;
                        //}
                        //else
                        // // lastNote.Next = x;
                        // // x.Prev = lastNote;
                        // // x.Prev.Next = x;
                        //if ((!x.NoteGenre.Equals("SLIDE")) && x.Prev.NoteType.Equals("STR")&&x.Prev.ConsecutiveSlide == null)
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
                        if (!x.NoteGenre.Equals("SLIDE"))
                        {
                            lastNote = x;
                        }
                        realLastNote = x;
                        timeStamp += x.TickTimeStamp;
                    }
                }

                List<Note> afterBar = new List<Note>();
                afterBar.Add(new MeasureChange(i, 0, CalculateQuaver(CalculateLeastMeasure(bar))));
                //Console.WriteLine();
                //Console.WriteLine("In bar "+i+", LeastMeasure is "+ CalculateLeastMeasure(bar)+", so quaver will be "+ CalculateQuaver(CalculateLeastMeasure(bar)));
                afterBar.AddRange(bar);
                this.chart.Add(FinishBar(afterBar, this.BPMChanges.ChangeNotes, i, CalculateQuaver(CalculateLeastMeasure(bar))));
            }
            //Console.WriteLine("TOTAL DELAY: "+this.TotalDelay);
            //Console.WriteLine("TOTAL COUNT: "+ this.chart.Count * 384);
            if (this.totalDelay < this.chart.Count * definition)
            {
                this.totalDelay = 0;
            }
            else
            {
                this.totalDelay -= this.chart.Count * definition;
            }
            this.totalNoteNumber += (this.tapNumber + this.holdNumber + this.slideNumber);
        }

        /// <summary>
        /// Compose chart in appropriate result.
        /// </summary>
        /// <returns>String of chart compiled</returns>
        public abstract string Compose();


        /// <summary>
        /// Override and compose with given arrays
        /// </summary>
        /// <param name="bpm">Override BPM array</param>
        /// <param name="measure">Override Measure array</param>
        /// <returns>Good Brother with override array</returns>
        public abstract string Compose(BPMChanges bpm, MeasureChanges measure);

        /// <summary>
        /// Return the least none 0 measure of bar.
        /// </summary>
        /// <param name="bar">bar to take in</param>
        /// <returns>List none 0 measure</returns>
        public static int CalculateLeastMeasure(List<Note> bar)
        {
            List<int> startTimeList = new List<int>();
            startTimeList.Add(0);
            foreach (Note x in bar)
            {
                if (!startTimeList.Contains(x.Tick))
                {
                    startTimeList.Add(x.Tick);
                }
                if (x.NoteType.Equals("BPM"))
                {
                    //Console.WriteLine(x.Compose(0));
                }
            }
            if (startTimeList[startTimeList.Count - 1] != 384)
            {
                startTimeList.Add(384);
            }
            List<int> intervalCandidates = new List<int>();
            int minimalInterval = GCD(startTimeList[0], startTimeList[1]);
            for (int i = 1; i < startTimeList.Count; i++)
            {
                minimalInterval = GCD(minimalInterval, startTimeList[i]);
            }
            return minimalInterval;
        }

        /// <summary>
        /// Return note number except Rest, BPM and Measure.
        /// </summary>
        /// <param name="Bar">bar of note to take in</param>
        /// <returns>Number</returns>
        public static int RealNoteNumber(List<Note> Bar)
        {
            int result = 0;
            foreach (Note x in Bar)
            {
                if (x.IsNote)
                {
                    result++;
                }
            }
            return result;
        }

        /// <summary>
        /// Judges if this bar contains notes
        /// </summary>
        /// <param name="Bar">Bar to analyze on</param>
        /// <returns>True if contains, false otherwise</returns>
        public static bool ContainNotes(List<Note> Bar)
        {
            bool result = false;
            foreach (Note x in Bar)
            {
                result = result || x.IsNote;
            }
            return result;
        }

        /// <summary>
        /// Generate appropriate length for hold and slide.
        /// </summary>
        /// <param name="length">Last Time</param>
        /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
        public static int CalculateQuaver(int length)
        {
            int result = 0;
            const int definition = 384;
            int divisor = GCD(definition, length);
            int quaver = definition / divisor, beat = length / divisor;
            result = quaver;
            return result;
        }

        /// <summary>
        /// Finish Bar writing byu adding specific rest note in between.
        /// </summary>
        /// <param name="bar">Bar to finish with</param>
        /// <param name="bpmChanges">BPMChange Notes</param>
        /// <param name="barNumber">Bar number of Bar</param>
        /// <param name="minimalQuaver">Minimal interval calculated from bar</param>
        /// <returns>Finished bar</returns>
        public static List<Note> FinishBar(List<Note> bar, List<BPMChange> bpmChanges, int barNumber, int minimalQuaver)
        {
            List<Note> result = new List<Note>();
            bool writeRest = true;
            result.Add(bar[0]);
            for (int i = 0; i < 384; i += 384 / minimalQuaver)
            {
                //Separate Touch and others to prevent ordering issue
                Note bpm = new Rest();
                List<Note> eachSet = new List<Note>();
                List<Note> touchEachSet = new List<Note>();

                //Set condition to write rest if appropriate
                writeRest = true;
                //Add Appropriate note into each set
                Note lastNote = new Rest();
                foreach (Note x in bar)
                {
                    if ((x.Tick == i) && x.IsNote && !(x.NoteType.Equals("TTP") || x.NoteType.Equals("THO")))
                    {
                        if (x.NoteSpecificGenre.Equals("BPM"))
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
                    else if ((x.Tick == i) && x.IsNote && (x.NoteType.Equals("TTP") || x.NoteType.Equals("THO")))
                    {
                        if (x.NoteSpecificGenre.Equals("BPM"))
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
                    if (!x.NoteSpecificGenre.Equals("BPM")&&!x.NoteSpecificGenre.Equals("SLIDE_START"))
                    {
                        lastNote = x.NewInstance();
                    }
                }
                //Searching for BPM change. If find one, get into front.
                if (bpm.BPM != 0)
                {
                    List<Note> adjusted = new List<Note>();
                    adjusted.Add(bpm);
                    adjusted.AddRange(touchEachSet);
                    adjusted.AddRange(eachSet);
                    eachSet = adjusted;
                }
                else
                {
                    List<Note> adjusted = new List<Note>();
                    adjusted.AddRange(touchEachSet);
                    adjusted.AddRange(eachSet);
                    eachSet = adjusted;
                }
                if (writeRest)
                {
                    //Console.WriteLine("There is no note at tick " + i + " of bar " + barNumber + ", Adding one");
                    eachSet.Add(new Rest("RST", barNumber, i));
                }
                result.AddRange(eachSet);
            }
            if (RealNoteNumber(result) != RealNoteNumber(bar))
            {
                string error = "";
                error += ("Bar notes not match in bar: " + barNumber) + "\n";
                error += ("Expected: " + RealNoteNumber(bar)) + "\n";
                foreach (Note x in bar)
                {
                    error += (x.Compose(1)) + "\n";
                }
                error += ("\nActual: " + RealNoteNumber(result)) + "\n";
                foreach (Note y in result)
                {
                    error += (y.Compose(1)) + "\n";
                }
                Console.WriteLine(error);
                throw new Exception("NOTE NUMBER IS NOT MATCHING");
            }
            bool hasFirstBPMChange = false;
            List<Note> changedResult = new List<Note>();
            Note potentialFirstChange = new Rest();
            {
                for (int i = 0; !hasFirstBPMChange && i < result.Count(); i++)
                {
                    if (result[i].NoteGenre.Equals("BPM") && result[i].Tick == 0)
                    {
                        changedResult.Add(result[i]);
                        potentialFirstChange = result[i];
                        hasFirstBPMChange = true;
                    }
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
        /// Return GCD of A and B.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns>GCD of A and B</returns>
        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        /// <summary>
        /// Return if this is a prime (1 counts)
        /// </summary>
        /// <param name="number">Number to inspect</param>
        /// <returns>True if is prime, false otherwise</returns>
        public static bool IsPrime(int number)
        {
            if (number < 1) return false;
            if (number == 2) return true;
            if (number % 2 == 0) return false;

            var boundary = (int)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
                if (number % i == 0)
                    return false;

            return true;
        }

        /// <summary>
        /// Take in and replace the current information.
        /// </summary>
        /// <param name="information">Dictionary containing information needed</param>
        public void TakeInformation(Dictionary<string, string> information)
        {
            foreach (KeyValuePair<string, string> x in information)
            {
                this.information.Add(x.Key, x.Value);
            }
        }

        public double GetTimeStamp(int bar, int tick)
        {
            double result = 0.0;
            int overallTick = bar * 384 + tick;
            if (overallTick != 0)
            {
                int maximumBPMIndex = 0;
                for (int i = 0; i < this.BPMChanges.ChangeNotes.Count; i++)
                {
                    if (this.BPMChanges.ChangeNotes[i].TickStamp <= overallTick)
                    {
                        maximumBPMIndex = i;
                    }
                }
                if (maximumBPMIndex == 0)
                {
                    result = 60 / this.BPMChanges.ChangeNotes[0].BPM * 4 / 384;
                }
                else
                {
                    for (int i = 1; i <= maximumBPMIndex; i++)
                    {
                        double previousTickTimeUnit = 60 / this.BPMChanges.ChangeNotes[i - 1].BPM * 4 / 384;
                        result += (this.BPMChanges.ChangeNotes[i].TickStamp - this.BPMChanges.ChangeNotes[i - 1].TickStamp) * previousTickTimeUnit;
                    }
                    double tickTimeUnit = 60 / this.BPMChanges.ChangeNotes[maximumBPMIndex].BPM * 4 / 384;
                    result += (overallTick - this.BPMChanges.ChangeNotes[maximumBPMIndex].TickStamp) * tickTimeUnit;
                }
            }
            return result;
        }

        /// <summary>
        /// Give time stamp given overall tick
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
                for (int i = 0; i < this.BPMChanges.ChangeNotes.Count && !foundMax; i++)
                {
                    if (this.BPMChanges.ChangeNotes[i].TickStamp <= overallTick)
                    {
                        maximumBPMIndex = i;
                    }
                    else
                    {
                        foundMax = true;
                    }
                }
                if (maximumBPMIndex == 0)
                {
                    result = GetBPMTimeUnit(this.BPMChanges.ChangeNotes[0].BPM) * overallTick;
                }
                else
                {
                    for (int i = 1; i <= maximumBPMIndex; i++)
                    {
                        double previousTickTimeUnit = GetBPMTimeUnit(this.BPMChanges.ChangeNotes[i - 1].BPM);
                        result += (this.BPMChanges.ChangeNotes[i].TickStamp - this.BPMChanges.ChangeNotes[i - 1].TickStamp) * previousTickTimeUnit;
                    }
                    double tickTimeUnit = GetBPMTimeUnit(this.BPMChanges.ChangeNotes[maximumBPMIndex].BPM);
                    result += (overallTick - this.BPMChanges.ChangeNotes[maximumBPMIndex].TickStamp) * tickTimeUnit;
                }
            }
            return result;
        }

        /// <summary>
        /// Give time stamp given overall tick
        /// </summary>
        /// <param name="overallTick">Note.Bar*384+Note.Tick</param>
        /// <returns>Appropriate time stamp in seconds</returns>
        public static double GetTimeStamp(BPMChanges bpmChanges, int overallTick)
        {
            double result = 0.0;
            if (overallTick != 0)
            {
                int maximumBPMIndex = 0;
                for (int i = 0; i < bpmChanges.ChangeNotes.Count; i++)
                {
                    if (bpmChanges.ChangeNotes[i].TickStamp <= overallTick)
                    {
                        maximumBPMIndex = i;
                    }
                }
                if (maximumBPMIndex == 0)
                {
                    result = GetBPMTimeUnit(bpmChanges.ChangeNotes[0].BPM) * overallTick;
                }
                else
                {
                    for (int i = 1; i <= maximumBPMIndex; i++)
                    {
                        double previousTickTimeUnit = GetBPMTimeUnit(bpmChanges.ChangeNotes[i - 1].BPM);
                        result += (bpmChanges.ChangeNotes[i].TickStamp - bpmChanges.ChangeNotes[i - 1].TickStamp) * previousTickTimeUnit;
                    }
                    double tickTimeUnit = GetBPMTimeUnit(bpmChanges.ChangeNotes[maximumBPMIndex].BPM);
                    result += (overallTick - bpmChanges.ChangeNotes[maximumBPMIndex].TickStamp) * tickTimeUnit;
                }
            }
            return result;
        }

        /// <summary>
        /// Return BPM tick unit of given bpm
        /// </summary>
        /// <param name="bpm">BPM to calculate</param>
        /// <returns>Tick Unit of BPM</returns>
        public static double GetBPMTimeUnit(double bpm)
        {
            double result = 60 / bpm * 4 / 384;
            return result;
        }

        /// <summary>
        /// For debug use: print out the note's time stamp in given bpm changes
        /// </summary>
        /// <param name="bpmChanges">The list of BPMChanges</param>
        /// <param name="inTake">The Note to test</param>
        /// <returns>String of result, consists tick time stamp, wait time stamp and last time stamp</returns>
        public static string GetNoteDetail(BPMChanges bpmChanges, Note inTake)
        {
            string result = "";
            result += inTake.Compose(1) + "\n";
            result += "This is a " + inTake.NoteSpecificGenre + " note,\n";
            result += "This note has overall tick of " + inTake.TickStamp + ", and therefor, the tick time stamp shall be " + GetTimeStamp(bpmChanges, inTake.TickStamp) + "\n";
            if (inTake.NoteGenre.Equals("SLIDE"))
            {
                result += "This note has wait length of " + inTake.WaitLength + ", and therefor, its wait tick stamp is " + inTake.WaitTickStamp + " with wait time stamp of " + GetTimeStamp(bpmChanges, inTake.WaitTickStamp) + "\n";
                result += "This note has last length of " + inTake.LastLength + ", and therefor, its last tick stamp is " + inTake.LastTickStamp + " with last time stamp of " + GetTimeStamp(bpmChanges, inTake.LastTickStamp) + "\n";
            }
            return result;
        }

        /// <summary>
        /// Return the BPM at certain tick
        /// </summary>
        /// <param name="overallTick">Tick to specify</param>
        /// <returns>BPM at that tick</returns>
        public double GetBPMByTick(int overallTick)
        {
            double result = this.bpmChanges.ChangeNotes[0].BPM;
            if (overallTick > 0)
            {
                int maximumBPMIndex = 0;
                for (int i = 0; i < this.bpmChanges.ChangeNotes.Count; i++)
                {
                    if (this.bpmChanges.ChangeNotes[i].TickStamp <= overallTick)
                    {
                        maximumBPMIndex = i;
                    }
                }
                result = this.bpmChanges.ChangeNotes[maximumBPMIndex].BPM;

            }
            return result;
        }

        /// <summary>
        /// Determine if there are BPM change in between ticks
        /// </summary>
        /// <param name="startTick">Tick to start with</param>
        /// <param name="endTick">Tick to end with</param>
        /// <returns></returns>
        public bool HasBPMChangeInBetween(int startTick, int endTick)
        {
            bool result = false;

            for (int i = 0; i < this.bpmChanges.ChangeNotes.Count && !result; i++)
            {
                if (this.bpmChanges.ChangeNotes[i].TickStamp > startTick && this.bpmChanges.ChangeNotes[i].TickStamp < endTick)
                {
                    result = true;
                }
            }

            return result;
        }

        public void ShiftByOffset(int overallTick)
        {
            List<Note> updatedNotes = new List<Note>();
            foreach (Note x in this.Notes)
            {
                if (!x.NoteType.Equals("BPM") || !x.NoteGenre.Equals("MEASURE") || (x.NoteType.Equals("BPM") && x.Bar != 0 && x.Tick != 0) || (x.NoteGenre.Equals("MEASURE") && x.Bar != 0 && x.Tick != 0))
                {
                    Note copy;
                    switch (x.NoteGenre)
                    {
                        case "TAP":
                        case "SLIDE_START":
                            copy = new Tap(x);
                            copy.Bar += overallTick / 384;
                            copy.Tick += overallTick % 384;
                            // if (copy.ConsecutiveSlide!=null)
                            // {
                            //     copy.ConsecutiveSlide.Bar += overallTick / 384;
                            //     copy.ConsecutiveSlide.Tick += overallTick % 384;
                            // }
                            copy.Update();
                            break;
                        case "HOLD":
                            copy = new Hold(x);
                            copy.Bar += overallTick / 384;
                            copy.Tick += overallTick % 384;
                            copy.Update();
                            break;
                        case "SLIDE":
                            copy = new Slide(x);
                            copy.Bar += overallTick / 384;
                            copy.Tick += overallTick % 384;
                            // if (copy.SlideStart != null)
                            // {
                            //     copy.SlideStart.Bar += overallTick / 384;
                            //     copy.SlideStart.Tick += overallTick % 384;
                            // }
                            // if (copy.ConsecutiveSlide != null)
                            // {
                            //     copy.ConsecutiveSlide.Bar += overallTick / 384;
                            //     copy.ConsecutiveSlide.Tick += overallTick % 384;
                            // }
                            copy.Update();
                            break;
                        case "BPM":
                            copy = new BPMChange(x);
                            copy.Bar += overallTick / 384;
                            copy.Tick += overallTick % 384;
                            copy.Update();
                            break;
                        case "MEASURE":
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
            }
            this.Notes = new List<Note>(updatedNotes);
            this.Update();
        }

        public void ShiftByOffset(int bar, int tick)
        {
            int overallTick = bar * 384 + tick;
            this.ShiftByOffset(overallTick);
        }

        public void RotateNotes(string method)
        {
            foreach (Note x in this.Notes)
            {
                x.Flip(method);
            }
            this.Update();
        }
    }
}