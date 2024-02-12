using System.Text;
using static MaiLib.ChartEnum;

namespace MaiLib;

public class SlideSeq : ICodeBlock
{
    public SlideSet SlideSet { get; private set; }
    public SlideSeq? InnerSlideSeq { get; private set; }

    public SlideSeq(SlideSet slideSet)
    {
        SlideSet = slideSet;
    }

    public SlideSeq(SlideSet slideSet, SlideSeq slideSeq)
    {
        SlideSet = slideSet;
        InnerSlideSeq = slideSeq;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        builder.Append(SlideSet.Compose(chartVersion));
        if (InnerSlideSeq is not null) builder.Append(InnerSlideSeq.Compose(chartVersion));
        return builder.ToString();
    }
}