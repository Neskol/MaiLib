namespace MaiLib;
using static MaiLib.NoteEnum;

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
    /// <param name="startTime">Tick of the hold note</param>
    /// <param name="lastTime">Last time of the hold note</param>
    public Hold(NoteType noteType, int bar, int startTime, string key, int lastTime)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        LastLength = lastTime;
        SpecialEffect = false;
        TouchSize = "M1";
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
    public Hold(NoteType noteType, int bar, int startTime, string key, int lastTime, bool specialEffect, string touchSize)
    {
        NoteType = noteType;
        Key = key;
        Bar = bar;
        Tick = startTime;
        LastLength = lastTime;
        SpecialEffect = specialEffect;
        TouchSize = touchSize;
        Update();
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

    public override string Compose(int format)
    {
        var result = "";
        if (format == 1 && !NoteType.Equals("THO"))
        {
            result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key + "\t" + LastLength;
        }
        else if (format == 1 && NoteType.Equals("THO"))
        {
            result = NoteType + "\t" + Bar + "\t" + Tick + "\t" + Key.ToCharArray()[0] + "\t" + LastLength + "\t" +
                     Key.ToCharArray()[1] + "\t" + SpecialEffect + "\tM1"; //M1 for regular note and L1 for Larger Note
        }
        else if (format == 0)
        {
            switch (NoteType)
            {
                case NoteType.HLD:
                    result += Convert.ToInt32(Key) + 1;
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    result += "h";
                    break;
                case NoteType.XHO:
                    result += Convert.ToInt32(Key) + 1 + "xh";
                    break;
                case NoteType.THO:
                    result += Key.ToCharArray()[1] + (Convert.ToInt32(Key.Substring(0, 1)) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    if (SpecialEffect) result += "f";
                    result += "h";
                    break;
            }

            if (TickBPMDisagree || Delayed)
                result += GenerateAppropriateLength(FixedLastLength);
            else
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