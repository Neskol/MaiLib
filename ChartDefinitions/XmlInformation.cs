using System.Runtime.InteropServices;
using System.Xml;

namespace MaiLib;

/// <summary>
///     Using Xml to store trackInformation
/// </summary>
public class XmlInformation : TrackInformation, IXmlUtility
{
    private const int UtageGenreId = 107;

    #region Constructors

    /// <summary>
    ///     Using take in Xml to store trackInformation:
    /// </summary>
    public XmlInformation()
    {
        Update();
    }

    public XmlInformation(string location)
    {
        {
            if (File.Exists(location + "Music.xml"))
            {
                InternalXml.Load(location + "Music.xml");
                Update();
            }
            else
            {
                Update();
            }
        }
    }

    #endregion

    public override void Update()
    {
        int genreId = 0;
        XmlNodeList? nameCandidate = InternalXml.GetElementsByTagName("name");
        XmlNodeList? bpmCandidate = InternalXml.GetElementsByTagName("bpm");
        XmlNodeList? chartCandidate = InternalXml.GetElementsByTagName("Notes");
        XmlNodeList? composerCandidate = InternalXml.GetElementsByTagName("artistName");
        XmlNodeList? genreCandidate = InternalXml.GetElementsByTagName("genreName");
        XmlNodeList? addVersionCandidate = InternalXml.GetElementsByTagName("AddVersion");
        XmlNodeList? sortNameCandidate = InternalXml.GetElementsByTagName("sortName");
        XmlNodeList? versionNumberCandidate = InternalXml.GetElementsByTagName("releaseTagName");
        XmlNodeList? utageKanjiCandidate = InternalXml.GetElementsByTagName("utageKanjiName");
        XmlNodeList? utageCommentCandidate = InternalXml.GetElementsByTagName("comment");
        XmlNodeList? utagePlayStyleCandidate = InternalXml.GetElementsByTagName("utagePlayStyle");
        XmlNodeList? utageFixedOptionsCandidate = InternalXml.GetElementsByTagName("FixedOption");


        //Add in name and music ID.
        ////Add BPM
        //this.information.Add("BPM",bpmCandidate[0].InnerText);
        foreach (XmlNode candidate in nameCandidate)
            if (TrackID.Equals(""))
            {
                XmlElement? idCandidate = candidate["id"] ?? throw new NullReferenceException();
                XmlElement? strCandidate = candidate["str"] ?? throw new NullReferenceException();
                TrackID = idCandidate.InnerText;
                TrackName = strCandidate.InnerText;
            }

        foreach (XmlNode candidate in bpmCandidate)
        {
            if (TrackBPM.Equals("")) TrackBPM = candidate.InnerText;
        }

        foreach (XmlNode candidate in sortNameCandidate)
        {
            if (TrackSortName.Equals("")) TrackSortName = candidate.InnerText;
        }

        foreach (XmlNode candidate in composerCandidate)
        {
            if (TrackComposer.Equals(""))
            {
                XmlElement? idCandidate = candidate["id"] ?? throw new NullReferenceException();
                XmlElement? strCandidate = candidate["str"] ?? throw new NullReferenceException();
                TrackComposerID = int.Parse(idCandidate.InnerText);
                TrackComposer = strCandidate.InnerText;
            }
        }

        foreach (XmlNode candidate in genreCandidate)
        {
            if (TrackGenre.Equals(""))
            {
                XmlElement? idCandidate = candidate["id"] ?? throw new NullReferenceException();
                XmlElement? strCandidate = candidate["str"] ?? throw new NullReferenceException();
                genreId = int.Parse(idCandidate.InnerText);
                TrackGenreID = genreId;
                TrackGenre = strCandidate.InnerText;
            }
        }

        foreach (XmlNode candidate in versionNumberCandidate)
        {
            if (TrackVersionNumber.Equals(""))
            {
                XmlElement? strCandidate = candidate["str"] ?? throw new NullReferenceException();
                TrackVersionNumber = strCandidate.InnerText;
            }
        }

        foreach (XmlNode candidate in addVersionCandidate)
        {
            if (TrackVersion.Equals(""))
            {
                XmlElement? idCandidate = candidate["id"] ?? throw new NullReferenceException();
                TrackVersionID = int.Parse(idCandidate.InnerText);
                TrackVersion = Version[int.Parse(idCandidate.InnerText)];
            }
        }

        foreach (XmlNode candidate in chartCandidate)
            try
            {
                XmlElement? pathCandidate = candidate["file"] ?? throw new NullReferenceException();
                pathCandidate = pathCandidate["path"] ?? throw new NullReferenceException();
                XmlElement? enableCandidate = candidate["isEnable"] ?? throw new NullReferenceException();
                if (pathCandidate.InnerText.Contains("00.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();

                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerIdCandidate =
                        notesDesignerCandidate["id"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();

                    if (genreId == UtageGenreId)
                    {
                        InformationDict["Utage Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;

                        InformationDict["Utage"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                            ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                            : "";
                        InformationDict["Utage Chart Maker"] = notesDesignerCandidate.InnerText;
                        InformationDict["Utage Maker ID"] = notesDesignerIdCandidate.InnerText;
                        InformationDict["Utage Chart Path"] = fileCandidate.InnerText;
                    }
                    else
                    {
                        InformationDict["Basic Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;

                        InformationDict["Basic"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                            ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                            : "";
                        InformationDict["Basic Chart Maker"] = notesDesignerCandidate.InnerText;
                        InformationDict["Basic Maker ID"] = notesDesignerIdCandidate.InnerText;
                        InformationDict["Basic Chart Path"] = fileCandidate.InnerText;
                    }
                }
                else if (pathCandidate.InnerText.Contains("01.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    InformationDict["Advanced Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerIdCandidate =
                   notesDesignerCandidate["id"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    InformationDict["Advanced"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    InformationDict["Advanced Chart Maker"] = notesDesignerCandidate.InnerText;
                    InformationDict["Advanced Maker ID"] = notesDesignerIdCandidate.InnerText;
                    InformationDict["Advanced Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("02.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    InformationDict["Expert Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerIdCandidate =
                   notesDesignerCandidate["id"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    InformationDict["Expert"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    InformationDict["Expert Chart Maker"] = notesDesignerCandidate.InnerText;
                    InformationDict["Expert Maker ID"] = notesDesignerIdCandidate.InnerText;
                    InformationDict["Expert Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("03.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    InformationDict["Master Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerIdCandidate =
                   notesDesignerCandidate["id"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    InformationDict["Master"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    InformationDict["Master Chart Maker"] = notesDesignerCandidate.InnerText;
                    InformationDict["Master Maker ID"] = notesDesignerIdCandidate.InnerText;
                    InformationDict["Master Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("04.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    InformationDict["Remaster Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerIdCandidate =
                   notesDesignerCandidate["id"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    InformationDict["Remaster"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    InformationDict["Remaster Chart Maker"] = notesDesignerCandidate.InnerText;
                    InformationDict["Remaster Maker ID"] = notesDesignerIdCandidate.InnerText;
                    InformationDict["Remaster Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("05.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    InformationDict["Utage Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    InformationDict["Utage"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    InformationDict["Utage Chart Maker"] = notesDesignerCandidate.InnerText;
                    InformationDict["Utage Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("11.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    InformationDict["Easy Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerIdCandidate =
                   notesDesignerCandidate["id"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    InformationDict["Easy"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    InformationDict["Easy Chart Maker"] = notesDesignerCandidate.InnerText;
                    InformationDict["Easy Maker ID"] = notesDesignerIdCandidate.InnerText;
                    InformationDict["Easy Chart Path"] = fileCandidate.InnerText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is no such chart: " + ex.Message);
            }

        foreach (XmlNode candidate in utageKanjiCandidate)
        {
            if (InformationDict["Utage Kanji"].Equals("")) InformationDict["Utage Kanji"] = candidate.InnerText;
        }

        foreach (XmlNode candidate in utageCommentCandidate)
        {
            if (InformationDict["Utage Comment"].Equals("")) InformationDict["Utage Comment"] = candidate.InnerText;
        }

        foreach (XmlNode candidate in utagePlayStyleCandidate)
        {
            if (InformationDict["Utage Play Style"].Equals("")) InformationDict["Utage Play Style"] = candidate.InnerText;
        }

        /// Need to figure this out later
        // foreach (XmlNode candidate in utageFixedOptionsCandidate)
        // {
        //     string optionName = "None";
        //     string optionValue = "None";
        //     if (candidate["_fixedOptionName"] is not null) optionName = candidate["_fixedOptionName"]?.InnerText ?? "None";
        //     if (candidate["_fixedOptionValue"] is not null) optionValue = candidate["_fixedOptionName"]?.InnerText ?? "None";
        //     if (optionName is not "None" or "") InformationDict.Add($"Utage Fixed Option {optionName}", optionValue);
        // }

        InformationDict["SDDX Suffix"] = StandardDeluxeSuffix;
    }

    /// <summary>
    ///     Generate new music.xml for export
    /// </summary>
    public void GenerateInternalXml()
    {
        InternalXml = new XmlDocument();
        //Create declaration
        XmlDeclaration? dec = InternalXml.CreateXmlDeclaration("1.0", "utf-8", "yes");
        InternalXml.AppendChild(dec);
        //Create Root and append attributes
        XmlElement? root = InternalXml.CreateElement("MusicData");
        InternalXml.AppendChild(root);
        XmlAttribute? xsi = InternalXml.CreateAttribute("xmlns:xsi");
        xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
        XmlAttribute? xsd = InternalXml.CreateAttribute("xmlns:xsd");

        //Create tags. *data name: inner text = music0xxxxx
        XmlElement? dataName = InternalXml.CreateElement("dataName");
        dataName.InnerText = "music" + CompensateZero(InformationDict["Music ID"]);
        root.AppendChild(dataName);
        XmlElement? netOpenName = InternalXml.CreateElement("netOpenName");
        XmlElement? netOpenNameId = InternalXml.CreateElement("id");
        netOpenNameId.InnerText = "0";
        XmlElement? netOpenNameStr = InternalXml.CreateElement("str");
        netOpenNameStr.InnerText = "Net190711";
        netOpenName.AppendChild(netOpenNameId);
        netOpenName.AppendChild(netOpenNameStr);
        root.AppendChild(netOpenName);
        XmlElement? releaseTagName = InternalXml.CreateElement("releaseTagName");
        XmlElement? releaseTagNameId = InternalXml.CreateElement("id");
        releaseTagNameId.InnerText = "1";
        XmlElement? releaseTagNameStr = InternalXml.CreateElement("str");
        releaseTagNameStr.InnerText = "Ver1.00.00";
        releaseTagName.AppendChild(releaseTagNameId);
        releaseTagName.AppendChild(releaseTagNameStr);
        root.AppendChild(releaseTagName);
        XmlElement? disable = InternalXml.CreateElement("disable");
        disable.InnerText = "false";
        root.AppendChild(disable);
        XmlElement? name = InternalXml.CreateElement("name");
        XmlElement? nameId = InternalXml.CreateElement("id");
        nameId.InnerText = TrackID;
        XmlElement? nameStr = InternalXml.CreateElement("str");
        nameStr.InnerText = TrackName;
        name.AppendChild(nameId);
        name.AppendChild(nameStr);
        root.AppendChild(name);
        XmlElement? rightsInfoName = InternalXml.CreateElement("rightsInfoName");
        XmlElement? rightsInfoNameId = InternalXml.CreateElement("id");
        rightsInfoNameId.InnerText = "0";
        XmlElement? rightsInfoNameStr = InternalXml.CreateElement("str");
        rightsInfoNameStr.InnerText = "";
        rightsInfoName.AppendChild(rightsInfoNameId);
        rightsInfoName.AppendChild(rightsInfoNameStr);
        root.AppendChild(rightsInfoName);
        XmlElement? sortName = InternalXml.CreateElement("sortName");
        sortName.InnerText = TrackSortName;
        root.AppendChild(sortName);
        XmlElement? artistName = InternalXml.CreateElement("artistName");
        XmlElement? artistNameId = InternalXml.CreateElement("id");
        artistNameId.InnerText = "0";
        XmlElement? artistNameStr = InternalXml.CreateElement("str");
        artistNameStr.InnerText = InformationDict["Composer"];
        artistName.AppendChild(artistNameId);
        artistName.AppendChild(artistNameStr);
        root.AppendChild(artistName);
        XmlElement? genreName = InternalXml.CreateElement("genreName");
        XmlElement? genreNameId = InternalXml.CreateElement("id");
        genreNameId.InnerText = TrackGenreID.ToString();
        XmlElement? genreNameStr = InternalXml.CreateElement("str");
        genreNameStr.InnerText = TrackGenre;
        genreName.AppendChild(genreNameId);
        genreName.AppendChild(genreNameStr);
        root.AppendChild(genreName);
        XmlElement? bpm = InternalXml.CreateElement("bpm");
        bpm.InnerText = TrackBPM;
        root.AppendChild(bpm);
        XmlElement? version = InternalXml.CreateElement("version");
        version.InnerText = "19000";
        root.AppendChild(version);
        XmlElement? addVersion = InternalXml.CreateElement("addVersion");
        XmlElement? addVersionId = InternalXml.CreateElement("id");
        addVersionId.InnerText = TrackVersionID.ToString();
        XmlElement? addVersionStr = InternalXml.CreateElement("str");
        addVersionStr.InnerText = ShortVersion[TrackVersionID];
        addVersion.AppendChild(addVersionId);
        addVersion.AppendChild(addVersionStr);
        root.AppendChild(addVersion);
        XmlElement? movieName = InternalXml.CreateElement("movieName");
        XmlElement? movieNameId = InternalXml.CreateElement("id");
        movieNameId.InnerText = TrackID;
        XmlElement? movieNameStr = InternalXml.CreateElement("str");
        movieNameStr.InnerText = TrackName;
        movieName.AppendChild(movieNameId);
        movieName.AppendChild(movieNameStr);
        root.AppendChild(movieName);
        XmlElement? cueName = InternalXml.CreateElement("cueName");
        XmlElement? cueNameId = InternalXml.CreateElement("id");
        cueNameId.InnerText = TrackID;
        XmlElement? cueNameStr = InternalXml.CreateElement("str");
        cueNameStr.InnerText = TrackName;
        cueName.AppendChild(cueNameId);
        cueName.AppendChild(cueNameStr);
        root.AppendChild(cueName);
        XmlElement? dressCode = InternalXml.CreateElement("dressCode");
        dressCode.InnerText = "false";
        root.AppendChild(dressCode);
        XmlElement? eventName = InternalXml.CreateElement("eventName");
        XmlElement? eventNameId = InternalXml.CreateElement("id");
        eventNameId.InnerText = "1";
        XmlElement? eventNameStr = InternalXml.CreateElement("str");
        eventNameStr.InnerText = "無期限常時解放";
        eventName.AppendChild(eventNameId);
        eventName.AppendChild(eventNameStr);
        root.AppendChild(eventName);
        XmlElement? subEventName = InternalXml.CreateElement("subEventName");
        XmlElement? subEventNameId = InternalXml.CreateElement("id");
        subEventNameId.InnerText = "1";
        XmlElement? subEventNameStr = InternalXml.CreateElement("str");
        subEventNameStr.InnerText = "無期限常時解放";
        subEventName.AppendChild(subEventNameId);
        subEventName.AppendChild(subEventNameStr);
        root.AppendChild(subEventName);
        XmlElement? lockType = InternalXml.CreateElement("lockType");
        lockType.InnerText = "0";
        root.AppendChild(lockType);
        XmlElement? subLockType = InternalXml.CreateElement("subLockType");
        subLockType.InnerText = "1";
        root.AppendChild(subLockType);
        XmlElement? dotNetListView = InternalXml.CreateElement("dotNetListView");
        dotNetListView.InnerText = "true";
        root.AppendChild(dotNetListView);
        XmlElement? notesData = InternalXml.CreateElement("notesData");
        int currentDiff;
        for (currentDiff = 0; currentDiff < 6; currentDiff++) try
        {
            XmlElement? noteCandidate = InternalXml.CreateElement("Notes");
            XmlElement? fileCandidate = InternalXml.CreateElement("file");
            XmlElement? pathCandidate = InternalXml.CreateElement("path");
            pathCandidate.InnerText = $"{CompensateZero(TrackID)}_0{currentDiff}.ma2";
            fileCandidate.AppendChild(pathCandidate);
            XmlElement? levelCandidate = InternalXml.CreateElement("level");
            XmlElement? levelDecimalCandidate = InternalXml.CreateElement("levelDecimal");
            XmlElement? notesDesignerCandidate = InternalXml.CreateElement("notesDesigner");
            XmlElement? notesDesignerIdCandidate = InternalXml.CreateElement("id");
            XmlElement? notesDesignerStrCandidate = InternalXml.CreateElement("str");
            XmlElement? notesTypeCandidate = InternalXml.CreateElement("notesType");
            notesTypeCandidate.InnerText = "0";
            XmlElement? musicLevelIDCandidate = InternalXml.CreateElement("musicLevelID");
            XmlElement? maxNotesCandidate = InternalXml.CreateElement("maxNotes");
            XmlElement? isEnabledCandidate = InternalXml.CreateElement("isEnabled");

            switch (currentDiff)
            {
                case 0:
                    notesDesignerStrCandidate.InnerText = InformationDict["Basic Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Basic Maker ID"].Equals("") ? "0" : InformationDict["Basic Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Basic"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Basic Max Note"].Equals("") ? "0" : InformationDict["Basic Max Note"];
                    break;
                case 1:
                    notesDesignerStrCandidate.InnerText = InformationDict["Advanced Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Advanced Maker ID"].Equals("") ? "0" : InformationDict["Advanced Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Advanced"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Advanced Max Note"].Equals("") ? "0" : InformationDict["Advanced Max Note"];
                    break;
                case 2:
                    notesDesignerStrCandidate.InnerText = InformationDict["Expert Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Expert Maker ID"].Equals("") ? "0" : InformationDict["Expert Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Expert"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Expert Max Note"].Equals("") ? "0" : InformationDict["Expert Max Note"];
                    break;
                case 3:
                    notesDesignerStrCandidate.InnerText = InformationDict["Master Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Master Maker ID"].Equals("") ? "0" : InformationDict["Master Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Master"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Master Max Note"].Equals("") ? "0" : InformationDict["Master Max Note"];
                    break;
                case 4:
                    notesDesignerStrCandidate.InnerText = InformationDict["Remaster Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Remaster Maker ID"].Equals("") ? "0" : InformationDict["Remaster Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Remaster"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Remaster Max Note"].Equals("") ? "0" : InformationDict["Remaster Max Note"];
                    break;
                case 5:
                    notesDesignerStrCandidate.InnerText = InformationDict["Utage Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Utage Maker ID"].Equals("") ? "0" : InformationDict["Utage Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Utage"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Utage Max Note"].Equals("") ? "0" : InformationDict["Utage Max Note"];
                    break;
                case 6:
                    notesDesignerStrCandidate.InnerText = InformationDict["Original Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Original Maker ID"].Equals("") ? "0" : InformationDict["Original Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Original"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Original Max Note"].Equals("") ? "0" : InformationDict["Original Max Note"];
                    break;
                case 11:
                    notesDesignerStrCandidate.InnerText = InformationDict["Easy Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict["Easy Maker ID"].Equals("") ? "0" : InformationDict["Easy Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict["Easy"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict["Easy Max Note"].Equals("") ? "0" : InformationDict["Easy Max Note"];
                    break;
                default:
                    // Reserves condition for more original charts
                    notesDesignerStrCandidate.InnerText = InformationDict[$"Original {currentDiff - 6} Chart Maker"];
                    notesDesignerIdCandidate.InnerText = InformationDict[$"Original {currentDiff - 6} Maker ID"].Equals("") ? "0" : InformationDict["Original Maker ID"];
                    isEnabledCandidate.InnerText = InformationDict[$"Original {currentDiff - 6}"].Equals("") ? "false" : "true"; // Because bool.ToString() returns True or False
                    maxNotesCandidate.InnerText = InformationDict[$"Original {currentDiff - 6} Max Note"].Equals("") ? "0" : InformationDict[$"Original {currentDiff - 6} Max Note"];
                    break;

            }

            notesDesignerCandidate.AppendChild(notesDesignerIdCandidate);
            notesDesignerCandidate.AppendChild(notesDesignerStrCandidate);
            if (!TrackLevels[currentDiff].Equals(""))
            {
                levelCandidate.InnerText = (Array.IndexOf(Level, TrackLevels[currentDiff]) + 1).ToString();
                musicLevelIDCandidate.InnerText = TrackDecimalLevels[currentDiff].Split('.')[1];
            }
            else
            {
                levelCandidate.InnerText = "0";
                musicLevelIDCandidate.InnerText = "0";
            }

            // levelCandidate.InnerText = TrackDecimalLevels[currentDiff].Equals("") ? "0" : TrackDecimalLevels[currentDiff];
            noteCandidate.AppendChild(fileCandidate);
            noteCandidate.AppendChild(levelCandidate);
            noteCandidate.AppendChild(levelDecimalCandidate);
            noteCandidate.AppendChild(notesDesignerCandidate);
            noteCandidate.AppendChild(notesTypeCandidate);
            noteCandidate.AppendChild(musicLevelIDCandidate);
            noteCandidate.AppendChild(maxNotesCandidate);
            noteCandidate.AppendChild(isEnabledCandidate);
            notesData.AppendChild(noteCandidate);
        }
        catch (KeyNotFoundException ex)
        {
            Console.WriteLine("{0} presented at difficulty {1}, skipping", ex.GetType(), currentDiff);
        }
        root.AppendChild(notesData);

        // Following are reserved for Utage charts
        XmlElement? utageKanji = InternalXml.CreateElement("utageKanjiName");
        utageKanji.InnerText = InformationDict["Utage Kanji"].Equals("") ? "" : InformationDict["Utage Kanji"];
        XmlElement? utageComment = InternalXml.CreateElement("comment");
        utageComment.InnerText = InformationDict["Utage Comment"].Equals("") ? "" : InformationDict["Utage Comment"];
        XmlElement? utagePlayStyle = InternalXml.CreateElement("utagePlayStyle");
        utagePlayStyle.InnerText = InformationDict["Utage Play Style"].Equals("") ? "0" : InformationDict["Utage Play Style"];
        XmlElement ? utageFixedOptionRoot = InternalXml.CreateElement("fixedOptions");
        for (int i = 0; i < 4; i++)
        {
            XmlElement? utageFixedOption = InternalXml.CreateElement("FixedOption");
            XmlElement? utageFixedOptionName = InternalXml.CreateElement("_fixedOptionName");
            utageFixedOptionName.InnerText = InformationDict["Genre ID"].Equals(UtageGenreId.ToString()) ? "None" : "";
            XmlElement? utageFixedOptionValue = InternalXml.CreateElement("_fixedOptionValue");
            utageFixedOptionValue.InnerText = InformationDict["Genre ID"].Equals(UtageGenreId.ToString()) ? "None" : "";
            utageFixedOption.AppendChild(utageFixedOptionName);
            utageFixedOption.AppendChild(utageFixedOptionValue);
            utageFixedOptionRoot.AppendChild(utageFixedOption);
        }
        root.AppendChild(utageKanji);
        root.AppendChild(utageComment);
        root.AppendChild(utagePlayStyle);
        root.AppendChild(utageFixedOptionRoot);


        XmlElement? jacketFile = InternalXml.CreateElement("jacketFile");
        XmlElement? thumbnailName = InternalXml.CreateElement("thumbnailName");
        XmlElement? rightFile = InternalXml.CreateElement("rightFile");
        XmlElement? priority = InternalXml.CreateElement("priority");
        priority.InnerText = "0";
        root.AppendChild(jacketFile);
        root.AppendChild(thumbnailName);
        root.AppendChild(rightFile);
        root.AppendChild(priority);
    }

    /// <summary>
    ///     Writes music.xml to location specified
    /// </summary>
    /// <param name="location">Target Location</param>
    public void WriteOutInformation(string location)
    {
        GenerateInternalXml();
        InternalXml.Save(location);
    }
}
