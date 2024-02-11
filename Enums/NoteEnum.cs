using System.ComponentModel;

namespace MaiLib;

/// <summary>
///     Holds enums of the Note
/// </summary>
public class NoteEnum
{
    /// <summary>
    ///     Flags the special state of each note
    /// </summary>
    public enum SpecialState
    {
        /// <value>Normal note, nothing special</value>
        Normal,

        /// <value>Break note</value>
        Break,

        /// <value>EX Note</value>
        EX,

        /// <value>EX Break</value>
        BreakEX,

        /// <value>Connecting Slide</value>
        ConnectingSlide
    }

    /// <summary>
    ///     Defines the general category of notes
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
    ///     Defines the specific genre of notes
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
    ///     Defines the possible Note Type
    /// </summary>
    public enum NoteType
    {
        // Dummy Rest Note
        /// <value>Rest Note</value>
        RST,

        // TAP Enums
        /// <value>Normal Tap Note</value>
        TAP,

        /// <value>Start of a Slide as Tap</value>
        STR,

        /// <value>Imaginary Slide Start that does not appear in a slide</value>
        NST,

        /// <value>Singular Sldie Start but does not have consecutive slide</value>
        NSS,

        /// <value>Touch Tap</value>
        TTP,

        // Hold Enums
        /// <value>Normal Hold</value>
        HLD,

        /// <value>Touch Hold</value>
        THO,

        // Slide Enums
        /// <value>Straight Star</value>
        SI_,

        /// <value>Circular Star Left</value>
        SCL,

        /// <value>Circular Star Right</value>
        SCR,

        /// <value>Line not intercepting Crossing Center</value>
        SV_,

        /// <value>Line not intercepting Crossing Center</value>
        SUL,

        /// <value>Line not intercepting Crossing Center</value>
        SUR,

        /// <value>WIFI Star</value>
        SF_,

        /// <value>Inflecting Line Left</value>
        SLL,

        /// <value>Inflecting Line Right</value>
        SLR,

        /// <value>Self-winding Left</value>
        SXL,

        /// <value>Self-winding Right</value>
        SXR,

        /// <value>Line not intercepting Crossing Center</value>
        SSL,

        /// <value>Line not intercepting Crossing Center</value>
        SSR,

        /// <value>Composite Slide Each</value>
        SLIDE_EACH,

        //Function Notes
        BPM,
        MEASURE
    }


    /// <summary>
    ///     Defines the possible flip methods of notes
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