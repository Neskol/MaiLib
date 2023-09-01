namespace MaiLib;

/// <summary>
///     Constructs Hold Note
/// </summary>
public class Hold : Note
{
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

    /// <summary>
    ///     Stores if this Touch Hold have special effect
    /// </summary>
    private readonly int specialEffect;

    /// <summary>
    ///     Stores the size of touch hold
    /// </summary>
    private readonly string touchSize;

#region Constructors
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
        specialEffect = 0;
        touchSize = "M1";
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
        this.specialEffect = specialEffect;
        this.touchSize = touchSize;
        Update();
    }

    /// <summary>
    ///     Construct a Hold from another note
    /// </summary>
    /// <param name="inTake">The intake note</param>
    /// <exception cref="NullReferenceException">Will raise exception if touch size is null</exception>
    public Hold(Note inTake)
    {
        Note.Copy(this, inTake);
        if (inTake.NoteGenre == "HOLD")
        {
            touchSize = ((Hold)inTake).TouchSize ?? throw new NullReferenceException();
            specialEffect = ((Hold)inTake).SpecialEffect;
        }
        else
        {
            touchSize = "M1";
            specialEffect = 0;
        }
    }
    #endregion

    /// <summary>
    ///     Returns if the note comes with Special Effect
    /// </summary>
    /// <value>0 if no, 1 if yes</value>
    public int SpecialEffect => specialEffect;

    /// <summary>
    ///     Returns the size of the note
    /// </summary>
    /// <value>M1 if regular, L1 if large</value>
    public string TouchSize => touchSize;

    public override string NoteGenre => "HOLD";

    public override bool IsNote => true;

    public override string NoteSpecificGenre
    {
        get
        {
            var result = "HOLD";
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
                case "HLD":
                    result += Convert.ToInt32(Key) + 1;
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    result += "h";
                    break;
                case "XHO":
                    result += Convert.ToInt32(Key) + 1 + "xh";
                    break;
                case "THO":
                    result += Key.ToCharArray()[1] + (Convert.ToInt32(Key.Substring(0, 1)) + 1).ToString();
                    if (NoteSpecialState == SpecialState.Break)
                        result += "b";
                    else if (NoteSpecialState == SpecialState.EX)
                        result += "x";
                    else if (NoteSpecialState == SpecialState.BreakEX) result += "bx";
                    if (SpecialEffect == 1) result += "f";
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