using System.Text;
using static MaiLib.ChartEnum;
namespace MaiLib;

public class SlideComp : ICodeBlock
{
    public SlideType SlideType { get; private set; }
    public Key Key { get; private set; }
    public SlideDuration SlideDuration { get; private set; }

    public SlideComp(SlideType slideType, Key key, SlideDuration slideDuration)
    {
        SlideType = slideType;
        Key = key;
        SlideDuration = slideDuration;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        builder.Append(SlideType.Compose(chartVersion));
        builder.Append(Key.Compose(chartVersion));
        builder.Append(SlideDuration.Compose(chartVersion));
        return builder.ToString();
    }
}