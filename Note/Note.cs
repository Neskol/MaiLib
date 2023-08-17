﻿namespace MaiLib;

/// <summary>
///     Basic note
/// </summary>
public abstract class Note : IEquatable<Note>, INote, IComparable
{
    public enum SpecialState
    {
        /// <summary>
        ///     Normal note, nothing special
        /// </summary>
        Normal,

        /// <summary>
        ///     Break note
        /// </summary>
        Break,

        /// <summary>
        ///     EX Note
        /// </summary>
        EX,

        /// <summary>
        ///     EX Break
        /// </summary>
        BreakEX,

        /// <summary>
        ///     Connecting Slide
        /// </summary>
        ConnectingSlide
    }

    /// <summary>
    ///     Construct an empty note
    /// </summary>
    public Note()
    {
        NoteType = "";
        Key = "";
        EndKey = "";
        Bar = 0;
        Tick = 0;
        FixedTick = 0;
        TickStamp = 0;
        TickTimeStamp = 0.0;
        LastLength = 0;
        LastTickStamp = 0;
        LastTimeStamp = 0.0;
        WaitLength = 0;
        WaitTickStamp = 0;
        WaitTimeStamp = 0.0;
        CalculatedLastTime = 0.0;
        CalculatedWaitTime = 0.0;
        TickBPMDisagree = false;
        BPM = 0;
        BPMChangeNotes = new List<BPMChange>();
    }

    /// <summary>
    ///     Construct a note from other note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    public Note(Note inTake)
    {
        NoteType = inTake.NoteType;
        Key = inTake.Key;
        EndKey = inTake.EndKey;
        Bar = inTake.Bar;
        Tick = inTake.Tick;
        TickStamp = inTake.TickStamp;
        TickTimeStamp = inTake.TickTimeStamp;
        LastLength = inTake.LastLength;
        LastTickStamp = inTake.LastTickStamp;
        LastTimeStamp = inTake.LastTimeStamp;
        WaitLength = inTake.WaitLength;
        WaitTickStamp = inTake.WaitTickStamp;
        WaitTimeStamp = inTake.WaitTimeStamp;
        CalculatedLastTime = inTake.CalculatedLastTime;
        CalculatedLastTime = inTake.CalculatedLastTime;
        TickBPMDisagree = inTake.TickBPMDisagree;
        BPM = inTake.BPM;
        BPMChangeNotes = inTake.BPMChangeNotes;
    }

    /// <summary>
    ///     The note type
    /// </summary>
    public string NoteType { get; set; }

    /// <summary>
    ///     The key
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    ///     The end key
    /// </summary>
    public string EndKey { get; set; }

    /// <summary>
    ///     The bar
    /// </summary>
    public int Bar { get => this.TickStamp/this.Definition }

    /// <summary>
    ///     The start time
    /// </summary>
    public int Tick { get; set; }

    /// <summary>
    ///     Start time fixed to BPM
    /// </summary>
    public int FixedTick { get; set; }

    /// <summary>
    ///     The absolute Tick Calculated by this.bar*384+this.Tick
    /// </summary>
    public int TickStamp { get; set; }

    /// <summary>
    ///     The start time stamp
    /// </summary>
    public double TickTimeStamp { get; set; }

    /// <summary>
    ///     The Wait length
    /// </summary>
    public int WaitLength { get; set; }

    /// <summary>
    ///     The stamp of Wait time ends in Ticks
    /// </summary>
    public int WaitTickStamp { get; set; }

    /// <summary>
    ///     The Calculated Wait time in seconds
    /// </summary>
    public double CalculatedWaitTime { get; set; }

    /// <summary>
    ///     The Last length
    /// </summary>
    public int LastLength { get; set; }

    /// <summary>
    ///     Fixed Tick Last length with fixed BPM
    /// </summary>
    public int FixedLastLength { get; set; }

    /// <summary>
    ///     The stamp when the Last time ends in Ticks
    /// </summary>
    public int LastTickStamp { get; set; }

    /// <summary>
    ///     The stamp when the Last time ends in seconds
    /// </summary>
    public double LastTimeStamp { get; set; }

    /// <summary>
    ///     The Calculated Last time
    /// </summary>
    public double CalculatedLastTime { get; set; }

    /// <summary>
    ///     Stores if the BPM of Wait or Last Tick is in different BPM
    /// </summary>
    public bool TickBPMDisagree { get; set; }

    /// <summary>
    ///     The delayed
    /// </summary>
    public bool Delayed { get; set; }

    /// <summary>
    ///     The BPM
    /// </summary>
    public double BPM { get; set; }

    /// <summary>
    ///     The stamp when the Wait time ends in seconds
    /// </summary>
    public double WaitTimeStamp { get; set; }

    /// <summary>
    ///     The previous note
    /// </summary>
    public Note? Prev { get; set; }

    /// <summary>
    ///     The next note
    /// </summary>
    public Note? Next { get; set; }

    /// <summary>
    ///     Stores all BPM change prior to this
    /// </summary>
    public List<BPMChange> BPMChangeNotes { get; set; }

    public SpecialState NoteSpecialState { get; set; }

    /// <summary>
    ///     Get the number value of Key
    /// </summary>
    /// <value>Number value of Key 0-7, exclude key group</value>
    public int KeyNum => int.Parse(Key.ToCharArray()[0].ToString());

    /// <summary>
    ///     Get the key group of the key - only for touch notes
    /// </summary>
    /// <value>Key group of the touch note</value>
    public string KeyGroup
    {
        get
        {
            var result = "";
            if (Key.ToCharArray().Count() > 1) result = Key.ToCharArray()[1].ToString();
            return result;
        }
    }

    /// <summary>
    ///     Get the number value of End Key
    /// </summary>
    /// <value>Number value of Key 0-7, exclude key group</value>
    public int EndKeyNum
    {
        get
        {
            var result = 0;
            if (!EndKey.Equals("")) result = int.Parse(EndKey.ToCharArray()[0].ToString());
            return result;
        }
    }

    /// <summary>
    ///     Return this.SpecificType
    /// </summary>
    /// <returns>string of specific genre (specific type of Tap, Slide, etc.)</returns>
    public abstract string NoteSpecificGenre { get; }

    /// <summary>
    ///     Return this.noteGenre
    /// </summary>
    /// <returns>string of note genre (general category of TAP, SLIDE and HOLD)</returns>
    public abstract string NoteGenre { get; }

    /// <summary>
    ///     Return if this is a true note
    /// </summary>
    /// <returns>True if is TAP,HOLD or SLIDE, false otherwise</returns>
    public abstract bool IsNote { get; }

    public int CompareTo(object? obj)
    {
        var result = 0;

        var another = obj as Note ?? throw new NullReferenceException("Note is not defined");

        //else if (this.NoteSpecificType().Equals("SLIDE")&&(this.NoteSpecificType().Equals("TAP")|| this.NoteSpecificType().Equals("HOLD")) && this.Tick == another.Tick && this.bar == another.Bar)
        //{
        //    result = -1;
        //}
        //else if (this.NoteSpecificType().Equals("SLIDE_START") && (another.NoteSpecificType().Equals("TAP") || another.NoteSpecificType().Equals("HOLD")) && this.Tick == another.Tick && this.bar == another.Bar)
        //{
        //    Console.WriteLine("STAR AND TAP");
        //    result = 1;
        //    Console.WriteLine(this.NoteSpecificType() + ".compareTo(" + another.NoteSpecificType() + ") is" + result);
        //    //Console.ReadKey();
        //}
        //if (this.Bar==another.Bar&&this.Tick==another.Tick)
        //{
        //    if (this.NoteGenre().Equals("BPM"))
        //    {
        //        result = -1;
        //    }
        //    else if (this.NoteGenre().Equals("MEASURE"))
        //    {
        //        result = 1;
        //    }
        //    else if ((this.NoteSpecificType().Equals("TAP")|| this.NoteSpecificType().Equals("HOLD"))&&another.NoteSpecificType().Equals("SLIDE_START"))
        //    {
        //        result= -1;
        //    }
        //}
        //else
        //{
        //    if (this.bar != another.Bar)
        //    {
        //        result = this.bar.CompareTo(another.Bar);
        //        //Console.WriteLine("this.compareTo(another) is" + result);
        //        //Console.ReadKey();
        //    }
        //    else result = this.Tick.CompareTo(another.Tick);
        //}
        if (Bar != another.Bar)
        {
            result = Bar.CompareTo(another.Bar);
        }
        else if (Bar == another.Bar && Tick != another.Tick)
        {
            result = Tick.CompareTo(another.Tick);
        }
        else
        {
            if (NoteSpecificGenre.Equals("BPM"))
                result = -1;
            //else if (this.NoteSpecificType().Equals("SLIDE")&&another.NoteSpecificType().Equals("SLIDE_START")&&this.Key.Equals(another.Key))
            //{
            //    result = 1;
            //}
            else result = 0;
        }

        return result;
    }

    public bool Equals(Note? other)
    {
        var result = false;
        if (other != null &&
            NoteType.Equals(other.NoteType) &&
            Key.Equals(other.Key) &&
            EndKey.Equals(other.EndKey) &&
            Bar == other.Bar &&
            Tick == other.Tick &&
            LastLength == other.LastLength &&
            BPM == other.BPM)
            result = true;
        return result;
    }

    public abstract bool CheckValidity();

    public abstract string Compose(int format);

    public virtual void Flip(string method)
    {
        if (Key != null && !Key.Equals("") && !(Key.Count() > 1 && Key.ToCharArray()[1] == 'C'))
        {
            #region FlipConditions

            switch (method)
            {
                case "Clockwise90":
                    switch (KeyNum)
                    {
                        case 0:
                            Key = "2" + KeyGroup;
                            break;
                        case 1:
                            Key = "3" + KeyGroup;
                            break;
                        case 2:
                            Key = "4" + KeyGroup;
                            break;
                        case 3:
                            Key = "5" + KeyGroup;
                            break;
                        case 4:
                            Key = "6" + KeyGroup;
                            break;
                        case 5:
                            Key = "7" + KeyGroup;
                            break;
                        case 6:
                            Key = "0" + KeyGroup;
                            break;
                        case 7:
                            Key = "1" + KeyGroup;
                            break;
                    }

                    switch (EndKeyNum)
                    {
                        case 0:
                            EndKey = "2";
                            break;
                        case 1:
                            EndKey = "3";
                            break;
                        case 2:
                            EndKey = "4";
                            break;
                        case 3:
                            EndKey = "5";
                            break;
                        case 4:
                            EndKey = "6";
                            break;
                        case 5:
                            EndKey = "7";
                            break;
                        case 6:
                            EndKey = "0";
                            break;
                        case 7:
                            EndKey = "1";
                            break;
                    }

                    break;
                case "Clockwise180":
                    switch (KeyNum)
                    {
                        case 0:
                            Key = "4" + KeyGroup;
                            break;
                        case 1:
                            Key = "5" + KeyGroup;
                            break;
                        case 2:
                            Key = "6" + KeyGroup;
                            break;
                        case 3:
                            Key = "7" + KeyGroup;
                            break;
                        case 4:
                            Key = "0" + KeyGroup;
                            break;
                        case 5:
                            Key = "1" + KeyGroup;
                            break;
                        case 6:
                            Key = "2" + KeyGroup;
                            break;
                        case 7:
                            Key = "3" + KeyGroup;
                            break;
                    }

                    switch (EndKeyNum)
                    {
                        case 0:
                            EndKey = "4";
                            break;
                        case 1:
                            EndKey = "5";
                            break;
                        case 2:
                            EndKey = "6";
                            break;
                        case 3:
                            EndKey = "7";
                            break;
                        case 4:
                            EndKey = "0";
                            break;
                        case 5:
                            EndKey = "1";
                            break;
                        case 6:
                            EndKey = "2";
                            break;
                        case 7:
                            EndKey = "3";
                            break;
                    }

                    break;
                case "Counterclockwise90":
                    switch (KeyNum)
                    {
                        case 0:
                            Key = "6" + KeyGroup;
                            break;
                        case 1:
                            Key = "7" + KeyGroup;
                            break;
                        case 2:
                            Key = "0" + KeyGroup;
                            break;
                        case 3:
                            Key = "1" + KeyGroup;
                            break;
                        case 4:
                            Key = "2" + KeyGroup;
                            break;
                        case 5:
                            Key = "3" + KeyGroup;
                            break;
                        case 6:
                            Key = "4" + KeyGroup;
                            break;
                        case 7:
                            Key = "5" + KeyGroup;
                            break;
                    }

                    switch (EndKeyNum)
                    {
                        case 0:
                            EndKey = "6";
                            break;
                        case 1:
                            EndKey = "7";
                            break;
                        case 2:
                            EndKey = "0";
                            break;
                        case 3:
                            EndKey = "1";
                            break;
                        case 4:
                            EndKey = "2";
                            break;
                        case 5:
                            EndKey = "3";
                            break;
                        case 6:
                            EndKey = "4";
                            break;
                        case 7:
                            EndKey = "5";
                            break;
                    }

                    break;
                case "Counterclockwise180":
                    switch (KeyNum)
                    {
                        case 0:
                            Key = "4" + KeyGroup;
                            break;
                        case 1:
                            Key = "5" + KeyGroup;
                            break;
                        case 2:
                            Key = "6" + KeyGroup;
                            break;
                        case 3:
                            Key = "7" + KeyGroup;
                            break;
                        case 4:
                            Key = "0" + KeyGroup;
                            break;
                        case 5:
                            Key = "1" + KeyGroup;
                            break;
                        case 6:
                            Key = "2" + KeyGroup;
                            break;
                        case 7:
                            Key = "3" + KeyGroup;
                            break;
                    }

                    switch (EndKeyNum)
                    {
                        case 0:
                            EndKey = "4";
                            break;
                        case 1:
                            EndKey = "5";
                            break;
                        case 2:
                            EndKey = "6";
                            break;
                        case 3:
                            EndKey = "7";
                            break;
                        case 4:
                            EndKey = "0";
                            break;
                        case 5:
                            EndKey = "1";
                            break;
                        case 6:
                            EndKey = "2";
                            break;
                        case 7:
                            EndKey = "3";
                            break;
                    }

                    break;
                case "UpSideDown":
                    if (NoteType.Equals("TTP") && (KeyGroup.Equals("E") || KeyGroup.Equals("D")))
                        switch (KeyNum)
                        {
                            case 0:
                                Key = "4" + KeyGroup;
                                break;
                            case 1:
                                Key = "3" + KeyGroup;
                                break;
                            case 2:
                                break;
                            case 3:
                                Key = "1" + KeyGroup;
                                break;
                            case 4:
                                Key = "0" + KeyGroup;
                                break;
                            case 5:
                                Key = "7" + KeyGroup;
                                break;
                            case 6:
                                break;
                            case 7:
                                Key = "5" + KeyGroup;
                                break;
                        }
                    else
                        switch (KeyNum)
                        {
                            case 0:
                                Key = "3" + KeyGroup;
                                break;
                            case 1:
                                Key = "2" + KeyGroup;
                                break;
                            case 2:
                                Key = "1" + KeyGroup;
                                break;
                            case 3:
                                Key = "0" + KeyGroup;
                                break;
                            case 4:
                                Key = "7" + KeyGroup;
                                break;
                            case 5:
                                Key = "6" + KeyGroup;
                                break;
                            case 6:
                                Key = "5" + KeyGroup;
                                break;
                            case 7:
                                Key = "4" + KeyGroup;
                                break;
                        }

                    switch (EndKeyNum)
                    {
                        case 0:
                            EndKey = "3";
                            break;
                        case 1:
                            EndKey = "2";
                            break;
                        case 2:
                            EndKey = "1";
                            break;
                        case 3:
                            EndKey = "0";
                            break;
                        case 4:
                            EndKey = "7";
                            break;
                        case 5:
                            EndKey = "6";
                            break;
                        case 6:
                            EndKey = "5";
                            break;
                        case 7:
                            EndKey = "4";
                            break;
                    }

                    if (NoteGenre.Equals("SLIDE"))
                        switch (NoteType)
                        {
                            case "SCL":
                                NoteType = "SCR";
                                break;
                            case "SCR":
                                NoteType = "SCL";
                                break;
                            case "SUL":
                                NoteType = "SUR";
                                break;
                            case "SUR":
                                NoteType = "SUL";
                                break;
                            case "SLL":
                                NoteType = "SLR";
                                break;
                            case "SLR":
                                NoteType = "SLL";
                                break;
                            case "SXL":
                                NoteType = "SXR";
                                break;
                            case "SXR":
                                NoteType = "SXL";
                                break;
                            case "SSL":
                                NoteType = "SSR";
                                break;
                            case "SSR":
                                NoteType = "SSL";
                                break;
                        }

                    break;
                case "LeftToRight":
                    if (NoteType.Equals("TTP") && (KeyGroup.Equals("E") || KeyGroup.Equals("D")))
                        switch (KeyNum)
                        {
                            case 0:
                                break;
                            case 1:
                                Key = "7" + KeyGroup;
                                break;
                            case 2:
                                Key = "6" + KeyGroup;
                                break;
                            case 3:
                                Key = "5" + KeyGroup;
                                break;
                            case 4:
                                break;
                            case 5:
                                Key = "3" + KeyGroup;
                                break;
                            case 6:
                                Key = "2" + KeyGroup;
                                break;
                            case 7:
                                Key = "1" + KeyGroup;
                                break;
                        }
                    else
                        switch (KeyNum)
                        {
                            case 0:
                                Key = "7" + KeyGroup;
                                break;
                            case 1:
                                Key = "6" + KeyGroup;
                                break;
                            case 2:
                                Key = "5" + KeyGroup;
                                break;
                            case 3:
                                Key = "4" + KeyGroup;
                                break;
                            case 4:
                                Key = "3" + KeyGroup;
                                break;
                            case 5:
                                Key = "2" + KeyGroup;
                                break;
                            case 6:
                                Key = "1" + KeyGroup;
                                break;
                            case 7:
                                Key = "0" + KeyGroup;
                                break;
                        }

                    switch (EndKeyNum)
                    {
                        case 0:
                            EndKey = "7";
                            break;
                        case 1:
                            EndKey = "6";
                            break;
                        case 2:
                            EndKey = "5";
                            break;
                        case 3:
                            EndKey = "4";
                            break;
                        case 4:
                            EndKey = "3";
                            break;
                        case 5:
                            EndKey = "2";
                            break;
                        case 6:
                            EndKey = "1";
                            break;
                        case 7:
                            EndKey = "0";
                            break;
                    }

                    if (NoteGenre.Equals("SLIDE"))
                        switch (NoteType)
                        {
                            case "SCL":
                                NoteType = "SCR";
                                break;
                            case "SCR":
                                NoteType = "SCL";
                                break;
                            case "SUL":
                                NoteType = "SUR";
                                break;
                            case "SUR":
                                NoteType = "SUL";
                                break;
                            case "SLL":
                                NoteType = "SLR";
                                break;
                            case "SLR":
                                NoteType = "SLL";
                                break;
                            case "SXL":
                                NoteType = "SXR";
                                break;
                            case "SXR":
                                NoteType = "SXL";
                                break;
                            case "SSL":
                                NoteType = "SSR";
                                break;
                            case "SSR":
                                NoteType = "SSL";
                                break;
                        }

                    break;
                default:
                    throw new NotImplementedException(
                        "METHOD SPECIFIED INVALID. EXPECT: Clockwise90, Clockwise180, Counterclockwise90, Counterclockwise180, UpSideDown, LeftToRight");
            }

            #endregion
        }

        Update();
    }

    public virtual bool Update()
    {
        // Console.WriteLine("This note has BPM note number of " + this.BPMChangeNotes.Count());
        var result = false;
        TickStamp = Bar * 384 + Tick;
        while (Tick >= 384)
        {
            Tick -= 384;
            Bar++;
        }

        // string noteInformation = "This note is "+this.NoteType+", in Tick "+ this.TickStamp+", ";
        //this.TickTimeStamp = this.GetTimeStamp(this.TickStamp);
        WaitTickStamp = TickStamp + WaitLength;
        //this.WaitTimeStamp = this.GetTimeStamp(this.WaitTickStamp);
        LastTickStamp = WaitTickStamp + LastLength;
        //this.LastTimeStamp = this.GetTimeStamp(this.LastTickStamp);
        if (!(NoteType.Equals("SLIDE") || NoteType.Equals("HOLD")))
            result = true;
        else if (CalculatedLastTime > 0 && CalculatedWaitTime > 0) result = true;
        return result;
    }

    public double GetTimeStamp(int overallTick)
    {
        var result = 0.0;
        if (overallTick != 0)
        {
            var maximumBPMIndex = 0;
            for (var i = 0; i < BPMChangeNotes.Count; i++)
                if (BPMChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
            if (maximumBPMIndex == 0)
            {
                result = GetBPMTimeUnit(BPMChangeNotes[0].BPM) * overallTick;
            }
            else
            {
                for (var i = 1; i <= maximumBPMIndex; i++)
                {
                    var previousTickTimeUnit = GetBPMTimeUnit(BPMChangeNotes[i - 1].BPM);
                    result += (BPMChangeNotes[i].TickStamp - BPMChangeNotes[i - 1].TickStamp) * previousTickTimeUnit;
                }

                var TickTimeUnit = GetBPMTimeUnit(BPMChangeNotes[maximumBPMIndex].BPM);
                result += (overallTick - BPMChangeNotes[maximumBPMIndex].TickStamp) * TickTimeUnit;
            }
        } //A serious improvement is needed for this method

        return result;
    }

    /// <summary>
    ///     Judges if two notes are of same time
    /// </summary>
    /// <param name="x">Comparing note</param>
    /// <returns>True if BAR=BAR & TICK=TICK</returns>
    public bool IsOfSameTime(Note x)
    {
        return Bar == x.Bar && Tick == x.Tick;
    }

    public abstract Note NewInstance();

    /// <summary>
    ///     Replace this.BPMChangeNotes from change table given
    /// </summary>
    /// <param name="changeTable">Change table contains BPM notes</param>
    public void ReplaceBPMChanges(BPMChanges changeTable)
    {
        BPMChangeNotes = new List<BPMChange>();
        BPMChangeNotes.AddRange(changeTable.ChangeNotes);
    }

    /// <summary>
    ///     Replace this.BPMChangeNotes from change table given
    /// </summary>
    /// <param name="changeTable">Change table contains BPM notes</param>
    public void ReplaceBPMChanges(List<BPMChange> changeTable)
    {
        BPMChangeNotes = new List<BPMChange>();
        BPMChangeNotes.AddRange(changeTable);
    }

    /// <summary>
    ///     Generate appropriate length for hold and slide.
    /// </summary>
    /// <param name="length">Last Time</param>
    /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
    public string GenerateAppropriateLength(int length)
    {
        var result = "";
        const int definition = 384;
        var divisor = GCD(definition, length);
        int quaver = definition / divisor, beat = length / divisor;
        result = "[" + quaver + ":" + beat + "]";
        return result;
    }

    /// <summary>
    ///     Return GCD of A and B.
    /// </summary>
    /// <param name="a">A</param>
    /// <param name="b">B</param>
    /// <returns>GCD of A and B</returns>
    private static int GCD(int a, int b)
    {
        return b == 0 ? a : GCD(b, a % b);
    }

    /// <summary>
    ///     Generate appropriate length for hold and slide.
    /// </summary>
    /// <param name="length">Last Time</param>
    /// <param name="BPM">BPM</param>
    /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
    public string GenerateAppropriateLength(int length, double BPM)
    {
        var result = "";
        var duration = Math.Round(LastTimeStamp - WaitTimeStamp, 4);
        switch (NoteGenre)
        {
            case "SLIDE":
                var sustain = Math.Round(WaitTimeStamp - TickTimeStamp, 4);
                result = "[" + sustain + "##" + duration + "]";
                break;
            case "HOLD":
                double startTime = Math.Round(startTime = TickTimeStamp, 4);
                result = "[" + startTime + "##" + duration + "]";
                break;
        }

        return result;
    }

    /// <summary>
    ///     Get BPM Time Tick unit of BPM
    /// </summary>
    /// <param name="BPM">BPM to calculate</param>
    /// <returns>BPM Tick Unit of BPM</returns>
    public static double GetBPMTimeUnit(double BPM)
    {
        var result = 60 / BPM * 4 / 384;
        return result;
    }

    public override bool Equals(object? obj)
    {
        var result = (this == null && obj == null) || (this != null && obj != null);
        if (result && obj != null)
        {
            var localNote = (Note)obj;
            result = TickStamp == localNote.TickStamp && Compose(1).Equals(localNote.Compose(1));
        }

        return result;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
