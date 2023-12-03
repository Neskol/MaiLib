using System.Text;
using static MaiLib.ChartEnum;
namespace MaiLib;

public class SlideGroupComp : ICodeBlock
{
    public TapComp? TapComp { get; private set; }
    public SlideSeq SlideSeq { get; private set; }
    public Key? Key { get; private set; }
    public StartPostfix? StartPostfix { get; private set; }

    public SlideGroupComp(TapComp tapComp, SlideSeq slideSeq)
    {
        TapComp = tapComp;
        SlideSeq = slideSeq;
    }

    public SlideGroupComp(Key key, StartPostfix startPostfix, SlideSeq slideSeq)
    {
        Key = key;
        StartPostfix = startPostfix;
        SlideSeq = slideSeq;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        if (TapComp is not null)
        {
            if (Key is not null) throw new ICodeBlock.ExcessiveComponentsException("SLIDE-GROUP-COMP", "KEY");
            if (StartPostfix is not null) throw new ICodeBlock.ExcessiveComponentsException("SLIDE-GROUP-COMP", "START-POSTFIX");
            builder.Append(TapComp.Compose(chartVersion));
            builder.Append(SlideSeq.Compose(chartVersion));
        }
        else if (Key is not null)
        {
            if (StartPostfix is null)
                throw new ICodeBlock.ComponentMissingException("SLIDE-GROUP-COMP", "START-POSTFIX");
            builder.Append(Key.Compose(chartVersion));
            builder.Append(StartPostfix.Compose(chartVersion));
            builder.Append(SlideSeq.Compose(chartVersion));
        }
        else throw new ICodeBlock.ComponentMissingException("SLIDE-GROUP-COMP", "TAP-COMP OR KEY");

        return builder.ToString();
    }
}