namespace MaiLib;

public class SimaiParserR : IParser
{
    public int Resolution { get; private set; }
    public int BarNum { get; private set; }
    public int TickNum { get; private set; }
    public SimaiScanner Scanner { get; private set; }

    public SimaiParserR()
    {
        Scanner = new SimaiScanner();
        Resolution = 384;
    }

    public SimaiParserR(SimaiScanner scanner)
    {
        Scanner = scanner;
        Resolution = 384;
    }

    public Chart ChartOfToken(string[] token)
    {
        throw new NotImplementedException();
    }

    public BPMChanges BPMChangesOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public MeasureChanges MeasureChangesOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Note NoteOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Note NoteOfToken(string token, int bar, int tick, double bpm)
    {
        throw new NotImplementedException();
    }

    public Tap TapOfToken(string token, int bar, int tick, double bpm)
    {
        throw new NotImplementedException();
    }

    public Tap TapOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Hold HoldOfToken(string token, int bar, int tick, double bpm)
    {
        throw new NotImplementedException();
    }

    public Hold HoldOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Slide SlideOfToken(string token, int bar, int tick, Note slideStart, double bpm)
    {
        throw new NotImplementedException();
    }

    public Slide SlideOfToken(string token)
    {
        throw new NotImplementedException();
    }
}