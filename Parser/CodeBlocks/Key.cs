using static MaiLib.ChartEnum;
namespace MaiLib;

public class Key : ICodeBlock
{
    public int Button { get; private set; }

    public Key(int key)
    {
        if (key is >= 1 and <= 8)
        {
            Button = key;
        }
        else throw new ICodeBlock.UnexpectedStringSuppliedException("KEY", "NUM 1-8", key.ToString());
    }

    public Key(string keyInString)
    {
        int key = int.Parse(keyInString);
        if (key is >= 1 and <= 8)
        {
            Button = key;
        }
        else throw new ICodeBlock.UnexpectedStringSuppliedException("KEY", "NUM 1-8", key.ToString());
    }

    public string Compose(ChartVersion chartVersion)
    {
        switch (chartVersion)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
            default:
                return Button.ToString();
        }
    }
}