using static MaiLib.ChartEnum;

namespace MaiLib;

public class Sensor : ICodeBlock
{
    public string SensorArea { get; private set; }
    private readonly string[] _allowedStrings = ["A", "B", "C", "D", "E", "F"];

    public string ExpectedStrings => String.Join(", ", _allowedStrings);

    public Sensor(string suppliedString)
    {
        if (_allowedStrings.Any(suppliedString.Equals))
        {
            SensorArea = suppliedString;
        }
        else throw new ICodeBlock.UnexpectedStringSuppliedException("SENSOR", ExpectedStrings, suppliedString);
    }

    public string Compose(ChartVersion chartVersion) => SensorArea;
}