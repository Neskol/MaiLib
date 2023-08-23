namespace MaiLib;

/// <summary>
///     Constructs Hold Note
/// </summary>
public class Hold : Note
{
    #region TypeEnums
    public enum HoldType
    {
        /// <summary>
        ///     Normal Hold
        /// </summary>
        HLD,

        /// <summary>
        ///     Touch Hold
        /// </summary>
        THO
    }

    /// <summary>
    ///     Stores enums of accepting Hold type
    /// </summary>
    /// <value></value>
    private readonly string[] allowedType = { "HLD", "XHO", "THO" };
    #endregion

    /// <summary>
    ///     Stores if this Touch Hold have special effect
    /// </summary>
    public int SpecialEffect { get; protected set; }

    /// <summary>
    ///     Stores the size of touch hold
    /// </summary>
    public string TouchSize { get; protected set; }

    /// <summary>
    ///     Construct a Hold Note
    /// </summary>
    /// <param name="noteType">HLD,XHO</param>
    /// <param name="key">Key of the hold note</param>
    /// <param name="bar">Bar of the hold note</param>
    /// <param name="startTime">Tick of the hold note</param>
    /// <param name="lastTime">Last time of the hold note</param>
    public Hold(string noteType, int bar, int startTime, string key, int lastTime)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        LastLength = lastTime;
        SpecialEffect = 0;
        TouchSize = "M1";
        NoteGenre = NoteGeneralCategories.Hold;
        Update();
    }

    /// <summary>
    ///     Construct a Touch Hold Note
    /// </summary>
    /// <param name="noteType">THO</param>
    /// <param name="key">Key of the hold note</param>
    /// <param name="bar">Bar of the hold note</param>
    /// <param name="startTime">Tick of the hold note</param>
    /// <param name="lastTime">Last time of the hold note</param>
    /// <param name="specialEffect">Store if the touch note ends with special effect</param>
    /// <param name="touchSize">Determines how large the touch note is</param>
    public Hold(string noteType, int bar, int startTime, string key, int lastTime, int specialEffect, string touchSize)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        LastLength = lastTime;
        SpecialEffect = specialEffect;
        TouchSize = touchSize;
        NoteGenre = NoteGeneralCategories.Hold;
        Update();
    }

    /// <summary>
    ///     Construct a Hold from another note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    /// <exception cref="NullReferenceException">Will raise exception if touch size is null</exception>
    public Hold(Note inTake)
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
        BPMChangeNotes = inTake.BPMChangeNotes;
        if (inTake.NoteGenre == NoteGeneralCategories.Hold)
        {
            TouchSize = ((Hold)inTake).TouchSize ?? throw new NullReferenceException();
            SpecialEffect = ((Hold)inTake).SpecialEffect;
        }
        else
        {
            TouchSize = "M1";
            SpecialEffect = 0;
        }
        NoteGenre = NoteGeneralCategories.Hold;
        Update();
    }

    public override bool IsNote => true;

    public override NoteSpecificCategories NoteSpecificGenre
    {
        get
        {
            var result = NoteSpecificCategories.Hold;
            switch (NoteType)
            {
                case "THO":
                    result =  NoteSpecificCategories.HoldTouch;
                    break;
                default:
                    result = NoteSpecificCategories.Hold;
                    break;

            }
            return result;
        }
        protected set
        {
        }
    }

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
        if (format == 1 && !NoteType.Equals("THO"))
        {
            result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key + "\t" + LastLength;
        }
        else if (format == 1 && NoteType.Equals("THO"))
        {
            result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + KeyNum + "\t" + LastLength + "\t" +
                     KeyGroup + "\t" + SpecialEffect + "\t" + TouchSize; //M1 for regular note and L1 for Larger Note
        }
        else if (format == 0)
        {
            switch (NoteSpecificGenre)
            {
                case NoteSpecificCategories.Hold:
                    result += (KeyNum + 1).ToString() + "h";
                    break;
                case NoteSpecificCategories.HoldTouch:
                    result += KeyGroup + (KeyNum + 1) + "h";
                    break;
                default:
                    throw new InvalidCastException("This TAP does not belongs to any category");
            }
            switch (NoteSpecialState)
            {
                case SpecialState.Break:
                    result += "b";
                    break;
                case SpecialState.EX:
                    result += "x";
                    break;
                case SpecialState.BreakEX:
                    result += "bx";
                    break;
                case SpecialState.Fireworks:
                    result += "f";
                    break;
                case SpecialState.SingularStart:
                    result += "!";
                    break;
            }
            result += GenerateAppropriateLength(LastLength);
        }

        return result;
    }

    public override Note NewInstance()
    {
        Note result = new Hold(this);
        return result;
    }
}