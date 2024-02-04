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

    /// <summary>
    ///     Constructor of simaiparser
    /// </summary>
    public SimaiParser()
    {
        PreviousSlideStart = new Tap();
    }

    public Chart ChartOfToken(string[] tokens)
        // Note: here chart will only return syntax after &inote_x= and each token is separated by ","
    {
        var notes = new List<Note>();
        var bpmChanges = new BPMChanges();
        var measureChanges = new MeasureChanges(4, 4);
        var bar = 0;
        var tick = 0;
        var currentBPM = 0.0;
        var tickStep = 0;
        for (var i = 0; i < tokens.Length; i++)
        {
            var eachPairCandidates = EachGroupOfToken(tokens[i]);
            string currentToken = ""; // Solely used to log current error
            int previousConnectingSlideTick = 0;
            try
            {
                foreach (var eachNote in eachPairCandidates)
                {
                    currentToken = eachNote;
                    // if (bar == 6)
                    // {
                    //     Console.WriteLine("This is bar 6");
                    // }
                    Note noteCandidate = NoteOfToken(eachNote, bar, tick, currentBPM);
                    var containsBPM = noteCandidate.NoteSpecificGenre is NoteSpecificGenre.BPM;
                    var containsMeasure = noteCandidate.NoteSpecificGenre is NoteSpecificGenre.MEASURE;

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
                        var quaverCandidate = eachNote.Replace("{", "").Replace("}", "");
                        tickStep = MaximumDefinition / int.Parse(quaverCandidate);
                        // MeasureChange changeNote = new MeasureChange(bar, tick, tickStep);
                        //notes.Add(changeNote);
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
        var isRest = token.Equals("");
        var isBPM = token.Contains(")");
        var isMeasure = token.Contains("}");
        var isSlide = ContainsSlideNotation(token);
        var isHold = !isSlide && token.Contains('h');

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
                var bpmCandidate = token.Replace("(", "").Replace(")", "");
                result = new BPMChange(bar, tick, double.Parse(bpmCandidate));
            }
            else if (isMeasure)
            {
                var quaverCandidate = token.Replace("{", "").Replace("}", "");
                result = new MeasureChange(bar, tick, int.Parse(quaverCandidate));
            }
            else if (!token.Contains('!') && !token.Equals("E") && !token.Equals(""))
            {
                result = TapOfToken(token, bar, tick, bpm);
                if (result.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START) PreviousSlideStart = (Tap)result;
            }
            else if (token.Contains('!'))
            {
                PreviousSlideStart = TapOfToken(token, bar, tick, bpm);
            }
        }

        return result;
    }

    public Tap TapOfToken(string token, int bar, int tick, double bpm)
    {
        var isEXBreak = token.Contains("b") && token.Contains("x");
        var isBreak = token.Contains("b") && !token.Contains("x");
        var isEXTap = token.Contains("x") && !token.Contains("b");
        var isTouch = token.Contains("A") ||
                      token.Contains("B") ||
                      token.Contains("C") ||
                      token.Contains("D") ||
                      token.Contains("E") ||
                      token.Contains("F");
        var result = new Tap();
        if (isTouch)
        {
            var hasSpecialEffect = token.Contains("f");
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
            var keyCandidate = int.Parse(token.Substring(0, 1)) - 1;
            if (token.Contains('_')||token.Contains('$'))
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
        var sustainSymbol = token.IndexOf("[");
        var keyCandidate = token.Substring(0, sustainSymbol); //key candidate is like tap grammar
        //Console.WriteLine(keyCandidate);
        var sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
        //Console.WriteLine(sustainCandidate);
        var key = "";
        var holdType = "";
        var specialEffect = 0;
        // bool sustainIsSecond = sustainCandidate.Contains("##");
        // if (sustainIsSecond)
        // {
        //     string[] secondCandidates = sustainCandidate.Split("##");

        // }
        SpecialState specialState = SpecialState.Normal;
        if (keyCandidate.Contains("C"))
        {
            holdType = "THO";
            key = "0C";
            if (keyCandidate.Contains("f")) specialEffect = 1;
        }
        else if (keyCandidate.Contains("x") && keyCandidate.Contains("b"))
        {
            key = keyCandidate.Replace("h", "");
            key = key.Replace("x", "");
            key = key.Replace("b", "");
            key = (int.Parse(key) - 1).ToString();
            holdType = "HLD";
            specialState = SpecialState.BreakEX;
        }
        else if (keyCandidate.Contains("b"))
        {
            key = keyCandidate.Replace("h", "");
            key = key.Replace("b", "");
            key = (int.Parse(key) - 1).ToString();
            holdType = "HLD";
            specialState = SpecialState.Break;
        }
        else if (keyCandidate.Contains("x"))
        {
            key = keyCandidate.Replace("h", "");
            key = key.Replace("x", "");
            key = (int.Parse(key) - 1).ToString();
            holdType = "HLD";
            specialState = SpecialState.EX;
        }
        else
        {
            key = keyCandidate.Replace("h", "");
            //Console.WriteLine(key);
            key = (int.Parse(key) - 1).ToString();
            holdType = "HLD";
        }

        var lastTimeCandidates = sustainCandidate.Split(":");
        var quaver = int.Parse(lastTimeCandidates[0]);
        var lastTick = MaximumDefinition / quaver;
        var times = int.Parse(lastTimeCandidates[1]);
        lastTick *= times;
        Hold candidate;
        bool noteTypeIsValid = Enum.TryParse(holdType, out NoteType typeCandidate);
        if (!noteTypeIsValid)
        {
            throw new Exception("Note type is invalid: Token given = " + holdType);
        }

        //Console.WriteLine(key);
        candidate = holdType.Equals("THO") ? new Hold(NoteType.THO, bar, tick, key, lastTick, specialEffect == 1, "M1") : new Hold(typeCandidate, bar, tick, key, lastTick);
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
        List<Slide> slideCandidates = new();
        foreach (string x in extractedTokens)
        {
            Slide connectCandidate = SlideOfToken(x, currentBar, currentTick, slideStart, bpm);
            // prevSlideKey = connectCandidate.EndKeyNum;
            // int[] endPointOfConcern = { 0, 1, 6, 7 };
            // if (connectCandidate.NoteType is NoteType.SCL && endPointOfConcern.Any(p => p == prevSlideKey))
            //     connectCandidate.NoteType = NoteType.SCR;
            // else if (connectCandidate.NoteType is NoteType.SCR && endPointOfConcern.Any(p => p == prevSlideKey))
            //     connectCandidate.NoteType = NoteType.SCL;
            slideCandidates.Add(connectCandidate);
            currentTick += connectCandidate.WaitLength + connectCandidate.LastLength;
            if (currentTick >= MaximumDefinition)
            {
                currentBar += currentTick / MaximumDefinition;
                currentTick %= MaximumDefinition;
            }
        }

        return new SlideGroup(slideCandidates);
    }

    public Slide SlideOfToken(string token, int bar, int tick, Note slideStart, double bpm)
    {
        Note slideStartCandidate = new Tap(slideStart);
        Note result;
        var endKeyCandidate = "";
        var sustainSymbol = 0;
        var sustainCandidate = "";
        NoteType noteType = NoteType.RST;
        bool isConnectingSlide = token.Contains("CN");
        int connectedSlideStart = isConnectingSlide ? int.Parse(token.Split("CN")[1]) : -1;
        if (isConnectingSlide) slideStartCandidate.Key = connectedSlideStart.ToString();
        var isEXBreak = token.Contains("b") && token.Contains("x");
        var isBreak = token.Contains("b") && !token.Contains("x");
        var isEX = !token.Contains("b") && token.Contains("x");
        SpecialState noteSpecialState = SpecialState.Normal;
        if (isEXBreak) noteSpecialState = SpecialState.BreakEX;
        else if (isBreak) noteSpecialState = SpecialState.Break;
        else if (isEX) noteSpecialState = SpecialState.EX;
        var timeAssigned = token.Contains("[");
        //Parse first section
        if (token.Contains("qq"))
        {
            endKeyCandidate = token.Substring(2, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SXR;
        }
        else if (token.Contains("q"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SUR;
        }
        else if (token.Contains("pp"))
        {
            endKeyCandidate = token.Substring(2, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SXL;
        }
        else if (token.Contains("p"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SUL;
        }
        else if (token.Contains("v"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SV_;
        }
        else if (token.Contains("w"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SF_;
        }
        else if (token.Contains("<"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            int[] endPointOfConcern = { 0, 1, 6, 7 };
            if (endPointOfConcern.Any(p => p == slideStartCandidate.KeyNum))
                noteType = NoteType.SCL;
            else noteType = NoteType.SCR;
            // noteType = NoteType.SCL;
        }
        else if (token.Contains(">"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            int[] endPointOfConcern = { 0, 1, 6, 7 };
            if (endPointOfConcern.Any(p => p == slideStartCandidate.KeyNum))
                noteType = NoteType.SCR;
            else noteType = NoteType.SCL;
            // noteType = NoteType.SCR;
        }
        else if (token.Contains("^"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            int endKeyNum = int.Parse(endKeyCandidate) - 1;
            if (endKeyNum < 0) endKeyNum += 8;
            int sclDistance = KeyDistance(slideStartCandidate.KeyNum, endKeyNum, NoteType.SCL);
            int scrDistance = KeyDistance(slideStartCandidate.KeyNum, endKeyNum, NoteType.SCR);
            if (sclDistance >= 4 && scrDistance >= 4)
                throw new Exception(
                    $"^ requires a distance 0<d<4. SCL distance: {sclDistance}, SCR distance: {scrDistance}. Connected Start Key: {connectedSlideStart} StartKey: {slideStartCandidate.KeyNum}, EndKey: {endKeyCandidate}, Token: {token}");
            else noteType = sclDistance < scrDistance ? NoteType.SCL : NoteType.SCR;
        }
        else if (token.Contains("s"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SSL;
        }
        else if (token.Contains("z"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SSR;
        }
        else if (token.Contains("V"))
        {
            endKeyCandidate = token.Substring(2, 1);
            var sllCandidate = int.Parse(slideStartCandidate.Key) - 2;
            var slrCandidate = int.Parse(slideStartCandidate.Key) + 2;
            var inflectionCandidate = int.Parse(token.Substring(1, 1)) - 1;

            // Revalue SLL and SLR candidate
            if (sllCandidate < 0)
                sllCandidate += 8;
            else if (sllCandidate > 7) sllCandidate -= 8;
            if (slrCandidate < 0)
                slrCandidate += 8;
            else if (slrCandidate > 7) slrCandidate -= 8;

            var isSLL = sllCandidate == inflectionCandidate;
            var isSLR = slrCandidate == inflectionCandidate;
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            if (isSLL)
                noteType = NoteType.SLL;
            else if (isSLR) noteType = NoteType.SLR;
            if (!(isSLL || isSLR))
            {
                Console.WriteLine("Start Key:" + slideStartCandidate.Key);
                Console.WriteLine("Expected inflection point: SLL for " + sllCandidate + " and SLR for " +
                                  slrCandidate);
                Console.WriteLine("Actual: " + inflectionCandidate);
                throw new InvalidDataException("THE INFLECTION POINT GIVEN IS NOT MATCHING!");
            }
        }

        else if (token.Contains("-"))
        {
            endKeyCandidate = token.Substring(1, 1);
            sustainSymbol = token.IndexOf("[");
            sustainCandidate = token.Substring(sustainSymbol + 1).Split("]")[0]; //sustain candidate is like 1:2
            noteType = NoteType.SI_;
        }

        //Console.WriteLine("Key Candidate: "+keyCandidate);
        var fixedKeyCandidate = int.Parse(endKeyCandidate) - 1;
        if (fixedKeyCandidate < 0) endKeyCandidate += 8;
        var isSecond = sustainCandidate.Contains("##");
        if (!isSecond)
        {
            var lastTimeCandidates = sustainCandidate.Split(":");
            var quaver = timeAssigned ? int.Parse(lastTimeCandidates[0]) : 0;
            var lastTick = timeAssigned ? MaximumDefinition / quaver : 0;
            var times = timeAssigned ? int.Parse(lastTimeCandidates[1]) : 0;
            lastTick *= times;
            result = new Slide(noteType, bar, tick, slideStartCandidate.Key, 96, lastTick,
                fixedKeyCandidate.ToString());
        }
        else
        {
            var timeCandidates = sustainCandidate.Split("##");
            var waitLengthCandidate = timeAssigned ? double.Parse(timeCandidates[0]) : 0;
            var lastLengthCandidate = timeAssigned ? double.Parse(timeCandidates[1]) : 0;
            var tickUnit = Chart.GetBPMTimeUnit(bpm, MaximumDefinition);
            var waitLength = timeAssigned ? (int)(waitLengthCandidate / tickUnit) : 0;
            var lastLength = timeAssigned ? (int)(lastLengthCandidate / tickUnit) : 0;
            result = new Slide(noteType, bar, tick, slideStartCandidate.Key, waitLength, lastLength,
                fixedKeyCandidate.ToString());
            result.CalculatedWaitTime = waitLengthCandidate;
            result.CalculatedLastTime = lastLengthCandidate;
        }

        result.BPM = bpm;
        if (isConnectingSlide)
        {
            result.NoteSpecialState = SpecialState.ConnectingSlide;
            result.Key = connectedSlideStart.ToString();
            result.WaitLength = 0;
        }
        else result.NoteSpecialState = noteSpecialState;

        // int[] endPointOfConcern = { 0, 1, 6, 7 };
        // if (result.NoteType is NoteType.SCL && KeyDistance(result.KeyNum,result.EndKeyNum,NoteType.SCL) > 4)
        //     result.NoteType = NoteType.SCR;
        // else if (result.NoteType is NoteType.SCR && KeyDistance(result.KeyNum,result.EndKeyNum,NoteType.SCL) > 4)
        //     result.NoteType = NoteType.SCL;

        return (Slide)result;
    }

    #region UnimplementedDummyMethods
    public Note NoteOfToken(string token)
    {
        Note result = new Rest();
        var isRest = token.Equals("");
        var isBPM = token.Contains(")");
        var isMeasure = token.Contains("}");
        var isSlide = ContainsSlideNotation(token);
        var isHold = !isSlide && token.Contains("[");
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
    ///     Deal with old, out-fashioned and illogical Simai Each Groups.
    /// </summary>
    /// <param name="token">Tokens that potentially contains each Groups</param>
    /// <returns>List of strings that is composed with single note.</returns>
    public static List<string> EachGroupOfToken(string token)
    {
        var result = new List<string>();
        var isSlide = ContainsSlideNotation(token);
        if (token.Contains("/"))
        {
            var candidate = token.Split("/");
            foreach (var tokenCandidate in candidate) result.AddRange(EachGroupOfToken(tokenCandidate));
        }
        else if (token.Contains(")") || token.Contains("}"))
        {
            var resultCandidate = ExtractParentheses(token);
            List<string> fixedCandidate = new();
            foreach (var candidate in resultCandidate) fixedCandidate.AddRange(ExtractParentheses(candidate));
            foreach (var candidate in fixedCandidate) result.AddRange(ExtractEachSlides(candidate));
        } //lol this is the most stupid code I have ever wrote
        else if (int.TryParse(token, out var eachPair))
        {
            var eachPairs = token.ToCharArray();
            foreach (var x in eachPairs) result.Add(x.ToString());
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
        else
        {
            result.Add(token);
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
        var result = new List<string>();
        var containsBPM = token.Contains(")");
        var containsMeasure = token.Contains("}");

        if (containsBPM)
        {
            var tokenCandidate = token.Split(")");
            List<string> tokenResult = new();
            for (var i = 0; i < tokenCandidate.Length; i++)
            {
                var x = tokenCandidate[i];
                if (x.Contains("(")) x += ")";
                if (!x.Equals("")) tokenResult.Add(x);
            }

            result.AddRange(tokenResult);
        }
        else if (containsMeasure)
        {
            var tokenCandidate = token.Split("}");
            List<string> tokenResult = new();
            for (var i = 0; i < tokenCandidate.Length; i++)
            {
                var x = tokenCandidate[i];
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
        var isSlide = ContainsSlideNotation(token);
        var result = new List<string>();
        if (!isSlide)
        {
            result.Add(token);
        }
        else
        {
            var candidates = token.Split("*");
            foreach (var symbol in candidates) result.AddRange(ExtractSlideStart(symbol));
            // result.AddRange(candidates);
        }

        return result;
    }

    public static List<string> ExtractSlideStart(string token)
    {
        List<string> result = new();
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
        List<string> result = new();
        var candidates = token.ToCharArray();

        static string KeyCandidate(string token)
        {
            string result = "";
            for (var i = 0; i < token.Length && result.Length < 3; i++)
                if (!IsSlideNotation(token[i]) && char.IsNumber(token[i]))
                    result += token[i].ToString();
            if (result.Length > 1) result = result[1].ToString();
            return ((int.Parse(result) - 1) % 8).ToString();
            // string slideNotation = AllowedSlideType.First(token.Contains) ?? throw new InvalidOperationException($"GIVEN TOKEN DOES NOT CONTAIN ANY SLIDE NOTATION: {token}");
            // string keyCandidate = token.Split(slideNotation)[1] ?? throw new InvalidOperationException($"GIVEN TOKEN CANNOT BE SEPARATED: {token}");
            // if (keyCandidate.Length == 2) keyCandidate = keyCandidate[1..];
            // return ((int.Parse(keyCandidate) - 1) % 8).ToString();
        }

        var slideStartExtracted = false;
        var normalSlideExtracted = false;
        var slideStartCandidate = "";
        var slideCandidate = "";
        var previousSlideNoteBuffer = "";
        var lastKeyCandidate = "";

        foreach (var symbol in candidates)
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
            if (newDurationCandidate.Contains("##"))
            {
                originalWaitDuration =
                    double.Parse(slideDurationCandidate.Split('[')[1].Split("##")[0]);
                averageDuration =
                    double.Parse(slideDurationCandidate.Split("##")[1].Split(']')[0]) / actualSlidePart;
                newDurationCandidate = $"[0##{Math.Round(averageDuration, 4)}]";
            }
            else if (newDurationCandidate.Contains(':'))
            {
                string durationCandidate = newDurationCandidate.Replace("[", "").Replace("]", "");
                string[] numList = durationCandidate.Split(':');
                int quaver = int.Parse(numList[0]) * actualSlidePart;
                int multiple = int.Parse(numList[1]);
                newDurationCandidate = $"[{quaver}:{multiple}]";
            }
            else throw new NotImplementedException("BPM # QUAVER : MULTIPLE is not yet supported");

            bool writeOriginalWaitTime = newDurationCandidate.Contains("##");
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

        // else if (result.Count(p=>p.Contains('[')) != result.Count - 1) throw new Exception("Duration must either be assigned individually or only once: " + String.Join(", ", result));
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
    #endregion
}

public class ParsingException(Exception ex, int bar, int tick, string token, string tokenSet)
    : Exception(
        $"Simai Parser encountered an error while parsing after bar {bar} tick {tick} when parsing token {token} in each group “{tokenSet}”: \n{ex.Message}\nOriginal Stack:\n{ex.StackTrace}\n");
