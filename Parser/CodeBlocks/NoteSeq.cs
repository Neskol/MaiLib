using System.Text;
using static MaiLib.ChartEnum;
namespace MaiLib;

public class NoteSeq : ICodeBlock
{
    public ChartDef? ChartDef { get; private set; }
    public NoteComp? NoteComp { get; private set; }
    public NoteSeq? InnerNoteSeq { get; private set; }

    public bool IsSingleComma { get; private set; }

    public NoteSeq(ChartDef chartDef, NoteSeq noteSeq)
    {
        ChartDef = chartDef;
        InnerNoteSeq = noteSeq;
        IsSingleComma = false;
    }

    public NoteSeq(NoteComp noteComp, NoteSeq noteSeq)
    {
        NoteComp = noteComp;
        InnerNoteSeq = noteSeq;
        IsSingleComma = false;
    }

    public NoteSeq(NoteComp noteComp)
    {
        NoteComp = noteComp;
        IsSingleComma = false;
    }

    public NoteSeq(bool isSingleComma)
    {
        IsSingleComma = isSingleComma;
    }

    public NoteSeq()
    {
        IsSingleComma = false;
    }

    public string Compose(ChartVersion chartVersion)
    {
        StringBuilder builder = new();
        if (ChartDef is not null)
        {
            if (InnerNoteSeq is not null)
            {
                builder.Append(ChartDef.Compose(chartVersion));
                builder.Append(InnerNoteSeq.Compose(chartVersion));
                builder.Append('\n');
            }
            else throw new ICodeBlock.ComponentMissingException("NOTE-SEQ", "CHART-DEF")
        }
        else if (NoteComp is not null)
        {
            builder.Append(NoteComp.Compose(chartVersion));
            builder.Append(',');
            if (InnerNoteSeq is not null)
            {
                builder.Append(InnerNoteSeq.Compose(chartVersion));
            }
        }
        else if (IsSingleComma)
        {
            builder.Append(',');
        }

        return builder.ToString();
    }
}