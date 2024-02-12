using static MaiLib.ChartEnum;

namespace MaiLib;

public class Measure : ICodeBlock
{
    public int Quaver { get; private set; }

    public Measure(int quaver)
    {
        Quaver = quaver;
    }

    public Measure(string quaver)
    {
        Quaver = int.Parse(quaver);
    }

    public string Compose(ChartVersion chartVersion)
    {
        switch (chartVersion)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
            default:
                return $"{{{Quaver}}}";
        }
    }
}