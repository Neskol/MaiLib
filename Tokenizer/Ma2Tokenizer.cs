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
        string[]? result = File.ReadAllLines(location);
        return result;
    }

    public string[] TokensFromText(string text)
    {
        string[]? result = text.Split("\n");
        return result;
    }
}