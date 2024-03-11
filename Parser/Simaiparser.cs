namespace MaiLib;

using static MaiLib.NoteEnum;

/// <summary>
///     Parse charts in Simai format
/// </summary>
public class SimaiParser : IParser
{
    /// <summary>
    ///     The maximum definition of a chart
    /// </summary>
    public static int MaximumDefinition = 384;

    private static readonly string[] AllowedSlideType =
        ["qq", "q", "pp", "p", "v", "w", "<", ">", "^", "s", "z", "V", "-"];

    private Tap PreviousSlideStart;
    private BPMChanges bpmChanges;

    /// <summary>
    ///     Constructor of simaiparser
    /// </summary>
    public SimaiParser()
    {
        PreviousSlideStart = new Tap();
        bpmChanges = new();
    }

    public Chart ChartOfToken(string[] tokens)
        // Note: here chart will only return syntax after &inote_x= and each token is separated by ","
    {
        List<Note>? notes = [];
        // var bpmChanges = new BPMChanges();
        MeasureChanges? measureChanges = new MeasureChanges(4, 4);
        int bar = 0;
        int tick = 0;
        double currentBPM = 0.0;
        int tickStep = 0;
        for (int i = 0; i < tokens.Length; i++)
        {
            List<string>? eachPairCandidates = EachGroupOfToken(tokens[i]);
            string currentToken = ""; // Solely used to log current error
            int previousConnectingSlideTick = 0;
            try
            {
                foreach (string? eachNote in eachPairCandidates)
                {
                    currentToken = eachNote;
                    bool containsGraceNote = eachNote.Contains('%');
                    Note noteCandidate = containsGraceNote
                        ? NoteOfToken(eachNote.Replace("%", ""), bar, tick, currentBPM)
                        : NoteOfToken(eachNote, bar, tick, currentBPM);

                    if (noteCandidate.NoteSpecificGenre is NoteSpecificGenre.BPM)
                    {
                        noteCandidate.Bar = bar;
                        noteCandidate.Tick = tick;
                        currentBPM = noteCandidate.BPM;
                        bpmChanges.Add((BPMChange)noteCandidate);
                    }
                    else if (noteCandidate.NoteSpecificGenre is NoteSpecificGenre.MEASURE)
                    {
                        string? quaverCandidate = eachNote.Replace("{", "").Replace("}", "");
                        tickStep = MaximumDefinition / int.Parse(quaverCandidate);
                    }

                    else /*if (currentBPM > 0.0)*/
                    {
                        if (noteCandidate.NoteSpecialState is SpecialState.ConnectingSlide)
                        {
                            noteCandidate.Tick += previousConnectingSlideTick;
                            previousConnectingSlideTick = noteCandidate.Tick + noteCandidate.LastLength;
                            noteCandidate.Update();
                        }
                        else if (noteCandidate.NoteGenre is NoteGenre.SLIDE)
                        {
                            previousConnectingSlideTick = noteCandidate.Tick + noteCandidate.WaitLength +
                                                          noteCandidate.LastLength;
                        }

                        notes.Add(noteCandidate);
                    }

                    // Tick rework
                    if (containsGraceNote)
                    {
                        tick++;
                        while (tick >= MaximumDefinition)
                        {
                            tick -= MaximumDefinition;
                            bar++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // throw ex;
                throw new ParsingException(ex, bar, tick, currentToken, string.Join(", ", eachPairCandidates));
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

    public Note NoteOfToken(string token, int bar, int tick, double bpm)
    {
        Note result = new Rest(bar, tick);
        bool isRest = token.Equals("");
        bool isBPM = token.Contains(")");
        bool isMeasure = token.Contains("}");
        bool isSlide = ContainsSlideNotation(token);
        bool isHold = !isSlide && token.Contains('h');

        if (!isRest)
        {
            if (isSlide)
            {
                bool containingBreak = token.Contains('b');
                if (containingBreak) token = token.Replace("b", "");
                SpecialState breakState = containingBreak ? SpecialState.Break : SpecialState.Normal;
                List<string> extractedToken = ExtractConnectingSlides(token);
                if (extractedToken.Count == 1) result = SlideOfToken(token, bar, tick, PreviousSlideStart, bpm);
                else result = SlideGroupOfToken(extractedToken, bar, tick, PreviousSlideStart, bpm);
                result.NoteSpecialState = breakState;
                result.Update();
            }
            else if (isHold)
            {
                result = HoldOfToken(token, bar, tick, bpm);
            }
            else if (isBPM)
            {
                string? bpmCandidate = token.Replace("(", "").Replace(")", "");
                result = new BPMChange(bar, tick, double.Parse(bpmCandidate));
            }
            else if (isMeasure)
            {
                string? quaverCandidate = token.Replace("{", "").Replace("}", "");
                result = new MeasureChange(bar, tick, int.Parse(quaverCandidate));
            }
            else if (!(token.Contains('!') || token.Contains('?')) && !token.Equals("E") && !token.Equals(""))
            {
                result = TapOfToken(token, bar, tick, bpm);
                if (result.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START) PreviousSlideStart = (Tap)result;
            }
            else if (token.Contains('!') || token.Contains('?'))
            {
                PreviousSlideStart = TapOfToken(token, bar, tick, bpm);
            }
        }

        return result;
    }

    public Tap TapOfToken(string token, int bar, int tick, double bpm)
    {
        bool isEXBreak = token.Contains("b") && token.Contains("x");
        bool isBreak = token.Contains("b") && !token.Contains("x");
        bool isEXTap = token.Contains("x") && !token.Contains("b");
        bool isTouch = token.Contains("A") ||
                       token.Contains("B") ||
                       token.Contains("C") ||
                       token.Contains("D") ||
                       token.Contains("E") ||
                       token.Contains("F");
        Tap? result = new Tap();
        if (isTouch)
        {
            bool hasSpecialEffect = token.Contains("f");
            // Simai spec allows C to substitute C1 but C1 is still highly preferable
            int keyCandidate = 0;
            if (!token.Contains('C'))
            {
                keyCandidate = int.Parse(token.Substring(1, 1)) - 1;
            }

            result = new Tap(NoteType.TTP, bar, tick, keyCandidate + token.Substring(0, 1), hasSpecialEffect, "M1");
        }
        else
        {
            int keyCandidate = int.Parse(token.Substring(0, 1)) - 1;
            if (token.Contains('_') || token.Contains('$'))
                result = new Tap(NoteType.STR, bar, tick, keyCandidate.ToString());
            else if (token.Contains('!')) result = new Tap(NoteType.NST, bar, tick, keyCandidate.ToString());
            else result = new Tap(NoteType.TAP, bar, tick, keyCandidate.ToString());
            if (isEXBreak) result.NoteSpecialState = SpecialState.BreakEX;
            else if (isEXTap) result.NoteSpecialState = SpecialState.EX;
            else if (isBreak) result.NoteSpecialState = SpecialState.Break;
        }

        result.BPM = bpm;
        return result;
    }

    public Hold HoldOfToken(string token, int bar, int tick, double bpm)
    {
        if (!token.Contains('[')) token += $"[{MaximumDefinition}:0]";
        if (!(token.Contains('[') && token.Contains(']')))
            throw new InvalidOperationException("GIVEN TOKEN DOES NOT CONTAIN SUSTAIN CANDIDATE");
        string keyCandidate = token.Split('[')[0]; //key candidate is like tap grammar
        string sustainCandidate = $"[{token.Split('[')[1]}"; //sustain candidate is like 1:2

        string key;
        bool specialEffect = false;
        NoteType noteType = NoteType.HLD;
        SpecialState specialState = SpecialState.Normal;
        if (keyCandidate.Contains('C'))
        {
            noteType = NoteType.THO;
            key = "0C";
            specialEffect = keyCandidate.Contains('f');
        }
        else if (keyCandidate.Contains('x') && keyCandidate.Contains('b'))
        {
            key = keyCandidate.Replace("h", "");
            key = key.Replace("x", "");
            key = key.Replace("b", "");
            key = (int.Parse(key) - 1).ToString();
            specialState = SpecialState.BreakEX;
        }
        else if (keyCandidate.Contains('b'))
        {
            key = keyCandidate.Replace("h", "");
            key = key.Replace("b", "");
            key = (int.Parse(key) - 1).ToString();
            specialState = SpecialState.Break;
        }
        else if (keyCandidate.Contains('x'))
        {
            key = keyCandidate.Replace("h", "");
            key = key.Replace("x", "");
            key = (int.Parse(key) - 1).ToString();
            specialState = SpecialState.EX;
        }
        else
        {
            key = keyCandidate.Replace("h", "");
            key = (int.Parse(key) - 1).ToString();
        }

        double lastTime = GetTimeCandidates(bpm, $"[{sustainCandidate}]")[1];
        Hold candidate = noteType is NoteType.THO
            ? new Hold(noteType, bar, tick, key, lastTime, specialEffect, "M1")
            : new Hold(noteType, bar, tick, key, lastTime);
        candidate.BPM = bpm;
        candidate.NoteSpecialState = specialState;
        return candidate;
    }

    public SlideGroup SlideGroupOfToken(List<string> extractedTokens, int bar, int tick, Note startNote, double bpm)
    {
        int currentBar = bar;
        int currentTick = tick;
        Note slideStart = startNote;
        // int prevSlideKey = -1;
        List<Slide> slideCandidates = [];
        foreach (string x in extractedTokens)
        {
            Slide connectCandidate = SlideOfToken(x, currentBar, currentTick, slideStart, bpm);
            connectCandidate.BPMChangeNotes = bpmChanges.ChangeNotes;
            connectCandidate.Update();
            slideCandidates.Add(connectCandidate);
            currentTick += connectCandidate.WaitLength + connectCandidate.LastLength;
            if (currentTick >= MaximumDefinition)
            {
                currentBar += currentTick / MaximumDefinition;
                currentTick %= MaximumDefinition;
            }
        }

        return new SlideGroup(slideCandidates) { BPMChangeNotes = bpmChanges.ChangeNotes };
    }

    public Slide SlideOfToken(string token, int bar, int tick, Note slideStart, double bpm)
    {
        if (!(token.Contains('[') && token.Contains(']')))
            throw new InvalidOperationException("GIVEN TOKEN DOES NOT CONTAIN SUSTAIN CANDIDATE: {token}");
        Note slideStartCandidate = new Tap(slideStart);
        Note result;
        string endKeyCandidate = "";
        string sustainCandidate = $"[{token.Split('[')[1].Split(']')[0]}]";
        NoteType noteType = NoteType.RST;
        bool isConnectingSlide = token.Contains("CN");
        int connectedSlideStart = isConnectingSlide ? int.Parse(token.Split("CN")[1]) : -1;
        if (isConnectingSlide) slideStartCandidate.Key = connectedSlideStart.ToString();
        bool isEXBreak = token.Contains('b') && token.Contains('x');
        bool isBreak = token.Contains('b') && !token.Contains('x');
        bool isEX = !token.Contains('b') && token.Contains('x');
        SpecialState noteSpecialState = SpecialState.Normal;
        if (isEXBreak) noteSpecialState = SpecialState.BreakEX;
        else if (isBreak) noteSpecialState = SpecialState.Break;
        else if (isEX) noteSpecialState = SpecialState.EX;
        bool timeAssigned = token.Contains('[');
        //Parse first section
        if (token.Contains("qq"))
        {
            endKeyCandidate = token.Substring(2, 1);
            noteType = NoteType.SXR;
        }
        else if (token.Contains('q'))
        {
            endKeyCandidate = token.Substring(1, 1);
            noteType = NoteType.SUR;
        }
        else if (token.Contains("pp"))
        {
            endKeyCandidate = token.Substring(2, 1);
            noteType = NoteType.SXL;
        }
        else if (token.Contains('p'))
        {
            endKeyCandidate = token.Substring(1, 1);
            noteType = NoteType.SUL;
        }
        else if (token.Contains('v'))
        {
            endKeyCandidate = token.Substring(1, 1);
            noteType = NoteType.SV_;
        }
        else if (token.Contains('w'))
        {
            endKeyCandidate = token.Substring(1, 1);
            noteType = NoteType.SF_;
        }
        else if (token.Contains('<'))
        {
            endKeyCandidate = token.Substring(1, 1);
            int[] endPointOfConcern = [0, 1, 6, 7];
            noteType = endPointOfConcern.Any(p => p == slideStartCandidate.KeyNum) ? NoteType.SCL : NoteType.SCR;
            // noteType = NoteType.SCL;
        }
        else if (token.Contains('>'))
        {
            endKeyCandidate = token.Substring(1, 1);
            int[] endPointOfConcern = [0, 1, 6, 7];
            noteType = endPointOfConcern.Any(p => p == slideStartCandidate.KeyNum) ? NoteType.SCR : NoteType.SCL;
            // noteType = NoteType.SCR;
        }
        else if (token.Contains('^'))
        {
            endKeyCandidate = token.Substring(1, 1);
            int endKeyNum = int.Parse(endKeyCandidate) - 1;
            if (endKeyNum < 0) endKeyNum += 8;
            int sclDistance = KeyDistance(slideStartCandidate.KeyNum, endKeyNum, NoteType.SCL);
            int scrDistance = KeyDistance(slideStartCandidate.KeyNum, endKeyNum, NoteType.SCR);
            if (sclDistance >= 4 && scrDistance >= 4)
                throw new InvalidOperationException(
                    $"^ requires a distance 0<d<4. SCL distance: {sclDistance}, SCR distance: {scrDistance}. Connected Start Key: {connectedSlideStart} StartKey: {slideStartCandidate.KeyNum}, EndKey: {endKeyCandidate}, Token: {token}");
            else noteType = sclDistance < scrDistance ? NoteType.SCL : NoteType.SCR;
        }
        else if (token.Contains('s'))
        {
            endKeyCandidate = token.Substring(1, 1);
            noteType = NoteType.SSL;
        }
        else if (token.Contains('z'))
        {
            endKeyCandidate = token.Substring(1, 1);
            noteType = NoteType.SSR;
        }
        else if (token.Contains('V'))
        {
            endKeyCandidate = token.Substring(2, 1);
            int sllCandidate = int.Parse(slideStartCandidate.Key) - 2;
            int slrCandidate = int.Parse(slideStartCandidate.Key) + 2;
            int inflectionCandidate = int.Parse(token.Substring(1, 1)) - 1;

            // Revalue SLL and SLR candidate
            if (sllCandidate < 0)
                sllCandidate += 8;
            else if (sllCandidate > 7) sllCandidate -= 8;
            if (slrCandidate < 0)
                slrCandidate += 8;
            else if (slrCandidate > 7) slrCandidate -= 8;

            bool isSLL = sllCandidate == inflectionCandidate;
            bool isSLR = slrCandidate == inflectionCandidate;
            if (isSLL)
                noteType = NoteType.SLL;
            else if (isSLR) noteType = NoteType.SLR;
            if (!(isSLL || isSLR))
            {
                Console.WriteLine("Start Key:" + slideStartCandidate.Key);
                Console.WriteLine("Expected inflection point: SLL for " + sllCandidate + " and SLR for " +
                                  slrCandidate);
                Console.WriteLine("Actual: " + inflectionCandidate);
                throw new InvalidOperationException("THE INFLECTION POINT GIVEN IS NOT MATCHING!");
            }
        }

        else if (token.Contains('-'))
        {
            endKeyCandidate = token.Substring(1, 1);
            noteType = NoteType.SI_;
        }

        //Console.WriteLine("Key Candidate: "+keyCandidate);
        int fixedKeyCandidate = int.Parse(endKeyCandidate) - 1;
        if (fixedKeyCandidate < 0) endKeyCandidate += 8;
        double[] durationResult = GetTimeCandidates(bpm, sustainCandidate, true);
        double waitLengthCandidate = durationResult[0];
        double lastLengthCandidate = durationResult[1];
        result = new Slide(noteType, bar, tick, waitLengthCandidate, lastLengthCandidate, slideStartCandidate.Key,
            fixedKeyCandidate.ToString()) { BPMChangeNotes = bpmChanges.ChangeNotes };
        result.BPM = bpm;
        if (isConnectingSlide)
        {
            result.NoteSpecialState = SpecialState.ConnectingSlide;
            result.Key = connectedSlideStart.ToString();
            if (connectedSlideStart == -1)
                throw new InvalidOperationException("This connecting start does not have start key");
            result.WaitLength = 0;
            result.CalculatedWaitTime = 0;
        }
        else result.NoteSpecialState = noteSpecialState;

        return (Slide)result;
    }

    #region UnimplementedDummyMethods

    public Note NoteOfToken(string token)
    {
        Note result = new Rest();
        bool isRest = token.Equals("");
        bool isBPM = token.Contains(")");
        bool isMeasure = token.Contains("}");
        bool isSlide = ContainsSlideNotation(token);
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
        }
        else if (isMeasure)
        {
            throw new NotImplementedException("IsMeasure is not supported in simai");
        }
        else
        {
            result = TapOfToken(token);
        }

        return result;
    }

    /// <summary>
    ///     Parse BPM change notes
    /// </summary>
    /// <param name="token">The parsed set of BPM change</param>
    /// <returns>Error: simai does not have this variable</returns>
    public BPMChanges BPMChangesOfToken(string token)
    {
        throw new NotImplementedException("Simai does not have this component");
    }

    /// <summary>
    ///     Parse Measure change notes
    /// </summary>
    /// <param name="token">The parsed set of Measured change</param>
    /// <returns>Error: simai does not have this variable</returns>
    public MeasureChanges MeasureChangesOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Tap TapOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Hold HoldOfToken(string token)
    {
        throw new NotImplementedException();
    }

    public Slide SlideOfToken(string token)
    {
        throw new NotImplementedException();
    }

    #endregion

    #region HeroticStaticHelperMethods

    /// <summary>
    ///     Deal with old, out-fashioned and illogical Simai Each Groups. Reworked with state machine.
    /// </summary>
    /// <param name="token">Tokens that potentially contains each Groups</param>
    /// <returns>List of strings that is composed with single note.</returns>
    public static List<string> EachGroupOfToken(string token)
    {
        string buffer = "";
        List<string> extractedParts = [];
        foreach (char c in token)
            switch (c)
            {
                case '/':
                    extractedParts.Add(buffer);
                    buffer = "";
                    break;
                case '(':
                case '{':
                    if (buffer.Length > 0) extractedParts.Add(buffer);
                    buffer = c.ToString();
                    break;
                case ')':
                case '}':
                    buffer += c;
                    extractedParts.Add(buffer);
                    buffer = "";
                    break;
                case '`':
                    buffer += '%'; // Might not be necessary since this method is reworked
                    extractedParts.Add(buffer);
                    buffer = "";
                    break;
                default:
                    buffer += c;
                    break;
            }

        if (buffer.Length > 0) extractedParts.Add(buffer);

        List<string> result = [];
        foreach (string part in extractedParts)
        {
            if (ContainsSlideNotation(part))
            {
                result.AddRange(ExtractEachSlides(part));
            }
            else if (int.TryParse(part, out int _))
            {
                result.AddRange(part.Select(eachTap => eachTap.ToString()));
            }
            else result.Add(part);
        }

        return result;
    }

    /// <summary>
    ///     Deal with annoying and vigorous Parentheses grammar of Simai
    /// </summary>
    /// <param name="token">Token that potentially contains multiple slide note</param>
    /// <returns>A list of strings extracts each note</returns>
    public static List<string> ExtractParentheses(string token)
    {
        List<string>? result = [];
        bool containsBPM = token.Contains(")");
        bool containsMeasure = token.Contains("}");

        if (containsBPM)
        {
            string[]? tokenCandidate = token.Split(")");
            List<string> tokenResult = [];
            for (int i = 0; i < tokenCandidate.Length; i++)
            {
                string? x = tokenCandidate[i];
                if (x.Contains("(")) x += ")";
                if (!x.Equals("")) tokenResult.Add(x);
            }

            result.AddRange(tokenResult);
        }
        else if (containsMeasure)
        {
            string[]? tokenCandidate = token.Split("}");
            List<string> tokenResult = [];
            for (int i = 0; i < tokenCandidate.Length; i++)
            {
                string? x = tokenCandidate[i];
                if (x.Contains("{")) x += "}";
                if (!x.Equals("")) tokenResult.Add(x);
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
    ///     Deal with annoying and vigorous Slide grammar of Simai
    /// </summary>
    /// <param name="token">Token that potentially contains multiple slide note</param>
    /// <returns>A list of slides extracts each note</returns>
    public static List<string> ExtractEachSlides(string token)
    {
        bool isSlide = ContainsSlideNotation(token);
        List<string>? result = [];
        if (!isSlide)
        {
            result.Add(token);
        }
        else
        {
            string[]? candidates = token.Split("*");
            foreach (string? symbol in candidates) result.AddRange(ExtractSlideStart(symbol));
            // result.AddRange(candidates);
        }

        return result;
    }

    public static List<string> ExtractSlideStart(string token)
    {
        List<string> result = [];
        bool slideNotationFound = IsSlideNotation(token[0]);
        string buffer = "";
        foreach (char x in token)
        {
            if (!slideNotationFound && IsSlideNotation(x))
            {
                buffer += '_';
                result.Add(buffer);
                buffer = x.ToString();
                slideNotationFound = true;
            }
            else buffer += x;
        }

        result.Add(buffer);
        return result;
    }

    /// <summary>
    ///     Extract connecting slides
    /// </summary>
    /// <param name="token">Token to parse of</param>
    /// <returns>Result of parsed tokens</returns>
    public static List<string> ExtractConnectingSlides(string token)
    {
        List<string> result = [];
        char[]? candidates = token.ToCharArray();

        static string KeyCandidate(string token)
        {
            string result = "";
            int inflectionV = token.Contains('V') ? 2 : 1; // V slide involves VXX
            for (int i = 0; i < token.Length && result.Length < inflectionV; i++)
                if (!IsSlideNotation(token[i]) && char.IsNumber(token[i]))
                    result += token[i].ToString();
            if (result.Length > 1) result = result[1].ToString();
            return ((int.Parse(result) - 1) % 8).ToString();
        }

        bool slideStartExtracted = false;
        bool normalSlideExtracted = false;
        string? slideStartCandidate = "";
        string? slideCandidate = "";
        string? previousSlideNoteBuffer = "";
        string? lastKeyCandidate = "";

        foreach (char symbol in candidates)
        {
            if (!slideStartExtracted && !IsSlideNotation(symbol))
            {
                slideStartCandidate += symbol;
            }
            else if (!normalSlideExtracted && !IsSlideNotation(symbol))
            {
                slideCandidate += symbol;
                previousSlideNoteBuffer += symbol;
            }
            else if (IsSlideNotation(symbol) && ((symbol == 'p' && previousSlideNoteBuffer.Equals("p")) ||
                                                 (symbol == 'q' && previousSlideNoteBuffer.Equals("q"))))
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
            else if (IsSlideNotation(symbol) &&
                     !((symbol == 'p' && previousSlideNoteBuffer.Equals("p")) ||
                       (symbol == 'q' && previousSlideNoteBuffer.Equals("q"))) && !normalSlideExtracted)
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

        // Compensation for the last slide
        if (slideCandidate.Length > 0 && !normalSlideExtracted) result.Add(slideCandidate);
        else if (slideCandidate.Length > 0 && normalSlideExtracted)
            result.Add(slideCandidate + "CN" + lastKeyCandidate);

        // Post processing of slide durations
        if (result.Count(p => p.Contains('[')) == 0)
        {
            throw new Exception("Extracted slides do not contain any duration setting: " + String.Join(", ", result));
        }
        // Catch for single duration connecting slides
        // This condition is faulty: it should be triggered when there are at least 2 slides
        else if (result.Count(p => AllowedSlideType.Any(p.Contains)) >= 2 && result.Count(p => p.Contains('[')) == 1)
        {
            static string ReplaceDuration(string oldValue, string newDuration)
            {
                string result = "";
                bool ignoring = false;
                foreach (char x in oldValue)
                {
                    ignoring = ignoring || x == '[';
                    if (!ignoring) result += x;
                    ignoring = ignoring && x != ']';
                }

                if (oldValue.Contains("CN")) return $"{result.Split("CN")[0]}{newDuration}CN{result.Split("CN")[1]}";
                else return oldValue + newDuration;
            }

            string slideDurationCandidate =
                result.Find(p => p.Contains('[')) ?? throw new Exception("Unexpected duration not found");
            string newDurationCandidate = "[" + slideDurationCandidate.Split('[')[1].Split("CN")[0];
            int actualSlidePart = result.Count(p => !p.Contains('_'));
            double originalWaitDuration = 0.0;
            double averageDuration = 0.0;
            bool isMeasureDuration =
                newDurationCandidate.Contains(':') && !newDurationCandidate.Contains('#'); // [Quaver : Beats]
            if (isMeasureDuration)
            {
                string durationCandidate = newDurationCandidate.Replace("[", "").Replace("]", "");
                string[] numList = durationCandidate.Split(':');
                int quaver = int.Parse(numList[0]) * actualSlidePart;
                int multiple = int.Parse(numList[1]);
                newDurationCandidate = $"[{quaver}:{multiple}]";
            }
            else
            {
                double[] durationResult = GetTimeCandidates(0.0, newDurationCandidate, true);
                originalWaitDuration = durationResult[0];
                averageDuration = durationResult[1] / actualSlidePart;
                newDurationCandidate = $"[0##{Math.Round(averageDuration, 4)}]";
            }

            // double[] durationResult = GetTimeCandidates(0.0, newDurationCandidate, true);
            // originalWaitDuration = durationResult[0];
            // averageDuration = durationResult[1] / actualSlidePart;
            // newDurationCandidate = $"[0##{Math.Round(averageDuration, 4)}]";

            bool writeOriginalWaitTime = !isMeasureDuration;
            // bool writeOriginalWaitTime = true; // This trigger is only used once
            for (int i = result[0].Contains('_') ? 1 : 0; i < result.Count; i++)
            {
                if (writeOriginalWaitTime)
                {
                    result[i] = ReplaceDuration(result[i],
                        $"[{Math.Round(originalWaitDuration, 4)}##{Math.Round(averageDuration, 4)}]");
                    writeOriginalWaitTime = false;
                }
                else result[i] = ReplaceDuration(result[i], newDurationCandidate);
            }
        }

        return result;
    }

    public static bool ContainsSlideNotation(string token)
        => token.Contains('-') ||
           token.Contains('v') ||
           token.Contains('w') ||
           token.Contains('<') ||
           token.Contains('>') ||
           token.Contains('p') ||
           token.Contains('q') ||
           token.Contains('s') ||
           token.Contains('z') ||
           token.Contains('V') ||
           token.Contains('^');

    public static bool IsSlideNotation(char token)
        => token == '-' ||
           token == 'v' ||
           token == 'w' ||
           token == '<' ||
           token == '>' ||
           token == 'p' ||
           token == 'q' ||
           token == 's' ||
           token == 'z' ||
           token == 'V' ||
           token == '^';

    public static int KeyDistance(int startKey, int endKey, NoteType direction)
    {
        int result = startKey == endKey ? 8 : 0;
        if (result != 8)
            switch (direction)
            {
                case NoteType.SCL:
                    result = startKey - endKey;
                    if (result <= 0) result += 8;
                    break;
                case NoteType.SCR:
                    result = endKey - startKey;
                    if (result <= 0) result += 8;
                    break;
            }

        return result;
    }

    public static double[] GetTimeCandidates(string input)
    {
        if (input.Contains(':') && !input.Contains('#'))
            throw new InvalidOperationException($"NO BPM CONTEXT IN THIS SETTING: {input}"); // [Quaver : Beats]
        return GetTimeCandidates(0, input);
    }

    public static double[] GetTimeCandidates(double bpm, string input)
    {
        double[] result = new double[2];
        if (!(input.Contains('[') || input.Contains(']')))
            throw new InvalidOperationException("GIVEN CANDIDATE DOES NOT CONTAIN DURATION SYMBOL [ AND ]");
        string durationCandidate = input.Replace("[", "").Replace("]", "");
        bool isMeasureDuration = input.Contains(':') && !input.Contains('#'); // [Quaver : Beats]
        bool isSlideTimedDuration = input.Contains("##") && !input.Contains(':'); // [WaitTime ## LastTime]
        bool isHoldTimedDuration = !isSlideTimedDuration && input.Contains('#') && !input.Contains(':'); // [# LastTime]
        bool isSlideBpmMeasureDuration =
            input.Contains("##") && input.Contains('#') && input.Contains(':'); // [WaitTime ## BPM # Quaver : Beats]

        bool isHoldBpmMeasureDuration =
            !isSlideBpmMeasureDuration && input.Contains('#') && input.Contains(':'); // [BPM # Quaver : Beats]

        if (isMeasureDuration)
        {
            double quaver = double.Parse(durationCandidate.Split(':')[0]);
            double beat = double.Parse(durationCandidate.Split(':')[1]);
            // double waitTimeCandidate = Chart.GetBPMTimeUnit(bpm, MaximumDefinition) * 96;
            double lastTimeCandidate =
                Chart.GetBPMTimeUnit(bpm, MaximumDefinition) * (MaximumDefinition / quaver) * beat;
            result[0] = 0;
            // result[0] = waitTimeCandidate;
            result[1] = lastTimeCandidate;
            return result;
        }
        else if (isSlideTimedDuration)
        {
            result[0] = double.Parse(durationCandidate.Split("##")[0]);
            result[1] = double.Parse(durationCandidate.Split("##")[1]);
            return result;
        }
        else if (isHoldTimedDuration)
        {
            bool isSlideReassignedFormat = durationCandidate.Split('#')[0].Length != 0;
            result[0] = isSlideReassignedFormat
                ? Chart.GetBPMTimeUnit(double.Parse(durationCandidate.Split('#')[0]), MaximumDefinition) * 96
                : 0;
            result[1] = isSlideReassignedFormat
                ? double.Parse(durationCandidate.Split('#')[1])
                : double.Parse(durationCandidate.Replace("#", ""));
            return result;
        }
        else if (isSlideBpmMeasureDuration)
        {
            result[0] = double.Parse(durationCandidate.Split("##")[0]);
            string extractedDurationCandidate = durationCandidate.Split("##")[1];
            double bpmCandidate = double.Parse(extractedDurationCandidate.Split('#')[0]);
            string extractedQuaverBeatCandidate = extractedDurationCandidate.Split('#')[1];
            double quaverCandidate = double.Parse(extractedQuaverBeatCandidate.Split(':')[0]);
            double beatCandidate = double.Parse(extractedQuaverBeatCandidate.Split(':')[1]);
            double lastTimeCandidate =
                Chart.GetBPMTimeUnit(bpmCandidate, MaximumDefinition) * (MaximumDefinition / quaverCandidate) *
                beatCandidate;
            result[1] = lastTimeCandidate;
            return result;
        }
        else if (isHoldBpmMeasureDuration)
        {
            double bpmCandidate = double.Parse(durationCandidate.Split('#')[0]);
            string extractedQuaverBeatCandidate = durationCandidate.Split('#')[1];
            double quaverCandidate = double.Parse(extractedQuaverBeatCandidate.Split(':')[0]);
            double beatCandidate = double.Parse(extractedQuaverBeatCandidate.Split(':')[1]);
            // double waitTimeCandidate =
            //     Chart.GetBPMTimeUnit(bpmCandidate, MaximumDefinition) * 96;
            double lastTimeCandidate =
                Chart.GetBPMTimeUnit(bpmCandidate, MaximumDefinition) * (MaximumDefinition / quaverCandidate) *
                beatCandidate;
            // result[0] = waitTimeCandidate;
            result[0] = 0;
            result[1] = lastTimeCandidate;
            return result;
        }
        else throw new InvalidOperationException($"NON OF THE DURATION PATTERNS MATCHED: {durationCandidate}");
    }

    public static double[] GetTimeCandidates(double bpm, string input, bool isSlide)
    {
        double[] result = GetTimeCandidates(bpm, input);
        // if (!(input.Contains('[') || input.Contains(']')))
        //     throw new InvalidOperationException("GIVEN CANDIDATE DOES NOT CONTAIN DURATION SYMBOL [ AND ]");
        string durationCandidate = input.Replace("[", "").Replace("]", "");
        bool isMeasureDuration = input.Contains(':') && !input.Contains('#'); // [Quaver : Beats]
        bool isSlideTimedDuration = input.Contains("##") && !input.Contains(':'); // [WaitTime ## LastTime]
        bool isHoldTimedDuration = !isSlideTimedDuration && input.Contains('#') && !input.Contains(':'); // [# LastTime]
        bool isSlideBpmMeasureDuration =
            input.Contains("##") && input.Contains('#') && input.Contains(':'); // [WaitTime ## BPM # Quaver : Beats]
        bool isHoldBpmMeasureDuration =
            !isSlideBpmMeasureDuration && input.Contains('#') && input.Contains(':'); // [BPM # Quaver : Beats]
        if ((isMeasureDuration || isHoldTimedDuration) && isSlide)
        {
            double waitTimeCandidate =
                Chart.GetBPMTimeUnit(bpm, MaximumDefinition) * 96;
            result[0] = waitTimeCandidate;
        }
        else if (isHoldBpmMeasureDuration && isSlide)
        {
            double bpmCandidate = double.Parse(durationCandidate.Split('#')[0]);
            double waitTimeCandidate =
                Chart.GetBPMTimeUnit(bpmCandidate, MaximumDefinition) * 96;
            result[0] = waitTimeCandidate;
        }

        return result;
    }

    #endregion
}

public class ParsingException(Exception ex, int bar, int tick, string token, string tokenSet)
    : Exception(
        $"Simai Parser encountered an error while parsing after bar {bar} tick {tick} when parsing token {token} in each group “{tokenSet}”: \n{ex.Message}\nOriginal Stack:\n{ex.StackTrace}\n");
