﻿using System.Diagnostics;

namespace MaiLib;

using static NoteEnum;
using static ChartEnum;

/// <summary>
///     Basic note
/// </summary>
public abstract class Note : IEquatable<Note>, INote, IComparable
{
    
    #region Constructors

    /// <summary>
    ///     Construct an empty note
    /// </summary>
    public Note()
    {
        NoteType = NoteType.RST;
        Key = "";
        EndKey = "";
        Definition = 384;
        // TickBPMDisagree = false;
        BPMChangeNotes = [];
        TouchSize = "M1";
        NoteSpecialState = SpecialState.Normal;
    }

    /// <summary>
    ///     Base Note Constructor with all fields
    /// </summary>
    /// <param name="type">Note Type</param>
    /// <param name="key">Key/Start Key</param>
    /// <param name="endKey">End Key for Slides</param>
    /// <param name="bar">Bar of the note</param>
    /// <param name="tick">Tick of the note</param>
    /// <param name="fixedTick">Tick fixed to BPM</param>
    /// <param name="tickStamp">Bar * Definition + Tick</param>
    /// <param name="tickTimeStamp">Exact time of Tick</param>
    /// <param name="lastLength">Sustaining length</param>
    /// <param name="lastTickStamp">End sustain tick</param>
    /// <param name="lastTimeStamp">Exact time of the end of sustain</param>
    /// <param name="waitLength">Wait Length</param>
    /// <param name="waitTickStamp">End wait tick</param>
    /// <param name="waitTimeStamp">Exact time of wait tick</param>
    /// <param name="calculatedLastTime">Last time calculated in exact time</param>
    /// <param name="calculatedWaitTime">Wait time calculated in exact time</param>
    /// <param name="tickBPMDisagree">Bool which BPM change happened between start and end</param>
    /// <param name="bpm">BPM</param>
    /// <param name="bpmChangeNotes">Change notes of the BPM</param>
    /// <param name="touchSize">Touch Size: M1 or L1</param>
    /// <param name="noteSpecialState">Special state of the note</param>
    public Note(NoteType type, string key, string endKey, int bar, int tick, int fixedTick, int tickStamp,
        double tickTimeStamp, int lastLength, int lastTickStamp, double lastTimeStamp, int waitLength,
        int waitTickStamp, int waitTimeStamp, double calculatedLastTime, double calculatedWaitTime,
        bool tickBPMDisagree, double bpm, List<BPMChange> bpmChangeNotes, string touchSize,
        SpecialState noteSpecialState)
    {
        NoteType = type;
        Key = key;
        EndKey = endKey;
        Bar = bar;
        Tick = tick;
        // FixedTick = fixedTick;
        TickStamp = tickStamp;
        TickTimeStamp = tickTimeStamp;
        LastLength = lastLength;
        LastTickStamp = lastTickStamp;
        LastTimeStamp = lastTimeStamp;
        WaitLength = waitLength;
        WaitTickStamp = waitTickStamp;
        WaitTimeStamp = waitTimeStamp;
        CalculatedLastTime = calculatedLastTime;
        CalculatedWaitTime = calculatedWaitTime;
        // TickBPMDisagree = tickBPMDisagree;
        // BPM = bpm;
        BPMChangeNotes = bpmChangeNotes;
        TouchSize = touchSize;
        NoteSpecialState = noteSpecialState;
        Definition = 384;
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
        // TickBPMDisagree = inTake.TickBPMDisagree;
        // BPM = inTake.BPM;
        BPMChangeNotes = inTake.BPMChangeNotes;
        NoteSpecialState = inTake.NoteSpecialState;
        TouchSize = "M1";
        Definition = 384;
    }

    #endregion

    #region Fields

    /// <summary>
    ///     The note type
    /// </summary>
    public NoteType NoteType { get; set; }

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
    public int Bar { get; set; }

    /// <summary>
    ///     The start time
    /// </summary>
    public int Tick { get; set; }

    // /// <summary>
    // ///     Start time fixed to BPM
    // /// </summary>
    // public int FixedTick { get; set; }

    /// <summary>
    ///     The absolute Tick Calculated by this.bar*384+this.Tick
    /// </summary>
    public int TickStamp { get; set; }

    /// <summary>
    ///     The start time stamp
    /// </summary>
    public double TickTimeStamp { get; set; }

    /// <summary>
    ///     The stamp when the Wait time ends in seconds
    /// </summary>
    public double WaitTimeStamp;

    /// <summary>
    ///     The Wait length
    /// </summary>
    public int WaitLength { get; set; }

    /// <summary>
    ///     The stamp of Wait time ends in Ticks
    /// </summary>
    public int WaitTickStamp { get; set; }
    // public int WaitTickStamp => LastTickStamp + WaitLength;

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
    // public int LastTickStamp => LastLength == 0 ? TickStamp : TickStamp + LastLength;

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
    public bool TickBPMDisagree =>
        BPMChangeNotes.Any(note => note.TickStamp > TickStamp && note.TickStamp < LastTickStamp);

    /// <summary>
    ///     Stores if the note is delayed
    /// </summary>
    public virtual bool Delayed => false;

    /// <summary>
    ///     The BPM
    /// </summary>
    public virtual double BPM
    {
        get
        {
            BPMChange? nearestBpm =
                BPMChangeNotes.Where(bpm => bpm.TickStamp <= TickStamp).MaxBy(note => note.TickStamp);
            return nearestBpm?.BPM ?? 0.0;
        }
        protected internal set
        {
            if (BPMChangeNotes.Count == 0) BPMChangeNotes.Add(new BPMChange(Bar, Tick, value));
        }
    }

    /// <summary>
    ///     The definition of the note which represented by the maximum tick of a 4/4 bar. By default, it is 384.
    /// </summary>
    public int Definition { get; protected set; }

    /// <summary>
    ///     Whether this note has a firework effect.
    /// </summary>
    public bool SpecialEffect { get; protected set; }

    /// <summary>
    ///     Determines the size of the touch. By default, it is M1
    /// </summary>
    public string TouchSize { get; protected set; }

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
            string? result = "";
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
            int result = 0;
            if (!EndKey.Equals("")) result = int.Parse(EndKey.ToCharArray()[0].ToString());
            return result;
        }
    }

    /// <summary>
    ///     Return this.SpecificType
    /// </summary>
    /// <returns>string of specific genre (specific type of Tap, Slide, etc.)</returns>
    public abstract NoteSpecificGenre NoteSpecificGenre { get; }

    /// <summary>
    ///     Return this.noteGenre
    /// </summary>
    /// <returns>string of note genre (general category of TAP, SLIDE and HOLD)</returns>
    public abstract NoteGenre NoteGenre { get; }

    /// <summary>
    ///     Return if this is a true note
    /// </summary>
    /// <returns>True if is TAP,HOLD or SLIDE, false otherwise</returns>
    public abstract bool IsNote { get; }

    #endregion

    public int CompareTo(object? obj)
    {
        int result = 0;

        Note? another = obj as Note ?? throw new NullReferenceException("Note is not defined");

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
            if (NoteSpecificGenre is NoteSpecificGenre.BPM)
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
        bool result = other != null &&
                      NoteType.Equals(other.NoteType) &&
                      Key.Equals(other.Key) &&
                      EndKey.Equals(other.EndKey) &&
                      Bar == other.Bar &&
                      Tick == other.Tick &&
                      LastLength == other.LastLength &&
                      Math.Abs(BPM - other.BPM) < double.Epsilon;
        return result;
    }

    /// <summary>
    ///     Copies all note properties of copyFrom to copyTo
    /// </summary>
    /// <param name="copyTo">Target note</param>
    public void CopyOver(Note copyTo)
    {
        copyTo.NoteType = this.NoteType;
        copyTo.Key = this.Key;
        copyTo.EndKey = this.EndKey;
        copyTo.Bar = this.Bar;
        copyTo.Tick = this.Tick;
        copyTo.TickStamp = this.TickStamp;
        copyTo.TickTimeStamp = this.TickTimeStamp;
        copyTo.LastLength = this.LastLength;
        // copyTo.LastTickStamp = this.LastTickStamp;
        copyTo.LastTimeStamp = this.LastTimeStamp;
        copyTo.WaitLength = this.WaitLength;
        // copyTo.WaitTickStamp = this.WaitTickStamp;
        copyTo.WaitTimeStamp = this.WaitTimeStamp;
        copyTo.CalculatedLastTime = this.CalculatedLastTime;
        copyTo.CalculatedLastTime = this.CalculatedLastTime;
        // copyTo.TickBPMDisagree = this.TickBPMDisagree;
        // copyTo.BPM = this.BPM;
        copyTo.BPMChangeNotes = this.BPMChangeNotes;
        copyTo.NoteSpecialState = this.NoteSpecialState;
        copyTo.TouchSize = this.TouchSize;
        copyTo.SpecialEffect = this.SpecialEffect;
        copyTo.Definition = this.Definition;
    }

    public abstract bool CheckValidity();

    public abstract string Compose(ChartVersion format);

    public virtual void Flip(FlipMethod method)
    {
        if (Key != null && !Key.Equals("") && !(Key.Count() > 1 && Key.ToCharArray()[1] == 'C'))
        {
            #region FlipConditions

            switch (method)
            {
                case FlipMethod.Clockwise90:
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
                case FlipMethod.Clockwise180:
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
                case FlipMethod.Counterclockwise90:
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
                case FlipMethod.Counterclockwise180:
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
                case FlipMethod.UpSideDown:
                    if (NoteType is NoteType.TTP && (KeyGroup.Equals("E") || KeyGroup.Equals("D")))
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

                    if (NoteGenre is NoteGenre.SLIDE)
                        switch (NoteType)
                        {
                            case NoteType.SCL:
                                NoteType = NoteType.SCR;
                                break;
                            case NoteType.SCR:
                                NoteType = NoteType.SCL;
                                break;
                            case NoteType.SUL:
                                NoteType = NoteType.SUR;
                                break;
                            case NoteType.SUR:
                                NoteType = NoteType.SUL;
                                break;
                            case NoteType.SLL:
                                NoteType = NoteType.SLR;
                                break;
                            case NoteType.SLR:
                                NoteType = NoteType.SLL;
                                break;
                            case NoteType.SXL:
                                NoteType = NoteType.SXR;
                                break;
                            case NoteType.SXR:
                                NoteType = NoteType.SXL;
                                break;
                            case NoteType.SSL:
                                NoteType = NoteType.SSR;
                                break;
                            case NoteType.SSR:
                                NoteType = NoteType.SSL;
                                break;
                        }

                    break;
                case FlipMethod.LeftToRight:
                    if (NoteType is NoteType.TTP && (KeyGroup.Equals("E") || KeyGroup.Equals("D")))
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

                    if (NoteGenre is NoteGenre.SLIDE)
                        switch (NoteType)
                        {
                            case NoteType.SCL:
                                NoteType = NoteType.SCR;
                                break;
                            case NoteType.SCR:
                                NoteType = NoteType.SCL;
                                break;
                            case NoteType.SUL:
                                NoteType = NoteType.SUR;
                                break;
                            case NoteType.SUR:
                                NoteType = NoteType.SUL;
                                break;
                            case NoteType.SLL:
                                NoteType = NoteType.SLR;
                                break;
                            case NoteType.SLR:
                                NoteType = NoteType.SLL;
                                break;
                            case NoteType.SXL:
                                NoteType = NoteType.SXR;
                                break;
                            case NoteType.SXR:
                                NoteType = NoteType.SXL;
                                break;
                            case NoteType.SSL:
                                NoteType = NoteType.SSR;
                                break;
                            case NoteType.SSR:
                                NoteType = NoteType.SSL;
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
        bool result = false;
        TickStamp = Bar * Definition + Tick;
        while (Tick >= Definition)
        {
            Tick -= Definition;
            Bar++;
        }

        if (CalculatedWaitTime != 0 && WaitLength == 0)
        {
            WaitTimeStamp = GetTimeStamp(TickStamp) + CalculatedWaitTime;
            WaitTickStamp = GetTickStampByTime(WaitTimeStamp);
            WaitLength = WaitTickStamp - TickStamp;
        }

        if (CalculatedLastTime != 0 && LastLength == 0)
        {
            int tickCandidate = NoteGenre is NoteGenre.HOLD ? TickStamp : WaitTickStamp;
            LastTimeStamp = GetTimeStamp(tickCandidate) + CalculatedLastTime;
            LastTickStamp = GetTickStampByTime(LastTimeStamp);
            LastLength = LastTickStamp - tickCandidate;
        }

        double bpmUnit = GetBPMTimeUnit(BPM);
        FixedLastLength = (int)double.Round(CalculatedLastTime / bpmUnit);
        WaitTickStamp = TickStamp + WaitLength;
        LastTickStamp = WaitTickStamp + LastLength;
        if (!(NoteGenre is NoteGenre.SLIDE || NoteGenre is NoteGenre.HOLD))
            result = true;
        else if (CalculatedLastTime > 0 && CalculatedWaitTime > 0) result = true;
        return result;
    }

    public double GetTimeStamp(int overallTick)
    {
        double result = 0.0;
        if (overallTick != 0)
        {
            int maximumBPMIndex = 0;
            for (int i = 0; i < BPMChangeNotes.Count; i++)
                if (BPMChangeNotes[i].TickStamp <= overallTick)
                    maximumBPMIndex = i;
            if (maximumBPMIndex == 0)
            {
                result = BPMChangeNotes[0].BPMTimeUnit * overallTick;
            }
            else
            {
                for (int i = 1; i <= maximumBPMIndex; i++)
                {
                    double previousTickTimeUnit = BPMChangeNotes[i - 1].BPMTimeUnit;
                    result += (BPMChangeNotes[i].TickStamp - BPMChangeNotes[i - 1].TickStamp) * previousTickTimeUnit;
                }

                double TickTimeUnit = GetBPMTimeUnit(BPMChangeNotes[maximumBPMIndex].BPM);
                result += (overallTick - BPMChangeNotes[maximumBPMIndex].TickStamp) * TickTimeUnit;
            }
        } //A serious improvement is needed for this method

        return result;
    }

    public static double GetTimeStamp(List<BPMChange> changeTable, int overallTick)
    {
        Note dummyNote = new Rest() { BPMChangeNotes = changeTable };
        return dummyNote.GetTimeStamp(overallTick);
    }

    public int GetTickStampByTime(double timeStamp)
    {
        BPMChange nearestBpmChange = BPMChangeNotes.Where(note => GetTimeStamp(note.TickStamp) <= timeStamp)
                                         .MaxBy(note => note.TickStamp) ??
                                     throw new InvalidOperationException("GIVEN NOTE DOES NOT CONTAIN BPM CHANGE");
        return (int)double.Round(nearestBpmChange.TickStamp +
                                 (timeStamp - GetTimeStamp(nearestBpmChange.TickStamp)) / nearestBpmChange.BPMTimeUnit);
    }

    public static int GetTickStampByTime(List<BPMChange> changeTable, double timeStamp)
    {
        BPMChange nearestBpmChange = changeTable.Where(note => GetTimeStamp(changeTable, note.TickStamp) <= timeStamp)
                                         .MaxBy(note => note.TickStamp) ??
                                     throw new InvalidOperationException("GIVEN NOTE DOES NOT CONTAIN BPM CHANGE");
        return (int)double.Round(nearestBpmChange.TickStamp +
                                 (timeStamp - GetTimeStamp(changeTable, nearestBpmChange.TickStamp)) /
                                 nearestBpmChange.BPMTimeUnit);
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
        BPMChangeNotes = [.. changeTable.ChangeNotes];
    }

    /// <summary>
    ///     Replace this.BPMChangeNotes from change table given
    /// </summary>
    /// <param name="changeTable">Change table contains BPM notes</param>
    public void ReplaceBPMChanges(List<BPMChange> changeTable)
    {
        BPMChangeNotes = [.. changeTable];
    }

    /// <summary>
    ///     Generate appropriate length for hold and slide.
    /// </summary>
    /// <param name="length">Last Time</param>
    /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
    public string GenerateAppropriateLength(int length)
    {
        string? result = "";
        int divisor = GCD(Definition, length);
        int quaver = Definition / divisor, beat = length / divisor;
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
        string? result = "";
        double duration = Math.Round(LastTimeStamp - WaitTimeStamp, 4);
        switch (NoteGenre)
        {
            case NoteGenre.SLIDE:
                double sustain = Math.Round(WaitTimeStamp - TickTimeStamp, 4);
                result = "[" + sustain + "##" + duration + "]";
                break;
            case NoteGenre.HOLD:
                double startTime = Math.Round(TickTimeStamp, 4);
                result = "[" + startTime + "##" + duration + "]";
                break;
        }

        return result;
    }

    /// <summary>
    ///     Get BPM Time Tick unit of BPM
    /// </summary>
    /// <param name="bpm">BPM to calculate</param>
    /// <returns>BPM Tick Unit of BPM</returns>
    public static double GetBPMTimeUnit(double bpm)
    {
        return 60 / bpm * 4 / 384;
    }

    /// <summary>
    ///     Get BPM Time Tick unit of BPM
    /// </summary>
    /// <param name="bpm">BPM to calculate</param>
    /// <param name="definition">Specified resolution - usually 384</param>
    /// <returns>BPM Tick Unit of BPM</returns>
    public static double GetBPMTimeUnit(double bpm, int definition)
    {
        return 60 / bpm * 4 / definition;
    }

    public override bool Equals(object? obj)
    {
        bool result = (this == null && obj == null) || (this != null && obj != null);
        if (result && obj != null)
        {
            Note? localNote = (Note)obj;
            result = TickStamp == localNote.TickStamp &&
                     Compose(ChartVersion.Ma2_104).Equals(localNote.Compose(ChartVersion.Ma2_104));
        }

        return result;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}