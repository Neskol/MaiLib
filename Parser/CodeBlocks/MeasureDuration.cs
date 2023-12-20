namespace MaiLib;

public class MeasureDuration : ICodeBlock
{
    public int Quaver { get; private set; }
    public int Multiple { get; private set; }

    public MeasureDuration(int quaver, int multiple)
    {
        Quaver = quaver;
        Multiple = multiple;
    }

    public string Compose(ChartEnum.ChartVersion chartVersion) => $"[{Quaver}:{Multiple}]";
}