using System.Text;
using static MaiLib.ChartEnum;

namespace MaiLib;

public class SingleNote : ICodeBlock
{
    public TouchNote? TouchNote { get; private set; }
    public NormalNote? NormalNote { get; private set; }

    public SingleNote(TouchNote touchNote)
    {
        TouchNote = touchNote;
    }

    public SingleNote(NormalNote normalNote)
    {
        NormalNote = normalNote;
    }

    public string Compose(ChartVersion chartVersion)
    {
        if (TouchNote is not null)
        {
            return TouchNote.Compose(chartVersion);
        }
        else if (NormalNote is not null)
        {
            return NormalNote.Compose(chartVersion);
        }
        else throw new ICodeBlock.ComponentMissingException("SINGLE-NOTE", "TOUCH-NOTE OR NORMAL-NOTE");
    }
}