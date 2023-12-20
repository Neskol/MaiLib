using static MaiLib.ChartEnum;
namespace MaiLib;

public class HoldComp : ICodeBlock
{
    public TapComp TapComp { get; private set; }
    public HoldDuration HoldDuration { get; set; }

    public HoldComp(TapComp tapComp, HoldDuration holdDuration)
    {
        TapComp = tapComp;
        HoldDuration = holdDuration;
    }

    public string Compose(ChartVersion chartVersion)
    {
        return TapComp.Compose(chartVersion) + "h" + HoldDuration.Compose(chartVersion);
    }
}
