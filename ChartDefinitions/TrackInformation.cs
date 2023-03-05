using System.Data;
using System.Xml;
using System.Xml.Linq;

namespace MaiLib
{
    /// <summary>
    /// Use xml to store track information
    /// </summary>
    public abstract class TrackInformation : IXmlUtility
    {
        /// <summary>
        /// Stores proper difficulties
        /// </summary>
        /// <value>1-15 Maimai level</value>
        public static readonly string[] level = { "1", "2", "3", "4", "5", "6", "7", "7+", "8", "8+", "9", "9+", "10", "10+", "11", "11+", "12", "12+", "13", "13+", "14", "14+", "15", "15+" };

        public static readonly string[] difficulty = { "Basic", "Advance", "Expert", "Master", "Remaster", "Utage", "Easy" };

        public static readonly string[] addVersion = { "Ver1.00.00"};

        /// <summary>
        /// Stores the genre name used in information
        /// </summary>
        /// <value>103 = Touhou, 105 = maimai</value>
        public static readonly string[] genre = { "東方Project", "maimai" };

        /// <summary>
        /// Stores prover maimai versions
        /// </summary>
        /// <value>Version name of each generation of Maimai</value>
        public static readonly string[] version = { "maimai", "maimai PLUS", "maimai GreeN", "maimai GreeN PLUS", "maimai ORANGE", "maimai ORANGE PLUS", "maimai PiNK", "maimai PiNK PLUS", "maimai MURASAKi", "maimai MURASAKi PLUS", "maimai MiLK", "maimai MiLK PLUS", "maimai FiNALE", "maimai DX", "maimai DX PLUS", "maimai DX Splash", "maimai DX Splash PLUS", "maimai DX UNiVERSE", "maimai DX UNiVERSE PLUS", "maimai DX FESTiVAL" };

        public static readonly string[] shortVersion = { "maimai", "PLUS", "GreeN", "GreeN PLUS", "ORANGE", "ORANGE PLUS", "PiNK", "PiNK PLUS", "MURASAKi", "MURASAKi PLUS", "MiLK", "MiLK PLUS", "FiNALE", "DX", "DX PLUS", "DX Splash", "DX Splash PLUS", "DX UNiVERSE", "DX UNiVERSE PLUS", "FESTiVAL" };

        public static string[] versionArray = {
"10000",
"10001",
"10002",
"11000",
"11001",
"11002",
"11003",
"11004",
"11005",
"11006",
"11007",
"12000",
"12001",
"12002",
"12003",
"12004",
"12005",
"12006",
"12007",
"12008",
"12009",
"13000",
"13001",
"13002",
"13003",
"13004",
"13005",
"13006",
"13007",
"13008",
"13009",
"13010",
"13011",
"14000",
"14001",
"14002",
"14003",
"14004",
"14006",
"14007",
"14008",
"14009",
"14010",
"15000",
"15003",
"15004",
"15005",
"15006",
"15007",
"15008",
"15009",
"15010",
"15011",
"15013",
"15014",
"15016",
"15017",
"15018",
"15019",
"16000",
"16001",
"16002",
"16003",
"16004",
"16005",
"16006",
"16007",
"16008",
"16009",
"16011",
"16012",
"16013",
"16014",
"17000",
"17001",
"17002",
"17003",
"17004",
"17005",
"17006",
"17007",
"17008",
"17009",
"17010",
"17011",
"17012",
"17013",
"17015",
"17016",
"17017",
"17018",
"18000",
"18001",
"18002",
"18003",
"18005",
"18006",
"18007",
"18008",
"18009",
"18010",
"18011",
"18012",
"18014",
"18015",
"18017",
"18018",
"18019",
"18020",
"18021",
"18022",
"18023",
"18500",
"18501",
"18502",
"18503",
"18504",
"18505",
"18506",
"18507",
"18508",
"18509",
"18511",
"18512",
"18599",
"19000",
"19001",
"19002",
"19003",
"19004",
"19005",
"19006",
"19007",
"19008",
"19009",
"19010",
"19011",
"19012",
"19013",
"19500",
"19501",
"19502",
"19503",
"19504",
"19505",
"19506",
"19507",
"19508",
"19509",
"19510",
"19511",
"19512",
"19513",
"19514",
"19900",
"19901",
"19902",
"19903",
"19904",
"19905",
"19906",
"19907",
"19908",
"19909",
"19910",
"19911",
"19912",
"19992",
"19993",
"19994",
"19995",
"19996",
"19997",
"19998",
"19999",
"20000",
"20001",
"20002",
"20003",
"20004",
"20005",
"20006",
"20007",
"20008",
"20009",
"20010",
"20011",
"20012",
"20013",
"20014",
"20015",
"20500",
"20501",
"20502",
"20503",
"20504",
"20505",
"20506",
"20507",
"20508",
"20509",
"20510",
"20511",
"20512",
"20513",
"20514",
"21000",
"21001",
"21002",
"21003",
"21004",
"21005",
"21006",
"21007",
"21008",
"21009",
"21010",
"21011",
"21012",
"21500",
"21501",
"21502",
"21503",
"21504",
"21505",
"21506",
"21507",
"21508",
"21509",
"21510",
"21511",
"21512",
"21513",
"22000",
"22001",
"22002",
"22003",
"22004",
"22005",
"22006",
"22007",
"22008",
"22009",
"22010",
"22011",
"22012",
"22013",
"22014",
"22015",
"22501",
"22502",
"22503",
"22504",
"22505",
};
        public static Dictionary<string, string> netOpenNameDic = new Dictionary<string, string>{
{"0", "Net190711"},
{"1", "Net190719"},
{"2", "Net190726"},
{"3", "Net190809"},
{"4", "Net190822"},
{"5", "Net190906"},
{"6", "Net190920"},
{"7", "Net191004"},
{"8", "Net191018"},
{"9", "Net191024"},
{"10", "Net191101"},
{"11", "Net191115"},
{"12", "Net191129"},
{"13", "Net191213"},
{"14", "Net191220"},
{"15", "Net200110"},
{"200123", "Net200123"},
{"200207", "Net200207"},
{"200214", "Net200214"},
{"200220", "Net200220"},
{"200306", "Net200306"},
{"200320", "Net200320"},
{"200605", "Net200605"},
{"200612", "Net200612"},
{"200619", "Net200619"},
{"200626", "Net200626"},
{"200710", "Net200710"},
{"200724", "Net200724"},
{"200807", "Net200807"},
{"200821", "Net200821"},
{"200904", "Net200904"},
{"200917", "Net200917"},
{"201001", "Net201001"},
{"201016", "Net201016"},
{"201030", "Net201030"},
{"201113", "Net201113"},
{"201127", "Net201127"},
{"201211", "Net201211"},
{"201225", "Net201225"},
{"210108", "Net210108"},
{"210121", "Net210121"},
{"210205", "Net210205"},
{"210219", "Net210219"},
{"210305", "Net210305"},
{"210318", "Net210318"},
{"210401", "Net210401"},
{"210409", "Net210409"},
{"210416", "Net210416"},
{"210428", "Net210428"},
{"210514", "Net210514"},
{"210528", "Net210528"},
{"210611", "Net210611"},
{"210625", "Net210625"},
{"210709", "Net210709"},
{"210723", "Net210723"},
{"210805", "Net210805"},
{"210820", "Net210820"},
{"210903", "Net210903"},
{"210916", "Net210916"},
{"210922", "Net210922"},
{"211001", "Net211001"},
{"211015", "Net211015"},
{"211029", "Net211029"},
{"211112", "Net211112"},
{"211126", "Net211126"},
{"211210", "Net211210"},
{"211216", "Net211216"},
{"211224", "Net211224"},
{"220107", "Net220107"},
{"220114", "Net220114"},
{"220128", "Net220128"},
{"220210", "Net220210"},
{"220225", "Net220225"},
{"220303", "Net220303"},
{"220311", "Net220311"},
{"220324", "Net220324"},
{"220401", "Net220401"},
{"220408", "Net220408"},
{"220415", "Net220415"},
{"220428", "Net220428"},
};
        public static Dictionary<string, string> releaseTagNameDic = new Dictionary<string, string>{
{"1", "Ver1.00.00"},
{"501", "Ver1.05.00"},
{"1001", "Ver1.10.00"},
{"1501", "Ver1.15.00"},
{"2001", "Ver1.20.00"},
{"2501", "Ver1.25.00"},
};
        public static Dictionary<string, string> rightsInfoDic = new Dictionary<string, string>{
{"0", ""},
{"1", "? 2016 暁なつめ?三嶋くろね／KADOKAWA／このすば製作委員会"},
{"2", "? 2017 つくしあきひと?竹書房／メイドインアビス製作委員会"},
{"3", "? Crypton Future Media, INC. www.piapro.net (+piapro logo)"},
{"4", "? Crypton Future Media, INC. www.piapro.net (+piapro logo) ?MTK/INTERNET Co., Ltd."},
{"5", "? Crypton Future Media, INC. www.piapro.net (+piapro logo) GUMI(Megpoid)?INTERNET Co., LTD."},
{"6", "? Crypton Future Media, INC. www.piapro.net (+piapro logo) デザイン協力：ねんどろいど"},
{"7", "? Cygames, Inc."},
{"8", "? Green Leaves / Wake Up, Girls！製作委員会"},
{"9", "?Koi?芳文社／ご注文は製作委員会ですか？"},
{"10", "? TAITO CORP.1996"},
{"11", "? TAITO CORP.1996 ?SEGA"},
{"12", "? カルロ?ゼン?KADOKAWA刊／幼女戦記製作委員会"},
{"13", "? 長月達平?株式会社KADOKAWA刊／Re:ゼロから始める異世界生活製作委員会"},
{"14", "?1st PLACE Co., Ltd. / IA PROJECT"},
{"15", "?2013 RK,KM/UMDP"},
{"16", "?2013ちょぼらうにょぽみ／竹書房　倉持南高校漫画研究部（竹書房?SPO?AT-X）"},
{"18", "?2014 Konami Digital Entertainment"},
{"20", "?2014 榎宮祐?株式会社ＫＡＤＯＫＡＷＡ メディアファクトリー刊／ノーゲーム?ノーライフ全権代理委員会"},
{"21", "?2015 サンカクヘッド／集英社?「干物妹！うまるちゃん」製作委員会"},
{"23", "?2016 うかみ／ＫＡＤＯＫＡＷＡ　アスキー?メディアワークス／ガヴリールドロップアウト製作委員会"},
{"25", "?2017 VOCALOMAKETS Powered by Bumpy Factory Corporation. 結月ゆかりは株式会社バンピーファクトリーの登録商標です 。"},
{"26", "?2017 サンカクヘッド／集英社?「干物妹！うまるちゃんR」製作委員会"},
{"27", "?2018 VOCALOMAKETS Powered by Bumpy Factory Corporation. ?Copyright  2014 AI,Inc. All Rights Reserved. ?AHS Co. Ltd."},
{"28", "?2018 VOCALOMAKETS Powered by Bumpy Factory Corporation. 結月ゆかりは株式会社バンピーファクトリーの登録商標です。"},
{"29", "?Ark Performance/少年画報社?アルペジオパートナーズ"},
{"31", "?BANDAI NAMCO Entertainment Inc."},
{"32", "?BANDAI NAMCO Games Inc."},
{"33", "?BANDAI NAMCO Games Inc. ? TAITO CORP.1996"},
{"34", "?BANDAI NAMCO Games Inc. ?SEGA"},
{"35", "?HARUKAZE"},
{"36", "?Junky"},
{"37", "?MTK/INTERNET Co., Ltd."},
{"38", "?NHN PlayArt Corp. ?DWANGO Co., Ltd."},
{"39", "?Sammy"},
{"40", "?SEGA"},
{"41", "?SEGA / f4samurai"},
{"42", "?SEGA/?RED"},
{"43", "?SEGA/ＧＯ！ＧＯ！５７５製作委員会"},
{"45", "?SEGA?RED Illustration:Kosuke Fujishima"},
{"46", "?SEGA?RED?白泉社"},
{"47", "?TAITO CORP.1978,2014"},
{"48", "?TOHOKU PENET K.K."},
{"49", "?あfろ?芳文社／野外活動サークル"},
{"50", "?けものフレンズプロジェクト"},
{"53", "?にじよめ"},
{"54", "?ヒロユキ?講談社／アホガール製作委員会"},
{"55", "?鎌池和馬／冬川基／アスキー?メディアワークス／PROJECT-RAILGUN S"},
{"57", "?三月?KADOKAWA刊／ひなこのーと製作委員会"},
{"58", "?上海アリス幻樂団"},
{"59", "?上海アリス幻樂団 「おてんば恋娘」"},
{"60", "?上海アリス幻樂団 「竹取飛翔　～ Lunatic Princess」"},
{"61", "?大川ぶくぶ/竹書房?キングレコード"},
{"63", "designed by HaltquinZ????(ましろま+Rinne.6)"},
{"64", "GUMI(Megpoid)?INTERNET Co., LTD."},
{"65", "ill.by 穂嶋 ? Crypton Future Media, INC. www.piapro.net (+piapro logo)"},
{"66", "illustration by AO FUJIMORI / 雪ミク2017 ? Crypton Future Media, INC.　www.piapro.net (+piapro logo)"},
{"67", "illustろこる(@tuno901)"},
{"68", "Licensed by BANDAI NAMCO Arts Inc./Lantis Records. ?毛魂一直線?ふゅーじょんぷろだくと／魔法少女俺製作委員会"},
{"69", "Licensed by REISSUE RECORDS inc. ? Crypton Future Media, INC. www.piapro.net (+piapro logo)"},
{"70", "Licensed by TOY'S FACTORY INC."},
{"71", "?2015 SPACE SHOWER NETWORKS INC. ?2015 SPACE SHOWER NETWORKS INC."},
{"72", "TVアニメ『Hi☆sCoool! セハガール』?SEGA/セハガガ学園理事会"},
{"73", "セガ?ハード?ガールズ ?SEGA TVアニメ『Hi☆sCoool! セハガール』?SEGA/セハガガ学園理事会"},
{"74", "プランチャイム"},
{"75", "蒲焼鰻 ?上海アリス幻樂団"},
{"76", "重音テト? 線/小山乃舞世/ツインドリル"},
{"77", "?けものフレンズプロジェクト2A"},
{"78", "U.S.A. Music by Claudio Accatino, Donatella Cirelli & Anna Maria Gioco  Words by Donatella Cirelli & Severino Lombardoni ? by THE SAIFAM GROUP SRL ? by EDIZIONI ZYX MUSIC S.R.L. All rights reserved. Used by permission. Rights for Japan administered by NICHION, INC. Licensed by Avex Music Publishing Inc."},
{"79", "? UUUM"},
{"80", "?円谷プロ ?2018 TRIGGER?雨宮哲／「GRIDMAN」製作委員会"},
{"81", "?KADOKAWA CORPORATION ?F.M.F Co,.Ltd."},
{"82", "?板垣恵介(秋田書店)／バキ製作委員会"},
{"83", "? コチンPa!製作委員会"},
{"84", "?ゾンビランドサガ製作委員会"},
{"85", "?円谷プロ ?怪獣娘2（ウルトラ怪獣擬人化計画）製作委員会"},
{"86", "?2019 VOCALOMAKETS Powered by Bumpy Factory Corporation. 結月ゆかりは株式会社バンピーファクトリーの登録商標です 。"},
{"87", "?2019 VOCALOMAKETS Powered by Bumpy Factory Corporation. 紲星あかりは株式会社バンピーファクトリーの登録商標です 。"},
{"88", "?円谷プロ ?怪獣娘黒（ウルトラ怪獣擬人化計画）製作委員会"},
{"89", "? 大川ぶくぶ/竹書房?キングレコード　?AC部"},
{"90", "? Crypton Future Media, INC. www.piapro.net (+piapro logo) 2012 by FUJIPACIFIC MUSIC INC. & DWANGO Co., Ltd."},
{"91", "?2015 by HIP LAND MUSIC CORPORATION INC. Licensed by Victor Entertainment"},
{"92", "Licensed by BANDAI NAMCO ARTS Inc./Lantis Records ?クール教信者?双葉社／ドラゴン生活向上委員会"},
{"93", "?赤坂アカ／集英社?かぐや様は告らせたい製作委員会"},
{"94", "?異世界かるてっと／ＫＡＤＯＫＡＷＡ"},
{"96", "?CHIROLU?ホビージャパン／白金の妖精姫を見守る会"},
{"97", "?TADAKOI PARTNERS  ?KADOKAWA CORPORATION"},
{"98", "? Crypton Future Media, INC. www.piapro.net (+piapro logo) ?NHN PlayArt Corp. ?DWANGO Co., Ltd."},
{"99", "?六厘舎"},
{"100", "?Rayark Inc."},
{"101", "? UMAMI CHAN ? やおきん"},
{"102", "?VAP"},
{"103", "?Koi?芳文社／ご注文は製作委員会ですか？？"},
{"104", "?伊藤いづも?芳文社／まちカドまぞく製作委員会"},
{"105", "Licensed by Sony Music Labels Inc."},
{"106", "?小林 立/スクウェアエニックス?咲阿知賀編製作委員会 ? BANDAI NAMCO Arts Inc./Lantis Records"},
{"107", "?2019 サンドロビッチ?ヤバ子，MAAM?小学館／シルバーマンジム"},
{"108", "?2017-2020 Ichikara Inc."},
{"109", "?OFFICIALHIGEDANDISM"},
{"110", "?Copyright 2014 AI,Inc. All Rights Reserved."},
{"111", "? lowiro 2021"},
{"112", "Emoji Powered by Twemoji, used under CC BY 4.0 / Includes alternation from original"},
{"113", "?OTOIRO Inc."},
{"114", "? 2017-2020 cover corp."},
{"115", "?Gynoid. Illustration by のう"},
{"116", "? SEGA ? Colorful Palette Inc. ? Crypton Future Media, INC. www.piapro.net (+piapro logo) All rights reserved."},
{"117", "? いらすとや"},
{"118", "Licensed by EMI Records, A UNIVERSAL MUSIC COMPANY"},
{"119", "Licensed by UNIVERSAL CLASSICS & JAZZ, A UNIVERSAL MUSIC COMPANY ?2019日本すみっコぐらし協会映画部"},
{"120", "?2006 谷川 流?いとうのいぢ／SOS団"},
{"122", "?山口悟?一迅社／はめふら製作委員会 Licensed by KING RECORD Co., Ltd."},
{"123", "ビッカメ娘?ナイセン"},
{"124", "?Avogado6"},
{"125", "?Copyright 2021 SSS LLC. ?2021 VOCALOMAKETS Powered by Bumpy Factory Corporation."},
{"126", "?上海アリス幻樂団 ?GSC/NN"},
{"127", "Licensed by Virgin Music, A UNIVERSAL MUSIC COMPANY"},
{"128", "?cosMo@Bousou-P/CHEMICAL SYSTEM"},
{"129", "?キノシタ ? Crypton Future Media, INC. www.piapro.net (+piapro logo) ?MTK/INTERNET Co., Ltd."},
{"130", "?あfろ?芳文社／野外活動委員会"},
{"131", "?2020 丈／KADOKAWA／宇崎ちゃん製作委員会"},
{"132", "Licensed by Sony Music Entertainment (Japan) Inc. Sony Music Entertainment (Japan) Inc. / 藍にいな"},
{"133", "?キノシタ ?MTK/INTERNET Co., Ltd."},
{"134", "?上海アリス幻樂団  ?アンノウンX / AQUASTYLE?DeNA?xeen  inspiredby 東方Project"},
{"135", "?ANYCOLOR, Inc."},
{"136", "? Bit192"},
{"137", "?2018 F.M.F"},
{"138", "Licensed by unBORDE / Warner Music Japan Inc. / A-Sketch inc.  ??Avogado6"},
{"139", "?クール教信者?双葉社／ドラゴン生活向上委員会"},
{"140", "?BANDAI NAMCO Arts Inc. ? Cygames, Inc."},
{"141", "?bushiroad All Rights Reserved. ? 2020 DONUTS Co. Ltd. All Rights Reserved."},
{"144", "? Crypton Future Media, INC. www.piapro.net (+piapro logo) ?OTOIRO Inc."},
};
        public static Dictionary<string, string> artistNameDic = new Dictionary<string, string>{
{"0", ""},
{"1", "「BAYONETTA(ベヨネッタ)」"},
{"2", "「BORDER BREAK UNION」メインテーマ"},
{"3", "「BORDER BREAK」"},
{"4", "「PHANTASY STAR PORTABLE」"},
{"5", "「PHANTASY STAR PORTABLE2 ∞」"},
{"6", "「PHANTASY STAR PORTABLE2」"},
{"7", "「サクラ大戦」"},
{"8", "「サクラ大戦奏組」"},
{"9", "「バーチャファイター」"},
{"10", "「新豪血寺一族 -煩悩解放-」"},
{"11", "「龍が如く 見参！」"},
{"12", "「龍が如く２」"},
{"13", "★STAR GUiTAR [cover]"},
{"14", "10Re;「学園ハンサム」"},
{"15", "164"},
{"16", "40mP"},
{"18", "A4paper"},
{"19", "AcuticNotes"},
{"20", "After the Rain（そらる×まふまふ）"},
{"21", "ALiCE'S EMOTiON feat. Ayumi Nomiya"},
{"22", "Aliesrite* (Ym1024 feat. lamie*)"},
{"23", "angela「アホガール」"},
{"24", "A-One"},
{"25", "ARM(IOSYS)"},
{"26", "ARM＋夕野ヨシミ (IOSYS) feat.miko"},
{"27", "ARM＋夕野ヨシミ(IOSYS)"},
{"28", "ARM＋夕野ヨシミ(IOSYS)feat. miko"},
{"29", "ARM＋夕野ヨシミ(IOSYS)feat. 藤咲かりん"},
{"30", "ARM＋夕野ヨシミ(IOSYS)feat. 藤枝あかね"},
{"31", "ARM＋夕野ヨシミ(IOSYS)feat.山本椛"},
{"32", "BNSI（Taku Inoue）「シンクロニカ」より"},
{"33", "capsule [cover]"},
{"34", "Caramell/Caramelldansen [cover]"},
{"35", "Cash Cash「ソニック ジェネレーションズ」"},
{"36", "Circle of friends(天月-あまつき-?un:c?伊東歌詞太郎?コニー?はしやん)"},
{"37", "CIRCRUSH"},
{"38", "ClariS　「魔法少女まどか☆マギカ」"},
{"39", "Clean Tears"},
{"40", "Clean Tears feat. Youna"},
{"41", "COSIO(ZUNTATA)「グルーヴコースター」より"},
{"42", "COSIO(ZUNTATA/TAITO)/沢城千春「電車でGO!」"},
{"43", "COSIO(ZUNTATA/TAITO)「カルテット」"},
{"44", "COSIO(ZUNTATA/TAITO)「ファンタジーゾーン」「RIDGE RACER」"},
{"45", "cosMo＠暴走P"},
{"46", "Cranky"},
{"47", "Cranky feat. まらしぃ ＆ てっぺい先生"},
{"48", "Crush 40「ソニックアドベンチャー2」"},
{"49", "cubesato"},
{"50", "cybermiso"},
{"51", "D.watt(IOSYS)＋らっぷびと"},
{"52", "D.watt(IOSYS)feat. Asana"},
{"53", "D.watt(IOSYS)feat.ちよこ"},
{"54", "daniwell"},
{"55", "dawn-system"},
{"56", "D-Cee"},
{"57", "DECO*27"},
{"58", "DECO*27 feat.echo"},
{"59", "DECO*27 feat.Tia"},
{"60", "DiGiTAL WiNG"},
{"61", "Dixie Flatline"},
{"62", "DJ YOSHITAKA「jubeat」より"},
{"63", "doriko"},
{"65", "E.G.G.「グルーヴコースター」より"},
{"66", "EasyPop"},
{"67", "EB(aka EarBreaker)"},
{"68", "EBIMAYO"},
{"69", "ELEMENTAS feat. NAGISA"},
{"70", "emon"},
{"71", "Eve"},
{"72", "EZFG"},
{"73", "Feryquitous"},
{"76", "Frums"},
{"77", "good-cool ft. Pete Klassen"},
{"78", "GOSH/manami"},
{"79", "GOSH/クラシック「悲愴」"},
{"80", "GYARI"},
{"81", "HALFBY"},
{"82", "Halozy"},
{"83", "hanzo/赤飯歌唱Ver"},
{"84", "Hi☆sCoool! セハガール"},
{"85", "Hiro"},
{"86", "Hiro feat.越田Rute隆人&ビートまりお"},
{"87", "Hiro(SEGA)「RIDGE RACER」「電車でGO!」"},
{"88", "Hiro(SEGA)「ファンタジーゾーン」"},
{"89", "Hiro/タクマロ"},
{"90", "Hiro/永江理奈"},
{"91", "Hiro/小山あかり"},
{"92", "Hiro/森山愛子"},
{"93", "Hiro/友香"},
{"94", "Hiro「Crackin’DJ」"},
{"95", "Hiro「maimai」より"},
{"96", "HiTECH NINJA"},
{"97", "HoneyWorks"},
{"98", "ika"},
{"99", "Innocent Keyと小宮真央、ココ、樹詩音"},
{"100", "Innocent Keyと梨本悠里、JUNCA、小宮真央、樹詩音、大瀬良あい"},
{"101", "INNOCENT NOIZE"},
{"102", "Ino(chronoize)"},
{"103", "IOSYSと愉快な⑨周年フレンズ"},
{"104", "iroha(sasaki)／kuma(alfred)"},
{"105", "IRON ATTACK!"},
{"106", "Jimmy Weckl"},
{"107", "Jimmy Weckl feat.高貴みな"},
{"108", "Jitterin' Jinn [covered by ろん]"},
{"109", "Jun Senoue"},
{"110", "Junky"},
{"111", "Kai/クラシック「G線上のアリア」"},
{"112", "Kai/光吉猛修"},
{"113", "Kai/能登有沙"},
{"114", "Kai「Wonderland Wars」"},
{"115", "kamome sano"},
{"116", "kanone feat. せんざい"},
{"117", "Keitarou Hanada「戦国大戦」"},
{"118", "kemu"},
{"119", "KeN/エンドケイプ"},
{"120", "KENTARO「Crackin’DJ」"},
{"121", "koutaq"},
{"122", "Last Note."},
{"123", "LeaF"},
{"124", "LindaAI-CUE(BNGI)「太鼓の達人」より"},
{"126", "livetune"},
{"127", "loos feat. Meramipop"},
{"128", "loos feat. 柊莉杏"},
{"129", "LOPIT(SEGA)"},
{"130", "M.S.S Project"},
{"131", "M2U"},
{"132", "Machico「この素晴らしい世界に祝福を！」"},
{"133", "MARETU"},
{"134", "MARUDARUMA"},
{"135", "Masayoshi Minoshima"},
{"136", "Masayoshi Minoshima feat. 綾倉盟"},
{"137", "mathru(かにみそP)"},
{"138", "MECHANiCAL PANDA「BORDER BREAK」"},
{"139", "m-flo [cover]"},
{"140", "MintJam feat.光吉猛修"},
{"141", "Mitchie M"},
{"142", "MY FIRST STORY"},
{"143", "MYTH & ROID「Re：ゼロから始める異世界生活」"},
{"144", "myu314 feat.あまね(COOL&CREATE)"},
{"145", "n.k"},
{"146", "Nankumo/CUBE3"},
{"147", "naotyu- feat. 佐々木恵梨"},
{"148", "n-buna"},
{"149", "n-buna × Orangestar"},
{"150", "n-buna feat.ヤギヌマカナ"},
{"151", "Neru"},
{"152", "niki"},
{"153", "NIKO [cover]"},
{"154", "Nizikawa"},
{"155", "nora2r"},
{"156", "ONE (song by ナナホシ管弦楽団)"},
{"157", "ONIGAWARA"},
{"159", "Orangestar"},
{"160", "OSTER project"},
{"161", "Otomania feat. 初音ミク"},
{"162", "out of service"},
{"163", "owl＊tree"},
{"164", "Palme"},
{"165", "paraoka"},
{"166", "Performed by SEGA Sound Unit [H.]"},
{"167", "Performed by 光吉猛修"},
{"168", "Petit Rabbit's「ご注文はうさぎですか？」"},
{"169", "Project Grimoire"},
{"170", "Q;indivi [cover]"},
{"171", "Queen P.A.L."},
{"174", "RADWIMPS"},
{"175", "RAMM feat. 若井友希"},
{"176", "Ras"},
{"177", "REDALiCE"},
{"178", "REDALiCE (HARDCORE TANO*C)"},
{"180", "RYOHEI KOHNO/森綾香"},
{"181", "RYOHEI KOHNO/保立美和子"},
{"182", "S!N?高橋菜々×ひとしずく?やま△"},
{"183", "samfree"},
{"184", "samfree feat.(V)???(V)かにぱん。(A-ONE)"},
{"185", "Sampling Masters MEGA"},
{"186", "sampling masters MEGA「パワードリフト」"},
{"187", "sasakure.UK"},
{"188", "sasakure.UK x DECO*27"},
{"189", "SC-3000(シーさん)/CV.相沢 舞、SG-1000(せん)/CV.芹澤 優、SG-1000Ⅱ(せんつー)/CV.大空直美、ゲームギア(ムギ)/CV.田 中美海、ロボピッチャ(ぴっちゃん)/CV.もものはるな「Hi☆sCoool! セハガール」 [アニメPV]"},
{"191", "SEGA Sound Unit [H.]"},
{"193", "Shandy kubota"},
{"194", "SHIKI"},
{"195", "Shoichiro Hirata"},
{"196", "Shoichiro Hirata feat.SUIMI"},
{"197", "SIAM SHADE [covered by 湯毛]"},
{"198", "Silver Forest"},
{"199", "siromaru + cranky"},
{"200", "sky_delta"},
{"201", "SLAVE.V-V-R"},
{"202", "Sou×マチゲリータ"},
{"203", "SOUND HOLIC feat. Nana Takahashi"},
{"204", "SOUND HOLIC feat. 匠眞"},
{"205", "Sta"},
{"206", "Sta feat.b"},
{"207", "Sta feat.tigerlily and DOT96"},
{"208", "Storyteller"},
{"209", "Street"},
{"210", "sumijun(Halozy) feat. ななひら(Confetto)"},
{"211", "supercell「化物語」"},
{"212", "SYNC.ART'S feat. 美里"},
{"213", "t+pazolite"},
{"214", "Taishi"},
{"215", "Takahiro Eguchi feat. 三澤秋"},
{"216", "Tanchiky"},
{"217", "Tatsh"},
{"218", "Team-D"},
{"219", "tilt-six feat.串伊トモミ"},
{"220", "Trefle「チェインクロニクル」"},
{"221", "Tsukasa(Arte Refact)"},
{"222", "un:c?ろん×じーざすP"},
{"223", "UNISON SQUARE GARDEN"},
{"224", "uno(IOSYS) feat.miko"},
{"225", "void (Mournfinale)"},
{"226", "void(Mournfinale)"},
{"227", "void＋夕野ヨシミ (IOSYS) feat.藤原鞠菜"},
{"228", "vox2（小野秀幸）"},
{"229", "WAiKURO"},
{"230", "Wake Up, Girls！"},
{"231", "wowaka"},
{"232", "xi"},
{"233", "YASUHIRO(康寛)"},
{"234", "YMCK"},
{"235", "Yosh(Survive Said The Prophet)"},
{"236", "Yuji Masubuchi(BNGI)「RIDGE RACER」"},
{"237", "Yuji Masubuchi(BNGI)「電車でGO!」「ファンタジーゾーン」"},
{"238", "Yuji Masubuchi「太鼓の達人」より"},
{"239", "zts"},
{"240", "あ～るの～と（いえろ～ぜぶら）"},
{"242", "アゴアニキ"},
{"243", "あべにゅうぷろじぇくと feat.佐倉紗織&井上みゆ「パチスロ快盗天使ツインエンジェル」"},
{"244", "あまね＋ビートまりお(COOL＆CREATE)"},
{"245", "アミティ(CV 菊池志穂)「ぷよぷよ」"},
{"247", "アルル(CV 園崎未恵)「ぷよぷよ」"},
{"248", "イロドリミドリ「CHUNITHM」"},
{"249", "うたたP"},
{"250", "うたよめ575＜正岡小豆(大坪由佳)小林抹茶(大橋彩香)＞"},
{"251", "うどんゲルゲ"},
{"252", "オワタP"},
{"253", "かいりきベア"},
{"254", "ガヴリール（富田美憂），ヴィーネ（大西沙織），サターニャ（大空直美），ラフィエル（花澤香菜）「ガヴリールドロッ プアウト」"},
{"255", "カタオカツグミ"},
{"256", "かねこちはる"},
{"257", "カラスは真っ白"},
{"258", "カラフル?サウンズ?ポート"},
{"259", "ガリガリさむし"},
{"260", "カルロス袴田(サイゼP)"},
{"261", "ギガ/れをる"},
{"262", "ギガ/れをる　ダンス　アルスマグナ"},
{"264", "キノシタ feat. 音街ウナ"},
{"266", "クーナ(CV 喜多村英梨)「PHANTASY STAR ONLINE 2」"},
{"267", "くちばしP"},
{"268", "ゴジマジP"},
{"269", "ササキトモコ「音声感情測定器ココロスキャン」"},
{"270", "ササノマリイ"},
{"271", "さつき が てんこもり"},
{"272", "さつき が てんこもり feat. YURiCa/花たん"},
{"273", "じまんぐ"},
{"274", "じん"},
{"275", "スーパーラバーズ「きみのためなら死ねる」"},
{"276", "スーパーラバーズ「赤ちゃんはどこからくるの？」"},
{"277", "すこっぷ"},
{"278", "スズム"},
{"280", "セブンスヘブンMAXION"},
{"281", "そらる?ろん×れるりり"},
{"282", "ターニャ?デグレチャフ(CV.悠木碧)「幼女戦記」"},
{"283", "ちょむP <advanced mix>"},
{"284", "てにをは"},
{"286", "どうぶつビスケッツ×PPP「けものフレンズ」"},
{"287", "とくP"},
{"288", "どぶウサギ"},
{"289", "トラボルタ"},
{"290", "ナイセン - momoco-"},
{"291", "ナノ feat.MY FIRST STORY"},
{"292", "ナノウ"},
{"293", "ナユタン星人"},
{"294", "にしもと先生、タクマ、どんちゃん「ちょっと盛りました。」"},
{"295", "にじよめちゃんとZIPたん"},
{"296", "ぬゆり"},
{"297", "ぬるはち"},
{"298", "ねこみりん + nora2r feat. 小宮真央"},
{"299", "ねじ式"},
{"300", "のぼる↑"},
{"301", "ノマ"},
{"302", "ハチ"},
{"304", "はっぱ隊 [cover]"},
{"305", "ぱなまん/カラスヤサボウ"},
{"306", "バルーン"},
{"307", "ビートまりお"},
{"308", "ビートまりお + ARM feat. 高橋名人"},
{"309", "ビートまりお(COOL＆CREATE)"},
{"310", "ビートまりお（COOL&CREATE）"},
{"311", "ビートまりお(COOL＆CREATE)+ ARM(IOSYS)"},
{"312", "ビートまりお母(尾崎順子)"},
{"313", "ヒゲドライバー"},
{"314", "ひとしずくP?やま△"},
{"315", "ピノキオP"},
{"316", "ピノキオピー"},
{"317", "ぽてんしゃる0"},
{"318", "ポリスピカデリー"},
{"319", "まふまふ"},
{"320", "みきとP"},
{"321", "モリモリあつし"},
{"322", "ヤスオ"},
{"323", "ゆうゆ"},
{"324", "ゆりん?柿チョコ×Neru"},
{"325", "ルゼ"},
{"326", "れるりり"},
{"327", "れるりり feat.ろん"},
{"328", "ろん×Junky"},
{"329", "ろん×黒魔"},
{"330", "ろん×田中秀和(MONACA)"},
{"331", "ワンダフル☆オポチュニティ！"},
{"332", "亜沙"},
{"333", "愛(C.V.大坪由佳) 麻衣(C.V.内田彩) ミイ(C.V.内田真礼)「あいまいみー」"},
{"334", "伊東歌詞太郎"},
{"335", "伊東歌詞太郎?ろん×まらしぃ"},
{"336", "伊東歌詞太郎?ろん×れるりり"},
{"337", "陰陽座"},
{"338", "下田麻美「Shining?Force CROSS ELYSION」"},
{"339", "加賀美 祥「学園ハンサム」"},
{"340", "歌組雪月花 夜々(原田ひとみ)/いろり(茅野愛衣)/小紫(小倉唯)「機巧少女は傷つかない」"},
{"341", "柿チョコ×みきとP"},
{"342", "角田信朗「ヒーローバンク」"},
{"344", "岸田教団＆THE明星ロケッツ"},
{"345", "暁Records"},
{"346", "桐生 一馬「龍が如く」"},
{"347", "劇団ひととせ「ひなこのーと」"},
{"348", "月鈴 那知(CV:今村 彩夏)"},
{"349", "月鈴 白奈(CV:高野 麻里佳)"},
{"350", "月鈴姉妹（イロドリミドリ）"},
{"351", "古代 祐三「ファンタジーゾーン」"},
{"352", "御形 アリシアナ(CV:福原 綾香)"},
{"353", "光吉猛修"},
{"354", "光吉猛修＆体操隊"},
{"355", "光吉猛修「デイトナ Championship USA」"},
{"356", "光吉猛修「バーチャロン」"},
{"357", "光吉猛修「ホルカ?トルカ」"},
{"358", "向日葵×emon(Tes.)"},
{"359", "高橋菜々×岡部啓一(MONACA)"},
{"360", "黒うさP"},
{"361", "骨盤P"},
{"362", "今日犬"},
{"363", "魂音泉"},
{"364", "佐野 信義「スペースハリアー」"},
{"365", "彩音"},
{"366", "削除"},
{"367", "三草康二郎「CODE OF JOKER」"},
{"368", "山根ミチル"},
{"373", "小仏 凪(CV:佐倉 薫)"},
{"374", "小野隊長とJimmy親分"},
{"376", "上坂すみれ「ポプテピピック」"},
{"377", "新小田夢童 ＆ キラ★ロッソ"},
{"378", "新庄かなえ(CV:三森すずこ)「てーきゅう」"},
{"379", "仁井山征弘 Feat. GREAT G and surprise"},
{"380", "青木 千紘「龍が如く 維新！」"},
{"381", "青木千紘/愛海"},
{"382", "石鹸屋"},
{"383", "霜月はるか"},
{"384", "打首獄門同好会"},
{"385", "大久保 博(BNGI)「デイトナ USA」"},
{"386", "大谷智哉, Douglas Robb from Hoobastank「ソニック フォース」"},
{"387", "大谷智哉, Jean Paul Makhlouf of Cash Cash「ソニック カラーズ」"},
{"388", "大谷智哉「ソニック ジェネレーションズ」"},
{"389", "大谷智哉「ソニック ロストワールド」"},
{"390", "大谷智哉「リズム怪盗R 皇帝ナポレオンの遺産」"},
{"391", "谷屋楽"},
{"392", "樽木栄一郎"},
{"393", "長沼英樹「ソニック ラッシュ」"},
{"394", "椎名もた (ぽわぽわP)"},
{"395", "天王洲 なずな(CV:山本 彩乃)"},
{"397", "田中マイミ"},
{"398", "土間うまる [CV.田中あいみ]「干物妹！うまるちゃん」"},
{"399", "土間うまる（田中あいみ）「干物妹！うまるちゃんR」"},
{"400", "怒髪天「百鬼大戦絵巻」"},
{"401", "東京アクティブNEETs"},
{"402", "東京スカパラダイスオーケストラ [cover]"},
{"403", "豚乙女"},
{"404", "日向電工"},
{"405", "猫叉Master「jubeat」より"},
{"406", "箱部 なる(CV:M?A?O)"},
{"407", "幡谷尚史 Arranged by SEGA Sound Unit [H.]「バーニングレンジャー」"},
{"408", "幡谷尚史「リズム怪盗R 皇帝ナポレオンの遺産」"},
{"409", "八王子P"},
{"410", "発熱巫女～ず"},
{"411", "舞風-MAIKAZE/沙紗飛鳥"},
{"412", "舞風-MAIKAZE/時音-TOKINE"},
{"413", "福山光晴"},
{"414", "福山光晴「Crackin’DJ」"},
{"415", "平井堅 [cover]"},
{"416", "米津玄師"},
{"417", "芳川よしの"},
{"418", "妹Ｓ「干物妹！うまるちゃんR」"},
{"420", "明坂 芹菜(CV:新田 恵海)"},
{"423", "矢鴇つかさ feat. kalon."},
{"424", "矢鴇つかさ feat. 三澤秋"},
{"425", "幽閉サテライト"},
{"426", "鈴木このみ「ノーゲーム?ノーライフ」"},
{"428", "和田たけあき(くらげP)"},
{"429", "鬱P feat.flower"},
{"430", "澤村 遥「龍が如く５ 夢、叶えし者」"},
{"431", "LOPIT"},
{"432", "あべにゅうぷろじぇくと feat.天月めぐる＆如月すみれ「ツインエンジェルBREAK」"},
{"433", "MOSAIC.WAV"},
{"434", "ave;new feat.佐倉紗織"},
{"435", "亜咲花"},
{"436", "大橋彩香"},
{"437", "パトリシア?オブ?エンド?黒木未知?夕莉シャチ?明日原ユウキ「ノラと皇女と野良猫ハート」"},
{"438", "fripSide"},
{"439", "そらまふうらさか"},
{"440", "cosMo VS dj TAKA「SOUND VOLTEX」より"},
{"441", "MASAKI（ZUNTATA）「グルーヴコースター3EXドリームパーティー」より"},
{"442", "Drop＆祇羽 feat. 葉月ゆら「太鼓の達人」より"},
{"443", "デッドボールP"},
{"444", "SOUND HOLIC feat. 3L"},
{"445", "Tsukasa"},
{"446", "HIRO/曲者P"},
{"447", "OSTERproject"},
{"448", "baker"},
{"449", "のりP"},
{"450", "BlackY fused with WAiKURO"},
{"451", "Sprite Recordings"},
{"452", "ビートまりお＋あまね（COOL&CREATE）"},
{"453", "ARM (IOSYS)"},
{"454", "箱部なる(CV:M?A?O)"},
{"455", "小仏凪(CV:佐倉薫)"},
{"456", "月鈴那知(CV:今村彩夏)"},
{"457", "リコ（CV：富田美憂）、レグ（CV：伊瀬茉莉也）"},
{"458", "kanone"},
{"459", "ジータ（CV:金元寿子）、ルリア（CV:東山奈央）、ヴィーラ（CV:今井麻美）、マリー（CV:長谷川明子）"},
{"460", "ペコリーヌ（CV：M?A?O）、コッコロ（CV：伊藤美来）、キャル（CV：立花理香）"},
{"461", "池頼広"},
{"462", "Shoichiro Hirata feat.Sana"},
{"463", "Demetori"},
{"464", "ゆうゆ / 篠螺悠那"},
{"465", "cosMo@暴走P"},
{"466", "Cres."},
{"467", "ちーむMOER"},
{"468", "ねこみりん feat.小宮真央"},
{"469", "光吉猛修の父"},
{"471", "Team Grimoire"},
{"472", "S!N?+α/あるふぁきゅん。×ALI PROJECT"},
{"473", "島爺×蜂屋ななし"},
{"474", "やしきん feat.でらっくま(CV:三森すずこ)"},
{"475", "みきとP feat. FantasticYouth"},
{"476", "164 feat. めいちゃん"},
{"477", "曲：kz(livetune)／歌：オンゲキシューターズ"},
{"480", "MintJam"},
{"481", "Nothing But Requiem (feat.Aikapin & Chiyoko)"},
{"482", "USAO"},
{"483", "舞ヶ原高校軽音部"},
{"484", "きくお"},
{"485", "じーざすP feat.kradness"},
{"486", "青島探偵事務所器楽捜査部B担"},
{"487", "YUC'e"},
{"489", "DIVELA feat.初音ミク"},
{"490", "DA PUMP"},
{"491", "T.M.Revolution [covered by 光吉猛修]"},
{"492", "サカナクション"},
{"493", "OxT"},
{"494", "どうぶつビスケッツ×PPP「けものフレンズ２」"},
{"495", "月鈴 那知（ヴァイオリン） 伴奏：イロドリミドリ"},
{"496", "はるまきごはん feat.初音ミク"},
{"497", "夏代孝明 feat. 初音ミク"},
{"498", "colate"},
{"499", "ポプ子(CV:五十嵐裕美)＆ピピ美(CV:松嵜麗)"},
{"500", "Fear, and Loathing in Las Vegas"},
{"501", "lumo"},
{"502", "トーマ"},
{"503", "otetsu"},
{"504", "まらしぃ"},
{"505", "フランシュシュ"},
{"506", "くっちー (DARKSIDE APPROACH)"},
{"508", "ヨルシカ"},
{"509", "あわのあゆむ"},
{"510", "BLACK STARS（ブラック指令?ペガッサ星人?シルバーブルーメ?ノーバ）"},
{"511", "アギラ?キングジョー?ガッツ星人（CV：飯田里穂?三森すずこ?松田利冴）"},
{"512", "立秋 feat.ちょこ"},
{"513", "Omoi feat. 初音ミク"},
{"515", "supercell"},
{"517", "GigaReol"},
{"520", "ンダホ&ぺけたん from Fischer's"},
{"521", "アイラ（CV：三森すずこ）シマ（CV:井口裕香）はな（CV:花澤香菜）"},
{"522", "有機酸"},
{"523", "SAMBA MASTER 佐藤"},
{"524", "くらげP"},
{"525", "曲：宮崎誠／歌：オンゲキシューターズ"},
{"526", "ユリイ?カノン feat.GUMI"},
{"527", "なきゃむりゃ"},
{"528", "削除 feat. Nikki Simmons"},
{"529", "ビートまりお × Cranky"},
{"530", "REDALiCE feat. Ayumi Nomiya"},
{"531", "cosMo＠暴走AlterEgo"},
{"532", "BlackY vs. WAiKURO"},
{"533", "Yooh"},
{"534", "Maozon"},
{"535", "C-Show"},
{"536", "Masahiro \"Godspeed\" Aoki VS Kai"},
{"537", "ああああ"},
{"538", "じーざす（ワンダフル☆オポチュニティ！）feat.Kradness"},
{"539", "SADA 2Futureanthem feat. ellie"},
{"540", "中山真斗"},
{"541", "Rigel Theatre feat. ミーウェル"},
{"542", "山本真央樹"},
{"543", "syudou"},
{"544", "曲：村カワ基成／歌：オンゲキシューターズ"},
{"545", "曲：鯨井国家／歌：藍原 椿(CV：橋本 ちなみ)"},
{"546", "曲：ゆよゆっぺ／歌：九條 楓(CV：佳村 はるか)"},
{"547", "Yunomi feat.nicamoq"},
{"548", "happy machine"},
{"549", "大国奏音"},
{"550", "OSTER project feat. かなたん"},
{"551", "owl＊tree feat.chi＊tree"},
{"552", "ハムちゃんず [covered by 光吉猛修]"},
{"553", "fhána"},
{"554", "ろくろ"},
{"555", "梅とら"},
{"556", "森羅万象"},
{"557", "Liz Triangle"},
{"558", "削除 feat. void (Mournfinale)"},
{"559", "KAH"},
{"560", "しーけー"},
{"561", "鈴木雅之"},
{"562", "アインズ(日野聡)、カズマ(福島潤)、スバル(小林裕介)、ターニャ(悠木碧)"},
{"563", "カクとイムラ(CV：松本慶祐)"},
{"564", "ラティナ(CV:高尾奏音)"},
{"565", "オーイシマサヨシ"},
{"566", "田中Ｂ"},
{"567", "すりぃ"},
{"569", "Syrufit + HDLV"},
{"570", "Silentroom"},
{"571", "六厘舎"},
{"572", "Ice"},
{"573", "xi vs sakuzyo"},
{"574", "Rabpit"},
{"575", "かめりあ(EDP)"},
{"576", "?sir"},
{"577", "UMAINA"},
{"578", "Masayoshi Minoshima × REDALiCE"},
{"579", "キノシタ feat. 音街ウナ?鏡音リン"},
{"580", "XYZ"},
{"581", "ヒゲドライバー feat.らいむっくま(CV:村川梨衣) ＆ れもんっくま(CV:MoeMi)"},
{"582", "technoplanet"},
{"583", "hanzo"},
{"584", "千本松 仁"},
{"585", "キノシタ feat. そらみこ (ホロライブ ときのそら＆さくらみこ)"},
{"586", "aran"},
{"587", "雄之助 feat. Sennzai"},
{"588", "PSYQUI"},
{"589", "Laur"},
{"590", "40mP feat. シャノ"},
{"591", "RD-Sounds feat.中恵光城"},
{"592", "達見 恵 featured by 佐野 宏晃"},
{"593", "*Luna feat.NORISTRY"},
{"594", "お月さま交響曲"},
{"595", "アリスシャッハと魔法の楽団"},
{"596", "llliiillliiilll"},
{"597", "ユリイ?カノン feat.ウォルピスカーター&まひる"},
{"598", "ELECTROCUTICA"},
{"599", "曲：中山真斗／歌：マーチングポケッツ [日向 千夏(CV：岡咲 美保)、柏木 美亜(CV：和氣 あず未)、東雲 つむぎ(CV：和 泉 風花)]"},
{"600", "曲：やしきん／歌：柏木 美亜(CV：和氣 あず未)"},
{"601", "ぺのれり"},
{"602", "Cranky feat.おもしろ三国志"},
{"603", "かいりきベア feat.松下"},
{"604", "Petit Rabbit's"},
{"605", "shami momo（吉田優子?千代田桃）／CV：小原好美?鬼頭明里"},
{"606", "LiSA"},
{"608", "StylipS"},
{"609", "紗倉ひびき（CV:ファイルーズあい）＆街雄鳴造（CV:石川界人）"},
{"610", "にじさんじ"},
{"611", "カンザキイオリ"},
{"612", "作曲：カジャ　作詞：傘村トータ　編曲：ねじ式　調声：シリア"},
{"613", "獅子志司"},
{"614", "ピコン"},
{"615", "ナナホシ管弦楽団"},
{"616", "かいりきベア feat.flower"},
{"617", "かいりきベア feat.GUMI"},
{"618", "DiGiTAL WiNG feat. 花たん"},
{"620", "REDALiCE & aran"},
{"621", "TANO*C Sound Team"},
{"622", "Akira Complex"},
{"623", "Team Grimoire vs Laur"},
{"624", "Aoi"},
{"625", "Ino(chronoize) feat. 柳瀬マサキ"},
{"626", "yaseta"},
{"627", "owl＊tree feat.yu＊tree"},
{"628", "onoken"},
{"629", "Official髭男dism"},
{"630", "かいりきベア feat.利香"},
{"631", "CielArc"},
{"633", "水野健治"},
{"634", "作詞作曲：椎名もた　編曲：KTG　映像：Yuma Saito"},
{"635", "Giga / 鏡音リン?レン / Vivid BAD SQUAD「プロジェクトセカイ カラフルステージ！ feat. 初音ミク」"},
{"636", "Orangestar feat.IA"},
{"637", "owl＊tree Remixed by Camellia"},
{"638", "ササノマリイ / 初音ミク / 25時、ナイトコードで。「プロジェクトセカイ カラフルステージ！ feat. 初音ミク」"},
{"639", "HiTECH NINJA vs Cranky"},
{"640", "DIVELA feat.桐谷こむぎ"},
{"641", "黒魔"},
{"642", "Juggernaut."},
{"643", "TAKU1175 ft.駄々子"},
{"644", "Endorfin."},
{"645", "ツミキ × 宮下遊"},
{"646", "nanobii"},
{"647", "BlackY feat. Risa Yuzuki"},
{"648", "Lime"},
{"649", "Capchii"},
{"651", "萩原 七々瀬(CV:東城 日沙子)"},
{"652", "葛城 華(CV:丸岡 和佳奈)"},
{"653", "小野 美苗(CV:伊藤 美来)"},
{"654", "ゆゆうた"},
{"655", "DJ Myosuke"},
{"656", "Massive New Krew"},
{"657", "IOSYS TRAX"},
{"658", "曲：大畑拓也／歌：bitter flavor [桜井 春菜(CV：近藤 玲奈)、早乙女 彩華(CV：中島 唯)]"},
{"659", "BlackY vs. Yooh"},
{"660", "曲：Q-MHz／歌：オンゲキシューターズ"},
{"661", "Mrs. GREEN APPLE"},
{"662", "原田知世"},
{"663", "涼宮ハルヒ（CV.平野 綾） TVアニメ「涼宮ハルヒの憂鬱」"},
{"664", "covered by 光吉猛修"},
{"665", "ずっと真夜中でいいのに。"},
{"667", "angela"},
{"668", "ビックカメラ"},
{"669", "東方LostWord feat.いとうかなこ"},
{"670", "hololive IDOL PROJECT"},
{"671", "Kanaria"},
{"672", "Ayase"},
{"673", "タケモトピアノCM"},
{"674", "P.I.N.A."},
{"675", "羽生まゐご"},
{"676", "煮ル果実"},
{"677", "はるまきごはん"},
{"678", "uma vs. モリモリあつし"},
{"679", "DOT96"},
{"680", "litmus*"},
{"681", "かめりあ feat. ななひら"},
{"682", "蜂屋ななし"},
{"683", "奏音"},
{"684", "五十嵐 撫子(CV:花井 美春)"},
{"685", "LeaF & Optie"},
{"686", "suzu"},
{"687", "黒沢ダイスケ VS 穴山大輔"},
{"688", "t+pazolite feat. しゃま(CV:種﨑 敦美) & みるく(CV:伊藤 あすか)"},
{"689", "さつき が てんこもり feat.96猫"},
{"690", "niki feat.noire"},
{"691", "梅干茶漬け"},
{"692", "Kai VS 大国奏音 VS 水野健治"},
{"693", "Umiai"},
{"694", "しょご/野村渉悟"},
{"695", "Nhato"},
{"696", "PRASTIK DANCEFLOOR"},
{"697", "Kobaryo"},
{"698", "An & Ryunosuke Kudo"},
{"699", "Limonène (サノカモメ+月島春果)"},
{"700", "HiTECH NINJA feat. RANASOL"},
{"701", "隣の庭は青い(庭師+Aoi)"},
{"702", "曲：齋藤大／歌：珠洲島 有栖(CV：長縄 まりあ)"},
{"703", "曲：本多友紀（Arte Refact）／歌：オンゲキシューターズ"},
{"704", "MAYA AKAI"},
{"705", "舞ヶ原シンセ研究会"},
{"707", "Acotto"},
{"708", "t+pazolite (HARDCORE TANO*C) feat.TANO*C ALL STARS"},
{"709", "YOASOBI"},
{"710", "Ado"},
{"711", "クレシェンドブルー [最上静香 (CV.田所あずさ)、北上麗花 (CV.平山笑美)、北沢志保 (CV.雨宮 天)、野々原 茜 (CV.小笠原早紀)、箱崎星梨花 (CV.麻倉もも)]"},
{"712", "DRAMATIC STARS [天道 輝 (CV.仲村宗悟), 桜庭 薫 (CV.内田雄馬), 柏木 翼 (CV.八代 拓)]"},
{"714", "COOL&CREATE × 宝鐘マリン"},
{"715", "Sta feat. b"},
{"717", "403"},
{"718", "s-don as Iriss-Frantzz"},
{"719", "Dachs"},
{"720", "オーイシマサヨシ×加藤純一"},
{"721", "ピノキオピー / 初音ミク / ワンダーランズ×ショウタイム「プロジェクトセカイ カラフルステージ！ feat. 初音ミク」"},
{"724", "SYNC.ART'S feat. 3L"},
{"725", "キノシタ"},
{"727", "ツユ"},
{"728", "Rain Drops"},
{"729", "かいりきベア?MARETU feat.初音ミク"},
{"730", "キタニタツヤ"},
{"731", "Chinozo"},
{"732", "john"},
{"733", "稲葉曇"},
{"734", "鹿乃と宇崎ちゃん"},
{"736", "Yunomi & nicamoq"},
{"737", "じーざす（ワンダフル☆オポチュニティ！）"},
{"739", "Unlucky Morpheus"},
{"740", "すりぃ×相沢"},
{"742", "須田景凪"},
{"743", "ARuFa"},
{"744", "Eve TVアニメ「呪術廻戦」第1クールオープニングテーマ"},
{"745", "Mitchie M / 初音ミク / MORE MORE JUMP！「プロジェクトセカイ カラフルステージ！ feat. 初音ミク」"},
{"746", "DECO*27 / 初音ミク / Leo/need「プロジェクトセカイ カラフルステージ！ feat. 初音ミク」"},
{"747", "岸田教団&THE明星ロケッツ×草野華余子「東方ダンマクカグラ」"},
{"748", "COOL&CREATE feat.ビートまりおとまろん「東方ダンマクカグラ」"},
{"749", "BlackYooh vs. siromaru"},
{"750", "烏屋茶房 feat. 利香"},
{"751", "パソコン音楽クラブ feat.ぷにぷに電機"},
{"752", "翡乃イスカ"},
{"753", "Noah"},
{"754", "rintaro soma"},
{"768", "スペシャルウィーク （CV.和氣あず未）、サイレンススズカ （CV.高野麻里佳）、トウカイテイオー （CV.Machico）"},
{"769", "Photon Maiden"},
{"770", "Lyrical Lily"},
{"771", "Tatsh x NAOKI"},
{"773", "Sampling Masters AYA"},
{"774", "NAOKI feat.小坂りゆ"},
{"775", "NAOKI underground"},
{"778", "柊マグネタイト"},
{"783", "和田アキ子"},
{"784", "Mastermind(xi+nora2r)"},
{"785", "x0o0x_"},
{"787", "Bitplane feat. 葉月ゆら"},
{"789", "linear ring"},
{"790", "ちいたな feat.flower"},
{"791", "作詞：セガ社員、作曲：若草恵、歌：135"},
};
        public static Dictionary<string, string> addVersionDic = new Dictionary<string, string>{
{"0", "maimai"},
{"1", "maimaiPLUS"},
{"2", "GreeN"},
{"3", "GreeNPLUS"},
{"4", "ORANGE"},
{"5", "ORANGEPLUS"},
{"6", "PiNK"},
{"7", "PiNKPLUS"},
{"8", "MURASAKi"},
{"9", "MURASAKiPLUS"},
{"10", "MiLK"},
{"11", "MiLKPLUS"},
{"12", "FiNALE"},
{"13", "maimaDX"},
{"14", "maimaDXPLUS"},
{"15", "Splash"},
{"16", "SplashPLUS"},
{"17", "UNiVERSE"},
{"18", "UNiVERSEPLUS"},
};
        public static Dictionary<string, string> eventNameDic = new Dictionary<string, string>{
{"1", "無期限常時解放"},
{"21091621", "210916_02_1"},
{"21111225", "211112_02_5"},
{"22011422", "220114_02_2"},
{"22030321", "220303_02_1"},
{"22032421", "220324_02_1"},
{"22040121", "220401_02_1"},
{"22040821", "220408_02_1"},
{"22041521", "220415_02_1"},
{"22041522", "220415_02_2"},
{"22042821", "220428_02_1"},
{"22042822", "220428_02_2"},
};
        public static Dictionary<string, string> subEventNameDic = new Dictionary<string, string>{
{"0", "解放なし"},
{"1", "無期限常時解放"},
{"22032421", "220324_02_1"},
};
        public static Dictionary<string, string> notesDesignerDic = new Dictionary<string, string>{
{"0", ""},
{"1", "mai-Star"},
{"2", "はっぴー"},
{"3", "某S氏"},
{"4", "Jack"},
{"5", "Techno Kitchen"},
{"6", "ロシェ@ペンギン"},
{"7", "rioN"},
{"8", "Revo@LC"},
{"9", "ぴちネコ"},
{"10", "チャン@DP皆伝"},
{"11", "譜面-100号"},
{"12", "ニャイン"},
{"13", "maimai TEAM"},
{"14", "合作だよ"},
{"15", "しろいろ"},
{"16", "畳返し"},
{"17", "如月 ゆかり"},
{"19", "原田ひろゆき"},
{"20", "Moon Strix"},
{"21", "玉子豆腐"},
{"22", "ものくろっく"},
{"23", "LabiLabi"},
{"24", "小鳥遊さん"},
{"25", "ミストルティン"},
{"26", "すきやき奉行"},
{"27", "サファ太"},
{"29", "緑風 犬三郎"},
{"31", "じゃこレモン"},
{"32", "華火職人"},
{"34", "譜面ボーイズからの挑戦状"},
{"35", "“H”ack"},
{"37", "Garakuta Scramble!"},
{"38", "“H”ack underground"},
{"39", "“Carpe diem” ＊ HAN?BI"},
{"40", "小鳥遊さん fused with Phoenix"},
{"41", "safaTAmago"},
{"42", "JAQ"},
{"43", "Phoenix"},
{"44", "-ZONE- SaFaRi"},
{"45", "PANDORA BOXXX"},
{"46", "PANDORA PARADOXXX"},
{"47", "シチミヘルツ"},
{"48", "うさぎランドリー"},
{"49", "7.3Hz＋Jack"},
{"50", "譜面-100号とはっぴー"},
{"51", "群青リコリス"},
{"52", "jacK on Phoenix"},
{"53", "KTM"},
{"54", "シチミッピー"},
{"55", "Safata.Hz"},
{"56", "隅田川星人"},
{"57", "ゲキ*チュウマイ Fumen Team"},
{"58", "Sukiyaki vs Happy"},
{"59", "7.3GHz vs Phoenix"},
{"60", "?よ?ょ／∪ヽ”┠  (十,3?了??"},
{"61", "譜面男子学院 中堅 小鳥 遊"},
{"62", "七味星人"},
{"63", "アマリリス"},
{"64", "しちみりこりす"},
{"65", "Anomaly Labyrinth"},
{"66", "ものくロシェ"},
{"67", "7.3GHz"},
{"68", "サファ太 vs -ZONE- SaFaRi"},
{"69", "The ALiEN"},
{"70", "さふぁた"},
{"71", "アミノハバキリ"},
{"72", "Redarrow"},
{"73", "みそかつ侍"},
{"74", "ロシアンブラック"},
{"75", "隅田川華火大会"},
{"76", "はっぴー respects for 某S氏"},
{"77", "7.3連発華火"},
{"78", "-ZONE-Phoenix"},
{"79", "小鳥遊さん vs 華火職人"},
{"80", "SHICHIMI☆CAT"},
{"81", "あまくちジンジャー"},
{"82", "あまくちジンジャー＠やべー新人"},
{"83", "The ALiEN vs. Phoenix"},
{"84", "翠楼屋"},
{"85", "Jack & Licorice Gunjyo"},
{"86", "7.3GHz -F?r The Legends-"},
{"87", "一ノ瀬 リズ"},
{"88", "超七味星人"},
{"89", "KOP3rd with 翡翠マナ"},
{"90", "小鳥遊チミ"},
{"91", "作譜：翠楼屋"},
{"92", "Hz-R.Arrow"},
{"93", "チェシャ猫とハートのジャック"},
{"999", "-"},
};


        /// <summary>
        /// Set of track information stored
        /// </summary>
        private Dictionary<string, string> information;

        /// <summary>
        /// Internal stored information set
        /// </summary>
        private XmlDocument takeInValue;

        /// <summary>
        /// Empty constructor
        /// </summary>
        public TrackInformation()
        {
            this.takeInValue = new XmlDocument();
            this.information = new Dictionary<string, string>();
            this.FormatInformation();
            this.Update();
        }

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
        /// Add in necessary nodes in information.
        /// </summary>
        public void FormatInformation()
        {
            this.information = new Dictionary<string, string>
                    {
                        { "Name", "" },
                        { "Sort Name", "" },
                        { "Music ID", "" },
                        { "Genre", "" },
                        { "Version", "" },
                        {"Version Number",""},
                        { "BPM", "" },
                        { "Composer", "" },
                        { "Easy", "" },
                        { "Easy Decimal","" },
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
                        { "Utage Chart Maker", "" },
                        { "Utage Chart Path", "" },
                        {"SDDX Suffix",""}
                    };
        }

        /// <summary>
        /// Add in necessary nodes in information for dummy chart.
        /// </summary>
        public void FormatDummyInformation()
        {
            this.information = new Dictionary<string, string>
                    {
                        { "Name", "Dummy" },
                        { "Sort Name", "DUMMY" },
                        { "Music ID", "000000" },
                        { "Genre", "maimai" },
                        { "Version", "maimai" },
                        {"Version Number","101"},
                        { "BPM", "120" },
                        { "Composer", "SEGA" },
                        { "Easy", "1" },
                        { "Easy Decimal","1.0" },
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
                        { "Utage Chart Maker", "SEGA" },
                        { "Utage Chart Path", "000000_05.ma2" },
                        {"SDDX Suffix","SD"}
                    };
        }

        /// <summary>
        /// Return the track name
        /// </summary>
        /// <value>this.TrackName</value>
        public string TrackName
        {
            get { return this.Information.GetValueOrDefault("Name") ?? throw new NullReferenceException("Name is not defined"); }
            set { this.information["Name"] = value; }
        }

        /// <summary>
        /// Return the sort name (basically English or Katakana)
        /// </summary>
        /// <value>this.SortName</value>
        public string TrackSortName
        {
            get { return this.Information.GetValueOrDefault("Sort Name") ?? throw new NullReferenceException("Sort Name is not defined"); }
            set { this.information["Sort Name"] = value; }
        }

        /// <summary>
        /// Return the track ID (4 digit, having 00 for SD 01 for DX)
        /// </summary>
        /// <value>this.TrackID</value>
        public string TrackID
        {
            get { return this.Information.GetValueOrDefault("Music ID") ?? throw new NullReferenceException("Music ID is not defined"); }
            set { this.information["Music ID"] = value; }
        }

        /// <summary>
        /// Return the track genre (By default cabinet 6 categories)
        /// </summary>
        /// <value>this.TrackGenre</value>
        public string TrackGenre
        {
            get { return this.Information.GetValueOrDefault("Genre") ?? throw new NullReferenceException("Genre is not defined"); }
            set { this.information["Genre"] = value; }
        }

        /// <summary>
        /// Return the track global BPM
        /// </summary>
        /// <value>this.TrackBPM</value>
        public string TrackBPM
        {
            get { return this.Information.GetValueOrDefault("BPM") ?? throw new NullReferenceException("Genre is not defined"); }
            set { this.information["BPM"] = value; }
        }

        /// <summary>
        /// Return the track composer
        /// </summary>
        /// <value>this.TrackComposer</value>
        public string TrackComposer
        {
            get { return this.Information.GetValueOrDefault("Composer") ?? throw new NullReferenceException("Genre is not defined"); }
            set { this.information["Composer"] = value; }
        }

        /// <summary>
        /// Return the most representative level of the track = set by default master
        /// </summary>
        /// <value>this.TrackLevel</value>
        public string TrackSymbolicLevel
        {
            get 
            {
                if (this.Information.TryGetValue("Utage", out string? utageLevel) && level != null && !utageLevel.Equals(""))
                {
                    return utageLevel;
                }
                else if (this.Information.TryGetValue("Remaster", out string? remasLevel) && remasLevel != null && !remasLevel.Equals(""))
                {
                    return remasLevel;
                }
                else if (this.Information.TryGetValue("Master", out string? masterLevel) && masterLevel != null && !masterLevel.Equals(""))
                {
                    return masterLevel;
                }
                else if (this.Information.TryGetValue("Expert", out string? expertLevel) && expertLevel != null && !expertLevel.Equals(""))
                {
                    return expertLevel;
                }
                else if (this.Information.TryGetValue("Advanced", out string? advanceLevel) && advanceLevel != null && !advanceLevel.Equals(""))
                {
                    return advanceLevel;
                }
                else if (this.Information.TryGetValue("Basic", out string? basicLevel) && basicLevel != null && !basicLevel.Equals(""))
                {
                    return basicLevel;
                }
                else if (this.Information.TryGetValue("Easy", out string? easyLevel) && easyLevel != null && !easyLevel.Equals(""))
                {
                    return easyLevel;
                }
                else return "ORIGINAL";
            }
            set 
            { 
                this.information["Master"] = value; 
            }
        }

        /// <summary>
        /// Return track levels by [Easy, Basic, Advance, Expert, Master, Remaster, Utage]
        /// </summary>
        public string[] TrackLevels
        {
            get
            {
                string[] result = { this.Information["Easy"],
                    this.Information["Basic"],
                    this.Information["Advanced"],
                    this.Information["Expert"],
                    this.Information["Master"],
                    this.Information["Remaster"],
                    this.Information["Utage"] };
                return result;
            }
            set
            {
                this.Information["Easy"] = value[0];
                this.Information["Basic"] = value[1];
                this.Information["Advanced"] = value[2];
                this.Information["Expert"] = value[3];
                this.Information["Master"] = value[4];
                this.Information["Remaster"] = value[5];
                this.Information["Utage"] = value[6];
            }
        }

        /// <summary>
        /// Return track decimal levels by [Easy, Basic, Advance, Expert, Master, Remaster, Utage] * Utage returns utage level
        /// </summary>
        public string[] TrackDecimalLevels
        {
            get
            {
                string[] result = { this.Information["Easy Decimal"],
                    this.Information["Basic Decimal"],
                    this.Information["Advanced Decimal"],
                    this.Information["Expert Decimal"],
                    this.Information["Master Decimal"],
                    this.Information["Remaster Decimal"],
                    this.Information["Utage"] };
                return result;
            }
            set
            {
                this.Information["Easy Decimal"] = value[0];
                this.Information["Basic Decimal"] = value[1];
                this.Information["Advanced Decimal"] = value[2];
                this.Information["Expert Decimal"] = value[3];
                this.Information["Master Decimal"] = value[4];
                this.Information["Remaster Decimal"] = value[5];
                this.Information["Utage"] = value[6];
            }
        }

        /// <summary>
        /// Return the suffix of Track title for export
        /// </summary>
        /// <value>this.TrackSubstituteName"_DX" if is DX chart</value>
        public string DXChartTrackPathSuffix
        {
            get
            {
                string musicID = this.Information.GetValueOrDefault("Music ID") ?? throw new NullReferenceException("Music ID is not Defined");
                if (musicID.Length > 4)
                    return "_DX";
                else return "";
            }
        }

        /// <summary>
        /// Returns if the track is Standard or Deluxe
        /// </summary>
        /// <value>SD if standard, DX if deluxe</value>
        public string StandardDeluxePrefix
        {
            get
            {
                string musicID = this.Information.GetValueOrDefault("Music ID") ?? throw new NullReferenceException("Music ID is not Defined");
                if (musicID.Length > 4)
                    return "DX";
                else return "SD";
            }
        }

        /// <summary>
        /// Title suffix for better distinguish
        /// </summary>
        /// <value>[SD] if Standard and [DX] if Deluxe</value>
        public string StandardDeluxeSuffix
        {
            get
            {
                return "[" + this.StandardDeluxePrefix + "]";
            }
        }

        /// <summary>
        /// See if the chart is DX chart.
        /// </summary>
        /// <value>True if is DX, false if SD</value>
        public bool IsDXChart
        {
            get
            {
                string musicID = this.Information.GetValueOrDefault("Music ID") ?? throw new NullReferenceException("Music ID is not Defined");
                return musicID.Length > 4;
            }
        }

        /// <summary>
        /// Return the XML node that has same name with
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlNodeList GetMatchNodes(string name)
        {
            XmlNodeList result = this.takeInValue.GetElementsByTagName(name);
            return result;
        }

        /// <summary>
        /// Return this.TrackVersion
        /// </summary>
        /// <value>this.TrackVersion</value>
        public string TrackVersion
        {

            get
            {
                string version = this.Information.GetValueOrDefault("Version") ?? throw new NullReferenceException("Version is not Defined");
                return version;
            }
            set { this.information["Version"] = value; }
        }

        /// <summary>
        /// Return this.TrackVersionNumber
        /// </summary>
        /// <value>this.TrackVersionNumber</value>
        public string TrackVersionNumber
        {

            get
            {
                string versionNumber = this.Information.GetValueOrDefault("Version Number") ?? throw new NullReferenceException("Version is not Defined");
                return versionNumber;
            }
            set { this.information["Version Number"] = value; }
        }

        /// <summary>
        /// Give access to TakeInValue if necessary
        /// </summary>
        /// <value>this.TakeInValue as XMLDocument</value>
        public XmlDocument TakeInValue
        {
            get { return takeInValue; }
            set { this.takeInValue = value; }
        }

        /// <summary>
        /// Give access to this.Information
        /// </summary>
        /// <value>this.Information as Dictionary</value>
        public Dictionary<string, string> Information
        {
            get { return information; }
            set { this.information = value; }
        }

        /// <summary>
        /// Save the information to given path
        /// </summary>
        /// <param name="location">Path to save the information</param>
        public void Save(string location)
        {
            this.takeInValue.Save(location);
        }

        /// <summary>
        /// Update information
        /// </summary>
        public abstract void Update();

        /// <summary>
        /// Compensate 0 for music IDs
        /// </summary>
        /// <param name="intake">Music ID</param>
        /// <returns>0..+#Music ID and |Music ID|==6</returns>
        public static string CompensateZero(string intake)
        {
            try
            {
                string result = intake;
                while (result.Length < 6 && intake != null)
                {
                    result = "0" + result;
                }
                return result;
            }
            catch (NullReferenceException ex)
            {
                return "Exception raised: " + ex.Message;
            }
        }

        /// <summary>
        /// Compensate 0 for short music IDs
        /// </summary>
        /// <param name="intake">Music ID</param>
        /// <returns>0..+#Music ID and |Music ID|==4</returns>
        public static string CompensateShortZero(string intake)
        {
            try
            {
                string result = intake;
                while (result.Length < 4 && intake != null)
                {
                    result = "0" + result;
                }
                return result;
            }
            catch (NullReferenceException ex)
            {
                return "Exception raised: " + ex.Message;
            }
        }
    }
}