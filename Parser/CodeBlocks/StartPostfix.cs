using static MaiLib.ChartEnum;

namespace MaiLib;

public class StartPostfix : ICodeBlock
{
    public string? Postfix { get; private set; }
    private readonly string[] _allowedStrings = ["$", "!", "@"];

    public string ExpectedStrings => String.Join(", ", _allowedStrings);

    public StartPostfix()
    {
    }

    public StartPostfix(string suppliedString)
    {
        if (_allowedStrings.Any(suppliedString.Equals))
            Postfix = suppliedString;
        else throw new ICodeBlock.UnexpectedStringSuppliedException("SENSOR", ExpectedStrings, suppliedString);
    }

    public string Compose(ChartVersion chartVersion) => Postfix ?? "";
}