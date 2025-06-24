namespace MaiLib;

using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;
using System.Text;

public class Simai : Chart
{
    #region Constructors

    /// <summary>
    ///     Empty constructor
    /// </summary>
    public Simai()
    {
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
    }

    /// <summary>
    ///     Constructs Simai chart directly from path specified
    /// </summary>
    /// <param name="location"></param>
    public Simai(string location)
    {
        string[]? tokens = new SimaiTokenizer().Tokens(location);
        Chart? chart = new SimaiParser().ChartOfToken(tokens);
        Notes = new List<Note>(chart.Notes);
        BPMChanges = new BPMChanges(chart.BPMChanges);
        MeasureChanges = new MeasureChanges(chart.MeasureChanges);
        Information = new Dictionary<string, string>(chart.Information);
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
        Update();
    }

    /// <summary>
    ///     Construct Simai from given parameters
    /// </summary>
    /// <param name="notes">Notes to take in</param>
    /// <param name="bpmChanges">BPM change to take in</param>
    /// <param name="measureChanges">Measure change to take in</param>
    public Simai(List<Note> notes, BPMChanges bpmChanges, MeasureChanges measureChanges)
    {
        Notes = notes;
        BPMChanges = bpmChanges;
        MeasureChanges = measureChanges;
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
        this.Update();
    }

    public Simai(Chart takenIn)
    {
        Notes = takenIn.Notes;
        BPMChanges = takenIn.BPMChanges;
        MeasureChanges = takenIn.MeasureChanges;
        ChartType = ChartType.Standard;
        ChartVersion = ChartVersion.Simai;
        this.Update();
    }

    #endregion

    public override string Compose()
    {
        Update();
        switch (ChartVersion)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
                StringBuilder result = new();
                Note? mostDelayedNote = Notes.MaxBy(note => note.LastTickStamp);
                if (mostDelayedNote is not null)
                {
                    TotalDelay = mostDelayedNote.LastTickStamp - StoredChart.Count * Definition;
                }
                int delayBar = TotalDelay / Definition + 2;
                List<Note> firstBpm = [];
                foreach (Note? bpm in Notes)
                    if (bpm.NoteSpecificGenre is NoteSpecificGenre.BPM)
                        firstBpm.Add(bpm);
                foreach (List<Note>? bar in StoredChart)
                {
                    Note lastNote = new MeasureChange();
                    int currentQuaver = 0;
                    int commaCompiled = 0;
                    foreach (Note? x in bar)
                    {
                        switch (lastNote.NoteSpecificGenre)
                        {
                            case NoteSpecificGenre.MEASURE:
                                currentQuaver = (lastNote as MeasureChange ??
                                                 throw new InvalidCastException("This note is not measure change")).Quaver;
                                break;
                            case NoteSpecificGenre.BPM:
                                break;
                            default:
                                if (x.IsOfSameTime(lastNote) && x.IsNote && lastNote.IsNote)
                                {
                                    result.Append('/');
                                }
                                else
                                {
                                    result.Append(',');
                                    commaCompiled++;
                                }

                                break;
                        }

                        result.Append(x.Compose(ChartVersion));
                        lastNote = x;
                    }

                    result.Append(",\n");
                    commaCompiled++;
                    if (commaCompiled != currentQuaver)
                    {
                        Console.WriteLine("Notes in bar: " + bar[0].Bar);
                        foreach (Note? x in bar) Console.WriteLine(x.Compose(ChartVersion.Debug));
                        Console.WriteLine(result);
                        Console.WriteLine("Expected comma number: " + currentQuaver);
                        Console.WriteLine("Actual comma number: " + commaCompiled);
                        throw new NullReferenceException("COMMA COMPILED MISMATCH IN BAR " + bar[0].Bar);
                    }
                }

                for (int i = 0; i < delayBar + 1; i++) result.Append("{1},\n");
                result.Append("E\n");
                return result.ToString();
            default:
                return base.Compose();
        }
    }

    public override bool CheckValidity()
    {
        bool result = this == null;
        // Not yet implemented
        return result;
    }

    /// <summary>
    ///     Reconstruct the chart with given arrays
    /// </summary>
    /// <param name="bpm">New BPM Changes</param>
    /// <param name="measure">New Measure Changes</param>
    /// <returns>New Composed Chart</returns>
    public override string Compose(BPMChanges bpm, MeasureChanges measure)
    {
        BPMChanges? sourceBPM = BPMChanges;
        MeasureChanges? sourceMeasures = MeasureChanges;
        BPMChanges = bpm;
        MeasureChanges = measure;
        Update();

        string? result = Compose();
        BPMChanges = sourceBPM;
        MeasureChanges = sourceMeasures;
        Update();
        return result;
    }

    public override void Update()
    {
        ComposeSlideGroup();
        ComposeSlideEachGroup();
        base.Update();
    }
}