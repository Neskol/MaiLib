using static MaiLib.ChartEnum;

namespace MaiLib;

public class TimeDuration : ICodeBlock
{
    public double? SecondsOfDuration { get; private set; }
    public double? BPM { get; private set; }
    public int? Quaver { get; private set; }
    public int? Multiple { get; private set; }

    public TimeDuration(double secondsOfDuration)
    {
        SecondsOfDuration = secondsOfDuration;
    }

    public TimeDuration(double bpm, int quaver, int multiple)
    {
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
                if (SecondsOfDuration is not null) return $"[#{SecondsOfDuration}]";
                if (BPM is not null)
                {
                    if (Quaver is null) throw new ICodeBlock.ComponentMissingException("TIME-DURATION", "QUAVER");
                    if (Multiple is null)
                        throw new ICodeBlock.ComponentMissingException("TIME-DURATION", "MULTIPLE");
                    return $"[{BPM}#{Quaver}:{Multiple}]";
                }

                throw new ICodeBlock.ComponentMissingException("TIME-DURATION", "SECONDS-OF-DURATION OR BPM");
        }
    }
}