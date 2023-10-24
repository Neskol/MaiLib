namespace MaiLib;
using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;
using System.Text;

/// <summary>
///     Implementation of chart in ma2 format.
/// </summary>
public class Ma2 : Chart, ICompiler
{
    #region Constructors
    /// <summary>
    ///     Default Constructor.
    /// </summary>
    public Ma2()
    {
        ChartType = ChartType.Standard;
        ChartType = ChartType.DX;
    }

    /// <summary>
    ///     Construct Ma2 with given notes, bpm change definitions and measure change definitions.
    /// </summary>
    /// <param name="notes">Notes in Ma2</param>
    /// <param name="bpmChanges">BPM Changes: Initial BPM is NEEDED!</param>
    /// <param name="measureChanges">Measure Changes: Initial Measure is NEEDED!</param>
    public Ma2(List<Note> notes, BPMChanges bpmChanges, MeasureChanges measureChanges)
    {
        Notes = new List<Note>(notes);
        BPMChanges = new BPMChanges(bpmChanges);
        MeasureChanges = new MeasureChanges(measureChanges);
        Update();
    }

    /// <summary>
    ///     Construct GoodBrother from location specified
    /// </summary>
    /// <param name="location">MA2 location</param>
    public Ma2(string location)
    {
        var tokens = new Ma2Tokenizer().Tokens(location);
        var takenIn = new Ma2Parser().ChartOfToken(tokens);
        Notes = new List<Note>(takenIn.Notes);
        BPMChanges = new BPMChanges(takenIn.BPMChanges);
        MeasureChanges = new MeasureChanges(takenIn.MeasureChanges);
        StoredChart = new List<List<Note>>(takenIn.StoredChart);
        Information = new Dictionary<string, string>(takenIn.Information);
        Update();
    }

    /// <summary>
    ///     Construct Ma2 with tokens given
    /// </summary>
    /// <param name="tokens">Tokens given</param>
    public Ma2(string[] tokens)
    {
        var takenIn = new Ma2Parser().ChartOfToken(tokens);
        Notes = takenIn.Notes;
        BPMChanges = takenIn.BPMChanges;
        MeasureChanges = takenIn.MeasureChanges;
        StoredChart = new List<List<Note>>(takenIn.StoredChart);
        Information = new Dictionary<string, string>(takenIn.Information);
        Update();
    }

    /// <summary>
    ///     Construct Ma2 with existing values
    /// </summary>
    /// <param name="takenIn">Existing good brother</param>
    public Ma2(Chart takenIn)
    {
        Notes = new List<Note>(takenIn.Notes);
        BPMChanges = new BPMChanges(takenIn.BPMChanges);
        MeasureChanges = new MeasureChanges(takenIn.MeasureChanges);
        StoredChart = new List<List<Note>>(takenIn.StoredChart);
        Information = new Dictionary<string, string>(takenIn.Information);
        Update();
    }
    #endregion

    public override bool CheckValidity()
    {
        var result = this == null;
        // Not yet implemented
        return result;
    }

    public override string Compose()
    {
        Update();
        StringBuilder result = new StringBuilder();
        string targetVersion;
        switch (ChartVersion)
        {
            case ChartVersion.Ma2_103:
                targetVersion = "1.03.00";
                break;
            case ChartVersion.Ma2_104:
                targetVersion = "1.04.00";
                break;
            default:
                throw new InvalidOperationException("Given Chart Type is not valid for MA2 composition. Type given: " + ChartVersion);
        }
        string header1 = "VERSION\t0.00.00\t" + targetVersion + "\nFES_MODE\t0\n";
        string header2 = "RESOLUTION\t" + Definition + "\nCLK_DEF\t" + Definition + "\nCOMPATIBLE_CODE\tMA2\n";
        result.Append(header1);
        result.Append(BPMChanges.InitialChange);
        result.Append(MeasureChanges.InitialChange);
        result.Append(header2);
        result.Append("\n");

        result.Append(BPMChanges.Compose());
        result.Append(MeasureChanges.Compose());
        result.Append("\n");

        foreach (var bar in StoredChart)
            foreach (var x in bar)
                if (!x.Compose(1).Equals(""))
                    result.Append(x.Compose(1) + "\n");
        result.Append("\n");
        return result.ToString();
    }

    /// <summary>
    ///     Extracts the special slide containers created by Simai
    /// </summary>
    /// <exception cref="InvalidOperationException">If slide container is casted wrongly, this exception will be raised</exception>
    public void ExtractSlideEachGroup()
    {
        List<Note> adjusted = new();
        List<Slide> slideCandidates = new();
        foreach (var x in Notes)
            switch (x.NoteSpecificGenre)
            {
                case NoteEnum.NoteSpecificGenre.SLIDE_EACH:
                    var candidate = x as SlideEachSet ??
                                    throw new InvalidOperationException("THIS IS NOT A SLIDE EACH");
                    if (candidate.SlideStart != null) adjusted.Add(candidate.SlideStart);
                    if (candidate.InternalSlides.Count > 0) slideCandidates.AddRange(candidate.InternalSlides);
                    break;
                case NoteEnum.NoteSpecificGenre.SLIDE_GROUP:
                    var groupCandidate = x as SlideGroup ??
                                         throw new InvalidOperationException("THIS IS NOT A SLIDE GROUP");
                    if (groupCandidate.InternalSlides.Count > 0) adjusted.AddRange(groupCandidate.InternalSlides);
                    break;
                default:
                    adjusted.Add(x);
                    break;
            }

        foreach (var x in slideCandidates)
            switch (x.NoteSpecificGenre)
            {
                case NoteSpecificGenre.SLIDE_GROUP:
                    var groupCandidate = x as SlideGroup ??
                                         throw new InvalidOperationException("THIS IS NOT A SLIDE GROUP");
                    if (groupCandidate.InternalSlides.Count > 0) adjusted.AddRange(groupCandidate.InternalSlides);
                    break;
                default:
                    adjusted.Add(x);
                    break;
            }

        Notes = adjusted;
    }

    /// <summary>
    ///     Override and compose with given arrays
    /// </summary>
    /// <param name="bpm">Override BPM array</param>
    /// <param name="measure">Override Measure array</param>
    /// <returns>Good Brother with override array</returns>
    public override string Compose(BPMChanges bpm, MeasureChanges measure)
    {
        Update();
        StringBuilder result = new StringBuilder();
        string targetVersion;
        switch (ChartVersion)
        {
            case ChartVersion.Ma2_103:
                targetVersion = "1.03.00";
                break;
            case ChartVersion.Ma2_104:
                targetVersion = "1.04.00";
                break;
            default:
                throw new InvalidOperationException("Given Chart Type is not valid for MA2 composition. Type given: " + ChartVersion);
        }
        string header1 = "VERSION\t0.00.00\t" + targetVersion + "\nFES_MODE\t0\n";
        string header2 = "RESOLUTION\t" + Definition + "\nCLK_DEF\t" + Definition + "\nCOMPATIBLE_CODE\tMA2\n";
        result.Append(header1);
        result.Append(bpm.InitialChange);
        result.Append(measure.InitialChange);
        result.Append(header2);
        result.Append("\n");

        result.Append(bpm.Compose());
        result.Append(measure.Compose());
        result.Append("\n");

        foreach (var bar in StoredChart)
            foreach (var x in bar)
                if (!x.Compose(1).Equals(""))
                    result.Append(x.Compose(1) + "\n");
        result.Append("\n");
        return result.ToString();
    }

    public override void Update()
    {
        ExtractSlideEachGroup();
        base.Update();
    }
}
