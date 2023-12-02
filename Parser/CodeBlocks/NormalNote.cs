using static MaiLib.ChartEnum;
namespace MaiLib;

public class NormalNote
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

    public NormalNote(SlidGroupComp slidGroupComp)
    {
        SlideGroupComp = slidGroupComp;
    }

    public string Compose(ChartVersion chartVersion)
    {

    }
}