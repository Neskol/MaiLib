using static MaiLib.ChartEnum;
namespace MaiLib;

public class SlideConnectedSeq : ICodeBlock
{
    public SlideType SlideType { get; private set; }
    public Key Key { get; private set; }

    public string Compose(ChartVersion chartVersion)
    {
        throw new NotImplementedException();
    }
}