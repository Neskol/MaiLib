using System.Xml;

namespace MaiLib;

/// <summary>
///     Use xml to store track information
/// </summary>
public abstract class TrackInformation : IXmlUtility
{
    /// <summary>
    ///     Stores proper difficulties
    /// </summary>
    /// <value>1-15 Maimai level</value>
    public static readonly string[] level =
    {
        "1", "2", "3", "4", "5", "6", "7", "7+", "8", "8+", "9", "9+", "10", "10+", "11", "11+", "12", "12+", "13",
        "13+", "14", "14+", "15", "15+"
    }; // TODO: Convert to enum

    public static readonly string[] difficulty =
        { "Basic", "Advance", "Expert", "Master", "Remaster", "Utage", "Easy" }; // TODO: Convert to enum

    public static readonly string[] addVersion = { "Ver1.00.00" };

    /// <summary>
    ///     Stores the genre name used in information
    /// </summary>
    /// <value>103 = Touhou, 105 = maimai</value>
    public static readonly string[] genre = { "東方Project", "maimai" }; // TODO: Convert to enum

    /// <summary>
    ///     Stores prover maimai versions
    /// </summary>
    /// <value>Version name of each generation of Maimai</value>
    public static readonly string[] version =
    {
        "maimai", "maimai PLUS", "maimai GreeN", "maimai GreeN PLUS", "maimai ORANGE", "maimai ORANGE PLUS",
        "maimai PiNK", "maimai PiNK PLUS", "maimai MURASAKi", "maimai MURASAKi PLUS", "maimai MiLK", "maimai MiLK PLUS",
        "maimai FiNALE", "maimai DX", "maimai DX PLUS", "maimai DX Splash", "maimai DX Splash PLUS",
        "maimai DX UNiVERSE", "maimai DX UNiVERSE PLUS", "maimai DX FESTiVAL", "maimai DX FESTiVAL PLUS",
        "maimai DX BUDDiES"
    }; // TODO: Convert to enum

    public static readonly string[] shortVersion =
    {
        "maimai", "PLUS", "GreeN", "GreeN PLUS", "ORANGE", "ORANGE PLUS", "PiNK", "PiNK PLUS", "MURASAKi",
        "MURASAKi PLUS", "MiLK", "MiLK PLUS", "FiNALE", "DX", "DX PLUS", "DX Splash", "DX Splash PLUS", "DX UNiVERSE",
        "DX UNiVERSE PLUS", "DX FESTiVAL", "DX FESTiVAL PLUS", "DX BUDDiES"
    }; // TODO: Convert to enum

    // public static string[] versionArray;
    public static Dictionary<string, string> netOpenNameDic = new();
    public static Dictionary<string, string> releaseTagNameDic = new();
    public static Dictionary<string, string> rightsInfoDic = new();
    public static Dictionary<string, string> artistNameDic = new();
    public static Dictionary<string, string> addVersionDic = new();
    public static Dictionary<string, string> eventNameDic = new();
    public static Dictionary<string, string> subEventNameDic = new();
    public static Dictionary<string, string> notesDesignerDic = new();

    /// <summary>
    ///     Set of track information stored
    /// </summary>
    private Dictionary<string, string> information;

    /// <summary>
    ///     Internal stored information set
    /// </summary>
    private XmlDocument takeInValue;

    #region Constructors

    /// <summary>
    ///     Empty constructor
    /// </summary>
    public TrackInformation()
    {
        takeInValue = new XmlDocument();
        information = new Dictionary<string, string>();
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
        get => Information.GetValueOrDefault("Name") ?? throw new NullReferenceException("Name is not defined");
        set => information["Name"] = value;
    }

    /// <summary>
    ///     Return the sort name (basically English or Katakana)
    /// </summary>
    /// <value>this.SortName</value>
    public string TrackSortName
    {
        get => Information.GetValueOrDefault("Sort Name") ??
               throw new NullReferenceException("Sort Name is not defined");
        set => information["Sort Name"] = value;
    }

    /// <summary>
    ///     Return the track ID (4 digit, having 00 for SD 01 for DX)
    /// </summary>
    /// <value>this.TrackID</value>
    public string TrackID
    {
        get => Information.GetValueOrDefault("Music ID") ?? throw new NullReferenceException("Music ID is not defined");
        set => information["Music ID"] = value;
    }

    /// <summary>
    ///     Return the track genre (By default cabinet 6 categories)
    /// </summary>
    /// <value>this.TrackGenre</value>
    public string TrackGenre
    {
        get => Information.GetValueOrDefault("Genre") ?? throw new NullReferenceException("Genre is not defined");
        set => information["Genre"] = value;
    }

    /// <summary>
    ///     Return the track global BPM
    /// </summary>
    /// <value>this.TrackBPM</value>
    public string TrackBPM
    {
        get => Information.GetValueOrDefault("BPM") ?? throw new NullReferenceException("Genre is not defined");
        set => information["BPM"] = value;
    }

    /// <summary>
    ///     Return the track composer
    /// </summary>
    /// <value>this.TrackComposer</value>
    public string TrackComposer
    {
        get => Information.GetValueOrDefault("Composer") ?? throw new NullReferenceException("Genre is not defined");
        set => information["Composer"] = value;
    }

    /// <summary>
    ///     Return the most representative level of the track = set by default master
    /// </summary>
    /// <value>this.TrackLevel</value>
    public string TrackSymbolicLevel
    {
        get
        {
            if (Information.TryGetValue("Utage", out string? utageLevel) && level != null && !utageLevel.Equals(""))
                return utageLevel;
            if (Information.TryGetValue("Remaster", out string? remasLevel) && remasLevel != null &&
                !remasLevel.Equals(""))
                return remasLevel;
            if (Information.TryGetValue("Master", out string? masterLevel) && masterLevel != null &&
                !masterLevel.Equals(""))
                return masterLevel;
            if (Information.TryGetValue("Expert", out string? expertLevel) && expertLevel != null &&
                !expertLevel.Equals(""))
                return expertLevel;
            if (Information.TryGetValue("Advanced", out string? advanceLevel) && advanceLevel != null &&
                !advanceLevel.Equals(""))
                return advanceLevel;
            if (Information.TryGetValue("Basic", out string? basicLevel) && basicLevel != null &&
                !basicLevel.Equals(""))
                return basicLevel;
            if (Information.TryGetValue("Easy", out string? easyLevel) && easyLevel != null && !easyLevel.Equals(""))
                return easyLevel;
            return "ORIGINAL";
        }
        set => information["Master"] = value;
    }

    /// <summary>
    ///     Return track levels by [Easy, Basic, Advance, Expert, Master, Remaster, Utage]
    /// </summary>
    public string[] TrackLevels
    {
        get
        {
            string[] result =
            {
                Information["Easy"],
                Information["Basic"],
                Information["Advanced"],
                Information["Expert"],
                Information["Master"],
                Information["Remaster"],
                Information["Utage"]
            };
            return result;
        }
        set
        {
            Information["Easy"] = value[0];
            Information["Basic"] = value[1];
            Information["Advanced"] = value[2];
            Information["Expert"] = value[3];
            Information["Master"] = value[4];
            Information["Remaster"] = value[5];
            Information["Utage"] = value[6];
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
            {
                Information["Easy Decimal"],
                Information["Basic Decimal"],
                Information["Advanced Decimal"],
                Information["Expert Decimal"],
                Information["Master Decimal"],
                Information["Remaster Decimal"],
                Information["Utage Decimal"]
            };
            return result;
        }
        set
        {
            Information["Easy Decimal"] = value[0];
            Information["Basic Decimal"] = value[1];
            Information["Advanced Decimal"] = value[2];
            Information["Expert Decimal"] = value[3];
            Information["Master Decimal"] = value[4];
            Information["Remaster Decimal"] = value[5];
            Information["Utage Decimal"] = value[6];
        }
    }

    /// <summary>
    ///     Return the suffix of Track title for export
    /// </summary>
    /// <value>this.TrackSubstituteName"_DX" if is DX chart</value>
    public string DXChartTrackPathSuffix
    {
        get
        {
            string? musicID = Information.GetValueOrDefault("Music ID") ??
                              throw new NullReferenceException("Music ID is not Defined");
            if (musicID.Length > 4)
                return "_DX";
            return "";
        }
    }

    /// <summary>
    ///     Returns if the track is Standard or Deluxe
    /// </summary>
    /// <value>SD if standard, DX if deluxe</value>
    public string StandardDeluxePrefix
    {
        get
        {
            string? musicID = Information.GetValueOrDefault("Music ID") ??
                              throw new NullReferenceException("Music ID is not Defined");
            if (musicID.Length > 4)
                return "DX";
            return "SD";
        }
    }

    /// <summary>
    ///     Title suffix for better distinguish
    /// </summary>
    /// <value>[SD] if Standard and [DX] if Deluxe</value>
    public string StandardDeluxeSuffix => "[" + StandardDeluxePrefix + "]";

    /// <summary>
    ///     See if the chart is DX chart.
    /// </summary>
    /// <value>True if is DX, false if SD</value>
    public bool IsDXChart
    {
        get
        {
            string? musicID = Information.GetValueOrDefault("Music ID") ??
                              throw new NullReferenceException("Music ID is not Defined");
            return musicID.Length > 4;
        }
    }

    /// <summary>
    ///     Return this.TrackVersion
    /// </summary>
    /// <value>this.TrackVersion</value>
    public string TrackVersion
    {
        get
        {
            string? version = Information.GetValueOrDefault("Version") ??
                              throw new NullReferenceException("Version is not Defined");
            return version;
        }
        set => information["Version"] = value;
    }

    /// <summary>
    ///     Return this.TrackVersionNumber
    /// </summary>
    /// <value>this.TrackVersionNumber</value>
    public string TrackVersionNumber
    {
        get
        {
            string? versionNumber = Information.GetValueOrDefault("Version Number") ??
                                    throw new NullReferenceException("Version is not Defined");
            return versionNumber;
        }
        set => information["Version Number"] = value;
    }

    /// <summary>
    ///     Give access to TakeInValue if necessary
    /// </summary>
    /// <value>this.TakeInValue as XMLDocument</value>
    public XmlDocument TakeInValue
    {
        get => takeInValue;
        set => takeInValue = value;
    }

    /// <summary>
    ///     Give access to this.Information
    /// </summary>
    /// <value>this.Information as Dictionary</value>
    public Dictionary<string, string> Information
    {
        get => information;
        set => information = value;
    }

    /// <summary>
    ///     Return the XML node that has same name with
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public XmlNodeList GetMatchNodes(string name)
    {
        XmlNodeList? result = takeInValue.GetElementsByTagName(name);
        return result;
    }

    /// <summary>
    ///     Save the information to given path
    /// </summary>
    /// <param name="location">Path to save the information</param>
    public void Save(string location)
    {
        takeInValue.Save(location);
    }

    /// <summary>
    ///     Update information
    /// </summary>
    public abstract void Update();

    // /// <summary>
    // /// Construct track information from given location
    // /// </summary>
    // /// <param name="location">Place to load</param>
    // public TrackInformation(string location)
    // {
    //     {
    //         this.takeInValue = new XmlDocument();
    //         if (File.Exists(location + "Music.xml"))
    //         {
    //             this.takeInValue.Load(location + "Music.xml");
    //             this.information=new Dictionary<string, string>();
    //             this.FormatInformation();
    //             this.Update();
    //         }
    //         else
    //         {
    //             this.information=new Dictionary<string, string>();
    //             this.FormatInformation();
    //         }

    //     }
    // }

    /// <summary>
    ///     Add in necessary nodes in information.
    /// </summary>
    public void FormatInformation()
    {
        information = new Dictionary<string, string>
        {
            { "Name", "" },
            { "Sort Name", "" },
            { "Music ID", "" },
            { "Genre", "" },
            { "Version", "" },
            { "Version Number", "" },
            { "BPM", "" },
            { "Composer", "" },
            { "Easy", "" },
            { "Easy Decimal", "" },
            { "Easy Chart Maker", "" },
            { "Easy Chart Path", "" },
            { "Basic", "" },
            { "Basic Decimal", "" },
            { "Basic Chart Maker", "" },
            { "Basic Chart Path", "" },
            { "Advanced", "" },
            { "Advanced Decimal", "" },
            { "Advanced Chart Maker", "" },
            { "Advanced Chart Path", "" },
            { "Expert", "" },
            { "Expert Decimal", "" },
            { "Expert Chart Maker", "" },
            { "Expert Chart Path", "" },
            { "Master", "" },
            { "Master Decimal", "" },
            { "Master Chart Maker", "" },
            { "Master Chart Path", "" },
            { "Remaster", "" },
            { "Remaster Decimal", "" },
            { "Remaster Chart Maker", "" },
            { "Remaster Chart Path", "" },
            { "Utage", "" },
            { "Utage Decimal", "" },
            { "Utage Chart Maker", "" },
            { "Utage Chart Path", "" },
            { "SDDX Suffix", "" }
        };
    }

    /// <summary>
    ///     Add in necessary nodes in information for dummy chart.
    /// </summary>
    public void FormatDummyInformation()
    {
        information = new Dictionary<string, string>
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