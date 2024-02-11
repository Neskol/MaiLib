namespace MaiLib;

using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;

/// <summary>
///     Constructs Hold Note
/// </summary>
public class Hold : Note
{
    #region Constructors

    /// <summary>
    ///     Construct a Hold Note
    /// </summary>
    /// <param name="noteType">HLD,XHO</param>
    /// <param name="key">Key of the hold note</param>
    /// <param name="bar">Bar of the hold note</param>
    /// <param name="startTick">Tick of the hold note</param>
    /// <param name="lastLength">Last time of the hold note in ticks</param>
    public Hold(NoteType noteType, int bar, int startTick, string key, int lastLength)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTick;
        LastLength = lastLength;
        SpecialEffect = false;
        TouchSize = "M1";
        // Update();
    }

    /// <summary>
    ///     Construct a Hold Note
    /// </summary>
    /// <param name="noteType">HLD,XHO</param>
    /// <param name="key">Key of the hold note</param>
    /// <param name="bar">Bar of the hold note</param>
    /// <param name="startTick">Tick of the hold note</param>
    /// <param name="lastTime">Last time of the hold note in seconds</param>
    public Hold(NoteType noteType, int bar, int startTick, string key, double lastTime)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTick;
        CalculatedLastTime = lastTime;
        SpecialEffect = false;
        TouchSize = "M1";
        // Update();
    }

    /// <summary>
    ///     Construct a Touch Hold Note
    /// </summary>
    /// <param name="noteType">THO</param>
    /// <param name="key">Key of the hold note</param>
    /// <param name="bar">Bar of the hold note</param>
    /// <param name="startTick">Tick of the hold note</param>
    /// <param name="lastTime">Last time of the hold note in seconds</param>
    /// <param name="specialEffect">Store if the touch note ends with special effect</param>
    /// <param name="touchSize">Determines how large the touch note is</param>
    public Hold(NoteType noteType, int bar, int startTick, string key, double lastTime, bool specialEffect,
        string touchSize)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTick;
        CalculatedLastTime = lastTime;
        SpecialEffect = specialEffect;
        TouchSize = touchSize;
        // Update();
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
    public Hold(NoteType noteType, int bar, int startTime, string key, int lastTime, bool specialEffect,
        string touchSize)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        LastLength = lastTime;
        SpecialEffect = specialEffect;
        TouchSize = touchSize;
        // Update();
    }

    /// <summary>
    ///     Construct a Hold from another note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    /// <exception cref="NullReferenceException">Will raise exception if touch size is null</exception>
    public Hold(Note inTake)
    {
        inTake.CopyOver(this);
        if (inTake.NoteGenre is not NoteEnum.NoteGenre.HOLD)
        {
            TouchSize = ((Hold)inTake).TouchSize ?? throw new NullReferenceException();
            SpecialEffect = ((Hold)inTake).SpecialEffect;
        }
        else
        {
            TouchSize = "M1";
            SpecialEffect = false;
        }
    }

    #endregion

    public override NoteGenre NoteGenre => NoteGenre.HOLD;

    public override bool IsNote => true;

    public override NoteSpecificGenre NoteSpecificGenre => NoteEnum.NoteSpecificGenre.HOLD;

    //TODO: REWRITE THIS
    public override bool CheckValidity()
    {
        return true;
    }

    public override string Compose(ChartVersion format)
    {
        string? result = "";
        switch (NoteType)
        {
            case NoteType.HLD:
                switch (format)
                {
                    case ChartVersion.Simai:
                    case ChartVersion.SimaiFes:
                    default:
                        result += KeyNum + 1;
                        if (NoteSpecialState == SpecialState.Break)
                            result += "b";
                        else if (NoteSpecialState == SpecialState.EX)
                            result += "x";
                        else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                        if (SpecialEffect) result += "f";
                        result += "h";
                        if (TickBPMDisagree || Delayed)
                            result += GenerateAppropriateLength(FixedLastLength);
                        else
                            result += GenerateAppropriateLength(LastLength);
                        if (format is ChartVersion.Debug) result += "_" + Tick;
                        break;
                    case ChartVersion.Ma2_103:
                        string typeCandidate =
                            NoteSpecialState is SpecialState.EX || NoteSpecialState is SpecialState.BreakEX
                                ? "XHO"
                                : NoteType.ToString();
                        result = typeCandidate + "\t" + Bar + "\t" + Tick + "\t" + Key + "\t" + LastLength;
                        break;
                    case ChartVersion.Ma2_104:
                        switch (NoteSpecialState)
                        {
                            case SpecialState.EX:
                                result += "EX";
                                break;
                            case SpecialState.Break:
                                result += "BR";
                                break;
                            case SpecialState.BreakEX:
                                result += "BX";
                                break;
                            case SpecialState.ConnectingSlide:
                                result += "CN";
                                break;
                            default:
                                result += "NM";
                                break;
                        }

                        result += NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key + "\t" + LastLength;
                        break;
                }

                break;
            case NoteType.THO:
                switch (format)
                {
                    case ChartVersion.Simai:
                    case ChartVersion.SimaiFes:
                    default:
                        result += KeyGroup + (KeyNum + 1).ToString();
                        if (NoteSpecialState == SpecialState.Break)
                            result += "b";
                        else if (NoteSpecialState == SpecialState.EX)
                            result += "x";
                        else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                        if (SpecialEffect) result += "f";
                        result += "h";
                        if (TickBPMDisagree || Delayed)
                            result += GenerateAppropriateLength(FixedLastLength);
                        else
                            result += GenerateAppropriateLength(LastLength);
                        if (format is ChartVersion.Debug) result += "_" + Tick;
                        break;
                    case ChartVersion.Ma2_103:
                        result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + KeyNum + "\t" + LastLength + "\t" +
                                 KeyGroup + "\t" + (SpecialEffect ? 1 : 0) + "\t" + TouchSize;
                        break;
                    case ChartVersion.Ma2_104:
                        switch (NoteSpecialState)
                        {
                            case SpecialState.EX:
                                result += "EX";
                                break;
                            case SpecialState.Break:
                                result += "BR";
                                break;
                            case SpecialState.BreakEX:
                                result += "BX";
                                break;
                            case SpecialState.ConnectingSlide:
                                result += "CN";
                                break;
                            default:
                                result += "NM";
                                break;
                        }

                        result += NoteType + "\t" + Bar + "\t" + Tick + "\t" + KeyNum + "\t" + LastLength + "\t" +
                                  KeyGroup + "\t" + (SpecialEffect ? 1 : 0) + "\t" + TouchSize;
                        break;
                }

                break;
        }

        return result;
    }

    public override Note NewInstance()
    {
        Note result = new Hold(this);
        return result;
    }
}