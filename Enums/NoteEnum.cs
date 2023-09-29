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
    /// Defines the possible Note Type
    /// </summary>
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
        SSR
    }
}
