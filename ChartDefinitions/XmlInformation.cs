using System.Xml;

namespace MaiLib;

/// <summary>
///     Using Xml to store trackInformation
/// </summary>
public class XmlInformation : TrackInformation, IXmlUtility
{
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

    public override void Update()
    {
        var nameCandidate = TakeInValue.GetElementsByTagName("name");
        var bpmCandidate = TakeInValue.GetElementsByTagName("bpm");
        var chartCandidate = TakeInValue.GetElementsByTagName("Notes");
        var composerCandidate = TakeInValue.GetElementsByTagName("artistName");
        var genreCandidate = TakeInValue.GetElementsByTagName("genreName");
        var addVersionCandidate = TakeInValue.GetElementsByTagName("AddVersion");
        var sortNameCandidate = TakeInValue.GetElementsByTagName("sortName");
        var versionNumberCandidate = TakeInValue.GetElementsByTagName("releaseTagName");
        //Add in name and music ID.
        ////Add BPM
        //this.information.Add("BPM",bpmCandidate[0].InnerText);
        foreach (XmlNode candidate in nameCandidate)
            if (TrackID.Equals(""))
            {
                var idCandidate = candidate["id"] ?? throw new NullReferenceException();
                var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                TrackID = idCandidate.InnerText;
                TrackName = strCandidate.InnerText;
            }

        foreach (XmlNode candidate in chartCandidate)
            try
            {
                var pathCandidate = candidate["file"] ?? throw new NullReferenceException();
                pathCandidate = pathCandidate["path"] ?? throw new NullReferenceException();
                var enableCandidate = candidate["isEnable"] ?? throw new NullReferenceException();
                if (pathCandidate.InnerText.Contains("00.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Basic Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Basic"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Basic Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Basic Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("01.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Advanced Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Advanced"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Advanced Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Advanced Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("02.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Expert Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Expert"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Expert Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Expert Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("03.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Master Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Master"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Master Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Master Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("04.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Remaster Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Remaster"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Remaster Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Remaster Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("05.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Utage Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Utage"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Utage Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Utage Chart Path"] = fileCandidate.InnerText;
                }
                else if (pathCandidate.InnerText.Contains("11.ma2") && enableCandidate.InnerText.Equals("true"))
                {
                    var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                    var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                    Information["Easy Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                    var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                    var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                    notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                    var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                    fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                    Information["Easy"] = int.Parse(musicLevelIDCandidate.InnerText) != 0
                        ? level[int.Parse(musicLevelIDCandidate.InnerText) - 1]
                        : "";
                    Information["Easy Chart Maker"] = notesDesignerCandidate.InnerText;
                    Information["Easy Chart Path"] = fileCandidate.InnerText;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There is no such chart: " + ex.Message);
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
                var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                TrackComposer = strCandidate.InnerText;
            }
        }

        foreach (XmlNode candidate in genreCandidate)
        {
            if (TrackGenre.Equals(""))
            {
                var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                TrackGenre = strCandidate.InnerText;
            }
        }

        foreach (XmlNode candidate in versionNumberCandidate)
        {
            if (TrackVersionNumber.Equals(""))
            {
                var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                TrackVersionNumber = strCandidate.InnerText;
            }
        }

        foreach (XmlNode candidate in addVersionCandidate)
        {
            if (TrackVersion.Equals(""))
            {
                var idCandidate = candidate["id"] ?? throw new NullReferenceException();
                TrackVersion = version[int.Parse(idCandidate.InnerText)];
            }
        }

        Information["SDDX Suffix"] = StandardDeluxeSuffix;
    }

    /// <summary>
    ///     Generate new music.xml for export
    /// </summary>
    public void GenerateEmptyStoredXML()
    {
        TakeInValue = new XmlDocument();
        //Create declaration
        var dec = TakeInValue.CreateXmlDeclaration("1.0", "utf-8", "yes");
        TakeInValue.AppendChild(dec);
        //Create Root and append attributes
        var root = TakeInValue.CreateElement("MusicData");
        TakeInValue.AppendChild(root);
        var xsi = TakeInValue.CreateAttribute("xmlns:xsi");
        xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
        var xsd = TakeInValue.CreateAttribute("xmlns:xsd");

        //Create tags. *data name: inner text = music0xxxxx
        var dataName = TakeInValue.CreateElement("dataName");
        dataName.InnerText = "music" + CompensateZero(Information["Music ID"]);
        root.AppendChild(dataName);
        var netOpenName = TakeInValue.CreateElement("netOpenName");
        var netOpenNameId = TakeInValue.CreateElement("id");
        netOpenNameId.InnerText = "0";
        var netOpenNameStr = TakeInValue.CreateElement("str");
        netOpenNameStr.InnerText = "Net190711";
        netOpenName.AppendChild(netOpenNameId);
        netOpenName.AppendChild(netOpenNameStr);
        root.AppendChild(netOpenName);
        var releaseTagName = TakeInValue.CreateElement("releaseTagName");
        var releaseTagNameId = TakeInValue.CreateElement("id");
        releaseTagNameId.InnerText = "1";
        var releaseTagNameStr = TakeInValue.CreateElement("str");
        releaseTagNameStr.InnerText = "Ver1.00.00";
        releaseTagName.AppendChild(releaseTagNameId);
        releaseTagName.AppendChild(releaseTagNameStr);
        root.AppendChild(releaseTagName);
        var disable = TakeInValue.CreateElement("disable");
        disable.InnerText = "false";
        root.AppendChild(disable);
        var name = TakeInValue.CreateElement("name");
        var nameId = TakeInValue.CreateElement("id");
        nameId.InnerText = TrackID;
        var nameStr = TakeInValue.CreateElement("str");
        nameStr.InnerText = TrackName;
        name.AppendChild(nameId);
        name.AppendChild(nameStr);
        root.AppendChild(name);
        var rightsInfoName = TakeInValue.CreateElement("rightsInfoName");
        var rightsInfoNameId = TakeInValue.CreateElement("id");
        rightsInfoNameId.InnerText = "0";
        var rightsInfoNameStr = TakeInValue.CreateElement("str");
        rightsInfoNameStr.InnerText = "";
        rightsInfoName.AppendChild(rightsInfoNameId);
        rightsInfoName.AppendChild(rightsInfoNameStr);
        root.AppendChild(rightsInfoName);
        var sortName = TakeInValue.CreateElement("sortName");
        sortName.InnerText = TrackSortName;
        root.AppendChild(sortName);
        var artistName = TakeInValue.CreateElement("artistName");
        var artistNameId = TakeInValue.CreateElement("id");
        artistNameId.InnerText = "0";
        var artistNameStr = TakeInValue.CreateElement("str");
        artistNameStr.InnerText = Information["Composer"];
        artistName.AppendChild(artistNameId);
        artistName.AppendChild(artistNameStr);
        root.AppendChild(artistName);
        var genreName = TakeInValue.CreateElement("genreName");
        var genreNameId = TakeInValue.CreateElement("id");
        genreNameId.InnerText = "10" + Array.IndexOf(genre, Information["Genre"]);
        var genreNameStr = TakeInValue.CreateElement("str");
        genreNameStr.InnerText = TrackGenre;
        genreName.AppendChild(genreNameId);
        genreName.AppendChild(genreNameStr);
        root.AppendChild(genreName);
        var bpm = TakeInValue.CreateElement("bpm");
        bpm.InnerText = TrackBPM;
        root.AppendChild(bpm);
        var version = TakeInValue.CreateElement("version");
        version.InnerText = "19000";
        root.AppendChild(version);
        var addVersion = TakeInValue.CreateElement("addVersion");
        var addVersionId = TakeInValue.CreateElement("id");
        addVersionId.InnerText = TrackVersionNumber;
        var addVersionStr = TakeInValue.CreateElement("str");
        addVersionStr.InnerText = shortVersion[int.Parse(TrackVersionNumber.Substring(1))];
        addVersion.AppendChild(addVersionId);
        addVersion.AppendChild(addVersionStr);
        root.AppendChild(addVersion);
        var movieName = TakeInValue.CreateElement("movieName");
        var movieNameId = TakeInValue.CreateElement("id");
        movieNameId.InnerText = TrackID;
        var movieNameStr = TakeInValue.CreateElement("str");
        movieNameStr.InnerText = TrackName;
        movieName.AppendChild(movieNameId);
        movieName.AppendChild(movieNameStr);
        root.AppendChild(movieName);
        var cueName = TakeInValue.CreateElement("cueName");
        var cueNameId = TakeInValue.CreateElement("id");
        cueNameId.InnerText = TrackID;
        var cueNameStr = TakeInValue.CreateElement("str");
        cueNameStr.InnerText = TrackName;
        cueName.AppendChild(cueNameId);
        cueName.AppendChild(cueNameStr);
        root.AppendChild(cueName);
        var dressCode = TakeInValue.CreateElement("dressCode");
        dressCode.InnerText = "false";
        root.AppendChild(dressCode);
        var eventName = TakeInValue.CreateElement("eventName");
        var eventNameId = TakeInValue.CreateElement("id");
        eventNameId.InnerText = "1";
        var eventNameStr = TakeInValue.CreateElement("str");
        eventNameStr.InnerText = "無期限常時解放";
        eventName.AppendChild(eventNameId);
        eventName.AppendChild(eventNameStr);
        root.AppendChild(eventName);
        var subEventName = TakeInValue.CreateElement("subEventName");
        var subEventNameId = TakeInValue.CreateElement("id");
        subEventNameId.InnerText = "1";
        var subEventNameStr = TakeInValue.CreateElement("str");
        subEventNameStr.InnerText = "無期限常時解放";
        subEventName.AppendChild(subEventNameId);
        subEventName.AppendChild(subEventNameStr);
        root.AppendChild(subEventName);
        var lockType = TakeInValue.CreateElement("lockType");
        lockType.InnerText = "0";
        root.AppendChild(lockType);
        var subLockType = TakeInValue.CreateElement("subLockType");
        subLockType.InnerText = "1";
        root.AppendChild(subLockType);
        var dotNetListView = TakeInValue.CreateElement("dotNetListView");
        dotNetListView.InnerText = "true";
        root.AppendChild(dotNetListView);
        var notesData = TakeInValue.CreateElement("notesData");
        for (var i = 0; i < 7; i++)
        {
            var noteCandidate = TakeInValue.CreateElement("Notes");
            var fileCandidate = TakeInValue.CreateElement("file");
            var pathCandidate = TakeInValue.CreateElement("path");
            pathCandidate.InnerText = CompensateZero(TrackID) + "_0" + i + ".ma2";
            fileCandidate.AppendChild(pathCandidate);
            var levelCandidate = TakeInValue.CreateElement("level");
            var levelDecimalCandidate = TakeInValue.CreateElement("levelDecimal");
            var notesDesignerCandidate = TakeInValue.CreateElement("notesDesigner");
            var notesDesignerIdCandidate = TakeInValue.CreateElement("id");
            var notesDesignerStrCandidate = TakeInValue.CreateElement("str");
            var notesTypeCandidate = TakeInValue.CreateElement("notesType");
            notesTypeCandidate.InnerText = "0";
            var musicLevelIDCandidate = TakeInValue.CreateElement("musicLevelID");
            var isEnabledCandidate = TakeInValue.CreateElement("isEnabled");

            switch (i)
            {
                case 0:
                    notesDesignerStrCandidate.InnerText = Information["Easy Chart Maker"];
                    var designerIndex =
                        Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
                    if (designerIndex > 1)
                        notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
                            Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                    isEnabledCandidate.InnerText = "true";
                    break;
                case 1:
                    notesDesignerStrCandidate.InnerText = Information["Basic Chart Maker"];
                    designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
                    if (designerIndex > 1)
                        notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
                            Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                    isEnabledCandidate.InnerText = "true";
                    break;
                case 2:
                    notesDesignerStrCandidate.InnerText = Information["Advanced Chart Maker"];
                    designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
                    if (designerIndex > 1)
                        notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
                            Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                    isEnabledCandidate.InnerText = "true";
                    break;
                case 3:
                    notesDesignerStrCandidate.InnerText = Information["Expert Chart Maker"];
                    designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
                    if (designerIndex > 1)
                        notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
                            Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                    isEnabledCandidate.InnerText = "true";
                    break;
                case 4:
                    notesDesignerStrCandidate.InnerText = Information["Master Chart Maker"];
                    designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
                    if (designerIndex > 1)
                        notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
                            Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                    isEnabledCandidate.InnerText = "true";
                    break;
                case 5:
                    notesDesignerStrCandidate.InnerText = Information["Remaster Chart Maker"];
                    designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
                    if (designerIndex > 1)
                        notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
                            Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                    isEnabledCandidate.InnerText = "true";
                    break;
                case 6:
                    notesDesignerStrCandidate.InnerText = Information["Utage Chart Maker"];
                    designerIndex = Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText);
                    if (designerIndex > 1)
                        notesDesignerIdCandidate.InnerText = artistNameDic.Keys.ToArray()[
                            Array.IndexOf(artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                    isEnabledCandidate.InnerText = "true";
                    break;
            }

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

        var jacketFile = TakeInValue.CreateElement("jacketFile");
        var thumbnailName = TakeInValue.CreateElement("thumbnailName");
        var rightFile = TakeInValue.CreateElement("rightFile");
        var priority = TakeInValue.CreateElement("priority");
        priority.InnerText = "0";
        root.AppendChild(jacketFile);
        root.AppendChild(thumbnailName);
        root.AppendChild(rightFile);
        root.AppendChild(priority);
    }

    public XmlElement CreateNotesInformation(Dictionary<string, string> information, int chartIndex)
    {
        var result = TakeInValue.CreateElement("Notes");

        return result;
    }
}