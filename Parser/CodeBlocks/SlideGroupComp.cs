using System.Text;
using static MaiLib.ChartEnum;

namespace MaiLib;

public class SlideGroupComp : ICodeBlock
{
    public TapComp TapComp { get; private set; }
    public SlideSeq SlideSeq { get; private set; }

    public SlideGroupComp(TapComp tapComp, SlideSeq slideSeq)
    {
        TapComp = tapComp;
        SlideSeq = slideSeq;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        builder.Append(TapComp.Compose(chartVersion));
        builder.Append(SlideSeq.Compose(chartVersion));
        return builder.ToString();
    }
}