using static MaiLib.ChartEnum;

namespace MaiLib;

public class SlideTimeDuration : ICodeBlock
{
    public double SecondsOfWait { get; private set; }
    public double? SecondsOfDuration { get; private set; }
    public double? BPM { get; private set; }
    public int? Quaver { get; private set; }
    public int? Multiple { get; private set; }

    public SlideTimeDuration(double secondsOfWait, double secondsOfDuration)
    {
        SecondsOfWait = secondsOfWait;
        SecondsOfDuration = secondsOfDuration;
    }

    public SlideTimeDuration(double secondsOfWait, double bpm, int quaver, int multiple)
    {
        SecondsOfWait = secondsOfWait;
        BPM = bpm;
        Quaver = quaver;
        Multiple = multiple;
    }


    public string Compose(ChartVersion chartVersion)
    {
        switch (chartVersion)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
            default:
                if (SecondsOfDuration is not null) return $"[{SecondsOfWait}##{SecondsOfDuration}]";
                if (BPM is not null)
                {
                    if (Quaver is null) throw new ICodeBlock.ComponentMissingException("SLIDE-TIME-DURATION", "QUAVER");
                    if (Multiple is null)
                        throw new ICodeBlock.ComponentMissingException("SLIDE-TIME-DURATION", "MULTIPLE");
                    return $"[{SecondsOfWait}##{BPM}#{Quaver}:{Multiple}]";
                }

                throw new ICodeBlock.ComponentMissingException("SLIDE-TIME-DURATION", "SECONDS-OF-DURATION OR BPM");
        }
    }
}