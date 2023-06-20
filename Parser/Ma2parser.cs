using System.Runtime.CompilerServices;

namespace MaiLib
{
    /// <summary>
    /// Give enums of parameters of Standard Keys
    /// </summary>
    public enum StdParam { Type, Bar, Tick, Key, WaitTime, LastTime, EndKey };
    /// <summary>
    /// Give enums of parameters of Deluxe Tap/Slide Keys
    /// </summary>
    public enum DxTapParam { Type, Bar, Tick, Key, KeyGroup, SpecialEffect, NoteSize };
    /// <summary>
    /// Give enums of parameters of Deluxe Hold Keys
    /// </summary>
    public enum DxHoldParam { Type, Bar, Tick, Key, LastTime, KeyGroup, SpecialEffect, NoteSize };
    /// <summary>
    /// Parses ma2 file into Ma2 chart format
    /// </summary>
    public class Ma2Parser : IParser
    {
        private Tap PreviousSlideStart;
        /// <summary>
        /// Empty constructor
        /// </summary>
        public Ma2Parser()
        {
            PreviousSlideStart = new Tap();
        }

        public Chart ChartOfToken(string[] token)
        {
            BPMChanges bpmChanges = new BPMChanges();
            MeasureChanges measureChanges = new MeasureChanges();
            List<Note> notes = new List<Note>();
            if (token != null)
            {
                foreach (string x in token)
                {
                    string typeCandidate = x.Split('\t')[(int)StdParam.Type];
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
                        string[] bpmCandidate = x.Split('\t');
                        BPMChange candidate = new BPMChange(Int32.Parse(bpmCandidate[1]),
                            Int32.Parse(bpmCandidate[2]),
                            Double.Parse(bpmCandidate[3]));
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
                        string[] measureCandidate = x.Split('\t');
                        measureChanges.Add(Int32.Parse(measureCandidate[(int)StdParam.Bar]),
                            Int32.Parse(measureCandidate[(int)StdParam.Tick]),
                            Int32.Parse(measureCandidate[(int)StdParam.Key]),
                            Int32.Parse(measureCandidate[(int)StdParam.WaitTime]));
                    }
                    else if (isNOTE)
                    {
                        Note candidate = NoteOfToken(x);
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
            }
            foreach (Note note in notes)
            {
                note.BPMChangeNotes = bpmChanges.ChangeNotes;
                if (bpmChanges.ChangeNotes.Count > 0 && note.BPMChangeNotes.Count == 0)
                {
                    throw new IndexOutOfRangeException("BPM COUNT DISAGREE");
                }
                if (bpmChanges.ChangeNotes.Count == 0)
                {
                    throw new IndexOutOfRangeException("BPM CHANGE COUNT DISAGREE");
                }
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
            return new MeasureChanges(Int32.Parse(token.Split('\t')[1]), Int32.Parse(token.Split('\t')[2]));
        }

        public Note NoteOfToken(string token)
        {
            string[] candidate = token.Split('\t');
            int bar = Int32.Parse(candidate[(int)StdParam.Bar]);
            int tick = Int32.Parse(candidate[(int)StdParam.Tick]);
            return this.NoteOfToken(token, bar, tick, 0.0);
        }

        public Note NoteOfToken(string token, int bar, int tick, double bpm)
        {
            Note result = new Rest();
            bool isTap = token.Split('\t')[(int)StdParam.Type].Contains("TAP")
                || token.Split('\t')[(int)StdParam.Type].Contains("STR")
                || token.Split('\t')[(int)StdParam.Type].Contains("TTP")
                || token.Split('\t')[(int)StdParam.Type].Equals("XTP")
                || token.Split('\t')[(int)StdParam.Type].Equals("XST")
                || token.Split('\t')[(int)StdParam.Type].Equals("BRK")
                || token.Split('\t')[(int)StdParam.Type].Equals("BST");
            bool isHold = token.Split('\t')[(int)StdParam.Type].Contains("HLD")
                || token.Split('\t')[(int)StdParam.Type].Equals("XHO")
                || token.Split('\t')[(int)StdParam.Type].Contains("THO");
            bool isSlide = token.Split('\t')[(int)StdParam.Type].Contains("SI_")
                || token.Split('\t')[(int)StdParam.Type].Contains("SV_")
                || token.Split('\t')[(int)StdParam.Type].Contains("SF_")
                || token.Split('\t')[(int)StdParam.Type].Contains("SCL")
                || token.Split('\t')[(int)StdParam.Type].Contains("SCR")
                || token.Split('\t')[(int)StdParam.Type].Contains("SUL")
                || token.Split('\t')[(int)StdParam.Type].Contains("SUR")
                || token.Split('\t')[(int)StdParam.Type].Contains("SLL")
                || token.Split('\t')[(int)StdParam.Type].Contains("SLR")
                || token.Split('\t')[(int)StdParam.Type].Contains("SXL")
                || token.Split('\t')[(int)StdParam.Type].Contains("SXR")
                || token.Split('\t')[(int)StdParam.Type].Contains("SSL")
                || token.Split('\t')[(int)StdParam.Type].Contains("SSR");
            if (isTap)
            {
                result = TapOfToken(token);
            }
            else if (isHold)
            {
                result = HoldOfToken(token);
            }
            else if (isSlide)
            {
                result = SlideOfToken(token);
                // result.SlideStart = PreviousSlideStart;
            }
            if (result.Tick == 384)
            {
                result.Tick = 0;
                result.Bar++;
            }
            string[] candidate = token.Split('\t');
            if (candidate[(int)DxTapParam.Type].Length > 3)
            {
                string specialProperty = "";
                specialProperty = candidate[(int)DxTapParam.Type].Substring(0, 2);
                result.NoteType = result.NoteType.Substring(2);
                switch (specialProperty)
                {
                    case "BR":
                        result.NoteSpecialState = Note.SpecialState.Break;
                        break;
                    case "EX":
                        result.NoteSpecialState = Note.SpecialState.EX;
                        break;
                    case "BX":
                        result.NoteSpecialState = Note.SpecialState.BreakEX;
                        break;
                    case "CN":
                        result.NoteSpecialState = Note.SpecialState.ConnectingSlide;
                        break;
                    case "NM":
                    case "":
                    default:
                        result.NoteSpecialState = Note.SpecialState.Normal;
                        break;
                }
            }
            if (bpm>0.0) result.BPM = bpm;
            if (result.NoteSpecificGenre.Equals("SLIDE_START"))
            {
                PreviousSlideStart = (Tap)result;
            }
            return result;
        }

        public Hold HoldOfToken(string token, int bar, int tick, double bpm)
        {
            Note result = new Rest();
            string[] candidate = token.Split('\t');
            if (candidate[(int)DxTapParam.Type].Contains("THO")) //Basically all THO falls in this line
            {
                string noteSize = candidate.Count() > 7 ? candidate[(int)DxHoldParam.NoteSize] : "M1";
                result = new Hold(candidate[(int)DxHoldParam.Type],
                bar,
                tick,
                candidate[(int)DxHoldParam.Key] + candidate[(int)DxHoldParam.KeyGroup], int.Parse(candidate[(int)DxHoldParam.LastTime]),
                int.Parse(candidate[(int)DxHoldParam.SpecialEffect]),
                noteSize);
            }
            else
                result = new Hold(candidate[(int)StdParam.Type],
                            int.Parse(candidate[(int)StdParam.Bar]),
                            int.Parse(candidate[(int)StdParam.Tick]),
                            candidate[(int)StdParam.Key],
                            int.Parse(candidate[(int)StdParam.WaitTime]));
            if (bpm > 0.0) result.BPM = bpm;
            result.NoteSpecialState = result.NoteType.Equals("XHO") ? Note.SpecialState.EX : Note.SpecialState.Normal;
            return (Hold)result;
        }

        public Hold HoldOfToken(string token)
        {
            string[] candidate = token.Split('\t');
            int bar = int.Parse(candidate[(int)StdParam.Bar]);
            int tick = int.Parse(candidate[(int)StdParam.Tick]);
            return this.HoldOfToken(token, bar, tick, 0.0);
        }

        public Slide SlideOfToken(string token, int bar, int tick, Note slideStart, double bpm)
        {
            string[] candidate = token.Split('\t');
            if (!slideStart.Key.Equals(candidate[(int)StdParam.Key]) || slideStart.Bar != bar || slideStart.Tick != tick)
            {
                //Console.WriteLine("Expected key: " + candidate[(int)StdParam.KeyOrParam]);
                //Console.WriteLine("Actual key: " + PreviousSlideStart.Key);
                //Console.WriteLine("Previous Slide Start: " + PreviousSlideStart.Compose((int)StdParam.Bar));
                //throw new Exception("THE SLIDE START DOES NOT MATCH WITH THE DEFINITION OF THIS NOTE!");
                PreviousSlideStart = new Tap("NST", bar, tick, candidate[(int)StdParam.Key]);
            }
            Slide result = new Slide(candidate[(int)StdParam.Type],
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
            string[] candidate = token.Split('\t');
            int bar = int.Parse(candidate[(int)StdParam.Bar]);
            int tick = int.Parse(candidate[(int)StdParam.Tick]);
            if (!PreviousSlideStart.Key.Equals(candidate[(int)StdParam.Key]) || PreviousSlideStart.Bar != bar || PreviousSlideStart.Tick != tick)
            {
                //Console.WriteLine("Expected key: " + candidate[(int)StdParam.Key]);
                //Console.WriteLine("Actual key: " + PreviousSlideStart.Key);
                //Console.WriteLine("Previous Slide Start: " + PreviousSlideStart.Compose((int)StdParam.Bar));
                //throw new Exception("THE SLIDE START DOES NOT MATCH WITH THE DEFINITION OF THIS NOTE!");
                PreviousSlideStart = new Tap("NST", bar, tick, candidate[(int)StdParam.Key]);
            }
            return this.SlideOfToken(token, bar, tick, PreviousSlideStart, 0.0);
        }


        public Tap TapOfToken(string token, int bar, int tick, double bpm)
        {
            Note result = new Rest();
            string[] candidate = token.Split('\t');
            if (candidate[(int)StdParam.Type].Contains("TTP"))
            {
                string noteSize = candidate.Length > 7 ? candidate[7] : "M1";
                result = new Tap(candidate[(int)DxTapParam.Type],
                bar,
                tick,
                candidate[(int)DxTapParam.Key] + candidate[(int)DxTapParam.KeyGroup],
                int.Parse(candidate[(int)DxTapParam.SpecialEffect]),
                noteSize);
            }
            else
                result = new Tap(candidate[(int)StdParam.Type],
                    int.Parse(candidate[(int)StdParam.Bar]),
                    int.Parse(candidate[(int)StdParam.Tick]),
                    candidate[(int)StdParam.Key]);
            if (bpm > 0.0) result.BPM = bpm;
            result.NoteSpecialState = result.NoteType.Equals("XTP")
            || result.NoteType.Equals("XST") ? Note.SpecialState.EX : Note.SpecialState.Normal;
            result.NoteSpecialState = result.NoteType.Equals("BRK")
            || result.NoteType.Equals("BST") ? Note.SpecialState.Break : Note.SpecialState.Normal;
            return (Tap)result;
        }

        public Tap TapOfToken(string token)
        {
            string[] candidate = token.Split('\t');
            int bar = int.Parse(candidate[(int)StdParam.Bar]);
            int tick = int.Parse(candidate[(int)StdParam.Tick]);
            return this.TapOfToken(token, bar, tick, 0.0);
        }
    }

}

