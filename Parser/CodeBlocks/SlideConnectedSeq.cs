using System.Text;
using static MaiLib.ChartEnum;
namespace MaiLib;

public class SlideConnectedSeq : ICodeBlock
{
    public SlideType SlideType { get; private set; }
    public Key Key { get; private set; }
    public SlideDuration SlideDuration { get; private set; }
    public SlideConnectedSeq? InnerSlideConnectedSeq { get; private set; }

    public SlideConnectedSeq(SlideType slideType, Key key, SlideDuration slideDuration, SlideConnectedSeq slideConnectedSeq)
    {
        SlideType = slideType;
        Key = key;
        SlideDuration = slideDuration;
        InnerSlideConnectedSeq = slideConnectedSeq;
    }

    public SlideConnectedSeq(SlideType slideType, Key key, SlideDuration slideDuration)
    {
        SlideType = slideType;
        Key = key;
        SlideDuration = slideDuration;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(SlideType.Compose(chartVersion));
        builder.Append(Key.Compose(chartVersion));
        builder.Append(SlideDuration.Compose(chartVersion));
        if (InnerSlideConnectedSeq is not null) builder.Append(InnerSlideConnectedSeq.Compose(chartVersion));
        return builder.ToString();
    }
}