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
        var takeIn = File.ReadAllLines(location);
        var storage = "";
        foreach (var line in takeIn) storage += line;
        return TokensFromText(storage);
    }

    public string[] TokensFromText(string text)
    {
        var storage = text;
        var result = new String(text.ToCharArray().Where(c=>!Char.IsWhiteSpace(c)).ToArray()).Split(',');
        return result;
    }


    /// <summary>
    ///     Update candidates from texts specified
    /// </summary>
    /// <param name="input">Text to be tokenized</param>
    public void UpdateFromText(string input)
    {
        var storage = input;
        var result = storage.Split("&");
        var titleCandidate = "";
        var bpmCandidate = "";
        var artistCandidate = "";
        var chartDesigner = "";
        var shortIdCandidate = "";
        var genreCandidate = "";
        var versionCandidate = "";

        foreach (var item in result)
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
                if (shortIdCandidate.Length <= 6 && int.TryParse(shortIdCandidate, out var id))
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
                var easyCandidate = item.Replace("lv_1=", "");
                simaiTrackInformation.Information["Easy"] = easyCandidate;
            }
            else if (item.Contains("des_1"))
            {
                var easyChartCandidate = item.Replace("des_1=", "");
                simaiTrackInformation.Information["Easy Chart Maker"] = easyChartCandidate;
            }
            else if (item.Contains("lv_2"))
            {
                var basicCandidate = item.Replace("lv_2=", "");
                simaiTrackInformation.Information["Basic"] = basicCandidate;
            }
            else if (item.Contains("des_2"))
            {
                var basicChartCandidate = item.Replace("des_2=", "");
                simaiTrackInformation.Information["Basic Chart Maker"] = basicChartCandidate;
            }
            else if (item.Contains("lv_3"))
            {
                var advancedCandidate = item.Replace("lv_3=", "");
                simaiTrackInformation.Information["Advanced"] = advancedCandidate;
            }
            else if (item.Contains("des_3"))
            {
                var advancedChartCandidate = item.Replace("des_3=", "");
                simaiTrackInformation.Information["Advanced Chart Maker"] = advancedChartCandidate;
            }
            else if (item.Contains("lv_4"))
            {
                var expertCandidate = item.Replace("lv_4=", "");
                simaiTrackInformation.Information["Expert"] = expertCandidate;
            }
            else if (item.Contains("des_4"))
            {
                var expertChartCandidate = item.Replace("des_4=", "");
                simaiTrackInformation.Information["Expert Chart Maker"] = expertChartCandidate;
            }
            else if (item.Contains("lv_5"))
            {
                var masterCandidate = item.Replace("lv_5=", "");
                simaiTrackInformation.Information["Master"] = masterCandidate;
            }
            else if (item.Contains("des_5"))
            {
                var masterChartCandidate = item.Replace("des_5=", "");
                simaiTrackInformation.Information["Master Chart Maker"] = masterChartCandidate;
            }
            else if (item.Contains("lv_6"))
            {
                var remasterCandidate = item.Replace("lv_6=", "");
                simaiTrackInformation.Information["Remaster"] = remasterCandidate;
            }
            else if (item.Contains("des_6"))
            {
                var remasterChartCandidate = item.Replace("des_6=", "");
                simaiTrackInformation.Information["Remaster Chart Maker"] = remasterChartCandidate;
            }
            else if (item.Contains("lv_7"))
            {
                var utageCandidate = item.Replace("lv_7=", "");
                simaiTrackInformation.Information["Utage"] = utageCandidate;
            }
            else if (item.Contains("des_7"))
            {
                var utageChartCandidate = item.Replace("des_7=", "");
                simaiTrackInformation.Information["Utage Chart Maker"] = utageChartCandidate;
            }
            else if (item.Contains("inote_2"))
            {
                var noteCandidate = item.Replace("inote_2=", "");
                chartCandidates.Add("2", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_3"))
            {
                var noteCandidate = item.Replace("inote_3=", "");
                chartCandidates.Add("3", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_4"))
            {
                var noteCandidate = item.Replace("inote_4=", "");
                chartCandidates.Add("4", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_5"))
            {
                var noteCandidate = item.Replace("inote_5=", "");
                chartCandidates.Add("5", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_6"))
            {
                var noteCandidate = item.Replace("inote_6=", "");
                chartCandidates.Add("6", TokensFromText(noteCandidate));
            }
            else if (item.Contains("inote_7"))
            {
                var noteCandidate = item.Replace("inote_7=", "");
                chartCandidates.Add("7", TokensFromText(noteCandidate));
            }
            // TODO: Fix this when note >= 8
            else if (item.Contains("inote_"))
            {
                var noteCandidate = item.Replace("inote_=", "");
                chartCandidates.Add("Default", TokensFromText(noteCandidate));
            }
    }

    /// <summary>
    ///     Update candidates from texts specified
    /// </summary>
    /// <param name="path">Location of text to be tokenized</param>
    public void UpdateFromPath(string path)
    {
        var takeIn = File.ReadAllLines(path);
        var storage = "";
        foreach (var line in takeIn) storage += line;
        UpdateFromText(storage);
    }
}