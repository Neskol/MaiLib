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
                TakeInValue.Load(location + "Music.xml");
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
        XmlNodeList? nameCandidate = TakeInValue.GetElementsByTagName("name");
        XmlNodeList? bpmCandidate = TakeInValue.GetElementsByTagName("bpm");
        XmlNodeList? chartCandidate = TakeInValue.GetElementsByTagName("Notes");
        XmlNodeList? composerCandidate = TakeInValue.GetElementsByTagName("artistName");
        XmlNodeList? genreCandidate = TakeInValue.GetElementsByTagName("genreName");
        XmlNodeList? addVersionCandidate = TakeInValue.GetElementsByTagName("AddVersion");
        XmlNodeList? sortNameCandidate = TakeInValue.GetElementsByTagName("sortName");
        XmlNodeList? versionNumberCandidate = TakeInValue.GetElementsByTagName("releaseTagName");
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
                TrackVersion = version[int.Parse(idCandidate.InnerText)];
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
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();

                    if (genreId == UtageGenreId)
                    {
                        Information["Utage Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;

                        Information["Utage"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                            ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                            : "";
                        Information["Utage Chart Maker"] = notesDesignerCandidate.InnerText;
                        Information["Utage Chart Path"] = fileCandidate.InnerText;
                    }
                    else
                    {
                        Information["Basic Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;

                        Information["Basic"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                            ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                            : "";
                        Information["Basic Chart Maker"] = notesDesignerCandidate.InnerText;
                        Information["Basic Chart Path"] = fileCandidate.InnerText;
                    }
                }
                else if (pathCandidate.InnerText.Contains("01.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Advanced Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Advanced"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Advanced Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Advanced Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("02.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Expert Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Expert"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Expert Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Expert Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("03.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Master Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Master"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Master Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Master Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("04.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Remaster Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Remaster"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Remaster Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Remaster Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("05.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Utage Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Utage"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Utage Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Utage Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("11.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    XmlElement? levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    XmlElement? levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Easy Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    XmlElement? musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    XmlElement? notesDesignerCandidate =
                        candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    XmlElement? fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Easy"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? Level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Easy Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Easy Chart Path"] = fileCandidate.InnerText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is no such chart: " + ex.Message);
            }

        Information["SDDX Suffix"] = StandardDeluxeSuffix;
    }

    /// <summary>
    ///     Generate new music.xml for export
    /// </summary>
    public void GenerateInternalXml()
    {
        TakeInValue = new XmlDocument();
        //Create declaration
        XmlDeclaration? dec = TakeInValue.CreateXmlDeclaration("1.0", "utf-8", "yes");
        TakeInValue.AppendChild(dec);
        //Create Root and append attributes
        XmlElement? root = TakeInValue.CreateElement("MusicData");
        TakeInValue.AppendChild(root);
        XmlAttribute? xsi = TakeInValue.CreateAttribute("xmlns:xsi");
        xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
        XmlAttribute? xsd = TakeInValue.CreateAttribute("xmlns:xsd");

        //Create tags. *data name: inner text = music0xxxxx
        XmlElement? dataName = TakeInValue.CreateElement("dataName");
        dataName.InnerText = "music" + CompensateZero(Information["Music ID"]);
        root.AppendChild(dataName);
        XmlElement? netOpenName = TakeInValue.CreateElement("netOpenName");
        XmlElement? netOpenNameId = TakeInValue.CreateElement("id");
        netOpenNameId.InnerText = "0";
        XmlElement? netOpenNameStr = TakeInValue.CreateElement("str");
        netOpenNameStr.InnerText = "Net190711";
        netOpenName.AppendChild(netOpenNameId);
        netOpenName.AppendChild(netOpenNameStr);
        root.AppendChild(netOpenName);
        XmlElement? releaseTagName = TakeInValue.CreateElement("releaseTagName");
        XmlElement? releaseTagNameId = TakeInValue.CreateElement("id");
        releaseTagNameId.InnerText = "1";
        XmlElement? releaseTagNameStr = TakeInValue.CreateElement("str");
        releaseTagNameStr.InnerText = "Ver1.00.00";
        releaseTagName.AppendChild(releaseTagNameId);
        releaseTagName.AppendChild(releaseTagNameStr);
        root.AppendChild(releaseTagName);
        XmlElement? disable = TakeInValue.CreateElement("disable");
        disable.InnerText = "false";
        root.AppendChild(disable);
        XmlElement? name = TakeInValue.CreateElement("name");
        XmlElement? nameId = TakeInValue.CreateElement("id");
        nameId.InnerText = TrackID;
        XmlElement? nameStr = TakeInValue.CreateElement("str");
        nameStr.InnerText = TrackName;
        name.AppendChild(nameId);
        name.AppendChild(nameStr);
        root.AppendChild(name);
        XmlElement? rightsInfoName = TakeInValue.CreateElement("rightsInfoName");
        XmlElement? rightsInfoNameId = TakeInValue.CreateElement("id");
        rightsInfoNameId.InnerText = "0";
        XmlElement? rightsInfoNameStr = TakeInValue.CreateElement("str");
        rightsInfoNameStr.InnerText = "";
        rightsInfoName.AppendChild(rightsInfoNameId);
        rightsInfoName.AppendChild(rightsInfoNameStr);
        root.AppendChild(rightsInfoName);
        XmlElement? sortName = TakeInValue.CreateElement("sortName");
        sortName.InnerText = TrackSortName;
        root.AppendChild(sortName);
        XmlElement? artistName = TakeInValue.CreateElement("artistName");
        XmlElement? artistNameId = TakeInValue.CreateElement("id");
        artistNameId.InnerText = "0";
        XmlElement? artistNameStr = TakeInValue.CreateElement("str");
        artistNameStr.InnerText = Information["Composer"];
        artistName.AppendChild(artistNameId);
        artistName.AppendChild(artistNameStr);
        root.AppendChild(artistName);
        XmlElement? genreName = TakeInValue.CreateElement("genreName");
        XmlElement? genreNameId = TakeInValue.CreateElement("id");
        genreNameId.InnerText = TrackGenreID.ToString();
        XmlElement? genreNameStr = TakeInValue.CreateElement("str");
        genreNameStr.InnerText = TrackGenre;
        genreName.AppendChild(genreNameId);
        genreName.AppendChild(genreNameStr);
        root.AppendChild(genreName);
        XmlElement? bpm = TakeInValue.CreateElement("bpm");
        bpm.InnerText = TrackBPM;
        root.AppendChild(bpm);
        XmlElement? version = TakeInValue.CreateElement("version");
        version.InnerText = "19000";
        root.AppendChild(version);
        XmlElement? addVersion = TakeInValue.CreateElement("addVersion");
        XmlElement? addVersionId = TakeInValue.CreateElement("id");
        addVersionId.InnerText = TrackVersionNumber;
        XmlElement? addVersionStr = TakeInValue.CreateElement("str");
        addVersionStr.InnerText = shortVersion[int.Parse(TrackVersionNumber.Substring(1))];
        addVersion.AppendChild(addVersionId);
        addVersion.AppendChild(addVersionStr);
        root.AppendChild(addVersion);
        XmlElement? movieName = TakeInValue.CreateElement("movieName");
        XmlElement? movieNameId = TakeInValue.CreateElement("id");
        movieNameId.InnerText = TrackID;
        XmlElement? movieNameStr = TakeInValue.CreateElement("str");
        movieNameStr.InnerText = TrackName;
        movieName.AppendChild(movieNameId);
        movieName.AppendChild(movieNameStr);
        root.AppendChild(movieName);
        XmlElement? cueName = TakeInValue.CreateElement("cueName");
        XmlElement? cueNameId = TakeInValue.CreateElement("id");
        cueNameId.InnerText = TrackID;
        XmlElement? cueNameStr = TakeInValue.CreateElement("str");
        cueNameStr.InnerText = TrackName;
        cueName.AppendChild(cueNameId);
        cueName.AppendChild(cueNameStr);
        root.AppendChild(cueName);
        XmlElement? dressCode = TakeInValue.CreateElement("dressCode");
        dressCode.InnerText = "false";
        root.AppendChild(dressCode);
        XmlElement? eventName = TakeInValue.CreateElement("eventName");
        XmlElement? eventNameId = TakeInValue.CreateElement("id");
        eventNameId.InnerText = "1";
        XmlElement? eventNameStr = TakeInValue.CreateElement("str");
        eventNameStr.InnerText = "無期限常時解放";
        eventName.AppendChild(eventNameId);
        eventName.AppendChild(eventNameStr);
        root.AppendChild(eventName);
        XmlElement? subEventName = TakeInValue.CreateElement("subEventName");
        XmlElement? subEventNameId = TakeInValue.CreateElement("id");
        subEventNameId.InnerText = "1";
        XmlElement? subEventNameStr = TakeInValue.CreateElement("str");
        subEventNameStr.InnerText = "無期限常時解放";
        subEventName.AppendChild(subEventNameId);
        subEventName.AppendChild(subEventNameStr);
        root.AppendChild(subEventName);
        XmlElement? lockType = TakeInValue.CreateElement("lockType");
        lockType.InnerText = "0";
        root.AppendChild(lockType);
        XmlElement? subLockType = TakeInValue.CreateElement("subLockType");
        subLockType.InnerText = "1";
        root.AppendChild(subLockType);
        XmlElement? dotNetListView = TakeInValue.CreateElement("dotNetListView");
        dotNetListView.InnerText = "true";
        root.AppendChild(dotNetListView);
        XmlElement? notesData = TakeInValue.CreateElement("notesData");
        for (int i = 0; i < 7; i++)
        {
            XmlElement? noteCandidate = TakeInValue.CreateElement("Notes");
            XmlElement? fileCandidate = TakeInValue.CreateElement("file");
            XmlElement? pathCandidate = TakeInValue.CreateElement("path");
            pathCandidate.InnerText = CompensateZero(TrackID) + "_0" + i + ".ma2";
            fileCandidate.AppendChild(pathCandidate);
            XmlElement? levelCandidate = TakeInValue.CreateElement("level");
            XmlElement? levelDecimalCandidate = TakeInValue.CreateElement("levelDecimal");
            XmlElement? notesDesignerCandidate = TakeInValue.CreateElement("notesDesigner");
            XmlElement? notesDesignerIdCandidate = TakeInValue.CreateElement("id");
            XmlElement? notesDesignerStrCandidate = TakeInValue.CreateElement("str");
            XmlElement? notesTypeCandidate = TakeInValue.CreateElement("notesType");
            notesTypeCandidate.InnerText = "0";
            XmlElement? musicLevelIDCandidate = TakeInValue.CreateElement("musicLevelID");
            XmlElement? isEnabledCandidate = TakeInValue.CreateElement("isEnabled");

            // switch (i)
            // {
            //     case 0:
            //         notesDesignerStrCandidate.InnerText = Information["Easy Chart Maker"];
            //         int designerIndex =
            //             Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
            //         if (designerIndex > 1)
            //             notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
            //                 Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
            //         isEnabledCandidate.InnerText = "true";
            //         break;
            //     case 1:
            //         notesDesignerStrCandidate.InnerText = Information["Basic Chart Maker"];
            //         designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
            //         if (designerIndex > 1)
            //             notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
            //                 Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
            //         isEnabledCandidate.InnerText = "true";
            //         break;
            //     case 2:
            //         notesDesignerStrCandidate.InnerText = Information["Advanced Chart Maker"];
            //         designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
            //         if (designerIndex > 1)
            //             notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
            //                 Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
            //         isEnabledCandidate.InnerText = "true";
            //         break;
            //     case 3:
            //         notesDesignerStrCandidate.InnerText = Information["Expert Chart Maker"];
            //         designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
            //         if (designerIndex > 1)
            //             notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
            //                 Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
            //         isEnabledCandidate.InnerText = "true";
            //         break;
            //     case 4:
            //         notesDesignerStrCandidate.InnerText = Information["Master Chart Maker"];
            //         designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
            //         if (designerIndex > 1)
            //             notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
            //                 Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
            //         isEnabledCandidate.InnerText = "true";
            //         break;
            //     case 5:
            //         notesDesignerStrCandidate.InnerText = Information["Remaster Chart Maker"];
            //         designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
            //         if (designerIndex > 1)
            //             notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
            //                 Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
            //         isEnabledCandidate.InnerText = "true";
            //         break;
            //     case 6:
            //         notesDesignerStrCandidate.InnerText = Information["Utage Chart Maker"];
            //         designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
            //         if (designerIndex > 1)
            //             notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
            //                 Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
            //         isEnabledCandidate.InnerText = "true";
            //         break;
            // }

            notesDesignerCandidate.AppendChild(notesDesignerIdCandidate);
            notesDesignerCandidate.AppendChild(notesDesignerStrCandidate);
            if (!TrackLevels[i].Equals(""))
            {
                levelCandidate.InnerText = TrackLevels[i];
                musicLevelIDCandidate.InnerText = TrackLevels[i];
            }
            else
            {
                levelCandidate.InnerText = "0";
            }

            if (!TrackDecimalLevels[i].Equals(""))
                levelCandidate.InnerText = TrackDecimalLevels[i];
            else levelCandidate.InnerText = "0";
            noteCandidate.AppendChild(fileCandidate);
            noteCandidate.AppendChild(levelCandidate);
            noteCandidate.AppendChild(levelDecimalCandidate);
            noteCandidate.AppendChild(notesDesignerCandidate);
            noteCandidate.AppendChild(notesTypeCandidate);
            noteCandidate.AppendChild(musicLevelIDCandidate);
            noteCandidate.AppendChild(isEnabledCandidate);
            root.AppendChild(noteCandidate);
        }

        XmlElement? jacketFile = TakeInValue.CreateElement("jacketFile");
        XmlElement? thumbnailName = TakeInValue.CreateElement("thumbnailName");
        XmlElement? rightFile = TakeInValue.CreateElement("rightFile");
        XmlElement? priority = TakeInValue.CreateElement("priority");
        priority.InnerText = "0";
        root.AppendChild(jacketFile);
        root.AppendChild(thumbnailName);
        root.AppendChild(rightFile);
        root.AppendChild(priority);
    }

    public XmlElement CreateNotesInformation(Dictionary<string, string> information, int chartIndex)
    {
        XmlElement? result = TakeInValue.CreateElement("Notes");

        return result;
    }
}
