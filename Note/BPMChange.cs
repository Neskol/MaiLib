namespace MaiLib;

/// <summary>
///     BPMChange note for Simai
/// </summary>
public class BPMChange : Note
{
    /// <summary>
    ///     Construct Empty
    /// </summary>
    public BPMChange()
    {
        NoteType = NoteEnum.NoteType.BPM;
        Key = "";
        Bar = 0;
        Tick = 0;
        BPM = 0;
        Update();
    }
#region Constructors
    /// <summary>
    ///     Construct BPMChange with given bar, tick, BPM
    /// </summary>
    /// <param name="bar">Bar</param>
    /// <param name="startTime">tick</param>
    /// <param name="BPM">BPM</param>
    public BPMChange(int bar, int startTime, double BPM)
    {
        Bar = bar;
        Tick = startTime;
        this.BPM = BPM;
        Update();
    }

    /// <summary>
    ///     Construct BPMChange with take in value
    /// </summary>
    /// <param name="takeIn">Take in BPMChange</param>
    public BPMChange(BPMChange takeIn)
    {
        Bar = takeIn.Bar;
        Tick = takeIn.Tick;
        BPM = takeIn.BPM;
        Update();
    }

    /// <summary>
    ///     Construct BPMChange with take in value
    /// </summary>
    /// <param name="takeIn">Take in note</param>
    public BPMChange(Note takeIn)
    {
        Bar = takeIn.Bar;
        Tick = takeIn.Tick;
        Update();
    }
#endregion

    public override NoteEnum.NoteGenre NoteGenre => NoteEnum.NoteGenre.BPM;

    public override bool IsNote => true;

    public override NoteEnum.NoteSpecificGenre NoteSpecificGenre => NoteEnum.NoteSpecificGenre.BPM;


    public override bool CheckValidity()
    {
        return BPM != 0;
    }

    public override string Compose(int format)
    {
        var result = "";
        if (format == 0) result += "(" + BPM + ")";
        //result += "(" + this.BPM + "_" + this.Bar + "_" + this.Tick + ")";
        //else result += "(" + this.BPM + "_" + this.Bar + "_" + this.Tick + ")";
        return result;
    }

    public override bool Equals(object? obj)
    {
        var result = false;
        if (this == obj && this == null)
        {
            result = true;
        }
        else if (this != null && obj != null)
        {
            var candidate = (BPMChange)obj;
            if (GetHashCode() == candidate.GetHashCode())
                result = true;
            else if (Bar == candidate.Bar)
                if (Tick == candidate.Tick && BPM == candidate.BPM)
                    result = true;
        }

        return result;
    }

    public override Note NewInstance()
    {
        Note result = new BPMChange(this);
        return result;
    }

    public override int GetHashCode()
    {
        // string hash = this.Bar + "0" + this.Tick + "0" + this.BPM;
        // return int.Parse(hash);
        return base.GetHashCode();
    }
}