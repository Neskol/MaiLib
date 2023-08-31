namespace MaiLib;

/// <summary>
///     Defines measure change note that indicates a measure change in bar.
/// </summary>
public class MeasureChange : Note
{
    #region Constructors
    /// <summary>
    ///     Construct Empty
    /// </summary>
    public MeasureChange()
    {
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

    public override string NoteGenre => "MEASURE";

    public override bool IsNote => false;

    public override string NoteSpecificGenre => "MEASURE";

    public override bool CheckValidity()
    {
        return Quaver > 0;
    }

    public override string Compose(int format)
    {
        var result = "";
        if (format == 0) result += "{" + Quaver + "}";
        //result += "{" + this.Quaver+"_"+this.Tick + "}";
        return result;
    }

    public override Note NewInstance()
    {
        Note result = new MeasureChange(this);
        return result;
    }
}