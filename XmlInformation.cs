using System.Data;
using System.Xml;

namespace MaiLib
{
    /// <summary>
    /// Using Xml to store trackInformation
    /// </summary>
    public class XmlInformation : TrackInformation, IXmlUtility
    {
        /// <summary>
        /// Using take in Xml to store trackInformation:
        /// </summary>
        public XmlInformation()
        {
            this.Update();
        }

        public XmlInformation(string location)
        {
            {
                if (File.Exists(location + "Music.xml"))
                {
                    this.TakeInValue.Load(location + "Music.xml");
                    this.Update();
                }
                else
                {
                    this.Update();
                }

            }
        }

        /// <summary>
        /// Generate new music.xml for export
        /// </summary>
        public void GenerateEmptyStoredXML()
        {
            this.TakeInValue = new XmlDocument();
            //Create declaration
            XmlDeclaration dec = this.TakeInValue.CreateXmlDeclaration("1.0", "utf-8", "yes");
            this.TakeInValue.AppendChild(dec);
            //Create Root and append attributes
            XmlElement root = this.TakeInValue.CreateElement("MusicData");
            this.TakeInValue.AppendChild(root);
            XmlAttribute xsi = this.TakeInValue.CreateAttribute("xmlns:xsi");
            xsi.Value = "http://www.w3.org/2001/XMLSchema-instance";
            XmlAttribute xsd = this.TakeInValue.CreateAttribute("xmlns:xsd");
            //root.AppendChild(xsi);
            //root.AppendChild(xsd);

            //Create tags. *data name: inner text = music0xxxxx
            XmlElement dataName = this.TakeInValue.CreateElement("dataName");
            dataName.InnerText = "music" + CompensateZero(this.Information["Music ID"]);
            root.AppendChild(dataName);
            XmlElement netOpenName = this.TakeInValue.CreateElement("netOpenName");
            XmlElement netOpenNameId = this.TakeInValue.CreateElement("id");
            netOpenNameId.InnerText = "0";
            XmlElement netOpenNameStr = this.TakeInValue.CreateElement("str");
            netOpenNameStr.InnerText = "Net190711";
            netOpenName.AppendChild(netOpenNameId);
            netOpenName.AppendChild(netOpenNameStr);
            root.AppendChild(netOpenName);
            XmlElement releaseTagName = this.TakeInValue.CreateElement("releaseTagName");
            XmlElement releaseTagNameId = this.TakeInValue.CreateElement("id");
            releaseTagNameId.InnerText = "1";
            XmlElement releaseTagNameStr = this.TakeInValue.CreateElement("str");
            releaseTagNameStr.InnerText = "Ver1.00.00";
            releaseTagName.AppendChild(releaseTagNameId);
            releaseTagName.AppendChild(releaseTagNameStr);
            root.AppendChild(releaseTagName);
            XmlElement disable = this.TakeInValue.CreateElement("disable");
            disable.InnerText = "false";
            root.AppendChild(disable);
            XmlElement name = this.TakeInValue.CreateElement("name");
            XmlElement nameId = this.TakeInValue.CreateElement("id");
            nameId.InnerText = this.TrackID;
            XmlElement nameStr = this.TakeInValue.CreateElement("str");
            nameStr.InnerText = this.TrackName;
            name.AppendChild(nameId);
            name.AppendChild(nameStr);
            root.AppendChild(name);
            XmlElement rightsInfoName = this.TakeInValue.CreateElement("rightsInfoName");
            XmlElement rightsInfoNameId = this.TakeInValue.CreateElement("id");
            rightsInfoNameId.InnerText = "0";
            XmlElement rightsInfoNameStr = this.TakeInValue.CreateElement("str");
            rightsInfoNameStr.InnerText = "";
            rightsInfoName.AppendChild(rightsInfoNameId);
            rightsInfoName.AppendChild(rightsInfoNameStr);
            root.AppendChild(rightsInfoName);
            XmlElement sortName = this.TakeInValue.CreateElement("sortName");
            sortName.InnerText = this.TrackSortName;
            root.AppendChild(sortName);
            XmlElement artistName = this.TakeInValue.CreateElement("artistName");
            XmlElement artistNameId = this.TakeInValue.CreateElement("id");
            artistNameId.InnerText = "0";
            XmlElement artistNameStr = this.TakeInValue.CreateElement("str");
            artistNameStr.InnerText = this.Information["Composer"];
            artistName.AppendChild(artistNameId);
            artistName.AppendChild(artistNameStr);
            root.AppendChild(artistName);
            XmlElement genreName = this.TakeInValue.CreateElement("genreName");
            XmlElement genreNameId = this.TakeInValue.CreateElement("id");
            genreNameId.InnerText = "10" + Array.IndexOf(TrackInformation.genre, this.Information["Genre"]).ToString();
            XmlElement genreNameStr = this.TakeInValue.CreateElement("str");
            genreNameStr.InnerText = this.TrackGenre;
            genreName.AppendChild(genreNameId);
            genreName.AppendChild(genreNameStr);
            root.AppendChild(genreName);
            XmlElement bpm = this.TakeInValue.CreateElement("bpm");
            bpm.InnerText = this.TrackBPM;
            root.AppendChild(bpm);
            XmlElement version = this.TakeInValue.CreateElement("version");
            version.InnerText = "19000";
            root.AppendChild(version);
            XmlElement addVersion = this.TakeInValue.CreateElement("addVersion");
            XmlElement addVersionId = this.TakeInValue.CreateElement("id");
            addVersionId.InnerText = this.TrackVersionNumber;
            XmlElement addVersionStr = this.TakeInValue.CreateElement("str");            
            addVersionStr.InnerText = TrackInformation.shortVersion[int.Parse(this.TrackVersionNumber.Substring(1))];
            addVersion.AppendChild(addVersionId);
            addVersion.AppendChild(addVersionStr);
            root.AppendChild(addVersion);
            XmlElement movieName = this.TakeInValue.CreateElement("movieName");
            XmlElement movieNameId = this.TakeInValue.CreateElement("id");
            movieNameId.InnerText = this.TrackID;
            XmlElement movieNameStr = this.TakeInValue.CreateElement("str");
            movieNameStr.InnerText = this.TrackName;
            movieName.AppendChild(movieNameId);
            movieName.AppendChild(movieNameStr);
            root.AppendChild(movieName);
            XmlElement cueName = this.TakeInValue.CreateElement("cueName");
            XmlElement cueNameId = this.TakeInValue.CreateElement("id");
            cueNameId.InnerText = this.TrackID;
            XmlElement cueNameStr = this.TakeInValue.CreateElement("str");
            cueNameStr.InnerText = this.TrackName;
            cueName.AppendChild(cueNameId);
            cueName.AppendChild(cueNameStr);
            root.AppendChild(cueName);
            XmlElement dressCode = this.TakeInValue.CreateElement("dressCode");
            dressCode.InnerText = "false";
            root.AppendChild(dressCode);
            XmlElement eventName = this.TakeInValue.CreateElement("eventName");
            XmlElement eventNameId = this.TakeInValue.CreateElement("id");
            eventNameId.InnerText = "1";
            XmlElement eventNameStr = this.TakeInValue.CreateElement("str");
            eventNameStr.InnerText = "無期限常時解放";
            eventName.AppendChild(eventNameId);
            eventName.AppendChild(eventNameStr);
            root.AppendChild(eventName);
            XmlElement subEventName = this.TakeInValue.CreateElement("subEventName");
            XmlElement subEventNameId = this.TakeInValue.CreateElement("id");
            subEventNameId.InnerText = "1";
            XmlElement subEventNameStr = this.TakeInValue.CreateElement("str");
            subEventNameStr.InnerText = "無期限常時解放";
            subEventName.AppendChild(subEventNameId);
            subEventName.AppendChild(subEventNameStr);
            root.AppendChild(subEventName);
            XmlElement lockType = this.TakeInValue.CreateElement("lockType");
            lockType.InnerText = "0";
            root.AppendChild(lockType);
            XmlElement subLockType = this.TakeInValue.CreateElement("subLockType");
            subLockType.InnerText = "1";
            root.AppendChild(subLockType);
            XmlElement dotNetListView = this.TakeInValue.CreateElement("dotNetListView");
            dotNetListView.InnerText = "true";
            root.AppendChild(dotNetListView);
            XmlElement notesData = this.TakeInValue.CreateElement("notesData");
            for (int i = 0; i < 7; i++)
            {
                XmlElement noteCandidate = this.TakeInValue.CreateElement("Notes");
                XmlElement fileCandidate = this.TakeInValue.CreateElement("file");
                XmlElement pathCandidate = this.TakeInValue.CreateElement("path");
                pathCandidate.InnerText = CompensateZero(this.TrackID) + "_0" + i + ".ma2";
                fileCandidate.AppendChild(pathCandidate);
                XmlElement levelCandidate = this.TakeInValue.CreateElement("level");
                XmlElement levelDecimalCandidate = this.TakeInValue.CreateElement("levelDecimal");
                XmlElement notesDesignerCandidate = this.TakeInValue.CreateElement("notesDesigner");
                XmlElement notesDesignerIdCandidate = this.TakeInValue.CreateElement("id");
                XmlElement notesDesignerStrCandidate = this.TakeInValue.CreateElement("str");
                XmlElement notesTypeCandidate = this.TakeInValue.CreateElement("notesType");
                notesTypeCandidate.InnerText = "0";
                XmlElement musicLevelIDCandidate = this.TakeInValue.CreateElement("musicLevelID");
                XmlElement isEnabledCandidate = this.TakeInValue.CreateElement("isEnabled");

                switch (i)
                {
                    case 0:
                        notesDesignerStrCandidate.InnerText = this.Information["Easy Chart Maker"];
                        notesDesignerIdCandidate.InnerText = TrackInformation.artistNameDic.Keys.ToArray()[Array.IndexOf(TrackInformation.artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                        isEnabledCandidate.InnerText = "true";
                        break;
                    case 1:
                        notesDesignerStrCandidate.InnerText = this.Information["Basic Chart Maker"];
                        notesDesignerIdCandidate.InnerText = TrackInformation.artistNameDic.Keys.ToArray()[Array.IndexOf(TrackInformation.artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                        isEnabledCandidate.InnerText = "true";
                        break;
                    case 2:
                        notesDesignerStrCandidate.InnerText = this.Information["Advanced Chart Maker"];
                        notesDesignerIdCandidate.InnerText = TrackInformation.artistNameDic.Keys.ToArray()[Array.IndexOf(TrackInformation.artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                        isEnabledCandidate.InnerText = "true";
                        break;
                    case 3:
                        notesDesignerStrCandidate.InnerText = this.Information["Expert Chart Maker"];
                        notesDesignerIdCandidate.InnerText = TrackInformation.artistNameDic.Keys.ToArray()[Array.IndexOf(TrackInformation.artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                        isEnabledCandidate.InnerText = "true";
                        break;
                    case 4:
                        notesDesignerStrCandidate.InnerText = this.Information["Master Chart Maker"];
                        notesDesignerIdCandidate.InnerText = TrackInformation.artistNameDic.Keys.ToArray()[Array.IndexOf(TrackInformation.artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                        isEnabledCandidate.InnerText = "true";
                        break;
                    case 5:
                        notesDesignerStrCandidate.InnerText = this.Information["Remaster Chart Maker"];
                        notesDesignerIdCandidate.InnerText = TrackInformation.artistNameDic.Keys.ToArray()[Array.IndexOf(TrackInformation.artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                        isEnabledCandidate.InnerText = "true";
                        break;
                    case 6:
                        notesDesignerStrCandidate.InnerText = this.Information["Utage Chart Maker"];
                        notesDesignerIdCandidate.InnerText = TrackInformation.artistNameDic.Keys.ToArray()[Array.IndexOf(TrackInformation.artistNameDic.Values.ToArray(), notesDesignerStrCandidate.InnerText)];
                        isEnabledCandidate.InnerText = "true";
                        break;
                }
                notesDesignerCandidate.AppendChild(notesDesignerIdCandidate);
                notesDesignerCandidate.AppendChild(notesDesignerStrCandidate);
                if (!this.TrackLevels[i].Equals(""))
                {
                    levelCandidate.InnerText = TrackLevels[i];
                    musicLevelIDCandidate.InnerText = TrackLevels[i];
                }
                else levelCandidate.InnerText = "0";
                if (!this.TrackDecimalLevels[i].Equals(""))
                {
                    levelCandidate.InnerText = TrackDecimalLevels[i];
                }
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
            XmlElement jacketFile = this.TakeInValue.CreateElement("jacketFile");
            XmlElement thumbnailName = this.TakeInValue.CreateElement("thumbnailName");
            XmlElement rightFile = this.TakeInValue.CreateElement("rightFile");
            XmlElement priority = this.TakeInValue.CreateElement("priority");
            priority.InnerText = "0";
            root.AppendChild(jacketFile);
            root.AppendChild(thumbnailName);
            root.AppendChild(rightFile);
            root.AppendChild(priority);
        }

        public XmlElement CreateNotesInformation(Dictionary<string, string> information, int chartIndex)
        {
            XmlElement result = this.TakeInValue.CreateElement("Notes");

            return result;
        }

        public override void Update()
        {
            XmlNodeList nameCandidate = this.TakeInValue.GetElementsByTagName("name");
            XmlNodeList bpmCandidate = this.TakeInValue.GetElementsByTagName("bpm");
            XmlNodeList chartCandidate = this.TakeInValue.GetElementsByTagName("Notes");
            XmlNodeList composerCandidate = this.TakeInValue.GetElementsByTagName("artistName");
            XmlNodeList genreCandidate = this.TakeInValue.GetElementsByTagName("genreName");
            XmlNodeList addVersionCandidate = this.TakeInValue.GetElementsByTagName("AddVersion");
            XmlNodeList sortNameCandidate = this.TakeInValue.GetElementsByTagName("sortName");
            XmlNodeList versionNumberCandidate = this.TakeInValue.GetElementsByTagName("releaseTagName");
            //Add in name and music ID.
            ////Add BPM
            //this.information.Add("BPM",bpmCandidate[0].InnerText);
            foreach (XmlNode candidate in nameCandidate)
            {
                if (this.TrackID.Equals(""))
                {
                    var idCandidate = candidate["id"] ?? throw new NullReferenceException();
                    var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                    this.TrackID = idCandidate.InnerText;
                    this.TrackName = strCandidate.InnerText;
                }
            }
            foreach (XmlNode candidate in chartCandidate)
            {
                try
                {
                    var pathCandidate = candidate["file"] ?? throw new NullReferenceException();
                    pathCandidate = pathCandidate["path"] ?? throw new NullReferenceException();
                    var enableCandidate = candidate["isEnable"] ?? throw new NullReferenceException();
                    if (pathCandidate.InnerText.Contains("00.ma2") && enableCandidate.InnerText.Equals("true"))
                    {
                        var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                        var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                        this.Information["Basic Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                        var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                        var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                        notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                        var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                        fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                        this.Information["Basic"] = level[Int32.Parse(musicLevelIDCandidate.InnerText) - 1];
                        this.Information["Basic Chart Maker"] = notesDesignerCandidate.InnerText;
                        this.Information["Basic Chart Path"] = fileCandidate.InnerText;
                    }
                    else if (pathCandidate.InnerText.Contains("01.ma2") && enableCandidate.InnerText.Equals("true"))
                    {
                        var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                        var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                        this.Information["Advanced Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                        var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                        var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                        notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                        var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                        fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                        this.Information["Advanced"] = level[Int32.Parse(musicLevelIDCandidate.InnerText) - 1];
                        this.Information["Advanced Chart Maker"] = notesDesignerCandidate.InnerText;
                        this.Information["Advanced Chart Path"] = fileCandidate.InnerText;
                    }
                    else if (pathCandidate.InnerText.Contains("02.ma2") && enableCandidate.InnerText.Equals("true"))
                    {
                        var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                        var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                        this.Information["Expert Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                        var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                        var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                        notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                        var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                        fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                        this.Information["Expert"] = level[Int32.Parse(musicLevelIDCandidate.InnerText) - 1];
                        this.Information["Expert Chart Maker"] = notesDesignerCandidate.InnerText;
                        this.Information["Expert Chart Path"] = fileCandidate.InnerText;
                    }
                    else if (pathCandidate.InnerText.Contains("03.ma2") && enableCandidate.InnerText.Equals("true"))
                    {
                        var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                        var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                        this.Information["Master Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                        var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                        var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                        notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                        var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                        fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                        this.Information["Master"] = level[Int32.Parse(musicLevelIDCandidate.InnerText) - 1];
                        this.Information["Master Chart Maker"] = notesDesignerCandidate.InnerText;
                        this.Information["Master Chart Path"] = fileCandidate.InnerText;
                    }
                    else if (pathCandidate.InnerText.Contains("04.ma2") && enableCandidate.InnerText.Equals("true"))
                    {
                        var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                        var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                        this.Information["Remaster Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                        var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                        var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                        notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                        var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                        fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                        this.Information["Remaster"] = level[Int32.Parse(musicLevelIDCandidate.InnerText) - 1];
                        this.Information["Remaster Chart Maker"] = notesDesignerCandidate.InnerText;
                        this.Information["Remaster Chart Path"] = fileCandidate.InnerText;
                    }
                    else if (pathCandidate.InnerText.Contains("05.ma2") && enableCandidate.InnerText.Equals("true"))
                    {
                        var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                        var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                        this.Information["Utage Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                        var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                        var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                        notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                        var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                        fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                        this.Information["Utage"] = level[Int32.Parse(musicLevelIDCandidate.InnerText) - 1];
                        this.Information["Utage Chart Maker"] = notesDesignerCandidate.InnerText;
                        this.Information["Utage Chart Path"] = fileCandidate.InnerText;
                    }
                    else if (pathCandidate.InnerText.Contains("11.ma2") && enableCandidate.InnerText.Equals("true"))
                    {
                        var levelCandidate = candidate["level"] ?? throw new NullReferenceException();
                        var levelDecimalCandidate = candidate["levelDecimal"] ?? throw new NullReferenceException();
                        this.Information["Easy Decimal"] = levelCandidate.InnerText + "." + levelDecimalCandidate.InnerText;
                        var musicLevelIDCandidate = candidate["musicLevelID"] ?? throw new NullReferenceException();
                        var notesDesignerCandidate = candidate["notesDesigner"] ?? throw new NullReferenceException();
                        notesDesignerCandidate = notesDesignerCandidate["str"] ?? throw new NullReferenceException();
                        var fileCandidate = candidate["file"] ?? throw new NullReferenceException();
                        fileCandidate = fileCandidate["path"] ?? throw new NullReferenceException();
                        this.Information["Easy"] = level[Int32.Parse(musicLevelIDCandidate.InnerText) - 1];
                        this.Information["Easy Chart Maker"] = notesDesignerCandidate.InnerText;
                        this.Information["Easy Chart Path"] = fileCandidate.InnerText;
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine("There is no such chart: " + ex.Message);
                }
            }

            foreach (XmlNode candidate in bpmCandidate)
            {
                {
                    if (this.TrackBPM.Equals(""))
                    {
                        this.TrackBPM = candidate.InnerText;
                    }
                }
            }

            foreach (XmlNode candidate in sortNameCandidate)
            {
                {
                    if (this.TrackSortName.Equals(""))
                    {
                        this.TrackSortName = candidate.InnerText;
                    }
                }
            }

            foreach (XmlNode candidate in composerCandidate)
            {
                {
                    if (this.TrackComposer.Equals(""))
                    {
                        var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                        this.TrackComposer = strCandidate.InnerText;
                    }
                }
            }

            foreach (XmlNode candidate in genreCandidate)
            {
                {
                    if (this.TrackGenre.Equals(""))
                    {
                        var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                        this.TrackGenre = strCandidate.InnerText;
                    }
                }
            }

            foreach (XmlNode candidate in versionNumberCandidate)
            {
                {
                    if (this.TrackVersionNumber.Equals(""))
                    {
                        var strCandidate = candidate["str"] ?? throw new NullReferenceException();
                        this.TrackVersionNumber = strCandidate.InnerText;
                    }
                }
            }

            foreach (XmlNode candidate in addVersionCandidate)
            {
                {
                    if (this.TrackVersion.Equals(""))
                    {
                        var idCandidate = candidate["id"] ?? throw new NullReferenceException();
                        this.TrackVersion = version[Int32.Parse(idCandidate.InnerText)];
                    }
                }
            }
            this.Information["SDDX Suffix"] = this.StandardDeluxeSuffix;
        }
    }
}
