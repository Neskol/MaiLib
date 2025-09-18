namespace MaiLib;

using static NoteEnum;
using static ChartEnum;

/// <summary>
///     Construct a Slide note (With START!)
/// </summary>
public class Slide : Note
{
    #region Constructors

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
    /// <param name="startTick">Start Time</param>
    /// <param name="lastLength">Last Time</param>
    /// <param name="endKey">0-7</param>
    public Slide(NoteType noteType, int bar, int startTick, string key, int waitLength, int lastLength, string endKey)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTick;
        WaitLength = waitLength;
        LastLength = lastLength;
        EndKey = endKey;
        // Delayed = WaitLength != 96;
        Update();
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
    /// <param name="startTick">Start Time</param>
    /// <param name="waitTime">Wait Time in Seconds</param>
    /// <param name="lastTime">Last Time in Seconds</param>
    /// <param name="endKey">0-7</param>
    public Slide(NoteType noteType, int bar, int startTick, double waitTime, double lastTime, string key, string endKey)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTick;
        CalculatedWaitTime = waitTime;
        CalculatedLastTime = lastTime;
        EndKey = endKey;
        // Delayed = WaitLength != 96;
        // Update(); // This update could cause issue when change table is not yet assigned or update.
    }

    /// <summary>
    ///     Construct a Slide from another note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    public Slide(Note inTake)
    {
        inTake.CopyOver(this);
    }

    #endregion

    public override bool Delayed => WaitLength != Definition / 4;

    public override NoteGenre NoteGenre => NoteGenre.SLIDE;

    public override bool IsNote => true;

    public override NoteSpecificGenre NoteSpecificGenre => NoteSpecificGenre.SLIDE;

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
                    #region DetailedSlideTypes

                    case NoteType.SI_:
                        result += "-";
                        break;
                    case NoteType.SV_:
                        result += "v";
                        break;
                    case NoteType.SF_:
                        result += "w";
                        break;
                    case NoteType.SCL:
                        if (int.Parse(Key) == 0 || int.Parse(Key) == 1 || int.Parse(Key) == 6 || int.Parse(Key) == 7)
                            result += "<";
                        else
                            result += ">";
                        break;
                    case NoteType.SCR:
                        if (int.Parse(Key) == 0 || int.Parse(Key) == 1 || int.Parse(Key) == 6 || int.Parse(Key) == 7)
                            result += ">";
                        else
                            result += "<";
                        break;
                    case NoteType.SUL:
                        result += "p";
                        break;
                    case NoteType.SUR:
                        result += "q";
                        break;
                    case NoteType.SSL:
                        result += "s";
                        break;
                    case NoteType.SSR:
                        result += "z";
                        break;
                    case NoteType.SLL:
                        result += "V" + GenerateInflection(this);
                        break;
                    case NoteType.SLR:
                        result += "V" + GenerateInflection(this);
                        break;
                    case NoteType.SXL:
                        result += "pp";
                        break;
                    case NoteType.SXR:
                        result += "qq";
                        break;

                    #endregion
                }

                result += (EndKeyNum + 1).ToString();
                if (NoteSpecialState == SpecialState.Break)
                    result += "b";
                else if (NoteSpecialState == SpecialState.EX)
                    result += "x";
                else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                if (TickBPMDisagree || Delayed)
                {
                    //result += GenerateAppropriateLength(this.LastLength, this.BPM);
                    if (NoteSpecialState != SpecialState.ConnectingSlide)
                        result += GenerateAppropriateLength(LastLength, BPM);
                    else result += GenerateAppropriateLength(FixedLastLength); //TODO: FIX THIS LAST LENGTH
                }
                else
                {
                    result += GenerateAppropriateLength(LastLength);
                }

                if (format is ChartVersion.Debug)
                {
                    result += "_" + Tick;
                    result += "_" + Key;
                }

                break;
            case ChartVersion.Ma2_103:
                result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key + "\t" + WaitLength + "\t" + LastLength +
                         "\t" +
                         EndKey;
                break;
            case ChartVersion.Ma2_104:
            case ChartVersion.Ma2_105:
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

                result += NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key + "\t" + WaitLength + "\t" + LastLength +
                          "\t" +
                          EndKey;
                break;
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
        int result = x.KeyNum + 1;
        if (x.NoteType is NoteType.SLR)
            result += 2;
        else if (x.NoteType is NoteType.SLL) result -= 2;

        if (result > 8)
            result -= 8;
        else if (result < 1) result += 8;

        if (result == x.KeyNum + 1 || result == x.EndKeyNum + 1)
        {
            //Deal with result;
            if (result > 4)
                result -= 4;
            else if (result <= 4) result += 4;

            //Deal with note type;
            if (x.NoteType is NoteType.SLL)
                x.NoteType = NoteType.SLR;
            else if (x.NoteType is NoteType.SLR)
                x.NoteType = NoteType.SLL;
            else
                throw new InvalidDataException("INFLECTION CANNOT BE USED OTHER THAN SLL AND SLR!");
        }

        return result;
    }

    public override Note NewInstance()
    {
        Note result = new Slide(this);
        return result;
    }
}