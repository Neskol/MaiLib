using System.ComponentModel;
namespace MaiLib;

/// <summary>
/// Holds enums of the Note
/// </summary>
public class NoteEnum
{
    /// <summary>
    /// Flags the special state of each note
    /// </summary>
    public enum SpecialState
    {
        [Description("Normal note, nothing special")]
        Normal,

        [Description("Break note")]
        Break,

        [Description("EX Note")]
        EX,

        [Description("EX Break")]
        BreakEX,

        [Description("Connecting Slide")]
        ConnectingSlide
    }

    /// <summary>
    /// Defines the general category of notes
    /// </summary>
    public enum NoteGenre
    {
        REST,
        TAP,
        HOLD,
        SLIDE,
        BPM,
        MEASURE
    }

    /// <summary>
    /// Defines the specific genre of notes
    /// </summary>
    public enum NoteSpecificGenre
    {
        REST,
        TAP,
        SLIDE_START,
        HOLD,
        SLIDE,
        SLIDE_EACH,
        SLIDE_GROUP,
        BPM,
        MEASURE
    }

    /// <summary>
    /// Defines the possible Note Type
    /// </summary>
    public enum NoteType
    {
        // Dummy Rest Note
        [Description("Rest Note")]
        RST,

        // TAP Enums
        [Description("Normal Tap Note")]
        TAP,
        [Description("Start of a Slide as Tap")]
        STR,
        [Description("Imaginary Slide Start that does not appear in a slide")]
        NST,
        [Description("Singular Sldie Start but does not have consecutive slide")]
        NSS,
        [Description("Touch Tap")]
        TTP,

        // Hold Enums
        [Description("Normal Hold")]
        HLD,
        [Description("EX Hold")]
        XHO,
        [Description("Touch Hold")]
        THO,

        // Slide Enums
        [Description("Straight Star")]
        SI_,
        [Description("Circular Star Left")]
        SCL,
        [Description("Circular Star Right")]
        SCR,
        [Description("Line not intercepting Crossing Center")]
        SV_,
        [Description("Line not intercepting Crossing Center")]
        SUL,
        [Description("Line not intercepting Crossing Center")]
        SUR,
        [Description("WIFI Star")]
        SF_,
        [Description("Inflecting Line Left")]
        SLL,
        [Description("Inflecting Line Right")]
        SLR,
        [Description("Self-winding Left")]
        SXL,
        [Description("Self-winding Right")]
        SXR,
        [Description("Line not intercepting Crossing Center")]
        SSL,
        [Description("Line not intercepting Crossing Center")]
        SSR,
        [Description("Composite Slide Each")]
        SLIDE_EACH,

        //Function Notes
        BPM,
        MEASURE
    }


    /// <summary>
    /// Defines the possible flip methods of notes
    /// </summary>
    public enum FlipMethod
    {
        UpSideDown,
        Clockwise90,
        Clockwise180,
        Counterclockwise90,
        Counterclockwise180,
        LeftToRight
    }
}
