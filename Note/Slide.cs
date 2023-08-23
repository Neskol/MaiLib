﻿namespace MaiLib;

/// <summary>
///     Construct a Slide note (With START!)
/// </summary>
public class Slide : Note
{
    /// <summary>
    ///     Defines the special property of the slide
    /// </summary>
    public enum SlideProperty
    {
        /// <summary>
        ///     Normal Slide
        /// </summary>
        Normal,

        /// <summary>
        ///     Slide without Start Tap
        /// </summary>
        NoStart,

        /// <summary>
        ///     Connecting Slide
        /// </summary>
        Connecting
    }

    /// <summary>
    ///     Defines what type of slide could be used
    /// </summary>
    public enum SlideType
    {
        #region SlideDescription

        /// <summary>
        ///     Straight Slide
        /// </summary>
        SI_,

        /// <summary>
        ///     Left circle slide aka Counterclockwise
        /// </summary>
        SCL,

        /// <summary>
        ///     Right circle slide aka Clockwise
        /// </summary>
        SCR,

        /// <summary>
        ///     Line not intercepting Crossing Center
        /// </summary>
        SV_,

        /// <summary>
        ///     U Star Left
        /// </summary>
        SUL,

        /// <summary>
        ///     U Star Right
        /// </summary>
        SUR,

        /// <summary>
        ///     Wifi Star
        /// </summary>
        SF_,

        /// <summary>
        ///     Inflecting Line Left
        /// </summary>
        SLL,

        /// <summary>
        ///     Inflecting Line Right
        /// </summary>
        SLR,

        /// <summary>
        ///     Self-winding Left
        /// </summary>
        SXL,

        /// <summary>
        ///     Self-winding Right
        /// </summary>
        SXR,

        /// <summary>
        ///     S Star
        /// </summary>
        SSL,

        /// <summary>
        ///     Z Star
        /// </summary>
        SSR

        #endregion
    }

    private readonly string[] allowedType =
        { "SI_", "SV_", "SF_", "SCL", "SCR", "SUL", "SUR", "SLL", "SLR", "SXL", "SXR", "SSL", "SSR" };

    /// <summary>
    ///     Empty Constructor
    /// </summary>
    public Slide()
    {
    }

    /// <summary>
    ///     Construct a Slide Note (Valid only if Start Key matches a start!)
    /// </summary>
    /// <param name="noteType">
    ///     SI_(Straight),SCL,SCR,SV_(Line not intercepting Crossing Center),SUL,SUR,SF_(Wifi),SLL(Infecting
    ///     Line),SLR(Infecting),SXL(Self winding),SXR(Self winding),SSL,SSR
    /// </param>
    /// <param name="key">0-7</param>
    /// <param name="bar">Bar in</param>
    /// <param name="startTime">Start Time</param>
    /// <param name="lastTime">Last Time</param>
    /// <param name="endKey">0-7</param>
    public Slide(string noteType, int bar, int startTime, string key, int waitTime, int lastTime, string endKey)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        WaitLength = waitTime;
        LastLength = lastTime;
        EndKey = endKey;
        Delayed = WaitLength != 96;
        Update();
    }

    /// <summary>
    ///     Construct a Slide from another note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    public Slide(Note inTake)
    {
        NoteType = inTake.NoteType;
        Key = inTake.Key;
        EndKey = inTake.EndKey;
        Bar = inTake.Bar;
        Tick = inTake.Tick;
        TickStamp = inTake.TickStamp;
        TimeStamp = inTake.TimeStamp;
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

    public override string NoteGenre => "SLIDE";

    public override bool IsNote => true;

    public override string NoteSpecificGenre => "SLIDE";

    public override bool CheckValidity()
    {
        var result = false;
        foreach (var x in allowedType) result = result || NoteType.Equals(x);
        result = result && NoteType.Length == 3;
        result = result && Key.Length <= 2;
        return result;
    }

    public override string Compose(int format)
    {
        var result = "";
        if (format == 1)
        {
            result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key + "\t" + WaitLength + "\t" + LastLength + "\t" +
                     EndKey;
        }
        else if (format == 0)
        {
            switch (NoteType)
            {
                case "SI_":
                    result += "-";
                    break;
                case "SV_":
                    result += "v";
                    break;
                case "SF_":
                    result += "w";
                    break;
                case "SCL":
                    if (int.Parse(Key) == 0 || int.Parse(Key) == 1 || int.Parse(Key) == 6 || int.Parse(Key) == 7)
                        result += "<";
                    else
                        result += ">";
                    break;
                case "SCR":
                    if (int.Parse(Key) == 0 || int.Parse(Key) == 1 || int.Parse(Key) == 6 || int.Parse(Key) == 7)
                        result += ">";
                    else
                        result += "<";
                    break;
                case "SUL":
                    result += "p";
                    break;
                case "SUR":
                    result += "q";
                    break;
                case "SSL":
                    result += "s";
                    break;
                case "SSR":
                    result += "z";
                    break;
                case "SLL":
                    result += "V" + GenerateInflection(this);
                    break;
                case "SLR":
                    result += "V" + GenerateInflection(this);
                    break;
                case "SXL":
                    result += "pp";
                    break;
                case "SXR":
                    result += "qq";
                    break;
            }

            result += (Convert.ToInt32(EndKey) + 1).ToString();
            if (NoteSpecialState == SpecialState.Break)
                result += "b";
            else if (NoteSpecialState == SpecialState.EX)
                result += "x";
            else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
            if (TickBPMDisagree || Delayed)
            {
                //result += GenerateAppropriateLength(this.LastLength, this.BPM);
                if (NoteSpecialState != SpecialState.ConnectingSlide && WaitLength != 96)
                    result += GenerateAppropriateLength(LastLength, BPM);
                else result += GenerateAppropriateLength(FixedLastLength);
            }
            else
            {
                result += GenerateAppropriateLength(LastLength);
            }
            //result += "_" + this.Tick;
            //result += "_" + this.Key;
        }

        return result;
    }

    /// <summary>
    ///     Return inflection point of SLL and SLR
    /// </summary>
    /// <param name="x">This note</param>
    /// <returns>Infection point of this note</returns>
    public static int GenerateInflection(Note x)
    {
        var result = int.Parse(x.Key) + 1;
        if (x.NoteType.Equals("SLR"))
            result += 2;
        else if (x.NoteType.Equals("SLL")) result -= 2;

        if (result > 8)
            result -= 8;
        else if (result < 1) result += 8;

        if (result == int.Parse(x.Key) + 1 || result == int.Parse(x.EndKey) + 1)
        {
            //Deal with result;
            if (result > 4)
                result -= 4;
            else if (result <= 4) result += 4;

            //Deal with note type;
            if (x.NoteType.Equals("SLL"))
                x.NoteType = "SLR";
            else if (x.NoteType.Equals("SLR"))
                x.NoteType = "SLL";
            else
                throw new InvalidDataException("INFLECTION POINT IS THE SAME WITH ONE OF THE KEY!");
        }

        return result;
    }

    public override Note NewInstance()
    {
        Note result = new Slide(this);
        return result;
    }
}