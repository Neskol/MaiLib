using System.Text;
using static MaiLib.ChartEnum;

namespace MaiLib;

public class NormalNote : ICodeBlock
{
    public TapComp? TapComp { get; private set; }
    public HoldComp? HoldComp { get; private set; }
    public SlideGroupComp? SlideGroupComp { get; private set; }

    public NormalNote(TapComp tapComp)
    {
        TapComp = tapComp;
    }

    public NormalNote(HoldComp holdComp)
    {
        HoldComp = holdComp;
    }

    public NormalNote(SlideGroupComp slidGroupComp)
    {
        SlideGroupComp = slidGroupComp;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        if (TapComp is not null)
            builder.Append(TapComp.Compose(chartVersion));
        else if (HoldComp is not null)
            builder.Append(HoldComp.Compose(chartVersion));
        else if (SlideGroupComp is not null)
            builder.Append(SlideGroupComp.Compose(chartVersion));
        else throw new ICodeBlock.ComponentMissingException("NORMAL-NOTE", "TAP-COMP OR HOLD-COMP OR SLIDE-GROUP-COMP");

        return builder.ToString();
    }
}