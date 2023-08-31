namespace MaiLib;

/// <summary>
///     Store measure change notes in a chart
/// </summary>
public class MeasureChanges
{
    #region Constructors
    /// <summary>
    ///     Construct an empty Measure Change
    /// </summary>
    public MeasureChanges()
    {
        Bar = new List<int>();
        Tick = new List<int>();
        Quavers = new List<int>();
        Beats = new List<int>();
        ChangeNotes = new List<MeasureChange>();
    }

    /// <summary>
    ///     Construct Measure Change with existing one
    /// </summary>
    public MeasureChanges(MeasureChanges takeIn)
    {
        Bar = new List<int>(takeIn.Bar);
        Tick = new List<int>(takeIn.Tick);
        Quavers = new List<int>(takeIn.Quavers);
        Beats = new List<int>(takeIn.Beats);
        ChangeNotes = new List<MeasureChange>(takeIn.ChangeNotes);
    }

    /// <summary>
    ///     Take in initial quavers and beats, incase MET_CHANGE is not specified
    /// </summary>
    /// <param name="initialQuaver">Initial Quaver</param>
    /// <param name="initialBeat">Initial Beat</param>
    public MeasureChanges(int initialQuaver, int initialBeat)
    {
        Bar = new List<int>();
        Tick = new List<int>();
        Quavers = new List<int>();
        Beats = new List<int>();
        ChangeNotes = new List<MeasureChange>();
        InitialQuavers = initialQuaver;
        InitialBeats = initialBeat;
    }

    /// <summary>
    ///     Construct a measure of given beats
    /// </summary>
    /// <param name="bar"></param>
    /// <param name="tick"></param>
    /// <param name="quavers"></param>
    /// <param name="beats"></param>
    public MeasureChanges(List<int> bar, List<int> tick, List<int> quavers, List<int> beats)
    {
        Bar = bar;
        Tick = tick;
        Quavers = quavers;
        InitialQuavers = quavers[0];
        Beats = beats;
        InitialBeats = beats[0];
        ChangeNotes = new List<MeasureChange>();
    }
    #endregion

    public List<int> Bar { get; }
    public List<int> Tick { get; }
    public List<int> Quavers { get; }
    public List<int> Beats { get; }
    public List<MeasureChange> ChangeNotes { get; }
    public int InitialQuavers { get; }
    public int InitialBeats { get; }

    /// <summary>
    ///     Return first definitions
    /// </summary>
    public string InitialChange => "MET_DEF" + "\t" + InitialQuavers + "\t" + InitialBeats + "\n";

    /// <summary>
    ///     Add new measure changes to MeasureChanges
    /// </summary>
    /// <param name="bar">Bar which changes</param>
    /// <param name="tick">Tick which changes</param>
    /// <param name="quavers">Quavers which changes</param>
    /// <param name="beats">Beat which changes</param>
    public void Add(int bar, int tick, int quavers, int beats)
    {
        Bar.Add(bar);
        Tick.Add(tick);
        Quavers.Add(quavers);
        Beats.Add(beats);
    }

    public bool CheckValidity()
    {
        var result = Bar.IndexOf(0) == 0;
        result = result && Tick.IndexOf(0) == 0;
        result = result && !Quavers[0].Equals(null);
        result = result && !Beats[0].Equals(null);
        return result;
    }

    public string Compose()
    {
        var result = "";
        if (Bar.Count == 0)
            result += "MET" + "\t" + 0 + "\t" + 0 + "\t" + 4 + "\t" + 4 + "\n";
        else
            for (var i = 0; i < Bar.Count; i++)
                result += "MET" + "\t" + Bar[i] + "\t" + Tick[i] + "\t" + Quavers[i] + "\t" + Beats[i] + "\n";
        return result;
    }

    public void Update()
    {
    }
}