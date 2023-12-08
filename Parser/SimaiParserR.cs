namespace MaiLib;
using static MaiLib.TokenEnum;
using static MaiLib.NoteEnum;

public class SimaiParserR
{
    public int Resolution { get; private set; }
    public int Tick { get; private set; }
    public int CurrentBar => Tick / Resolution;
    public int CurrentTick => Tick % Resolution;
    public SimaiScanner Scanner { get; private set; }

    public SimaiParserR()
    {
        Scanner = new SimaiScanner();
        Resolution = 384;
    }

    public SimaiParserR(int resolution)
    {
        Scanner = new SimaiScanner();
        Resolution = resolution;
    }

    public SimaiParserR(SimaiScanner scanner)
    {
        Scanner = scanner;
        Resolution = 384;
    }

    public SimaiParserR(SimaiScanner scanner, int resolution)
    {
        Scanner = scanner;
        Resolution = resolution;
    }

    public Chart Parse()
    {
        Chart candidate = new Simai();
        List<Note> notes = new();
        NoteType noteType = NoteType.RST;
        Note currentNote;
        string buffer = "";
        int currentStepping;
        double currentBPM;
        while (Scanner.CurrentToken is not TokenType.EOS)
        {
            switch (Scanner.CurrentToken)
            {
                case TokenType.LPAREN:
                    noteType = NoteType.BPM;
                    break;
                case TokenType.RPAREN:
                    if (noteType is not NoteType.BPM)
                        throw new UnexpectedTokenException(Scanner.LineNum, Scanner.CharNum, Scanner.CurrentToken, ") was expecting a ( before.");
                    else if (!double.TryParse(buffer, out double bpm))
                        throw new UnexpectedTokenException(Scanner.LineNum, Scanner.CharNum, Scanner.CurrentToken,
                            $"Previously read tokens cannot be parsed into BPM: {buffer}");
                    else
                    {
                        notes.Add(new BPMChange(CurrentBar, CurrentTick, bpm));
                        currentBPM = bpm;
                        noteType = NoteType.RST;
                    }
                    break;
                case TokenType.LBRACE:
                    noteType = NoteType.MEASURE;
                    break;
                case TokenType.RBRACE:
                    if (noteType is not NoteType.MEASURE)
                        throw new UnexpectedTokenException(Scanner.LineNum, Scanner.CharNum, Scanner.CurrentToken, "} was expecting a { before.");
                    else if (!int.TryParse(buffer, out int quaver))
                        throw new UnexpectedTokenException(Scanner.LineNum, Scanner.CharNum, Scanner.CurrentToken,
                            $"Previously read tokens cannot be parsed into Quaver: {buffer}");
                    else
                    {
                        notes.Add(new MeasureChange(CurrentBar, CurrentTick, quaver));
                        currentStepping = quaver;
                        noteType = NoteType.RST;
                    }
                    break;
                case TokenType.DOT:
                case TokenType.NUM0:
                case TokenType.NUM1:
                case TokenType.NUM2:
                case TokenType.NUM3:
                case TokenType.NUM4:
                case TokenType.NUM5:
                case TokenType.NUM6:
                case TokenType.NUM7:
                case TokenType.NUM8:
                case TokenType.NUM9:
                case TokenType.A:
                case TokenType.B:
                case TokenType.C:
                case TokenType.D:
                case TokenType.E:
                case TokenType.F:
                    buffer += Scanner.CurrentChar;
                    break;
            }
        }
        return candidate;
    }

    public class UnexpectedTokenException : Exception
    {
        public UnexpectedTokenException(int lineNum, int charNum, TokenEnum.TokenType token, string reason) :
            base($"At Line {lineNum}: Unexpected token {token} at Line {lineNum} Char {charNum} with reason {reason}")
        {
        }
    }
}