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
        /// <summary>
        ///     Normal note, nothing special
        /// </summary>
        Normal,

        /// <summary>
        ///     Break note
        /// </summary>
        Break,

        /// <summary>
        ///     EX Note
        /// </summary>
        EX,

        /// <summary>
        ///     EX Break
        /// </summary>
        BreakEX,

        /// <summary>
        ///     Connecting Slide
        /// </summary>
        ConnectingSlide
    }

    public enum NoteType
    {
        // TAP Enums
        [Description("Normal Tap Note")]
        TAP,

        [Description("Start of a Slide as Tap")]
        STR,

        [Description("Slide start but has no consecutive slides follows it")]
        NST,

        [Description("Touch Tap")]
        TTP,

        // Hold Enums
        [Description("Normal Hold")]
        HLD,

        [Description("Touch Hold")]
        THO
        // Slide Enums
    }
}
