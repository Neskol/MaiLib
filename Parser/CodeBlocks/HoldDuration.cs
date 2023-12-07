namespace MaiLib;

public class HoldDuration : ICodeBlock
{
    public MeasureDuration? MeasureDuration { get; private set; }
    public TimeDuration? TimeDuration { get; private set; }

    public HoldDuration(MeasureDuration measureDuration)
    {
        MeasureDuration = measureDuration;
    }

    public HoldDuration(TimeDuration timeDuration)
    {
        TimeDuration = timeDuration;
    }

    public string Compose(ChartEnum.ChartVersion chartVersion)
    {
        if (MeasureDuration is not null)
        {
            return "[" + MeasureDuration.Compose(chartVersion) + "]";
        }
        else if (TimeDuration is not null)
        {
            return "[" + TimeDuration.Compose(chartVersion) + "]";
        }
        else throw new ICodeBlock.ComponentMissingException("HOLD-DURATION", "MEASURE-DURATION OR TIME-DURATION");
    }
}