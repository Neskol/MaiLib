using static MaiLib.NoteEnum;
using static MaiLib.TokenEnum;
namespace MaiLib
{
    public class SimaiScanner
    {
        public string[]? IncomingChart { get; private set; }
        public int LineNum { get; private set; }
        public int CharNum { get; private set; }
        public TokenType CurrentToken { get; private set; }

        public char? CurrentChar { get; private set; }

        public char NextChar { get => IncomingChart is null || LineNum >= IncomingChart.Length || CharNum >= IncomingChart[LineNum].Length ? ' ' : IncomingChart[LineNum][CharNum]; }

        public SimaiScanner()
        {
            LineNum = 0;
            CharNum = 0;
            CurrentToken = TokenType.EOS;
        }

        public SimaiScanner(string chart)
        {
            Load(chart);
            try
            {
                ScanAndAdvance();
            }
            catch (Exception ex)
            {
                CurrentToken = TokenType.EOS;
                // throw ex;
            }
        }

        public void Load(string chart)
        {
            IncomingChart = chart.Split(Environment.NewLine);
            LineNum = 0;
            CharNum = 0;
        }

        public void ScanAndAdvance()
        {
            if (IncomingChart is null) throw new NullReferenceException("The scanner is not provided with valid chart to scan.");
            if (LineNum < IncomingChart.Length)
            {
                if (IncomingChart[LineNum].Length == 0) CurrentToken = TokenType.BLANK;
                else if (CharNum < IncomingChart[LineNum].Length)
                {
                    switch (NextChar)
                    {
                        case '(':
                            CurrentToken = TokenType.LPAREN;
                            break;
                        case ')':
                            CurrentToken = TokenType.RPAREN;
                            break;
                        case '{':
                            CurrentToken = TokenType.LBRACE;
                            break;
                        case '}':
                            CurrentToken = TokenType.RBRACE;
                            break;
                        case '[':
                            CurrentToken = TokenType.LBRACKET;
                            break;
                        case ']':
                            CurrentToken = TokenType.RBRACKET;
                            break;
                        case ',':
                            CurrentToken = TokenType.COMMA;
                            break;
                        case '.':
                            CurrentToken = TokenType.DOT;
                            break;
                        case '#':
                            CurrentToken = TokenType.SHARP;
                            break;
                        case ':':
                            CurrentToken = TokenType.COLON;
                            break;
                        case '$':
                            CurrentToken = TokenType.DOLLAR;
                            break;
                        case '/':
                            CurrentToken = TokenType.SLASH;
                            break;
                        case '<':
                            CurrentToken = TokenType.LANGLE;
                            break;
                        case '>':
                            CurrentToken = TokenType.RANGLE;
                            break;
                        case '-':
                            CurrentToken = TokenType.DASH;
                            break;
                        case '*':
                            CurrentToken = TokenType.ASTERISK;
                            break;
                        case 'A':
                            CurrentToken = TokenType.A;
                            break;
                        case 'B':
                            CurrentToken = TokenType.B;
                            break;
                        case 'C':
                            CurrentToken = TokenType.C;
                            break;
                        case 'D':
                            CurrentToken = TokenType.D;
                            break;
                        case 'E':
                            CurrentToken = TokenType.E;
                            break;
                        case 'F':
                            CurrentToken = TokenType.F;
                            break;
                        case 'v':
                            CurrentToken = TokenType.SV;
                            break;
                        case 'V':
                            CurrentToken = TokenType.LV;
                            break;
                        case 'p':
                            CurrentToken = TokenType.P;
                            break;
                        case 'q':
                            CurrentToken = TokenType.Q;
                            break;
                        case 's':
                            CurrentToken = TokenType.S;
                            break;
                        case 'z':
                            CurrentToken = TokenType.Z;
                            break;
                        case '0':
                            CurrentToken = TokenType.NUM0;
                            break;
                        case '1':
                            CurrentToken = TokenType.NUM1;
                            break;
                        case '2':
                            CurrentToken = TokenType.NUM2;
                            break;
                        case '3':
                            CurrentToken = TokenType.NUM3;
                            break;
                        case '4':
                            CurrentToken = TokenType.NUM4;
                            break;
                        case '5':
                            CurrentToken = TokenType.NUM5;
                            break;
                        case '6':
                            CurrentToken = TokenType.NUM6;
                            break;
                        case '7':
                            CurrentToken = TokenType.NUM7;
                            break;
                        case '8':
                            CurrentToken = TokenType.NUM8;
                            break;
                        case '9':
                            CurrentToken = TokenType.NUM9;
                            break;
                        case 'b':
                            CurrentToken = TokenType.BREAK;
                            break;
                        case 'x':
                            CurrentToken = TokenType.EX;
                            break;
                        case 'h':
                            CurrentToken = TokenType.HOLD;
                            break;
                        case 'f':
                            CurrentToken = TokenType.FIREWORK;
                            break;
                        case ' ':
                            CurrentToken = TokenType.BLANK;
                            break;
                        default:
                            throw new UnexpectedCharacterException(LineNum, CharNum, IncomingChart[LineNum][CharNum]);
                    }

                    CurrentChar = IncomingChart[LineNum][CharNum];
                }
                if (CharNum < IncomingChart[LineNum].Length - 1)
                {
                    CharNum++;
                }
                else
                {
                    CharNum = 0;
                    LineNum++;
                }
            }
            else CurrentToken = TokenType.EOS;
        }
    }

    public class UnexpectedCharacterException : Exception
    {
        public UnexpectedCharacterException(int lineNum, int charNum, char token) :
        base($"At Line {lineNum}: Unexpected char {token} at Line {lineNum} Char {charNum}.")
        {
        }
    }
}
