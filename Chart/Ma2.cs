using static MaiLib.NoteEnum;
namespace MaiLib;

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
        Notes = new List<Note>();
        BPMChanges = new BPMChanges();
        MeasureChanges = new MeasureChanges();
        StoredChart = new List<List<Note>>();
        Information = new Dictionary<string, string>();
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
        StoredChart = new List<List<Note>>();
        Information = new Dictionary<string, string>();
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
        var result = "";
        const string header1 = "VERSION\t0.00.00\t1.03.00\nFES_MODE\t0\n";
        const string header2 = "RESOLUTION\t384\nCLK_DEF\t384\nCOMPATIBLE_CODE\tMA2\n";
        result += header1;
        result += BPMChanges.InitialChange;
        result += MeasureChanges.InitialChange;
        result += header2;
        result += "\n";

        result += BPMChanges.Compose();
        result += MeasureChanges.Compose();
        result += "\n";

        //foreach (Note x in this.Notes)
        //{
        //    if (!x.Compose(1).Equals(""))
        //    {
        //        result += x.Compose(1) + "\n";
        //    }
        //}
        foreach (var bar in StoredChart)
        foreach (var x in bar)
            if (!x.Compose(1).Equals(""))
                result += x.Compose(1) + "\n";
        result += "\n";
        return result;
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
        var result = "";
        const string header1 = "VERSION\t0.00.00\t1.03.00\nFES_MODE\t0\n";
        const string header2 = "RESOLUTION\t384\nCLK_DEF\t384\nCOMPATIBLE_CODE\tMA2\n";
        result += header1;
        result += bpm.InitialChange;
        result += measure.InitialChange;
        result += header2;
        result += "\n";

        result += bpm.Compose();
        result += measure.Compose();
        result += "\n";

        foreach (var y in Notes) result += y.Compose(1) + "\n";
        result += "\n";
        return result;
    }

    public override void Update()
    {
        ExtractSlideEachGroup();
        base.Update();
    }
}