namespace MaiLib;

using static ChartEnum;
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
        ChartVersion = ChartVersion.Ma2_103;
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
        ChartVersion = ChartVersion.Ma2_103;
        this.Update();
    }

    /// <summary>
    ///     Construct GoodBrother from location specified
    /// </summary>
    /// <param name="location">MA2 location</param>
    public Ma2(string location)
    {
        string[]? tokens = new Ma2Tokenizer().Tokens(location);
        Chart? takenIn = new Ma2Parser().ChartOfToken(tokens);
        Notes = new List<Note>(takenIn.Notes);
        BPMChanges = new BPMChanges(takenIn.BPMChanges);
        MeasureChanges = new MeasureChanges(takenIn.MeasureChanges);
        StoredChart = new List<List<Note>>(takenIn.StoredChart);
        Information = new Dictionary<string, string>(takenIn.Information);
        ChartVersion = ChartVersion.Ma2_103;
        this.Update();
    }

    /// <summary>
    ///     Construct Ma2 with tokens given
    /// </summary>
    /// <param name="tokens">Tokens given</param>
    public Ma2(string[] tokens)
    {
        Chart? takenIn = new Ma2Parser().ChartOfToken(tokens);
        Notes = takenIn.Notes;
        BPMChanges = takenIn.BPMChanges;
        MeasureChanges = takenIn.MeasureChanges;
        StoredChart = new List<List<Note>>(takenIn.StoredChart);
        Information = new Dictionary<string, string>(takenIn.Information);
        ChartVersion = ChartVersion.Ma2_103;
        this.Update();
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
        ChartVersion = ChartVersion.Ma2_103;
        this.Update();
    }

    #endregion

    public override bool CheckValidity()
    {
        bool result = this is null;
        // Not yet implemented
        return result;
    }

    public string GenerateNoteStatistics()
    {
        StringBuilder builder = new();
        builder.Append($"T_REC_TAP\t{NormalTapNum}\n");
        builder.Append($"T_REC_BRK\t{BreakTapNum}\n");
        builder.Append($"T_REC_XTP\t{ExTapNum}\n");
        if (ChartVersion is ChartVersion.Ma2_104 or ChartVersion.Ma2_105)
            builder.Append($"T_REC_BXX\t{BreakExTapNum}\n");
        builder.Append($"T_REC_HLD\t{NormalHoldNum}\n");
        builder.Append($"T_REC_XHO\t{ExHoldNum}\n");
        if (ChartVersion is ChartVersion.Ma2_104 or ChartVersion.Ma2_105)
        {
            builder.Append($"T_REC_BHO\t{BreakHoldNum}\n");
            builder.Append($"T_REC_BXH\t{BreakExHoldNum}\n");
        }

        builder.Append($"T_REC_STR\t{NormalSlideStartNum}\n");
        builder.Append($"T_REC_BST\t{BreakSlideStartNum}\n");
        builder.Append($"T_REC_XST\t{ExSlideStartNum}\n");
        if (ChartVersion is ChartVersion.Ma2_104 or ChartVersion.Ma2_105)
            builder.Append($"T_REC_XBS\t{BreakExSlideStartNum}\n");
        builder.Append($"T_REC_TTP\t{TouchTapNum}\n");
        builder.Append($"T_REC_THO\t{TouchHoldNum}\n");
        builder.Append($"T_REC_SLD\t{NormalSlideNum}\n");
        if (ChartVersion is ChartVersion.Ma2_104 or ChartVersion.Ma2_105)
            builder.Append($"T_REC_BSL\t{BreakSlideNum}\n");
        builder.Append($"T_REC_ALL\t{AllNoteRecNum}\n");

        builder.Append($"T_NUM_TAP\t{TapNum}\n");
        builder.Append($"T_NUM_BRK\t{BreakNum}\n");
        builder.Append($"T_NUM_HLD\t{HoldNum}\n");
        builder.Append($"T_NUM_SLD\t{SlideNum}\n");
        builder.Append($"T_NUM_ALL\t{AllNoteNum}\n");

        builder.Append($"T_JUDGE_TAP\t{TapJudgeNum}\n");
        builder.Append($"T_JUDGE_HLD\t{HoldJudgeNum}\n");
        builder.Append($"T_JUDGE_SLD\t{SlideJudgeNum}\n");
        builder.Append($"T_JUDGE_ALL\t{AllJudgeNum}\n");

        builder.Append($"TTM_EACHPAIRS\t{EachPairsNum}\n");
        builder.Append($"TTM_SCR_TAP\t{TapScore}\n");
        builder.Append($"TTM_SCR_BRK\t{BreakScore}\n");
        builder.Append($"TTM_SCR_HLD\t{HoldScore}\n");
        builder.Append($"TTM_SCR_SLD\t{SlideScore}\n");
        builder.Append($"TTM_SCR_ALL\t{AllScore}\n");
        builder.Append($"TTM_SCR_S\t{ScoreS}\n");
        builder.Append($"TTM_SCR_SS\t{ScoreSs}\n");
        builder.Append($"TTM_RAT_ACV\t{RatedAchievement}\n");
        return builder.ToString();
    }

    public override string Compose()
    {
        Update();
        switch (ChartVersion)
        {
            case ChartVersion.Ma2_103:
            case ChartVersion.Ma2_104:
            case ChartVersion.Ma2_105:
                base.Update();
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
                    case ChartVersion.Ma2_105:
                        targetVersion = "1.05.00";
                        break;
                    default:
                        return base.Compose(ChartVersion);
                }

                string header1 = $"VERSION\t0.00.00\t{targetVersion}\nFES_MODE\t{(IsUtage ? 1 : 0)}\n";
                string header2 = $"RESOLUTION\t{Definition}\nCLK_DEF\t{Definition}\nCOMPATIBLE_CODE\tMA2\n";
                result.Append(header1);
                result.Append(BPMChanges.InitialChange);
                result.Append(MeasureChanges.InitialChange);
                result.Append(header2);
                result.Append("\n");

                result.Append(BPMChanges.Compose());
                result.Append(MeasureChanges.Compose());
                result.Append("\n");

                foreach (List<Note>? bar in StoredChart)
                foreach (Note? x in bar)
                    if (!x.Compose(ChartVersion).Equals(""))
                        result.Append(x.Compose(ChartVersion) + "\n");
                result.Append("\n");
                result.Append(GenerateNoteStatistics());
                return result.ToString();
            default:
                return base.Compose();
        }
    }

    /// <summary>
    ///     Override and compose with given arrays
    /// </summary>
    /// <param name="bpm">Override BPM array</param>
    /// <param name="measure">Override Measure array</param>
    /// <returns>Good Brother with override array</returns>
    public override string Compose(BPMChanges bpm, MeasureChanges measure)
    {
        return new Ma2(Notes, bpm, measure).Compose(ChartVersion);
    }

    public override void Update()
    {
        ExtractSlideEachGroup();
        base.Update();
    }
}