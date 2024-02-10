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
        var tokens = new SimaiTokenizer().Tokens(location);
        var chart = new SimaiParser().ChartOfToken(tokens);
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
                StringBuilder result = new StringBuilder();
                Note? mostDelayedNote = Notes.MaxBy(note => note.LastTickStamp);
                if (mostDelayedNote is not null)
                {
                    TotalDelay = mostDelayedNote.LastTickStamp - StoredChart.Count * Definition;
                }
                var delayBar = TotalDelay / Definition + 2;
                //Console.WriteLine(chart.Compose());
                //foreach (BPMChange x in chart.BPMChanges.ChangeNotes)
                //{
                //    Console.WriteLine("BPM Change verified in " + x.Bar + " " + x.Tick + " of BPM" + x.BPM);
                //}
                var firstBpm = new List<Note>();
                foreach (var bpm in Notes)
                    if (bpm.NoteSpecificGenre is NoteSpecificGenre.BPM)
                        firstBpm.Add(bpm);
                // if (firstBpm.Count > 1)
                // {
                //     chart.Chart[0][0] = firstBpm[1];
                // }
                foreach (var bar in StoredChart)
                {
                    Note lastNote = new MeasureChange();
                    var currentQuaver = 0;
                    var commaCompiled = 0;
                    //result += bar[1].Bar;
                    foreach (var x in bar)
                    {
                        //if (x.Bar == 6)
                        //{
                        //    Console.WriteLine("This is bar 6");
                        //}
                        switch (lastNote.NoteSpecificGenre)
                        {
                            case NoteSpecificGenre.MEASURE:
                                currentQuaver = (lastNote as MeasureChange ??
                                                 throw new Exception("This note is not measure change")).Quaver;
                                break;
                            case NoteSpecificGenre.BPM:
                                break;
                            default:
                                if (x.IsOfSameTime(lastNote) && x.IsNote && lastNote.IsNote)
                                {
                                    result.Append("/");
                                }
                                else
                                {
                                    result.Append(",");
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
                        foreach (var x in bar) Console.WriteLine(x.Compose(ChartVersion.Debug));
                        Console.WriteLine(result);
                        Console.WriteLine("Expected comma number: " + currentQuaver);
                        Console.WriteLine("Actual comma number: " + commaCompiled);
                        throw new NullReferenceException("COMMA COMPILED MISMATCH IN BAR " + bar[0].Bar);
                    }
                }

                // for (var i = 0; i < delayBar + 1; i++) result.Append("{1},\n");
                result.Append("E\n");
                return result.ToString();
            default:
                return base.Compose();
        }
    }

    public override bool CheckValidity()
    {
        var result = this == null;
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
        var sourceBPM = BPMChanges;
        var sourceMeasures = MeasureChanges;
        BPMChanges = bpm;
        MeasureChanges = measure;
        Update();

        var result = Compose();
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