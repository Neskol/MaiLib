namespace MaiLib;

using static TokenEnum;

public class SimaiParserR
{
    public int Resolution { get; private set; }
    public int Tick { get; private set; }
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
        List<Note> notes = [];
        // NoteType noteType = NoteType.RST;
        while (Scanner.CurrentToken is not TokenType.EOS)
        {
            // if (notes.Count == 0 && Scanner.CurrentToken is not Token)
        }

        return candidate;
    }
}