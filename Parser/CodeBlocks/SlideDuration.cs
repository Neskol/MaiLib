namespace MaiLib;

public class SlideDuration : ICodeBlock
{
    public MeasureDuration? MeasureDuration { get; private set; }
    public SlideTimeDuration? SlideTimeDuration { get; private set; }

    public SlideDuration(MeasureDuration measureDuration)
    {
        MeasureDuration = measureDuration;
    }

    public SlideDuration(SlideTimeDuration slideTimeDuration)
    {
        SlideTimeDuration = slideTimeDuration;
    }

    public string Compose(ChartEnum.ChartVersion chartVersion)
    {
        if (MeasureDuration is not null) return "[" + MeasureDuration.Compose(chartVersion) + "]";

        if (SlideTimeDuration is not null) return "[" + SlideTimeDuration.Compose(chartVersion) + "]";

        throw new ICodeBlock.ComponentMissingException("SLIDE-DURATION", "MEASURE-DURATION OR SLIDE-TIME-DURATION");
    }
}