namespace MaiLib;

/// <summary>
///     Tokenizer of ma2 file
/// </summary>
public class Ma2Tokenizer : ITokenizer
{
    /// <summary>
    ///     Empty Constructor
    /// </summary>
    public Ma2Tokenizer()
    {
    }

    public string[] Tokens(string location)
    {
        var result = File.ReadAllLines(location);
        return result;
    }

    public string[] TokensFromText(string text)
    {
        var result = text.Split("\n");
        return result;
    }
}