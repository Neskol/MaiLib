using static MaiLib.NoteEnum;
namespace MaiLib
{
    public class SimaiScanner
    {
        public string[]? IncomingChart { get; private set; }
        public Dictionary<NoteType, string> TokenSet { get; protected set; }
        public int LineNum { get; private set; }
        public int CharNum { get; private set; }
        public string TokenBuffer { get; private set; }

        public SimaiScanner()
        {
            TokenSet = new Dictionary<NoteType, string>();
            TokenBuffer = "";
            LineNum = 0;
            CharNum = 0;
        }

        public SimaiScanner(string chart)
        {
            IncomingChart = chart.Split("\n");
            TokenSet = new Dictionary<NoteType, string>();
            TokenBuffer = "";
            LineNum = 0;
            CharNum = 0;
        }

        public void Load(string chart)
        {
            IncomingChart = chart.Split("\n");
        }

        public void Scan()
        {
            if (IncomingChart is null) throw new NullReferenceException("The scanner is not provided with valid chart to scan.");
            LineNum = 0;
            CharNum = 0;
            foreach (string line in IncomingChart)
            {
                NoteType status = NoteType.RST;
                foreach (char token in line.ToCharArray())
                {
                    switch (token)
                    {
                        case '{':
                            status = NoteType.MEASURE;
                            break;
                        case '}':
                            if (status is not NoteType.MEASURE) throw new NoteStatusNotMatchException(LineNum, CharNum, token, "Number or } expected", NoteType.MEASURE, status);
                            TokenSet.Add(status, TokenBuffer);
                            TokenBuffer = "";
                            status = NoteType.RST;
                            break;
                        case '(':
                            status = NoteType.BPM;
                            break;
                        case ')':
                            if (status is not NoteType.BPM) throw new NoteStatusNotMatchException(LineNum, CharNum, token, "Number or ) expected", NoteType.BPM, status);
                            TokenSet.Add(status, TokenBuffer);
                            TokenBuffer = "";
                            status = NoteType.RST;
                            break;
                        case 'h':
                            status = NoteType.HLD;
                            break;
                        case '-':
                            status = NoteType.SI_;
                            break;
                        case 'v':
                            status = NoteType.SV_;
                            break;
                        case 'w':
                            status = NoteType.SI_;
                            break;
                    }
                }
            }
        }
    }

    public class NoteStatusNotMatchException : Exception
    {
        public NoteStatusNotMatchException(int lineNum, int charNum, char token, string reason, NoteType expected, NoteType actual) :
        base($"At Line {lineNum}: Unexpected char {token} at {charNum} is found. Reason: {reason}. Expected Note Tpye: {expected} but actual {actual}.")
        {
        }
    }
}
