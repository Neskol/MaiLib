namespace MaiLib;

using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;

/// <summary>
///     Tap note
/// </summary>
public class Tap : Note
{
    protected Tap? TapFactory;

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

    public override string Compose(ChartVersion format)
    {
        string? result = "";
        switch (format)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
            default:
                switch (NoteType)
                {
                    case NoteType.NST:
                        result += (KeyNum + 1).ToString() + "!";
                        break;
                    case NoteType.NSS:
                        result += (KeyNum + 1).ToString() + "$";
                        break;
                    case NoteType.TTP:
                        result += KeyGroup + (KeyNum + 1).ToString();
                        break;
                    default:
                        result += (KeyNum + 1).ToString();
                        break;
                }

                if (NoteSpecialState == SpecialState.Break)
                    result += "b";
                else if (NoteSpecialState == SpecialState.EX)
                    result += "x";
                else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                if (SpecialEffect) result += "f";
                if (format is ChartVersion.Debug) result += "_" + Tick;
                break;
            case ChartVersion.Ma2_103:
                string typeCandidate = NoteType.ToString();
                switch (NoteSpecialState)
                {
                    case SpecialState.EX:
                        typeCandidate = NoteSpecificGenre is NoteSpecificGenre.SLIDE_START ? "XST" : "XTP";
                        break;
                    case SpecialState.Break:
                    case SpecialState.BreakEX:
                        typeCandidate = NoteSpecificGenre is NoteSpecificGenre.SLIDE_START ? "BST" : "BRK";
                        break;
                }

                result = NoteType is NoteType.TTP
                    ? typeCandidate + "\t" +
                      Bar + "\t" +
                      Tick + "\t" +
                      KeyNum + "\t" +
                      KeyGroup + "\t" +
                      (SpecialEffect ? 1 : 0) + "\t" +
                      TouchSize
                    : typeCandidate + "\t" + Bar + "\t" + Tick + "\t" + Key;
                break;
            case ChartVersion.Ma2_104:
                typeCandidate = NoteType.ToString();
                switch (NoteSpecialState)
                {
                    case SpecialState.EX:
                        typeCandidate = "EX" + typeCandidate;
                        break;
                    case SpecialState.Break:
                        typeCandidate = "BR" + typeCandidate;
                        break;
                    case SpecialState.BreakEX:
                        typeCandidate = "BX" + typeCandidate;
                        break;
                    default:
                        typeCandidate = "NM" + typeCandidate;
                        break;
                }

                result = NoteType is NoteType.TTP
                    ? typeCandidate + "\t" +
                      Bar + "\t" +
                      Tick + "\t" +
                      KeyNum + "\t" +
                      KeyGroup + "\t" +
                      (SpecialEffect ? 1 : 0) + "\t" +
                      TouchSize
                    : typeCandidate + "\t" + Bar + "\t" + Tick + "\t" + Key;
                break;
        }

        return result;
    }

    public override Note NewInstance()
    {
        TapFactory ??= new Tap(this);
        return TapFactory;
    }
}