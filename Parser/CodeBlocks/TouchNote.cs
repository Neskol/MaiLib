using System.Text;
using static MaiLib.ChartEnum;

namespace MaiLib;

public class TouchNote : ICodeBlock
{
    public Sensor Sensor { get; private set; }
    public TapComp TapComp { get; private set; }
    public bool IsFirework { get; private set; }
    public HoldDuration? HoldDuration { get; private set; }

    public TouchNote(Sensor sensor, TapComp tapComp, bool isFirework)
    {
        Sensor = sensor;
        TapComp = tapComp;
        IsFirework = isFirework;
    }

    public TouchNote(Sensor sensor, TapComp tapComp, bool isFirework, HoldDuration holdDuration)
    {
        Sensor = sensor;
        TapComp = tapComp;
        IsFirework = isFirework;
        HoldDuration = holdDuration;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        builder.Append(Sensor.Compose(chartVersion));
        builder.Append(TapComp.Compose(chartVersion));
        if (IsFirework) builder.Append(('f'));
        if (HoldDuration is not null) builder.Append(HoldDuration.Compose(chartVersion));
        return builder.ToString();
    }
}