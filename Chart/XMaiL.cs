using System.Xml;

namespace MaiLib;

/// <summary>
///     Using xml to store maicharts
/// </summary>
public class XMaiL : Chart, ICompiler
{
    /// <summary>
    ///     Storage of Xml file
    /// </summary>
    private readonly XmlDocument StoredXMailL;

    #region Constructors

    /// <summary>
    ///     Default constructor
    /// </summary>
    public XMaiL()
    {
        Notes = [];
        BPMChanges = new BPMChanges();
        MeasureChanges = new MeasureChanges();
        StoredChart = [];
        Information = [];
        StoredXMailL = new XmlDocument();
        Update();
    }

    /// <summary>
    ///     Construct XMaiL with given notes, bpm change definitions and measure change definitions.
    /// </summary>
    /// <param name="notes">Notes in XMaiL</param>
    /// <param name="bpmChanges">BPM Changes: Initial BPM is NEEDED!</param>
    /// <param name="measureChanges">Measure Changes: Initial Measure is NEEDED!</param>
    public XMaiL(List<Note> notes, BPMChanges bpmChanges, MeasureChanges measureChanges)
    {
        Notes = notes;
        BPMChanges = bpmChanges;
        MeasureChanges = measureChanges;
        StoredChart = [];
        Information = [];
        StoredXMailL = new XmlDocument();
        Update();
    }

    /// <summary>
    ///     Construct XMaiL with tokens given
    /// </summary>
    /// <param name="tokens">Tokens given</param>
    public XMaiL(string[] tokens)
    {
        Chart? takenIn = new Ma2Parser().ChartOfToken(tokens);
        Notes = takenIn.Notes;
        BPMChanges = takenIn.BPMChanges;
        MeasureChanges = takenIn.MeasureChanges;
        StoredChart = [];
        Information = [];
        StoredXMailL = new XmlDocument();
        Update();
    }

    /// <summary>
    ///     Construct XMaiL with existing values
    /// </summary>
    /// <param name="takenIn">Existing good brother</param>
    public XMaiL(Chart takenIn)
    {
        Notes = takenIn.Notes;
        BPMChanges = takenIn.BPMChanges;
        MeasureChanges = takenIn.MeasureChanges;
        StoredChart = [];
        Information = [];
        StoredXMailL = new XmlDocument();
        Update();
    }

    #endregion

    public override bool CheckValidity()
    {
        bool result = this == null;
        // Not yet implemented
        return result;
    }

    public override string Compose()
    {
        return StoredXMailL.ToString() ?? throw new NullReferenceException();
    }

    public override string Compose(BPMChanges bpm, MeasureChanges measure)
    {
        throw new NotImplementedException();
    }

    public override void Update()
    {
        XmlDeclaration? xmlDecl = StoredXMailL.CreateXmlDeclaration("1.0", "UTF-8", null);
        StoredXMailL.AppendChild(xmlDecl);
        XmlElement? root = StoredXMailL.CreateElement("XMaiL");
        XmlAttribute? rootVersion = StoredXMailL.CreateAttribute("1.0");
        root.Attributes.Append(rootVersion);
        StoredXMailL.AppendChild(root);
        XmlElement? information = StoredXMailL.CreateElement("TrackInformation");
    }
}
