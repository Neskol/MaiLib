namespace MaiLib;

/// <summary>
///     Tap note
/// </summary>
public class Tap : Note
{
    #region TypeEnums
    /// <summary>
    ///     The allowed tap type
    /// </summary>
    public enum TapType
    {
        /// <summary>
        ///     Normal tap
        /// </summary>
        TAP,

        /// <summary>
        ///     Start of a slide
        /// </summary>
        STR,

        /// <summary>
        ///     Start of a slide but have no consecutive slide following
        /// </summary>
        NST,

        /// <summary>
        ///     Touch Note
        /// </summary>
        TTP
    }

    /// <summary>
    ///     Stores enums of accepting tap notes
    /// </summary>
    /// <value></value>
    private readonly string[] allowedType = { "TAP", "STR", "BRK", "BST", "XTP", "XST", "TTP", "NST" };
    #endregion

    /// <summary>
    ///     Stores if the Touch note have special effect
    /// </summary>
    public int SpecialEffect { get; private set; }

    /// <summary>
    ///     Stores how big the note is: M1 for Regular and L1 for large
    /// </summary>
    public string TouchSize { get; private set; }

    /// <summary>
    ///     Empty Constructor Tap Note
    /// </summary>
    public Tap()
    {
        TouchSize = "M1";
        NoteGenre = NoteGeneralCategories.Tap;
        Update();
    }

    /// <summary>
    ///     Construct a Tap note
    /// </summary>
    /// <param name="noteType">TAP,STR,BRK,BST,XTP,XST,TTP; NST or NSS</param>
    /// <param name="key">0-7 representing each key</param>
    /// <param name="bar">Bar location</param>
    /// <param name="startTime">Start Location</param>
    public Tap(string noteType, int bar, int startTime, string key)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        SpecialEffect = 0;
        TouchSize = "M1";
        NoteGenre = NoteGeneralCategories.Tap;
        Update();
    }

    /// <summary>
    ///     Construct a Touch note with parameter taken in
    /// </summary>
    /// <param name="noteType">"TTP"</param>
    /// <param name="bar">Bar location</param>
    /// <param name="startTime">Start Location</param>
    /// <param name="key">Key</param>
    /// <param name="specialEffect">Effect after touch</param>
    /// <param name="touchSize">L=larger notes M=Regular</param>
    public Tap(string noteType, int bar, int startTime, string key, int specialEffect, string touchSize)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        SpecialEffect = specialEffect;
        TouchSize = touchSize;
        Update();
    }

    /// <summary>
    ///     Construct a Tap note form another note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    /// <exception cref="NullReferenceException">Will raise exception if touch size is null</exception>
    public Tap(Note inTake)
    {
        NoteType = inTake.NoteGenre is NoteGeneralCategories.Tap ? inTake.NoteType : "TAP";
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
        if (inTake.NoteSpecificGenre is NoteSpecificCategories.TapTouch)
        {
            TouchSize = ((Tap)inTake).TouchSize ?? throw new NullReferenceException();
            SpecialEffect = ((Tap)inTake).SpecialEffect;
        }
        else
        {
            TouchSize = "M1";
            SpecialEffect = 0;
        }
    }

    public override bool IsNote =>
        true;

    public override NoteSpecificCategories NoteSpecificGenre
    {
        get
        {
            NoteSpecificCategories result = NoteSpecificCategories.Tap;
            switch (NoteType)
            {
                case "STR":
                case "BST":
                case "NST":
                case "NSS":
                    result = NoteSpecificCategories.TapStart;
                    break;
                case "TTP":
                case "XTP":
                    result = NoteSpecificCategories.TapTouch;
                    break;
                case "TAP":
                case "BRK":
                case "XST":
                default:
                    result = NoteSpecificCategories.Tap;
                    break;
            }
            return result;
        }
        protected set { }
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
        if (format == 1 && NoteSpecificGenre is not NoteSpecificCategories.TapTouch)
            result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key;
        else if (format == 1 && NoteSpecificGenre is NoteSpecificCategories.TapTouch)
            result = NoteType + "\t" +
                     Bar + "\t" +
                     Tick + "\t" +
                     KeyNum + "\t" +
                     Key.ToCharArray()[0] + "\t" +
                     SpecialEffect + "\t" +
                     TouchSize; //M1 for regular note and L1 for Larger Note
        else if (format == 0)
        {
            switch (NoteSpecificGenre)
            {
                case NoteSpecificCategories.Tap:
                case NoteSpecificCategories.TapStart:
                    result += (KeyNum + 1).ToString();
                    break;
                case NoteSpecificCategories.TapTouch:
                    result += KeyGroup + (KeyNum + 1);
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
        }
        //result += "_" + this.Tick;
        return result;
    }

    public override Note NewInstance()
    {
        Note result = new Tap(this);
        return result;
    }
}