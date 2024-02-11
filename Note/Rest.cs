namespace MaiLib;

using static NoteEnum;
using static ChartEnum;

/// <summary>
///     Construct Rest Note solely for Simai
/// </summary>
public class Rest : Note
{
    #region Constructors

    /// <summary>
    ///     Construct empty
    /// </summary>
    public Rest()
    {
        NoteType = NoteType.RST;
        Bar = 0;
        Tick = 0;
        Update();
    }

    /// <summary>
    ///     Construct Rest Note with given information
    /// </summary>
    /// <param name="bar">Bar to take in</param>
    /// <param name="startTime">Start to take in</param>
    public Rest(int bar, int startTime)
    {
        NoteType = NoteType.RST;
        Bar = bar;
        Tick = startTime;
        Update();
    }

    /// <summary>
    ///     Construct with Note provided
    /// </summary>
    /// <param name="n">Note to take in</param>
    public Rest(Note n)
    {
        NoteType = NoteType.RST;
        Bar = n.Bar;
        Tick = n.Tick;
        BPMChangeNotes = n.BPMChangeNotes;
        Update();
    }

    #endregion

    public override NoteGenre NoteGenre => NoteGenre.REST;

    public override bool IsNote => false;

    public override NoteSpecificGenre NoteSpecificGenre => NoteSpecificGenre.REST;

    public override bool CheckValidity()
    {
        throw new NotImplementedException();
    }

    public override string Compose(ChartVersion format)
    {
        // return "r_" + this.Tick;
        switch (format)
        {
            case ChartVersion.Debug:
                return "r_" + Tick;
            default:
                return "";
        }
    }

    public override Note NewInstance()
    {
        Note result = new Rest(this);
        return result;
    }
}