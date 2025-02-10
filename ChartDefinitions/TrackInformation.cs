using System.Reflection.Metadata.Ecma335;
using System.Xml;

namespace MaiLib;

/// <summary>
///     Use xml to store track Information
/// </summary>
public abstract class TrackInformation : IXmlUtility
{
    /// <summary>
    ///     Stores proper difficulties
    /// </summary>
    /// <value>1-15 Maimai level</value>
    public static readonly string[] Level =
    [
        "1", "2", "3", "4", "5", "6", "7", "7+", "8", "8+", "9", "9+", "10", "10+", "11", "11+", "12", "12+", "13",
        "13+", "14", "14+", "15", "15+"
    ]; // TODO: Convert to enum

    /// <summary>
    ///     Stores prover maimai versions
    /// </summary>
    /// <value>Version name of each generation of Maimai</value>
    public static readonly string[] Version =
    [
        "maimai", "maimai PLUS", "maimai GreeN", "maimai GreeN PLUS", "maimai ORANGE", "maimai ORANGE PLUS",
        "maimai PiNK", "maimai PiNK PLUS", "maimai MURASAKi", "maimai MURASAKi PLUS", "maimai MiLK", "maimai MiLK PLUS",
        "maimai FiNALE", "maimai DX", "maimai DX PLUS", "maimai DX Splash", "maimai DX Splash PLUS",
        "maimai DX UNiVERSE", "maimai DX UNiVERSE PLUS", "maimai DX FESTiVAL", "maimai DX FESTiVAL PLUS",
        "maimai DX BUDDiES", "maimai DX BUDDiES PLUS", "maimai DX PRiSM"
    ]; // TODO: Convert to enum

    public static readonly string[] ShortVersion =
    [
        "maimai", "PLUS", "GreeN", "GreeN PLUS", "ORANGE", "ORANGE PLUS", "PiNK", "PiNK PLUS", "MURASAKi",
        "MURASAKi PLUS", "MiLK", "MiLK PLUS", "FiNALE", "DX", "DX PLUS", "DX Splash", "DX Splash PLUS", "DX UNiVERSE",
        "DX UNiVERSE PLUS", "DX FESTiVAL", "DX FESTiVAL PLUS", "DX BUDDiES", "DX BUDDiES PLUS", "DX PRiSM"
    ]; // TODO: Convert to enum

    #region Constructors

    /// <summary>
    ///     Empty constructor
    /// </summary>
    public TrackInformation()
    {
        InternalXml = new XmlDocument();
        InformationDict = [];
        FormatInformation();
        Update();
    }

    #endregion

    /// <summary>
    ///     Return the track name
    /// </summary>
    /// <value>this.TrackName</value>
    public string TrackName
    {
        get => InformationDict.GetValueOrDefault("Name") ?? throw new NullReferenceException("Name is not defined");
        set => InformationDict["Name"] = value;
    }

    /// <summary>
    ///     Return the sort name (basically English or Katakana)
    /// </summary>
    /// <value>this.SortName</value>
    public string TrackSortName
    {
        get => InformationDict.GetValueOrDefault("Sort Name") ??
               throw new NullReferenceException("Sort Name is not defined");
        set => InformationDict["Sort Name"] = value;
    }

    /// <summary>
    ///     Return the track ID (4 digit, having 00 for SD 01 for DX)
    /// </summary>
    /// <value>this.TrackID</value>
    public string TrackID
    {
        get => InformationDict.GetValueOrDefault("Music ID") ??
               throw new NullReferenceException("Music ID is not defined");
        set => InformationDict["Music ID"] = value;
    }

    public string TrackIDShort => this.TrackID.Length > 4 ? this.TrackID[4..] : this.TrackID;


    /// <summary>
    ///     Return the 6-digit track ID
    /// </summary>
    /// <value>this.TrackID</value>
    public string TrackIDLong => CompensateZero(TrackID);

    /// <summary>
    ///     Return the track genre (By default cabinet 6 categories)
    /// </summary>
    /// <value>this.TrackGenre</value>
    public string TrackGenre
    {
        get => InformationDict.GetValueOrDefault("Genre") ?? throw new NullReferenceException("Genre is not defined");
        set => InformationDict["Genre"] = value;
    }

    /// <summary>
    ///     Return the track genre ID, start from 101 to 107 by default
    /// </summary>
    /// <value>this.TrackGenreID</value>
    public int TrackGenreID
    {
        get => int.Parse(InformationDict.GetValueOrDefault("Genre ID") ??
                         throw new NullReferenceException("Genre is not defined"));
        set => InformationDict["Genre ID"] = value.ToString();
    }

    /// <summary>
    ///     Return the track global BPM
    /// </summary>
    /// <value>this.TrackBPM</value>
    public string TrackBPM
    {
        get => InformationDict.GetValueOrDefault("BPM") ?? throw new NullReferenceException("Genre is not defined");
        set => InformationDict["BPM"] = value;
    }

    /// <summary>
    ///     Return the track composer
    /// </summary>
    /// <value>this.TrackComposer</value>
    public string TrackComposer
    {
        get => InformationDict.GetValueOrDefault("Composer") ??
               throw new NullReferenceException("Genre is not defined");
        set => InformationDict["Composer"] = value;
    }

    /// <summary>
    ///     Return the track composer
    /// </summary>
    /// <value>this.TrackComposer</value>
    public int TrackComposerID
    {
        get => int.Parse(InformationDict.GetValueOrDefault("Composer ID") ??
                         throw new NullReferenceException("Genre is not defined"));
        set => InformationDict["Composer ID"] = value.ToString();
    }

    /// <summary>
    ///     Return the most representative level of the track = set by default master
    /// </summary>
    /// <value>this.TrackLevel</value>
    public string TrackSymbolicLevel
    {
        get
        {
            if (InformationDict.TryGetValue("Utage", out string? utageLevel) && Level != null && !utageLevel.Equals(""))
                return utageLevel;
            if (InformationDict.TryGetValue("Remaster", out string? remasLevel) && remasLevel != null &&
                !remasLevel.Equals(""))
                return remasLevel;
            if (InformationDict.TryGetValue("Master", out string? masterLevel) && masterLevel != null &&
                !masterLevel.Equals(""))
                return masterLevel;
            if (InformationDict.TryGetValue("Expert", out string? expertLevel) && expertLevel != null &&
                !expertLevel.Equals(""))
                return expertLevel;
            if (InformationDict.TryGetValue("Advanced", out string? advanceLevel) && advanceLevel != null &&
                !advanceLevel.Equals(""))
                return advanceLevel;
            if (InformationDict.TryGetValue("Basic", out string? basicLevel) && basicLevel != null &&
                !basicLevel.Equals(""))
                return basicLevel;
            if (InformationDict.TryGetValue("Easy", out string? easyLevel) && easyLevel != null &&
                !easyLevel.Equals(""))
                return easyLevel;
            return "ORIGINAL";
        }
        set => InformationDict["Master"] = value;
    }

    /// <summary>
    ///     Return track levels by [Easy, Basic, Advance, Expert, Master, Remaster, Utage]
    /// </summary>
    public string[] TrackLevels
    {
        get =>
        [
            InformationDict["Easy"],
            InformationDict["Basic"],
            InformationDict["Advanced"],
            InformationDict["Expert"],
            InformationDict["Master"],
            InformationDict["Remaster"],
            InformationDict["Utage"]
        ];
        set
        {
            InformationDict["Easy"] = value[0];
            InformationDict["Basic"] = value[1];
            InformationDict["Advanced"] = value[2];
            InformationDict["Expert"] = value[3];
            InformationDict["Master"] = value[4];
            InformationDict["Remaster"] = value[5];
            InformationDict["Utage"] = value[6];
        }
    }

    /// <summary>
    ///     Return track decimal levels by [Easy, Basic, Advance, Expert, Master, Remaster, Utage] * Utage returns utage level
    /// </summary>
    public string[] TrackDecimalLevels
    {
        get
        {
            string[] result =
            [
                InformationDict["Easy Decimal"],
                InformationDict["Basic Decimal"],
                InformationDict["Advanced Decimal"],
                InformationDict["Expert Decimal"],
                InformationDict["Master Decimal"],
                InformationDict["Remaster Decimal"],
                InformationDict["Utage Decimal"]
            ];
            return result;
        }
        set
        {
            InformationDict["Easy Decimal"] = value[0];
            InformationDict["Basic Decimal"] = value[1];
            InformationDict["Advanced Decimal"] = value[2];
            InformationDict["Expert Decimal"] = value[3];
            InformationDict["Master Decimal"] = value[4];
            InformationDict["Remaster Decimal"] = value[5];
            InformationDict["Utage Decimal"] = value[6];
        }
    }

    /// <summary>
    ///     Return the suffix of Track title for export
    /// </summary>
    /// <value>this.TrackSubstituteName"_DX" if is DX chart</value>
    public string DXChartTrackPathSuffix => IsDXChart ? "_DX" : "";

    /// <summary>
    ///     Returns if the track is Standard or Deluxe
    /// </summary>
    /// <value>SD if standard, DX if deluxe</value>
    public string StandardDeluxePrefix => IsDXChart ? "DX" : "SD";

    /// <summary>
    ///     Title suffix for better distinguish
    /// </summary>
    /// <value>[SD] if Standard and [DX] if Deluxe</value>
    public string StandardDeluxeSuffix => $"[{StandardDeluxePrefix}]";

    /// <summary>
    ///     See if the chart is DX chart.
    /// </summary>
    /// <value>True if is DX, false if SD</value>
    public bool IsDXChart => TrackIDLong[1] is '1';
    // {
    //     get
    //     {
    //         string? musicID = CompensateZero(InformationDict.GetValueOrDefault("Music ID") ??
    //                                          throw new NullReferenceException("Music ID is not Defined"));
    //         return int.Parse(musicID.Substring(2)).ToString().Length >= 4;
    //     }
    // }

    /// <summary>
    ///     See if the chart is DX Utage chart.
    /// </summary>
    /// <value>True if is DX Utage, false elsewise</value>
    public bool IsDXUtage => TrackIDLong[0] is '1';

    /// <summary>
    ///     Return this.TrackVersion
    /// </summary>
    /// <value>this.TrackVersion</value>
    public string TrackVersion
    {
        get
        {
            string? version = InformationDict.GetValueOrDefault("Version") ??
                              throw new NullReferenceException("Version is not Defined");
            return version;
        }
        set => InformationDict["Version"] = value;
    }

    /// <summary>
    ///     Return this.TrackVersion
    /// </summary>
    /// <value>this.TrackVersion</value>
    public int TrackVersionID
    {
        get => int.Parse(InformationDict.GetValueOrDefault("Version ID") ??
                         throw new NullReferenceException("Genre is not defined"));
        set => InformationDict["Version ID"] = value.ToString();
    }

    /// <summary>
    ///     Return this.TrackVersionNumber
    /// </summary>
    /// <value>this.TrackVersionNumber</value>
    public string TrackVersionNumber
    {
        get
        {
            string? versionNumber = InformationDict.GetValueOrDefault("Version Number") ??
                                    throw new NullReferenceException("Version is not Defined");
            return versionNumber;
        }
        set => InformationDict["Version Number"] = value;
    }

    /// <summary>
    ///     Give access to TakeInValue if necessary
    /// </summary>
    /// <value>this.TakeInValue as XMLDocument</value>
    public XmlDocument InternalXml { get; set; }

    /// <summary>
    ///     Give access to this.Information
    /// </summary>
    /// <value>this.Information as Dictionary</value>
    public Dictionary<string, string> InformationDict { get; set; }

    /// <summary>
    ///     Return the XML node that has same name with
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public XmlNodeList GetMatchNodes(string name)
    {
        XmlNodeList? result = InternalXml.GetElementsByTagName(name);
        return result;
    }

    /// <summary>
    ///     Save the Information to given path
    /// </summary>
    /// <param name="location">Path to save the Information</param>
    public void Save(string location)
    {
        InternalXml.Save(location);
    }

    /// <summary>
    ///     Update Information
    /// </summary>
    public abstract void Update();

    // /// <summary>
    // /// Construct track Information from given location
    // /// </summary>
    // /// <param name="location">Place to load</param>
    // public TrackInformation(string location)
    // {
    //     {
    //         this.TakeInValue = new XmlDocument();
    //         if (File.Exists(location + "Music.xml"))
    //         {
    //             this.TakeInValue.Load(location + "Music.xml");
    //             this.Information=new Dictionary<string, string>();
    //             this.FormatInformation();
    //             this.Update();
    //         }
    //         else
    //         {
    //             this.Information=new Dictionary<string, string>();
    //             this.FormatInformation();
    //         }

    //     }
    // }

    /// <summary>
    ///     Add in necessary nodes in Information.
    /// </summary>
    public void FormatInformation()
    {
        InformationDict = new Dictionary<string, string>
        {
            { "Name", "" },
            { "Sort Name", "" },
            { "Music ID", "" },
            { "Genre", "" },
            { "Genre ID", "" },
            { "Version", "" },
            { "Version ID", "" },
            { "Version Number", "" },
            { "BPM", "" },
            { "Composer", "" },
            { "Composer ID", "" },
            { "Easy", "" },
            { "Easy Decimal", "" },
            { "Easy Chart Maker", "" },
            { "Easy Maker ID", "" },
            { "Easy Chart Path", "" },
            { "Easy Max Note", "" },
            { "Basic", "" },
            { "Basic Decimal", "" },
            { "Basic Chart Maker", "" },
            { "Basic Maker ID", "" },
            { "Basic Chart Path", "" },
            { "Basic Max Note", "" },
            { "Advanced", "" },
            { "Advanced Decimal", "" },
            { "Advanced Chart Maker", "" },
            { "Advanced Maker ID", "" },
            { "Advanced Chart Path", "" },
            { "Advanced Max Note", "" },
            { "Expert", "" },
            { "Expert Decimal", "" },
            { "Expert Chart Maker", "" },
            { "Expert Maker ID", "" },
            { "Expert Chart Path", "" },
            { "Expert Max Note", "" },
            { "Master", "" },
            { "Master Decimal", "" },
            { "Master Chart Maker", "" },
            { "Master Maker ID", "" },
            { "Master Chart Path", "" },
            { "Master Max Note", "" },
            { "Remaster", "" },
            { "Remaster Decimal", "" },
            { "Remaster Chart Maker", "" },
            { "Remaster Maker ID", "" },
            { "Remaster Chart Path", "" },
            { "Remaster Max Note", "" },
            { "Utage", "" },
            { "Utage Decimal", "" },
            { "Utage Chart Maker", "" },
            { "Utage Maker ID", "" },
            { "Utage Chart Path", "" },
            { "Utage Max Note", "" },
            { "Utage Kanji", "" },
            { "Utage Comment", "" },
            { "Utage Play Style", "" },
            { "SDDX Suffix", "" }
        };
    }

    /// <summary>
    ///     Add in necessary nodes in Information for dummy chart.
    /// </summary>
    public void FormatDummyInformation()
    {
        InformationDict = new Dictionary<string, string>
        {
            { "Name", "Dummy" },
            { "Sort Name", "DUMMY" },
            { "Music ID", "000000" },
            { "Genre", "maimai" },
            { "Version", "maimai" },
            { "Version Number", "101" },
            { "BPM", "120" },
            { "Composer", "SEGA" },
            { "Easy", "1" },
            { "Easy Decimal", "1.0" },
            { "Easy Chart Maker", "SEGA" },
            { "Easy Chart Path", "000000_11.ma2" },
            { "Basic", "2" },
            { "Basic Decimal", "2.0" },
            { "Basic Chart Maker", "SEGA" },
            { "Basic Chart Path", "000000_00.ma2" },
            { "Advanced", "3" },
            { "Advanced Decimal", "3.0" },
            { "Advanced Chart Maker", "SEGA" },
            { "Advanced Chart Path", "000000_01.ma2" },
            { "Expert", "4" },
            { "Expert Decimal", "4.0" },
            { "Expert Chart Maker", "SEGA" },
            { "Expert Chart Path", "000000_02.ma2" },
            { "Master", "5" },
            { "Master Decimal", "5.0" },
            { "Master Chart Maker", "SEGA" },
            { "Master Chart Path", "000000_03.ma2" },
            { "Remaster", "6" },
            { "Remaster Decimal", "6.0" },
            { "Remaster Chart Maker", "SEGA" },
            { "Remaster Chart Path", "000000_04.ma2" },
            { "Utage", "11" },
            { "Utage Decimal", "11.0" },
            { "Utage Chart Maker", "SEGA" },
            { "Utage Chart Path", "000000_05.ma2" },
            { "SDDX Suffix", "SD" }
        };
    }

    /// <summary>
    ///     Compensate 0 for music IDs
    /// </summary>
    /// <param name="intake">Music ID</param>
    /// <returns>0..+#Music ID and |Music ID|==6</returns>
    public static string CompensateZero(string intake)
    {
        try
        {
            string? result = intake;
            while (result.Length < 6 && intake != null) result = "0" + result;
            return result;
        }
        catch (NullReferenceException ex)
        {
            return "Exception raised: " + ex.Message;
        }
    }

    /// <summary>
    ///     Compensate 0 for short music IDs
    /// </summary>
    /// <param name="intake">Music ID</param>
    /// <returns>0..+#Music ID and |Music ID|==4</returns>
    public static string CompensateShortZero(string intake)
    {
        try
        {
            string? result = intake;
            while (result.Length < 4 && intake != null) result = "0" + result;
            return result;
        }
        catch (NullReferenceException ex)
        {
            return "Exception raised: " + ex.Message;
        }
    }
}