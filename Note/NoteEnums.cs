namespace MaiLib;

public enum NoteGeneralCategories
{
    Tap, Hold, Slide, BPM
}

public enum NoteSpecificCategories
{
    Tap, TapStart, TapTouch, Hold, HoldTouch, Slide, BPM
}

public enum SpecialState
{
    Normal,
    Break,
    EX,
    BreakEX,
    ConnectingSlide,
    Fireworks,
    SingularSlide,
    SingularStart
}