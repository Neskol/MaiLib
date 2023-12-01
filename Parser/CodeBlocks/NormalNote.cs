using static MaiLib.ChartEnum;
namespace MaiLib;

public class NormalNote
{
    public TapComp? TapComp { get; private set; }
    public HoldComp? HoldComp { get; private set; }
    public SlideGroupComp? SlideGroupComp { get; private set; }
}