namespace MaiLib;

using static MaiLib.NoteEnum;

/// <summary>
///     Give enums of parameters of Standard Keys
/// </summary>
public enum StdParam
{
    Type,
    Bar,
    Tick,
    Key,
    WaitTime,
    LastTime,
    EndKey
}

/// <summary>
///     Give enums of parameters of Deluxe Tap/Slide Keys
/// </summary>
public enum DxTapParam
{
    Type,
    Bar,
    Tick,
    Key,
    KeyGroup,
    SpecialEffect,
    NoteSize
}

/// <summary>
///     Give enums of parameters of Deluxe Hold Keys
/// </summary>
public enum DxHoldParam
{
    Type,
    Bar,
    Tick,
    Key,
    LastTime,
    KeyGroup,
    SpecialEffect,
    NoteSize
}

/// <summary>
///     Parses ma2 file into Ma2 chart format
/// </summary>
public class Ma2Parser : IParser
{
    private Tap PreviousSlideStart;

    /// <summary>
    ///     Empty constructor
    /// </summary>
    public Ma2Parser()
    {
        PreviousSlideStart = new Tap();
    }

    public Chart ChartOfToken(string[] token)
    {
        BPMChanges? bpmChanges = new BPMChanges();
        MeasureChanges? measureChanges = new MeasureChanges();
        List<Note>? notes = new List<Note>();
        if (token != null)
            foreach (string? x in token)
            {
                string? typeCandidate = x.Split('\t')[(int)StdParam.Type];
                bool isBPM_DEF = typeCandidate.Equals("BPM_DEF");
                bool isMET_DEF = typeCandidate.Equals("MET_DEF");
                bool isBPM = typeCandidate.Equals("BPM");
                bool isMET = typeCandidate.Equals("MET");
                bool isNOTE = typeCandidate.Equals("TAP")
                              || typeCandidate.Equals("STR")
                              || typeCandidate.Equals("TTP")
                              || typeCandidate.Equals("XTP")
                              || typeCandidate.Equals("XST")
                              || typeCandidate.Equals("BRK")
                              || typeCandidate.Equals("BST")
                              || typeCandidate.Equals("HLD")
                              || typeCandidate.Equals("XHO")
                              || typeCandidate.Equals("THO")
                              || typeCandidate.Equals("SI_")
                              || typeCandidate.Equals("SV_")
                              || typeCandidate.Equals("SF_")
                              || typeCandidate.Equals("SCL")
                              || typeCandidate.Equals("SCR")
                              || typeCandidate.Equals("SUL")
                              || typeCandidate.Equals("SUR")
                              || typeCandidate.Equals("SLL")
                              || typeCandidate.Equals("SLR")
                              || typeCandidate.Equals("SXL")
                              || typeCandidate.Equals("SXR")
                              || typeCandidate.Equals("SSL")
                              || typeCandidate.Equals("SSR")
                              || (typeCandidate.Contains("NM") && typeCandidate.Length == 5)
                              || (typeCandidate.Contains("CN") && typeCandidate.Length == 5)
                              || (typeCandidate.Contains("EX") && typeCandidate.Length == 5)
                              || (typeCandidate.Contains("BR") && typeCandidate.Length == 5)
                              || (typeCandidate.Contains("BX") && typeCandidate.Length == 5);

                if (isBPM_DEF)
                {
                    bpmChanges = BPMChangesOfToken(x);
                }
                else if (isMET_DEF)
                {
                    measureChanges = MeasureChangesOfToken(x);
                }
                else if (isBPM)
                {
                    string[]? bpmCandidate = x.Split('\t');
                    BPMChange? candidate = new BPMChange(int.Parse(bpmCandidate[1]),
                        int.Parse(bpmCandidate[2]),
                        double.Parse(bpmCandidate[3]));
                    // foreach (BPMChange change in bpmChanges.ChangeNotes)
                    // {
                    //     if (change.TickStamp <= candidate.LastTickStamp)
                    //     {
                    //         candidate.BPMChangeNotes.Add(change);
                    //         Console.WriteLine("A BPM change note was added with overall tick of "+change.TickStamp + " with bpm of "+change.BPM);
                    //     }
                    // }
                    bpmChanges.Add(candidate);
                    bpmChanges.Update();
                }
                else if (isMET)
                {
                    string[]? measureCandidate = x.Split('\t');
                    measureChanges.Add(int.Parse(measureCandidate[(int)StdParam.Bar]),
                        int.Parse(measureCandidate[(int)StdParam.Tick]),
                        int.Parse(measureCandidate[(int)StdParam.Key]),
                        int.Parse(measureCandidate[(int)StdParam.WaitTime]));
                }
                else if (isNOTE)
                {
                    Note? candidate = NoteOfToken(x);
                    // foreach (BPMChange change in bpmChanges.ChangeNotes)
                    // {
                    //     if (change.TickStamp <= candidate.LastTickStamp)
                    //     {
                    //         candidate.BPMChangeNotes.Add(change);
                    //         Console.WriteLine("A BPM change note was added with overall tick of " + change.TickStamp + " with bpm of " + change.BPM);
                    //     }
                    // }
                    notes.Add(candidate);
                }
            }

        foreach (Note? note in notes)
        {
            note.BPMChangeNotes = bpmChanges.ChangeNotes;
            if (bpmChanges.ChangeNotes.Count > 0 && note.BPMChangeNotes.Count == 0)
                throw new IndexOutOfRangeException("BPM COUNT DISAGREE");
            if (bpmChanges.ChangeNotes.Count == 0) throw new IndexOutOfRangeException("BPM CHANGE COUNT DISAGREE");
        }

        Chart result = new Ma2(notes, bpmChanges, measureChanges);
        return result;
    }

    public BPMChanges BPMChangesOfToken(string token)
    {
        return new BPMChanges();
    }

    public MeasureChanges MeasureChangesOfToken(string token)
    {
        return new MeasureChanges(int.Parse(token.Split('\t')[1]), int.Parse(token.Split('\t')[2]));
    }

    public Note NoteOfToken(string token)
    {
        string[]? candidate = token.Split('\t');
        int bar = int.Parse(candidate[(int)StdParam.Bar]);
        int tick = int.Parse(candidate[(int)StdParam.Tick]);
        return NoteOfToken(token, bar, tick, 0.0);
    }

    public Note NoteOfToken(string token, int bar, int tick, double bpm)
    {
        Note result = new Rest();
        string noteTypeCandidate = token.Split('\t')[(int)StdParam.Type];
        string fesNoteState = "";
        if (noteTypeCandidate.Length > 3)
        {
            fesNoteState = noteTypeCandidate.Substring(0, 2); // First 2 characters are note state
            noteTypeCandidate = noteTypeCandidate.Substring(2);
            token = token.Substring(2);
        }

        bool isTap = noteTypeCandidate.Contains("TAP")
                     || noteTypeCandidate.Contains("STR")
                     || noteTypeCandidate.Contains("TTP")
                     || noteTypeCandidate.Equals("XTP")
                     || noteTypeCandidate.Equals("XST")
                     || noteTypeCandidate.Equals("BRK")
                     || noteTypeCandidate.Equals("BST");
        bool isHold = noteTypeCandidate.Contains("HLD")
                      || noteTypeCandidate.Equals("XHO")
                      || noteTypeCandidate.Contains("THO");
        bool isSlide = noteTypeCandidate.Contains("SI_")
                       || noteTypeCandidate.Contains("SV_")
                       || noteTypeCandidate.Contains("SF_")
                       || noteTypeCandidate.Contains("SCL")
                       || noteTypeCandidate.Contains("SCR")
                       || noteTypeCandidate.Contains("SUL")
                       || noteTypeCandidate.Contains("SUR")
                       || noteTypeCandidate.Contains("SLL")
                       || noteTypeCandidate.Contains("SLR")
                       || noteTypeCandidate.Contains("SXL")
                       || noteTypeCandidate.Contains("SXR")
                       || noteTypeCandidate.Contains("SSL")
                       || noteTypeCandidate.Contains("SSR");
        if (isTap)
            result = TapOfToken(token);
        else if (isHold)
            result = HoldOfToken(token);
        else if (isSlide) result = SlideOfToken(token);
        // result.SlideStart = PreviousSlideStart;
        if (result.Tick == 384)
        {
            result.Tick = 0;
            result.Bar++;
        }

        if (!fesNoteState.Equals(""))
        {
            // bool noteTypeIsValid = Enum.TryParse(noteTypeCandidate, out NoteType noteTypeEnum);
            // if (noteTypeIsValid)
            // {
            //     result.NoteType = noteTypeEnum;
            // }
            // else
            // {
            //     throw new Exception("Given note type is invalid. Note Type provided: "+noteTypeCandidate);
            // }
            switch (fesNoteState)
            {
                case "BR":
                    result.NoteSpecialState = SpecialState.Break;
                    break;
                case "EX":
                    result.NoteSpecialState = SpecialState.EX;
                    break;
                case "BX":
                    result.NoteSpecialState = SpecialState.BreakEX;
                    break;
                case "CN":
                    result.NoteSpecialState = SpecialState.ConnectingSlide;
                    break;
                //NM does not need extra case
            }
        }

        if (bpm > 0.0) result.BPM = bpm;
        if (result.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START) PreviousSlideStart = (Tap)result;
        return result;
    }

    public Hold HoldOfToken(string token, int bar, int tick, double bpm)
    {
        Note result = new Rest();
        string[]? candidate = token.Split('\t');
        SpecialState specialState = SpecialState.Normal;
        switch (candidate[(int)DxTapParam.Type])
        {
            case "XHO":
                candidate[(int)DxTapParam.Type] = "HLD";
                specialState = SpecialState.EX;
                break;
        }

        bool noteTypeIsValid = Enum.TryParse(candidate[(int)DxTapParam.Type], out NoteType typeCandidate);
        if (!noteTypeIsValid)
            throw new Exception("The given note type is invalid. Type provided: " + candidate[(int)DxTapParam.Type]);
        if (candidate[(int)DxTapParam.Type].Contains("THO")) //Basically all THO falls in this line
        {
            string? noteSize = candidate.Count() > 7 ? candidate[(int)DxHoldParam.NoteSize] : "M1";
            bool specialEffect = int.Parse(candidate[(int)DxHoldParam.SpecialEffect]) == 1;
            result = new Hold(typeCandidate,
                bar,
                tick,
                candidate[(int)DxHoldParam.Key] + candidate[(int)DxHoldParam.KeyGroup],
                int.Parse(candidate[(int)DxHoldParam.LastTime]),
                specialEffect,
                noteSize);
        }
        else
        {
            result = new Hold(typeCandidate,
                int.Parse(candidate[(int)StdParam.Bar]),
                int.Parse(candidate[(int)StdParam.Tick]),
                candidate[(int)StdParam.Key],
                int.Parse(candidate[(int)StdParam.WaitTime]));
        }

        if (bpm > 0.0) result.BPM = bpm;
        result.NoteSpecialState = specialState;
        return (Hold)result;
    }

    public Hold HoldOfToken(string token)
    {
        string[]? candidate = token.Split('\t');
        int bar = int.Parse(candidate[(int)StdParam.Bar]);
        int tick = int.Parse(candidate[(int)StdParam.Tick]);
        return HoldOfToken(token, bar, tick, 0.0);
    }

    public Slide SlideOfToken(string token, int bar, int tick, Note slideStart, double bpm)
    {
        string[]? candidate = token.Split('\t');
        bool noteTypeIsValid = Enum.TryParse(candidate[(int)DxTapParam.Type], out NoteType typeCandidate);
        if (!noteTypeIsValid)
            throw new Exception("Given Note Type is not valid. Given: " + candidate[(int)DxTapParam.Type]);
        if (!slideStart.Key.Equals(candidate[(int)StdParam.Key]) || slideStart.Bar != bar || slideStart.Tick != tick)
            //Console.WriteLine("Expected key: " + candidate[(int)StdParam.KeyOrParam]);
            //Console.WriteLine("Actual key: " + PreviousSlideStart.Key);
            //Console.WriteLine("Previous Slide Start: " + PreviousSlideStart.Compose((int)StdParam.Bar));
            //throw new Exception("THE SLIDE START DOES NOT MATCH WITH THE DEFINITION OF THIS NOTE!");
            PreviousSlideStart = new Tap(NoteType.NST, bar, tick, candidate[(int)StdParam.Key]);
        Slide? result = new Slide(typeCandidate,
            bar,
            tick,
            slideStart.Key,
            int.Parse(candidate[(int)StdParam.WaitTime]),
            int.Parse(candidate[(int)StdParam.LastTime]),
            candidate[(int)StdParam.EndKey]);
        if (bpm > 0.0) result.BPM = bpm;
        return result;
    }

    public Slide SlideOfToken(string token)
    {
        string[]? candidate = token.Split('\t');
        int bar = int.Parse(candidate[(int)StdParam.Bar]);
        int tick = int.Parse(candidate[(int)StdParam.Tick]);
        if (!PreviousSlideStart.Key.Equals(candidate[(int)StdParam.Key]) || PreviousSlideStart.Bar != bar ||
            PreviousSlideStart.Tick != tick)
            //Console.WriteLine("Expected key: " + candidate[(int)StdParam.Key]);
            //Console.WriteLine("Actual key: " + PreviousSlideStart.Key);
            //Console.WriteLine("Previous Slide Start: " + PreviousSlideStart.Compose((int)StdParam.Bar));
            //throw new Exception("THE SLIDE START DOES NOT MATCH WITH THE DEFINITION OF THIS NOTE!");
            PreviousSlideStart = new Tap(NoteType.NST, bar, tick, candidate[(int)StdParam.Key]);
        return SlideOfToken(token, bar, tick, PreviousSlideStart, 0.0);
    }


    public Tap TapOfToken(string token, int bar, int tick, double bpm)
    {
        Note result = new Rest();
        string[]? candidate = token.Split('\t');
        // Resolves 1.03 to 1.04 issue
        SpecialState specialState = SpecialState.Normal;
        switch (candidate[(int)DxTapParam.Type])
        {
            case "XST":
                candidate[(int)DxTapParam.Type] = "STR";
                specialState = SpecialState.EX;
                break;
            case "XTP":
                candidate[(int)DxTapParam.Type] = "TAP";
                specialState = SpecialState.EX;
                break;
            case "BST":
                candidate[(int)DxTapParam.Type] = "STR";
                specialState = SpecialState.Break;
                break;
            case "BRK":
                candidate[(int)DxTapParam.Type] = "TAP";
                specialState = SpecialState.Break;
                break;
        }

        bool noteTypeIsValid = Enum.TryParse(candidate[(int)DxTapParam.Type], out NoteType typeCandidate);
        if (!noteTypeIsValid)
            throw new Exception("Given Note Type is not valid. Given: " + candidate[(int)DxTapParam.Type]);
        if (candidate[(int)StdParam.Type].Contains("TTP"))
        {
            string? noteSize = candidate.Length > 7 ? candidate[7] : "M1";
            bool specialEffect = int.Parse(candidate[(int)DxTapParam.SpecialEffect]) == 1;
            result = new Tap(typeCandidate,
                bar,
                tick,
                candidate[(int)DxTapParam.Key] + candidate[(int)DxTapParam.KeyGroup],
                specialEffect,
                noteSize);
        }
        else
        {
            result = new Tap(typeCandidate,
                int.Parse(candidate[(int)StdParam.Bar]),
                int.Parse(candidate[(int)StdParam.Tick]),
                candidate[(int)StdParam.Key]);
        }

        result.NoteSpecialState = specialState;
        return (Tap)result;
    }

    public Tap TapOfToken(string token)
    {
        string[]? candidate = token.Split('\t');
        int bar = int.Parse(candidate[(int)StdParam.Bar]);
        int tick = int.Parse(candidate[(int)StdParam.Tick]);
        return TapOfToken(token, bar, tick, 0.0);
    }
}