using System.Text;
using static MaiLib.ChartEnum;

namespace MaiLib;
public class BeginSeq : ICodeBlock
{
    public BPM BPM { get; private set; }
    public Measure Measure { get; private set; }
    public NoteSeq NoteSeq { get; protected set; }

    public BeginSeq()
    {
        BPM = new BPM(120);
        Measure = new Measure(4);
        NoteSeq = new NoteSeq();
    }

    public BeginSeq(BPMChange bpm, MeasureChange measure, NoteSeq noteSeq)
    {
        BPM = new BPM(120);
        Measure = new Measure(4);
        NoteSeq = new NoteSeq();
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder result = new StringBuilder();
        result.Append(BPM.Compose(chartVersion));
        result.Append(Measure.Compose(chartVersion));
        result.Append(NoteSeq.Compose(chartVersion));
        result.Append("E\n");
        return result.ToString();
    }
}