using System.Text;
using static MaiLib.ChartEnum;
using static MaiLib.ICodeBlock;
namespace MaiLib;

public class ChartDef : ICodeBlock
{
    public BPM? BPM { get; private set; }
    public Measure? Measure { get; private set; }

    public ChartDef(BPM bpm)
    {
        BPM = bpm;
    }

    public ChartDef(Measure measure)
    {
        Measure = measure;
    }

    public ChartDef(BPM bpm, Measure measure)
    {
        BPM = bpm;
        Measure = measure;
    }

    public string Compose(ChartVersion chartVersion)
    {
        if (BPM is null && Measure is null) throw new ComponentMissingException("CHART-DEF", "BPM, MEASURE");
        StringBuilder builder = new();
        builder.Append(BPM.Compose(chartVersion));
        builder.Append(Measure.Compose(chartVersion));
        return builder.ToString();
    }
}