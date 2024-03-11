using static MaiLib.ChartEnum;

namespace MaiLib;

public class SlideType : ICodeBlock
{
    public string NoteType { get; private set; }
    public Key? InflectionKey { get; private set; }
    private readonly string[] _allowedStrings = ["-", ">", "<", "^", "v", "p", "q", "s", "z", "pp", "qq", "w"];

    public string ExpectedStrings => $"{String.Join(", ", _allowedStrings)}, V<Key>";

    public SlideType(string suppliedString)
    {
        if (suppliedString.Contains('V'))
        {
            if (suppliedString.Length != 2)
                throw new ICodeBlock.UnexpectedStringSuppliedException("SLIDE-TYPE", "V<key>", suppliedString);
            NoteType = "V";
            InflectionKey = new Key(suppliedString[1]);
        }
        else if (_allowedStrings.Any(suppliedString.Equals))
        {
            NoteType = suppliedString;
        }
        else throw new ICodeBlock.UnexpectedStringSuppliedException("SLIDE-TYPE", ExpectedStrings, suppliedString);
    }

    public string Compose(ChartVersion chartVersion)
    {
        return InflectionKey is null ? NoteType : NoteType + InflectionKey.Compose(chartVersion);
    }
}
