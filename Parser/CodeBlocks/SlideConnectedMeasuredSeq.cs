using System.Text;
using static MaiLib.ChartEnum;
namespace MaiLib;

public class SlideConnectedMeasuredSeq : ICodeBlock
{
    public SlideType SlideType { get; private set; }
    public Key Key { get; private set; }
    public SlideConnectedMeasuredSeq? InnerSlideConnectedSeq { get; private set; }

    public SlideConnectedMeasuredSeq(SlideType slideType, Key key, SlideConnectedMeasuredSeq slideConnectedSeq)
    {
        SlideType = slideType;
        Key = key;
        InnerSlideConnectedSeq = slideConnectedSeq;
    }

    public SlideConnectedMeasuredSeq(SlideType slideType, Key key)
    {
        SlideType = slideType;
        Key = key;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(SlideType.Compose(chartVersion));
        builder.Append(Key.Compose(chartVersion));
        if (InnerSlideConnectedSeq is not null) builder.Append(InnerSlideConnectedSeq.Compose(chartVersion));
        return builder.ToString();
    }
}
