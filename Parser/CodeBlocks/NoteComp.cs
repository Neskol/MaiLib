using System.Text;
using static MaiLib.ChartEnum;
namespace MaiLib;

public class NoteComp : ICodeBlock
{
    public SingleNote? SingleNote { get; private set; }
    public NoteComp? InnerNoteComp { get; private set; }
    public KeyComp? KeyComp { get; private set; }

    public NoteComp(SingleNote singleNote)
    {
        SingleNote = singleNote;
    }

    public NoteComp(SingleNote singleNote, NoteComp noteComp)
    {
        SingleNote = singleNote;
        InnerNoteComp = noteComp;
    }

    public NoteComp(KeyComp keyComp)
    {
        KeyComp = keyComp;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        if (SingleNote is not null)
        {
            builder.Append(SingleNote.Compose(chartVersion));
            if (InnerNoteComp is not null)
            {
                builder.Append('/');
                builder.Append(InnerNoteComp.Compose(chartVersion));
            }
        }
        else if (KeyComp is not null)
        {
            builder.Append(KeyComp.Compose(chartVersion));
        }
        else if (InnerNoteComp is not null)
        {
            throw new ICodeBlock.ComponentMissingException("NOTE-COMP", "SINGLE-NOTE");
        }
        else throw new ICodeBlock.ComponentMissingException("NOTE-COMP", "SINGLE NOTE, (NOTE-COMP), KEY-COMP");

        return builder.ToString();
    }
}