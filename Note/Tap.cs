namespace MaiLib;

/// <summary>
///     Tap note
/// </summary>
public class Tap : Note
{
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

    /// <summary>
    ///     Stores if the Touch note have special effect
    /// </summary>
    private int specialEffect;

    /// <summary>
    ///     Stores how big the note is: M1 for Regular and L1 for large
    /// </summary>
    private string touchSize;

    /// <summary>
    ///     Empty Constructor Tap Note
    /// </summary>
    public Tap()
    {
        touchSize = "M1";
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
        specialEffect = 0;
        touchSize = "M1";
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
        this.specialEffect = specialEffect;
        this.touchSize = touchSize;
        Update();
    }

    /// <summary>
    ///     Construct a Tap note form another note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    /// <exception cref="NullReferenceException">Will raise exception if touch size is null</exception>
    public Tap(Note inTake)
    {
        NoteType = inTake.NoteGenre.Equals("TAP") ? inTake.NoteType : "TAP";
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
        if (inTake.NoteGenre == "TAP")
        {
            touchSize = ((Tap)inTake).TouchSize ?? throw new NullReferenceException();
            SpecialEffect = ((Tap)inTake).SpecialEffect;
        }
        else
        {
            touchSize = "M1";
            SpecialEffect = 0;
        }
    }


    /// <summary>
    ///     Return this.specialEffect
    /// </summary>
    public int SpecialEffect
    {
        get => specialEffect;
        set => specialEffect = value;
    }

    /// <summary>
    ///     Return this.touchSize
    /// </summary>
    public string TouchSize
    {
        get => touchSize;
        set => touchSize = value;
    }

    public override string NoteGenre => "TAP";

    public override bool IsNote =>
        // if (this.NoteType.Equals("NST"))
        // {
        //     return false;
        // }
        // else return true;
        true;

    public override string NoteSpecificGenre
    {
        get
        {
            var result = "";
            switch (NoteType)
            {
                case "TAP":
                    result += "TAP";
                    break;
                case "STR":
                    result += "SLIDE_START";
                    break;
                case "BRK":
                    result += "TAP";
                    break;
                case "BST":
                    result += "SLIDE_START";
                    break;
                case "XTP":
                    result += "TAP";
                    break;
                case "XST":
                    result += "SLIDE_START";
                    break;
                case "TTP":
                    result += "TAP";
                    break;
                case "NST":
                    result += "SLIDE_START";
                    break;
                case "NSS":
                    result += "SLIDE_START";
                    break;
            }

            return result;
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
        // if (format == 1 && !(this.NoteType.Equals("TTP")) && !((this.NoteType.Equals("NST"))||this.NoteType.Equals("NSS")))
        // {
        //     result = this.NoteType + "\t" + this.Bar + "\t" + this.Tick + "\t" + this.Key;
        // }
        // else if (format == 1 && (this.NoteType.Equals("NST")||this.NoteType.Equals("NSS")))
        // {
        //     result = ""; //NST and NSS is just a place holder for slide
        // }
        if (format == 1 && !NoteType.Equals("TTP"))
            result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key;
        else if (format == 1 && NoteType.Equals("TTP"))
            result = NoteType + "\t" +
                     Bar + "\t" +
                     Tick + "\t" +
                     Key.ToCharArray()[1] + "\t" +
                     Key.ToCharArray()[0] + "\t" +
                     specialEffect + "\t" +
                     touchSize; //M1 for regular note and L1 for Larger Note
        else if (format == 0)
            switch (NoteType)
            {
                case "TAP":
                    result += (int.Parse(Key) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    break;
                case "STR":
                    result += (int.Parse(Key) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    break;
                case "BRK":
                    result += (int.Parse(Key) + 1) + "b";
                    break;
                case "BST":
                    result += (int.Parse(Key) + 1) + "b";
                    break;
                case "XTP":
                    result += (int.Parse(Key) + 1) + "x";
                    break;
                case "XST":
                    result += (int.Parse(Key) + 1) + "x";
                    break;
                case "NST":
                    result += (int.Parse(Key) + 1) + "!";
                    break;
                case "TTP":
                    result += Key.ToCharArray()[1] + (Convert.ToInt32(Key.Substring(0, 1)) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    if (SpecialEffect == 1) result += "f";
                    break;
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