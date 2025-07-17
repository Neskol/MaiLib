using static MaiLib.ChartEnum;

namespace MaiLib;

public class SlideSet : ICodeBlock
{
    public SlideComp? SlideComp { get; private set; }
    public SlideConnectedComp? SlideConnectedComp { get; private set; }

    public SlideSet(SlideComp slideComp)
    {
        SlideComp = slideComp;
    }

    public SlideSet(SlideConnectedComp slideConnectedComp)
    {
        SlideConnectedComp = slideConnectedComp;
    }

    public string Compose(ChartVersion chartVersion)
    {
        if (SlideComp is not null) return SlideComp.Compose(chartVersion);

        if (SlideConnectedComp is not null) return SlideConnectedComp.Compose(chartVersion);

        throw new ICodeBlock.ComponentMissingException("SLIDE-SET", "SLIDE-COMP OR SLIDE-CONNECTED-COMP");
    }
}