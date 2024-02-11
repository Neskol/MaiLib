namespace MaiLib;

using static NoteEnum;
using static ChartEnum;

/// <summary>
///     BPMChange note for Simai
/// </summary>
public class BPMChange : Note
{
    #region Constructors

    /// <summary>
    ///     Construct Empty
    /// </summary>
    public BPMChange()
    {
        NoteType = NoteEnum.NoteType.BPM;
        Key = "";
        Update();
    }

    /// <summary>
    ///     Construct BPMChange with given bar, tick, BPM
    /// </summary>
    /// <param name="bar">Bar</param>
    /// <param name="startTime">tick</param>
    /// <param name="BPM">BPM</param>
    public BPMChange(int bar, int startTime, double BPM)
    {
        NoteType = NoteEnum.NoteType.BPM;
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
        NoteType = NoteEnum.NoteType.BPM;
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
        NoteType = NoteEnum.NoteType.BPM;
        Bar = takeIn.Bar;
        Tick = takeIn.Tick;
        Update();
    }

    #endregion

    public override double BPM { get; protected internal set; }

    public double BPMTimeUnit => 60 / BPM * 4 / Definition;

    public override NoteGenre NoteGenre => NoteEnum.NoteGenre.BPM;

    public override bool IsNote => true;

    public override NoteSpecificGenre NoteSpecificGenre => NoteEnum.NoteSpecificGenre.BPM;


    public override bool CheckValidity()
    {
        return BPM != 0;
    }

    public override string Compose(ChartVersion format)
    {
        switch (format)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
                return "(" + BPM + ")";
            case ChartVersion.Debug:
                return "(" + BPM + "_" + Tick + ")";
            default:
                return "";
        }
    }

    public override bool Equals(object? obj)
    {
        bool result = false;
        if (this == obj && this == null)
        {
            result = true;
        }
        else if (this != null && obj != null)
        {
            BPMChange? candidate = (BPMChange)obj;
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