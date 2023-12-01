namespace MaiLib;
using static MaiLib.TokenEnum;
using static MaiLib.NoteEnum;

public class SimaiParserR
{
    public int Resolution { get; private set; }
    public int BarNum { get; private set; }
    public int TickNum { get; private set; }
    public SimaiScanner Scanner { get; private set; }

    public SimaiParserR()
    {
        Scanner = new SimaiScanner();
        Resolution = 384;
    }

    public SimaiParserR(SimaiScanner scanner)
    {
        Scanner = scanner;
        Resolution = 384;
    }

    public Chart Parse()
    {
        Chart candidate = new Simai();
        List<Note> notes = new();
        NoteType noteType = NoteType.RST;
        while (Scanner.CurrentToken is not TokenType.EOS)
        {
            // if (notes.Count == 0 && Scanner.CurrentToken is not Token)
        }
        return candidate;
    }
}