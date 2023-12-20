using static MaiLib.ChartEnum;
namespace MaiLib;

public class TapComp : ICodeBlock
{
    public Key Key { get; private set; }
    public StartPostfix? StartPostfix { get; private set; }
    public bool IsBreak { get; private set; }
    public bool IsEx { get; private set; }

    public TapComp(Key key, bool isBreak, bool isEx)
    {
        Key = key;
        IsBreak = isBreak;
        IsEx = isEx;
    }

    public string Compose(ChartVersion chartVersion)
    {
        string result = Key.Compose(chartVersion);
        if (IsBreak) result += "b";
        if (IsEx) result += "x";
        return result;
    }
}
