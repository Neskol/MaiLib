using static MaiLib.ChartEnum;
namespace MaiLib;

public class KeyComp : ICodeBlock
{
    public Key Key1 { get; private set; }
    public Key Key2 { get; private set; }

    public KeyComp(Key key1, Key key2)
    {
        Key1 = key1;
        Key2 = key2;
    }

    public string Compose(ChartVersion chartVersion)
    {
        return Key1.Compose(chartVersion) + Key2.Compose(chartVersion);
    }
}