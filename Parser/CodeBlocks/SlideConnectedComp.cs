using System.Text;
using static MaiLib.ChartEnum;

namespace MaiLib;

public class SlideConnectedComp : ICodeBlock
{
    public SlideConnectedSeq? SlideConnectedSeq { get; private set; }
    public SlideDuration? SlideDuration { get; private set; }
    public SlideConnectedMeasuredSeq? SlideConnectedMeasuredSeq { get; private set; }
    public bool IsBreak { get; private set; }

    public SlideConnectedComp(SlideConnectedSeq slideConnectedSeq, bool isBreak)
    {
        SlideConnectedSeq = slideConnectedSeq;
        IsBreak = isBreak;
    }

    public SlideConnectedComp(SlideConnectedMeasuredSeq slideConnectedMeasuredSeq, SlideDuration slideDuration,
        bool isBreak)
    {
        SlideConnectedMeasuredSeq = slideConnectedMeasuredSeq;
        SlideDuration = slideDuration;
        IsBreak = isBreak;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        if (SlideConnectedSeq is not null)
        {
            builder.Append(SlideConnectedSeq.Compose(chartVersion));
            if (IsBreak) builder.Append('b');
        }
        else if (SlideConnectedMeasuredSeq is not null)
        {
            builder.Append(SlideConnectedMeasuredSeq.Compose(chartVersion));
            if (SlideDuration is not null)
            {
                builder.Append(SlideDuration.Compose(chartVersion));
                if (IsBreak) builder.Append('b');
            }
            else throw new ICodeBlock.ComponentMissingException("SLIDE-CONNECTED", "SLIDE-DURATION");
        }
        else
            throw new ICodeBlock.ComponentMissingException("SLIDE-CONNECTED",
                "SLIDE-CONNECTED-SEQ OR SLIDE-CONNECTED-MEASURED-SEQ");

        return builder.ToString();
    }
}