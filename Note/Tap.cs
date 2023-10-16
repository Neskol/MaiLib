namespace MaiLib;
using static MaiLib.NoteEnum;

/// <summary>
///     Tap note
/// </summary>
public class Tap : Note
{
    /// <summary>
    ///     Empty Constructor Tap Note
    /// </summary>
    public Tap()
    {
        TouchSize = "M1";
        SpecialEffect = false;
        Update();
    }

#region Constructor
    /// <summary>
    ///     Construct a Tap note
    /// </summary>
    /// <param name="noteType">TAP,STR,BRK,BST,XTP,XST,TTP; NST or NSS</param>
    /// <param name="key">0-7 representing each key</param>
    /// <param name="bar">Bar location</param>
    /// <param name="startTime">Start Location</param>
    public Tap(NoteType noteType, int bar, int startTime, string key)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        SpecialEffect = false;
        TouchSize = "M1";
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
    public Tap(NoteType noteType, int bar, int startTime, string key, bool specialEffect, string touchSize)
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
        inTake.CopyOver(this);
        //NoteType = inTake.NoteGenre.Equals("TAP") ? inTake.NoteType : "TAP";
        if (inTake.NoteGenre is NoteEnum.NoteGenre.TAP)
        {
            TouchSize = ((Tap)inTake).TouchSize ?? throw new NullReferenceException();
            SpecialEffect = ((Tap)inTake).SpecialEffect;
        }
        else
        {
            TouchSize = "M1";
            SpecialEffect = false;
            NoteType = NoteType.TAP;
        }
    }
#endregion

    public override NoteGenre NoteGenre => NoteGenre.TAP;

    public override bool IsNote =>
        // if (this.NoteType.Equals("NST"))
        // {
        //     return false;
        // }
        // else return true;
        true;

    public override NoteSpecificGenre NoteSpecificGenre
    {
        get
        {
            NoteSpecificGenre result;
            switch (NoteType)
            {

                case NoteType.STR:
                case NoteType.NST:
                case NoteType.NSS:
                    result = NoteSpecificGenre.SLIDE_START;
                    break;
                case NoteType.TTP:
                case NoteType.TAP:
                default:
                    result = NoteSpecificGenre.TAP;
                    break;
            }
            return result;
        }
    }

    //TODO: REWRITE THIS
    public override bool CheckValidity()
    {
        return true;
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
                     SpecialEffect + "\t" +
                     TouchSize; //M1 for regular note and L1 for Larger Note
        else if (format == 0)
            switch (NoteType)
            {
                case NoteType.TAP:
                    result += (int.Parse(Key) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    break;
                case NoteType.NSS:
                    result += (int.Parse(Key) + 1).ToString() + "$";
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    break;
                case NoteType.NST:
                    result += (int.Parse(Key) + 1).ToString() + "!";
                    break;
                case NoteType.STR:
                    result += (int.Parse(Key) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    break;
                case NoteType.TTP:
                    result += Key.ToCharArray()[1] + (Convert.ToInt32(Key.Substring(0, 1)) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    if (SpecialEffect) result += "f";
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