using System.Text;
using static MaiLib.ChartEnum;
namespace MaiLib;

public class SlideConnected : ICodeBlock
{
    public SlideType SlideType { get; private set; }
    public Key Key { get; private set; }
    public SlideDuration SlideDuration { get; private set; }
    public SlideConnectedSeq? SlideConnectedSeq { get; private set; }
    public SlideConnectedMeasuredSeq? SlideConnectedMeasuredSeq { get; private set; }

    public SlideConnected(SlideType slideType, Key key, SlideDuration slideDuration,
        SlideConnectedSeq slideConnectedSeq)
    {
        SlideType = slideType;
        Key = key;
        SlideConnectedSeq = slideConnectedSeq;
    }

    public SlideConnected(SlideType slideType, Key key, SlideDuration slideDuration,
        SlideConnectedMeasuredSeq slideConnectedMeasuredSeq)
    {
        SlideType = slideType;
        Key = key;
        SlideConnectedMeasuredSeq = slideConnectedMeasuredSeq;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        builder.Append(SlideType.Compose(chartVersion));
        builder.Append(Key.Compose(chartVersion));
        builder.Append(SlideDuration.Compose(chartVersion));
        if (SlideConnectedSeq is not null) builder.Append(SlideConnectedSeq.Compose(chartVersion));
        else if (SlideConnectedMeasuredSeq is not null) builder.Append(SlideConnectedMeasuredSeq.Compose(chartVersion));
        else
            throw new ICodeBlock.ComponentMissingException("SLIDE-CONNECTED",
                "SLIDE-CONNECTED-SEQ OR SLIDE-CONNECTED-MEASURED-SEQ");
        return builder.ToString();
    }
}