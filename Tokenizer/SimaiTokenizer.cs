namespace MaiLib;

/// <summary>
///     Tokenize input file into tokens that parser can read
/// </summary>
public class SimaiTokenizer : ITokenizer
{
    /// <summary>
    ///     Stores the candidates of charts
    /// </summary>
    private readonly Dictionary<string, string[]> chartCandidates;

    /// <summary>
    ///     Stores the information to read
    /// </summary>
    private readonly TrackInformation simaiTrackInformation;

    /// <summary>
    ///     Constructs a tokenizer
    /// </summary>
    public SimaiTokenizer()
    {
        simaiTrackInformation = new XmlInformation();
        chartCandidates = new Dictionary<string, string[]>();
    }

    /// <summary>
    ///     Access the chart candidates
    /// </summary>
    public Dictionary<string, string[]> ChartCandidates => chartCandidates;

    /// <summary>
    ///     Access the chart information
    /// </summary>
    public TrackInformation SimaiTrackInformation => simaiTrackInformation;

    public string[] Tokens(string location)
    {
        string[]? takeIn = File.ReadAllLines(location);
        string? storage = "";
        foreach (string? line in takeIn) storage += line;
        return TokensFromText(storage);
    }

    public string[] TokensFromText(string text)
    {
        string? storage = text;
        string[]? result = new String(text.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray()).Split(',');
        return result;
    }


    /// <summary>
    ///     Update candidates from texts specified
    /// </summary>
    /// <param name="input">Text to be tokenized</param>
    public void UpdateFromText(string input)
    {
        string? storage = input;
        string[]? result = storage.Split("&");
        string? titleCandidate = "";
        string? bpmCandidate = "";
        string? artistCandidate = "";
        string? chartDesigner = "";
        string? shortIdCandidate = "";
        string? genreCandidate = "";
        string? versionCandidate = "";

        foreach (string? item in result)
            if (item.Contains("title"))
            {
                titleCandidate = item.Replace("title=", "").Replace("[SD]", "").Replace("[DX]", "");
                simaiTrackInformation.Information["Name"] = titleCandidate;
            }
            else if (item.Contains("wholebpm"))
            {
                bpmCandidate = item.Replace("wholebpm=", "");
                simaiTrackInformation.Information["BPM"] = bpmCandidate;
            }
            else if (item.Contains("artist"))
            {
                artistCandidate = item.Replace("artist=", "");
                simaiTrackInformation.Information["Composer"] = artistCandidate;
            }
            else if (item.Contains("des="))
            {
                chartDesigner = item.Replace("des=", "");
            }
            else if (item.Contains("shortid"))
            {
                shortIdCandidate = item.Replace("shortid=", "");
                simaiTrackInformation.Information["Music ID"] = shortIdCandidate;
                if (shortIdCandidate.Length <= 6 && int.TryParse(shortIdCandidate, out int id))
                {
                    if (shortIdCandidate.Length > 4)
                        simaiTrackInformation.Information["SDDX Suffix"] = "DX";
                    else simaiTrackInformation.Information["SDDX Suffix"] = "SD";
                }
            }
            else if (item.Contains("genre"))
            {
                genreCandidate = item.Replace("genre=", "");
                simaiTrackInformation.Information["Genre"] = genreCandidate;
            }
            else if (item.Contains("version"))
            {
                versionCandidate = item.Replace("version=", "");
                simaiTrackInformation.Information["Version"] = versionCandidate;
            }
            else if (item.Contains("lv_1"))
            {
                string? easyCandidate = item.Replace("lv_1=", "");
                simaiTrackInformation.Information["Easy"] = easyCandidate;
            }
            else if (item.Contains("des_1"))
            {
                string? easyChartCandidate = item.Replace("des_1=", "");
                simaiTrackInformation.Information["Easy Chart Maker"] = easyChartCandidate;
            }
            else if (item.Contains("lv_2"))
            {
                string? basicCandidate = item.Replace("lv_2=", "");
                simaiTrackInformation.Information["Basic"] = basicCandidate;
            }
            else if (item.Contains("des_2"))
            {
                string? basicChartCandidate = item.Replace("des_2=", "");
                simaiTrackInformation.Information["Basic Chart Maker"] = basicChartCandidate;
            }
            else if (item.Contains("lv_3"))
            {
                string? advancedCandidate = item.Replace("lv_3=", "");
                simaiTrackInformation.Information["Advanced"] = advancedCandidate;
            }
            else if (item.Contains("des_3"))
            {
                string? advancedChartCandidate = item.Replace("des_3=", "");
                simaiTrackInformation.Information["Advanced Chart Maker"] = advancedChartCandidate;
            }
            else if (item.Contains("lv_4"))
            {
                string? expertCandidate = item.Replace("lv_4=", "");
                simaiTrackInformation.Information["Expert"] = expertCandidate;
            }
            else if (item.Contains("des_4"))
            {
                string? expertChartCandidate = item.Replace("des_4=", "");
                simaiTrackInformation.Information["Expert Chart Maker"] = expertChartCandidate;
            }
            else if (item.Contains("lv_5"))
            {
                string? masterCandidate = item.Replace("lv_5=", "");
                simaiTrackInformation.Information["Master"] = masterCandidate;
            }
            else if (item.Contains("des_5"))
            {
                string? masterChartCandidate = item.Replace("des_5=", "");
                simaiTrackInformation.Information["Master Chart Maker"] = masterChartCandidate;
            }
            else if (item.Contains("lv_6"))
            {
                string? remasterCandidate = item.Replace("lv_6=", "");
                simaiTrackInformation.Information["Remaster"] = remasterCandidate;
            }
            else if (item.Contains("des_6"))
            {
                string? remasterChartCandidate = item.Replace("des_6=", "");
                simaiTrackInformation.Information["Remaster Chart Maker"] = remasterChartCandidate;
            }
            else if (item.Contains("lv_7"))
            {
                string? utageCandidate = item.Replace("lv_7=", "");
                simaiTrackInformation.Information["Utage"] = utageCandidate;
            }
            else if (item.Contains("des_7"))
            {
                string? utageChartCandidate = item.Replace("des_7=", "");
                simaiTrackInformation.Information["Utage Chart Maker"] = utageChartCandidate;
            }
            else if (item.Contains("inote_2"))
            {
                string? noteCandidate = item.Replace("inote_2=", "");
                chartCandidates.Add("2", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_3"))
            {
                string? noteCandidate = item.Replace("inote_3=", "");
                chartCandidates.Add("3", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_4"))
            {
                string? noteCandidate = item.Replace("inote_4=", "");
                chartCandidates.Add("4", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_5"))
            {
                string? noteCandidate = item.Replace("inote_5=", "");
                chartCandidates.Add("5", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_6"))
            {
                string? noteCandidate = item.Replace("inote_6=", "");
                chartCandidates.Add("6", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_7"))
            {
                string? noteCandidate = item.Replace("inote_7=", "");
                chartCandidates.Add("7", TokensFromText(noteCandidate));
            }
            // TODO: Fix this when note >= 8
            else if (item.Contains("inote_"))
            {
                string? noteCandidate = item.Replace("inote_=", "");
                chartCandidates.Add("Default", TokensFromText(noteCandidate));
            }
    }

    /// <summary>
    ///     Update candidates from texts specified
    /// </summary>
    /// <param name="path">Location of text to be tokenized</param>
    public void UpdateFromPath(string path)
    {
        string[]? takeIn = File.ReadAllLines(path);
        string? storage = "";
        foreach (string? line in takeIn) storage += line;
        UpdateFromText(storage);
    }
}