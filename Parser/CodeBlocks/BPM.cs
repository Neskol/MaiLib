using static MaiLib.ChartEnum;

namespace MaiLib;

public class BPM : ICodeBlock
{
    public double Speed { get; private set; }

    public BPM(double bpm)
    {
        Speed = bpm;
    }

    public BPM(string bpm)
    {
        Speed = double.Parse(bpm);
    }

    public string Compose(ChartVersion chartVersion)
    {
        switch (chartVersion)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
            default:
                return $"({Math.Round(Speed, 4)})";
        }
    }
}