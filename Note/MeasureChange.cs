namespace MaiLib;

using static NoteEnum;
using static ChartEnum;

/// <summary>
///     Defines measure change note that indicates a measure change in bar.
/// </summary>
public class MeasureChange : Note
{

    protected MeasureChange MeasureChangeFactory;
    
    #region Constructors

    /// <summary>
    ///     Construct Empty
    /// </summary>
    public MeasureChange()
    {
        NoteType = NoteType.MEASURE;
        Tick = 0;
        Quaver = 0;
        Update();
    }

    /// <summary>
    ///     Construct BPMChange with given bar, tick, BPM
    /// </summary>
    /// <param name="bar">Bar</param>
    /// <param name="tick">Tick</param>
    /// <param name="Quaver">Quaver</param>
    public MeasureChange(int bar, int tick, int quaver)
    {
        NoteType = NoteType.MEASURE;
        Bar = bar;
        Tick = tick;
        Quaver = quaver;
        Update();
    }

    /// <summary>
    ///     Construct measureChange from another takeIn
    /// </summary>
    /// <param name="takeIn">Another measure change note</param>
    public MeasureChange(MeasureChange takeIn)
    {
        NoteType = NoteType.MEASURE;
        Bar = takeIn.Bar;
        Tick = takeIn.Tick;
        Quaver = takeIn.Quaver;
        Update();
    }

    #endregion

    /// <summary>
    ///     Return this.quaver
    /// </summary>
    /// <value>Quaver</value>
    public int Quaver { get; }

    public override NoteEnum.NoteGenre NoteGenre => NoteEnum.NoteGenre.MEASURE;

    public override bool IsNote => false;

    public override NoteEnum.NoteSpecificGenre NoteSpecificGenre => NoteEnum.NoteSpecificGenre.MEASURE;

    public override bool CheckValidity()
    {
        return Quaver > 0;
    }

    public override string Compose(ChartVersion format)
    {
        switch (format)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
                return "{" + Quaver + "}";
            case ChartVersion.Debug:
                return "{" + Quaver + "_" + Tick + "}";
            default:
                return "";
        }
    }

    public override Note NewInstance()
    {
        MeasureChangeFactory ??= new MeasureChange(this);
        return MeasureChangeFactory;
    }
}