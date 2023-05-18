using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;

namespace MaiLib;

/// <summary>
/// Parse charts in Simai format
/// </summary>
public class SimaiParser : IParser
{
    /// <summary>
    /// The maximum definition of a chart
    /// </summary>
    public static int MaximumDefinition = 384;

    private Tap PreviousSlideStart;

    /// <summary>
    /// Constructor of simaiparser
    /// </summary>
    public SimaiParser()
    {
        PreviousSlideStart = new Tap();
    }

    /// <summary>
    /// Parse BPM change notes
    /// </summary>
    /// <param name="token">The parsed set of BPM change</param>
    /// <returns>Error: simai does not have this variable</returns>
    public BPMChanges BPMChangesOfToken(string token)
    {
        throw new NotImplementedException("Simai does not have this component");
    }

    public Chart ChartOfToken(string[] tokens)
    // Note: here chart will only return syntax after &inote_x= and each token is separated by ","
    {
        List<Note> notes = new List<Note>();
        BPMChanges bpmChanges = new BPMChanges();
        MeasureChanges measureChanges = new MeasureChanges(4, 4);
        int bar = 0;
        int tick = 0;
        double currentBPM = 0.0;
        int tickStep = 0;
        for (int i = 0; i < tokens.Length; i++)
        {
            List<string> eachPairCandidates = EachGroupOfToken(tokens[i]);
            foreach (string eachNote in eachPairCandidates)
            {
                Note noteCandidate = NoteOfToken(eachNote, bar, tick, currentBPM);
                bool containsBPM = noteCandidate.NoteSpecificGenre.Equals("BPM");
                bool containsMeasure = noteCandidate.NoteSpecificGenre.Equals("MEASURE");

                if (containsBPM)
                {
                    // string bpmCandidate = eachNote.Replace("(", "").Replace(")", "");
                    // noteCandidate = new BPMChange(bar, tick, Double.Parse(bpmCandidate));
                    noteCandidate.Bar = bar;
                    noteCandidate.Tick = tick;
                    //notes.Add(changeNote);
                    currentBPM = noteCandidate.BPM;
                    bpmChanges.Add((BPMChange)noteCandidate);
                }
                else if (containsMeasure)
                {

                    string quaverCandidate = eachNote.Replace("{", "").Replace("}", "");
                    tickStep = MaximumDefinition / Int32.Parse(quaverCandidate);
                    // MeasureChange changeNote = new MeasureChange(bar, tick, tickStep);
                    //notes.Add(changeNote);
                }

                else /*if (currentBPM > 0.0)*/
                {
                    notes.Add(noteCandidate);
                }
            }


            tick += tickStep;
            while (tick >= MaximumDefinition)
            {
                tick -= MaximumDefinition;
                bar++;
            }
        }
        Chart result = new Simai(notes, bpmChanges, measureChanges);
        return result;
    }

    public Hold HoldOfToken(string token, int bar, int tick, double bpm)
    {
        int sustainSymbol = token.IndexOf("[");
        string keyCandidate = token.Substring(0, sustainSymbol); //key candidate is like tap grammar
        //Console.WriteLine(keyCandidate);
        string sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
        //Console.WriteLine(sustainCandidate);
        string key = "";
        string holdType = "";
        int specialEffect = 0;
        // bool sustainIsSecond = sustainCandidate.Contains("##");
        // if (sustainIsSecond)
        // {
        //     string[] secondCandidates = sustainCandidate.Split("##");

        // }
        if (keyCandidate.Contains("C"))
        {
            holdType = "THO";
            key = "0C";
            if (keyCandidate.Contains("f"))
            {
                specialEffect = 1;
            }
        }
        else if (keyCandidate.Contains("x"))
        {
            key = keyCandidate.Replace("h", "");
            key = key.Replace("x", "");
            Console.WriteLine(key);
            key = (int.Parse(key) - 1).ToString();
            holdType = "XHO";
        }
        else
        {
            key = keyCandidate.Replace("h", "");
            //Console.WriteLine(key);
            key = (int.Parse(key) - 1).ToString();
            holdType = "HLD";
        }
        string[] lastTimeCandidates = sustainCandidate.Split(":");
        int quaver = int.Parse(lastTimeCandidates[0]);
        int lastTick = 384 / quaver;
        int times = int.Parse(lastTimeCandidates[1]);
        lastTick *= times;
        Hold candidate;
        key.Replace("h", "");
        //Console.WriteLine(key);
        if (holdType.Equals("THO"))
        {
            candidate = new Hold(holdType, bar, tick, key, lastTick, specialEffect, "M1");
        }
        else
        {
            candidate = new Hold(holdType, bar, tick, key, lastTick);
        }
        candidate.BPM = bpm;
        return candidate;
    }

    public Hold HoldOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public MeasureChanges MeasureChangesOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Note NoteOfToken(string token)
    {
        Note result = new Rest();
        bool isRest = token.Equals("");
        bool isBPM = token.Contains(")");
        bool isMeasure = token.Contains("}");
        bool isSlide = token.Contains("-") ||
        token.Contains("v") ||
        token.Contains("w") ||
        token.Contains("<") ||
        token.Contains(">") ||
        token.Contains("p") ||
        token.Contains("q") ||
        token.Contains("s") ||
        token.Contains("z") ||
        token.Contains("V");
        bool isHold = !isSlide && token.Contains("[");
        if (isSlide)
        {
            result = SlideOfToken(token);
        }
        else if (isHold)
        {
            result = HoldOfToken(token);
        }
        else if (isBPM)
        {
            //throw new NotImplementedException("IsBPM is not supported in simai");
            // string bpmCandidate = token;
            // bpmCandidate.Replace("(", "");
            // bpmCandidate.Replace(")", "");
            //result = new BPMChange(bar, tick, Double.Parse(bpmCandidate));
        }
        else if (isMeasure)
        {
            throw new NotImplementedException("IsMeasure is not supported in simai");
            // string quaverCandidate = token;
            // quaverCandidate.Replace("{", "");
            // quaverCandidate.Replace("}", "");
            //result = new MeasureChange(bar, tick, Int32.Parse(quaverCandidate));
        }
        else
        {
            result = TapOfToken(token);
        }
        return result;
    }

    public Note NoteOfToken(string token, int bar, int tick, double bpm)
    {
        Note result = new Rest("RST", bar, tick);
        bool isRest = token.Equals("");
        bool isBPM = token.Contains(")");
        bool isMeasure = token.Contains("}");
        bool isSlide = token.Contains("-") ||
        token.Contains("v") ||
        token.Contains("w") ||
        token.Contains("<") ||
        token.Contains(">") ||
        token.Contains("p") ||
        token.Contains("q") ||
        token.Contains("s") ||
        token.Contains("z") ||
        token.Contains("V");
        bool isHold = !isSlide && token.Contains("[");

        if (!isRest)
        {
            if (isSlide)
            {
                result = SlideOfToken(token, bar, tick, PreviousSlideStart, bpm);
            }
            else if (isHold)
            {
                result = HoldOfToken(token, bar, tick, bpm);
            }
            else if (isBPM)
            {
                string bpmCandidate = token.Replace("(", "").Replace(")", "");
                result = new BPMChange(bar, tick, Double.Parse(bpmCandidate));
            }
            else if (isMeasure)
            {
                string quaverCandidate = token.Replace("{", "").Replace("}", "");
                result = new MeasureChange(bar, tick, Int32.Parse(quaverCandidate));
            }
            else if (!token.Equals("E") && !token.Equals(""))
            {
                result = TapOfToken(token, bar, tick, bpm);
                if (result.NoteSpecificGenre.Equals("SLIDE_START"))
                {
                    PreviousSlideStart = (Tap)result;
                }
            }
        }

        return result;
    }

    public Slide SlideOfToken(string token, int bar, int tick, Note slideStart, double bpm)
    {
        Note slideStartCandidate = new Tap(slideStart);
        Note result;
        string endKeyCandidate = "";
        int sustainSymbol = 0;
        string sustainCandidate = "";
        string noteType = "";
        bool isConnectingSlide = token.Contains("CN");
        string connectedSlideStart = isConnectingSlide ? token.Split("CN")[1] : "";
        // if (isConnectingSlide) Console.ReadKey();
        bool isEXBreak = token.Contains("b") && token.Contains("x");
        bool isBreak = token.Contains("b") && !token.Contains("x");
        bool isEX = !token.Contains("b") && token.Contains("x");
        bool timeAssigned = token.Contains("[");
        //Parse first section
        if (token.Contains("qq"))
        {
            endKeyCandidate = token.Substring(2, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SXR";
        }
        else if (token.Contains("q"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SUR";
        }
        else if (token.Contains("pp"))
        {
            endKeyCandidate = token.Substring(2, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SXL";
        }
        else if (token.Contains("p"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SUL";
        }
        else if (token.Contains("v"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SV_";
        }
        else if (token.Contains("w"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SF_";
        }
        else if (token.Contains("<"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            if (PreviousSlideStart.Key.Equals("0") ||
                PreviousSlideStart.Key.Equals("1") ||
                PreviousSlideStart.Key.Equals("6") ||
                PreviousSlideStart.Key.Equals("7"))
            {
                noteType = "SCL";
            }
            else noteType = "SCR";

        }
        else if (token.Contains(">"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            if (PreviousSlideStart.Key.Equals("0") ||
                PreviousSlideStart.Key.Equals("1") ||
                PreviousSlideStart.Key.Equals("6") ||
                PreviousSlideStart.Key.Equals("7"))
            {
                noteType = "SCR";
            }
            else noteType = "SCL";

        }
        else if (token.Contains("s"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SSL";
        }
        else if (token.Contains("z"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SSR";
        }
        else if (token.Contains("V"))
        {
            endKeyCandidate = token.Substring(2, 1);
            int sllCandidate = int.Parse(slideStartCandidate.Key) + 2;
            int slrCandidate = int.Parse(slideStartCandidate.Key) - 2;
            int inflectionCandidate = int.Parse(token.Substring(1, 1)) - 1;
            ////Revalue inflection candidate
            //if (inflectionCandidate < 0)
            //{
            //    inflectionCandidate += 8;
            //}
            //else if (inflectionCandidate > 7)
            //{
            //    inflectionCandidate -= 8;
            //}

            //Revalue SLL and SLR candidate
            if (sllCandidate < 0)
            {
                sllCandidate += 8;
            }
            else if (sllCandidate > 7)
            {
                sllCandidate -= 8;
            }
            if (slrCandidate < 0)
            {
                slrCandidate += 8;
            }
            else if (slrCandidate > 7)
            {
                slrCandidate -= 8;
            }

            bool isSLL = sllCandidate == inflectionCandidate;
            bool isSLR = slrCandidate == inflectionCandidate;
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            if (isSLL)
            {
                noteType = "SLL";
            }
            else if (isSLR)
            {
                noteType = "SLR";
            }
            if (!(isSLL || isSLR))
            {
                Console.WriteLine("Start Key:" + slideStartCandidate.Key);
                Console.WriteLine("Expected inflection point: SLL for " + sllCandidate + " and SLR for " + slrCandidate);
                Console.WriteLine("Actual: " + inflectionCandidate);
                throw new InvalidDataException("THE INFLECTION POINT GIVEN IS NOT MATCHING!");
            }
        }

        else if (token.Contains("-"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = "SI_";
        }

        //Console.WriteLine("Key Candidate: "+keyCandidate);
        int fixedKeyCandidate = int.Parse(endKeyCandidate) - 1;
        if (fixedKeyCandidate < 0)
        {
            endKeyCandidate += 8;
        }
        bool isSecond = sustainCandidate.Contains("##");
        if (!isSecond)
        {
            string[] lastTimeCandidates = sustainCandidate.Split(":");
            int quaver = timeAssigned ? int.Parse(lastTimeCandidates[0]) : 0;
            int lastTick = timeAssigned ? 384 / quaver : 0;
            int times = timeAssigned ? int.Parse(lastTimeCandidates[1]) : 0;
            lastTick *= times;
            result = new Slide(noteType, bar, tick, slideStartCandidate.Key, 96, lastTick, fixedKeyCandidate.ToString());
        }
        else
        {
            string[] timeCandidates = sustainCandidate.Split("##");
            double waitLengthCandidate = timeAssigned ? double.Parse(timeCandidates[0]) : 0;
            double lastLengthCandidate = timeAssigned ? double.Parse(timeCandidates[1]) : 0;
            double tickUnit = Chart.GetBPMTimeUnit(bpm);
            int waitLength = timeAssigned ? (int)(waitLengthCandidate / tickUnit) : 0;
            int lastLength = timeAssigned ? (int)(lastLengthCandidate / tickUnit) : 0;
            result = new Slide(noteType, bar, tick, slideStartCandidate.Key, waitLength, lastLength, fixedKeyCandidate.ToString());
            result.CalculatedWaitTime = waitLengthCandidate;
            result.CalculatedLastTime = lastLengthCandidate;
        }

        result.BPM = bpm;
        if (isConnectingSlide)
        {
            result.NoteSpecialState = Note.SpecialState.ConnectingSlide;
            result.Key = connectedSlideStart;
        }
        return (Slide)result;
    }

    public Slide SlideOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Tap TapOfToken(string token, int bar, int tick, double bpm)
    {
        bool isEXBreak = token.Contains("b") && token.Contains("x");
        bool isBreak = token.Contains("b") && !token.Contains("x");
        bool isEXTap = token.Contains("x") && token.Contains("b");
        bool isTouch = token.Contains("A") ||
        token.Contains("B") ||
        token.Contains("C") ||
        token.Contains("D") ||
        token.Contains("E") ||
        token.Contains("F");
        Tap result = new Tap();
        if (isTouch)
        {
            bool hasSpecialEffect = token.Contains("f");
            int keyCandidate = Int32.Parse(token.Substring(1, 1)) - 1;
            if (hasSpecialEffect)
            {
                result = new Tap("TTP", bar, tick, keyCandidate.ToString() + token.Substring(0, 1), 1, "M1");
            }
            else result = new Tap("TTP", bar, tick, keyCandidate.ToString() + token.Substring(0, 1), 0, "M1");
        }
        else if (isEXBreak)
        {
            int keyCandidate = Int32.Parse(token.Substring(0, 1)) - 1;
            if (token.Contains("_"))
            {
                result = new Tap("STR", bar, tick, keyCandidate.ToString());
            }
            else result = new Tap("TAP", bar, tick, keyCandidate.ToString());
            result.NoteSpecialState = Note.SpecialState.BreakEX;
        }
        else if (isEXTap)
        {
            int keyCandidate = Int32.Parse(token.Substring(0, 1)) - 1;
            if (token.Contains("_"))
            {
                result = new Tap("XST", bar, tick, keyCandidate.ToString());
            }
            else result = new Tap("XTP", bar, tick, keyCandidate.ToString());
            result.NoteSpecialState = Note.SpecialState.EX;
        }
        else if (isBreak)
        {
            int keyCandidate = Int32.Parse(token.Substring(0, 1)) - 1;
            if (token.Contains("_"))
            {
                result = new Tap("BST", bar, tick, keyCandidate.ToString());
            }
            else result = new Tap("BRK", bar, tick, keyCandidate.ToString());
            result.NoteSpecialState = Note.SpecialState.Break;
        }
        else
        {
            int keyCandidate = Int32.Parse(token.Substring(0, 1)) - 1;
            if (token.Contains("_"))
            {
                result = new Tap("STR", bar, tick, keyCandidate.ToString());
            }
            else if (!token.Equals(""))
            {
                result = new Tap("TAP", bar, tick, keyCandidate.ToString());
            }

        }
        result.BPM = bpm;
        return result;
    }

    public Tap TapOfToken(string token)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deal with old, out-fashioned and illogical Simai Each Groups.
    /// </summary>
    /// <param name="token">Tokens that potentially contains each Groups</param>
    /// <returns>List of strings that is composed with single note.</returns>
    public static List<string> EachGroupOfToken(string token)
    {
        List<string> result = new List<string>();
        bool isSlide = token.Contains("-") ||
        token.Contains("v") ||
        token.Contains("w") ||
        token.Contains("<") ||
        token.Contains(">") ||
        token.Contains("p") ||
        token.Contains("q") ||
        token.Contains("s") ||
        token.Contains("z") ||
        token.Contains("V");
        if (token.Contains("/"))
        {
            string[] candidate = token.Split("/");
            foreach (string tokenCandidate in candidate)
            {
                result.AddRange(EachGroupOfToken(tokenCandidate));
            }
        }
        else if (token.Contains(")") || token.Contains("}"))
        {
            List<string> resultCandidate = ExtractParentheses(token);
            List<string> fixedCandidate = new();
            foreach (string candidate in resultCandidate)
            {
                fixedCandidate.AddRange(ExtractParentheses(candidate));
            }
            foreach (string candidate in fixedCandidate)
            {
                result.AddRange(ExtractEachSlides(candidate));
            }
        } //lol this is the most stupid code I have ever wrote
        else if (Int32.TryParse(token, out int eachPair))
        {
            char[] eachPairs = token.ToCharArray();
            foreach (char x in eachPairs)
            {
                result.Add(x.ToString());
            }
        }
        else if (isSlide)
        {
            //List<string> candidate = EachGroupOfToken(token);
            //foreach (string item in candidate)
            //{
            //    result.AddRange(ExtractEachSlides(item));
            //}
            result.AddRange(ExtractEachSlides(token));
        }
        else result.Add(token);
        return result;
    }

    /// <summary>
    /// Deal with annoying and vigours Parentheses grammar of Simai
    /// </summary>
    /// <param name="token">Token that potentially contains multiple slide note</param>
    /// <returns>A list of strings extracts each note</returns>
    public static List<string> ExtractParentheses(string token)
    {
        List<string> result = new List<string>();
        bool containsBPM = token.Contains(")");
        bool containsMeasure = token.Contains("}");

        if (containsBPM)
        {
            string[] tokenCandidate = token.Split(")");
            List<string> tokenResult = new();
            for (int i = 0; i < tokenCandidate.Length; i++)
            {
                string x = tokenCandidate[i];
                if (x.Contains("("))
                {
                    x += ")";
                }
                if (!x.Equals(""))
                {
                    tokenResult.Add(x);
                }
            }
            result.AddRange(tokenResult);
        }
        else if (containsMeasure)
        {
            string[] tokenCandidate = token.Split("}");
            List<string> tokenResult = new();
            for (int i = 0; i < tokenCandidate.Length; i++)
            {
                string x = tokenCandidate[i];
                if (x.Contains("{"))
                {
                    x += "}";
                }
                if (!x.Equals(""))
                {
                    tokenResult.Add(x);
                }
            }
            result.AddRange(tokenResult);
        }
        else
        {
            result.Add(token);
        }
        return result;
    }

    /// <summary>
    /// Deal with annoying and vigours Slide grammar of Simai
    /// </summary>
    /// <param name="token">Token that potentially contains multiple slide note</param>
    /// <returns>A list of slides extracts each note</returns>
    public static List<string> ExtractEachSlides(string token)
    {
        bool isSlide = token.Contains("-") ||
        token.Contains("v") ||
        token.Contains("w") ||
        token.Contains("<") ||
        token.Contains(">") ||
        token.Contains("p") ||
        token.Contains("q") ||
        token.Contains("s") ||
        token.Contains("z") ||
        token.Contains("V");
        List<string> result = new List<string>();
        if (!isSlide)
        {
            result.Add(token);
        }
        else
        {
            string[] candidates = token.Split("*");
            foreach (string symbol in candidates)
            {
                result.AddRange(ExtractConnectingSlides(symbol));
            }
            // if (!token.Contains("*"))
            // {
            //     string splitCandidate = token;
            //     //Parse first section
            //     if (splitCandidate.Contains("qq"))
            //     {
            //         result.AddRange(splitCandidate.Split("qq"));
            //         result[0] = result[0] + "_";
            //         result[1] = "qq" + result[1];
            //     }
            //     else if (splitCandidate.Contains("q"))
            //     {
            //         result.AddRange(splitCandidate.Split("q"));
            //         result[0] = result[0] + "_";
            //         result[1] = "q" + result[1];
            //     }
            //     else if (splitCandidate.Contains("pp"))
            //     {
            //         result.AddRange(splitCandidate.Split("pp"));
            //         result[0] = result[0] + "_";
            //         result[1] = "pp" + result[1];
            //     }
            //     else if (splitCandidate.Contains("p"))
            //     {
            //         result.AddRange(splitCandidate.Split("p"));
            //         result[0] = result[0] + "_";
            //         result[1] = "p" + result[1];
            //     }
            //     else if (splitCandidate.Contains("v"))
            //     {
            //         result.AddRange(splitCandidate.Split("v"));
            //         result[0] = result[0] + "_";
            //         result[1] = "v" + result[1];
            //     }
            //     else if (splitCandidate.Contains("w"))
            //     {
            //         result.AddRange(splitCandidate.Split("w"));
            //         result[0] = result[0] + "_";
            //         result[1] = "w" + result[1];
            //     }
            //     else if (splitCandidate.Contains("<"))
            //     {
            //         result.AddRange(splitCandidate.Split("<"));
            //         result[0] = result[0] + "_";
            //         result[1] = "<" + result[1];
            //     }
            //     else if (splitCandidate.Contains(">"))
            //     {
            //         result.AddRange(splitCandidate.Split(">"));
            //         result[0] = result[0] + "_";
            //         result[1] = ">" + result[1];
            //     }
            //     else if (splitCandidate.Contains("s"))
            //     {
            //         result.AddRange(splitCandidate.Split("s"));
            //         result[0] = result[0] + "_";
            //         result[1] = "s" + result[1];
            //     }
            //     else if (splitCandidate.Contains("z"))
            //     {
            //         result.AddRange(splitCandidate.Split("z"));
            //         result[0] = result[0] + "_";
            //         result[1] = "z" + result[1];
            //     }
            //     else if (splitCandidate.Contains("V"))
            //     {
            //         result.AddRange(splitCandidate.Split("V"));
            //         result[0] = result[0] + "_";
            //         result[1] = "V" + result[1];
            //     }
            //     else if (splitCandidate.Contains("-"))
            //     {
            //         result.AddRange(splitCandidate.Split("-"));
            //         result[0] = result[0] + "_";
            //         result[1] = "-" + result[1];
            //     }
            // }
            // else
            // {
            //     string[] components = token.Split("*");
            //     if (components.Length < 1)
            //     {
            //         throw new Exception("SLIDE TOKEN NOT VALID: \n" + token);
            //     }
            //     string splitCandidate = components[0];
            //     //Parse first section
            //     if (splitCandidate.Contains("qq"))
            //     {
            //         result.AddRange(splitCandidate.Split("qq"));
            //         result[0] = result[0] + "_";
            //         result[1] = "qq" + result[1];
            //     }
            //     else if (splitCandidate.Contains("q"))
            //     {
            //         result.AddRange(splitCandidate.Split("q"));
            //         result[0] = result[0] + "_";
            //         result[1] = "q" + result[1];
            //     }
            //     else if (splitCandidate.Contains("pp"))
            //     {
            //         result.AddRange(splitCandidate.Split("pp"));
            //         result[0] = result[0] + "_";
            //         result[1] = "pp" + result[1];
            //     }
            //     else if (splitCandidate.Contains("p"))
            //     {
            //         result.AddRange(splitCandidate.Split("p"));
            //         result[0] = result[0] + "_";
            //         result[1] = "p" + result[1];
            //     }
            //     else if (splitCandidate.Contains("v"))
            //     {
            //         result.AddRange(splitCandidate.Split("v"));
            //         result[0] = result[0] + "_";
            //         result[1] = "v" + result[1];
            //     }
            //     else if (splitCandidate.Contains("w"))
            //     {
            //         result.AddRange(splitCandidate.Split("w"));
            //         result[0] = result[0] + "_";
            //         result[1] = "w" + result[1];
            //     }
            //     else if (splitCandidate.Contains("<"))
            //     {
            //         result.AddRange(splitCandidate.Split("<"));
            //         result[0] = result[0] + "_";
            //         result[1] = "<" + result[1];
            //     }
            //     else if (splitCandidate.Contains(">"))
            //     {
            //         result.AddRange(splitCandidate.Split(">"));
            //         result[0] = result[0] + "_";
            //         result[1] = ">" + result[1];
            //     }
            //     else if (splitCandidate.Contains("s"))
            //     {
            //         result.AddRange(splitCandidate.Split("s"));
            //         result[0] = result[0] + "_";
            //         result[1] = "s" + result[1];
            //     }
            //     else if (splitCandidate.Contains("z"))
            //     {
            //         result.AddRange(splitCandidate.Split("z"));
            //         result[0] = result[0] + "_";
            //         result[1] = "z" + result[1];
            //     }
            //     else if (splitCandidate.Contains("V"))
            //     {
            //         result.AddRange(splitCandidate.Split("V"));
            //         result[0] = result[0] + "_";
            //         result[1] = "V" + result[1];
            //     }
            //     else if (splitCandidate.Contains("-"))
            //     {
            //         result.AddRange(splitCandidate.Split("-"));
            //         result[0] = result[0] + "_";
            //         result[1] = "-" + result[1];
            //     }
            //     //Add rest of slide: components after * is always 
            //     if (components.Length > 1)
            //     {
            //         for (int i = 1; i < components.Length; i++)
            //         {
            //             result.Add(components[i]);
            //         }
            //     }
            // }
        }
        return result;
    }

    /// <summary>
    /// Extract connecting slides
    /// </summary>
    /// <param name="token">Token to parse of</param>
    /// <returns>Result of parsed tokens</returns>
    public static List<string> ExtractConnectingSlides(string token)
    {
        List<string> result = new();
        char[] candidates = token.ToCharArray();
        static bool IsSlideNotation(char token)
        {
            return token == '-' ||
        token == 'v' ||
        token == 'w' ||
        token == '<' ||
        token == '>' ||
        token == 'p' ||
        token == 'q' ||
        token == 's' ||
        token == 'z' ||
        token == 'V';
        }

        static string KeyCandidate(string token)
        {
            string result = "";
            for (int i = 0; i < token.Length && result.Length == 0; i++)
            {
                if (!IsSlideNotation(token[i])) result = token[i].ToString();
            }
            return ((int.Parse(result) - 1) % 8).ToString();
        }

        bool slideStartExtracted = false;
        bool normalSlideExtracted = false;
        string slideStartCandidate = "";
        string slideCandidate = "";
        string previousSlideNoteBuffer = "";
        string lastKeyCandidate = "";

        foreach (char symbol in candidates)
        {
            // slideStartExtracted = IsSlideNotation(symbol) && result.Count > 0;
            // normalSlideExtracted = IsSlideNotation(symbol) && result.Count > 2;
            if (!slideStartExtracted && !IsSlideNotation(symbol))
            {
                slideStartCandidate += symbol;
            }
            else if (!normalSlideExtracted && !IsSlideNotation(symbol))
            {
                slideCandidate += symbol;
                previousSlideNoteBuffer += symbol;
            }
            else if (IsSlideNotation(symbol) && ((symbol == 'p' && previousSlideNoteBuffer.Equals("p")) || (symbol == 'q' && previousSlideNoteBuffer.Equals("q"))))
            {
                slideCandidate += symbol;
                previousSlideNoteBuffer += symbol;
            }
            else if (IsSlideNotation(symbol) && !slideStartExtracted)
            {
                if (slideStartCandidate.Length > 0) result.Add(slideStartCandidate + "_");
                slideCandidate += symbol;
                previousSlideNoteBuffer += symbol;
                slideStartExtracted = true;
            }
            else if (IsSlideNotation(symbol) && !((symbol == 'p' && previousSlideNoteBuffer.Equals("p")) || (symbol == 'q' && previousSlideNoteBuffer.Equals("q"))) && !normalSlideExtracted)
            {
                lastKeyCandidate = KeyCandidate(slideCandidate);
                result.Add(slideCandidate);
                normalSlideExtracted = true;
                slideCandidate = symbol.ToString();
                previousSlideNoteBuffer = symbol.ToString();
            }
            else if (IsSlideNotation(symbol))
            {
                result.Add(slideCandidate + "CN" + lastKeyCandidate);
                lastKeyCandidate = KeyCandidate(slideCandidate);
                slideCandidate = symbol.ToString();
                previousSlideNoteBuffer = symbol.ToString();
            }
            else
            {
                slideCandidate += symbol;
            }
        }
        if (slideCandidate.Length > 0 && !normalSlideExtracted) result.Add(slideCandidate);
        else if (slideCandidate.Length > 0 && normalSlideExtracted) result.Add(slideCandidate + "CN" + lastKeyCandidate);
        return result;
    }
}