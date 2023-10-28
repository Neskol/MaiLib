namespace MaiLib;

public class SimaiScanner
{
    public string[]? IncomingChart { get; private set; }
    public Dictionary<NoteEnum,string> TokenBuffer { get; protected set; }
    public int LineNum { get; private set; }
    public int CharNum { get; private set; }

    public SimaiScanner()
    {
        TokenBuffer = new Dictionary<NoteEnum, string>();
        LineNum = 0;
        CharNum = 0;
    }

    public SimaiScanner(string chart)
    {
        IncomingChart = chart.Split("\n");
        TokenBuffer = new Dictionary<NoteEnum, string>();
        LineNum = 0;
        CharNum = 0;
    }

    public void Load(string chart)
    {
        IncomingChart = chart.Split("\n");
    }
}