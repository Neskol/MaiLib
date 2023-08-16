using System.Numerics;
using System.Reflection;
using System.Reflection.Emit;
using System.Reflection.Metadata;
using System.Resources;

namespace MaiLib
{
    /// <summary>
    /// Basic note
    /// </summary>
    public abstract class Note : IEquatable<Note>, INote, IComparable
    {
        /// <summary>
        /// The note type
        /// </summary>
        public string NoteType { get; set; }

        /// <summary>
        /// The key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// The end key
        /// </summary>
        public string EndKey { get; set; }

        /// <summary>
        /// The bar
        /// </summary>
        public int Bar { get; set; }

        /// <summary>
        /// The start time
        /// </summary>
        public int Tick { get; set; }

        /// <summary>
        /// Start time fixed to BPM
        /// </summary>
        public int FixedTick { get; set; }

        /// <summary>
        /// The absolute Tick Calculated by this.bar*384+this.Tick
        /// </summary>
        public int TickStamp { get; set; }

        /// <summary>
        /// The start time stamp
        /// </summary>
        public double TickTimeStamp { get; set; }

        /// <summary>
        /// The Wait length
        /// </summary>
        public int WaitLength { get; set; }

        /// <summary>
        /// The stamp of Wait time ends in Ticks
        /// </summary>
        public int WaitTickStamp { get; set; }

        /// <summary>
        /// The stamp when the Wait time ends in seconds
        /// </summary>
        public double WaitTimeStamp;

        /// <summary>
        /// The Calculated Wait time in seconds
        /// </summary>
        public double CalculatedWaitTime { get; set; }

        /// <summary>
        /// The Last length
        /// </summary>
        public int LastLength { get; set; }

        /// <summary>
        /// Fixed Tick Last length with fixed BPM
        /// </summary>
        public int FixedLastLength { get; set; }

        /// <summary>
        /// The stamp when the Last time ends in Ticks
        /// </summary>
        public int LastTickStamp { get; set; }

        /// <summary>
        /// The stamp when the Last time ends in seconds
        /// </summary>
        public double LastTimeStamp { get; set; }

        /// <summary>
        /// The Calculated Last time
        /// </summary>
        public double CalculatedLastTime { get; set; }

        /// <summary>
        /// Stores if the BPM of Wait or Last Tick is in different BPM
        /// </summary>
        public bool TickBPMDisagree { get; set; }

        /// <summary>
        /// The delayed
        /// </summary>
        public bool Delayed { get; set; }

        /// <summary>
        /// The BPM
        /// </summary>
        public double BPM { get; set; }

        /// <summary>
        /// The previous note
        /// </summary>
        public Note? Prev { get; set; }

        /// <summary>
        /// The next note
        /// </summary>
        public Note? Next { get; set; }

        /// <summary>
        /// Stores all BPM change prior to this
        /// </summary>
        public List<BPMChange> BPMChangeNotes { get; set; }

        /// <summary>
        /// Construct an empty note
        /// </summary>
        public Note()
        {
            this.NoteType = "";
            this.Key = "";
            this.EndKey = "";
            this.Bar = 0;
            this.Tick = 0;
            this.FixedTick = 0;
            this.TickStamp = 0;
            this.TickTimeStamp = 0.0;
            this.LastLength = 0;
            this.LastTickStamp = 0;
            this.LastTimeStamp = 0.0;
            this.WaitLength = 0;
            this.WaitTickStamp = 0;
            this.WaitTimeStamp = 0.0;
            this.CalculatedLastTime = 0.0;
            this.CalculatedWaitTime = 0.0;
            this.TickBPMDisagree = false;
            this.BPM = 0;
            this.BPMChangeNotes = new List<BPMChange>();
        }

        /// <summary>
        /// Construct a note from other note
        /// </summary>
        /// <param name="inTake">The intake note</param>
        public Note(Note inTake)
        {
            this.NoteType = inTake.NoteType;
            this.Key = inTake.Key;
            this.EndKey = inTake.EndKey;
            this.Bar = inTake.Bar;
            this.Tick = inTake.Tick;
            this.TickStamp = inTake.TickStamp;
            this.TickTimeStamp = inTake.TickTimeStamp;
            this.LastLength = inTake.LastLength;
            this.LastTickStamp = inTake.LastTickStamp;
            this.LastTimeStamp = inTake.LastTimeStamp;
            this.WaitLength = inTake.WaitLength;
            this.WaitTickStamp = inTake.WaitTickStamp;
            this.WaitTimeStamp = inTake.WaitTimeStamp;
            this.CalculatedLastTime = inTake.CalculatedLastTime;
            this.CalculatedLastTime = inTake.CalculatedLastTime;
            this.TickBPMDisagree = inTake.TickBPMDisagree;
            this.BPM = inTake.BPM;
            this.BPMChangeNotes = inTake.BPMChangeNotes;
        }

        public enum SpecialState
        {
            /// <summary>
            /// Normal note, nothing special
            /// </summary>
            Normal,
            /// <summary>
            /// Break note
            /// </summary>
            Break,
            /// <summary>
            /// EX Note
            /// </summary>
            EX,
            /// <summary>
            /// EX Break
            /// </summary>
            BreakEX,
            /// <summary>
            /// Connecting Slide
            /// </summary>
            ConnectingSlide
        }

        public SpecialState NoteSpecialState { get; set; }

        /// <summary>
        /// Get the number value of Key
        /// </summary>
        /// <value>Number value of Key 0-7, exclude key group</value>
        public int KeyNum
        {
            get => int.Parse(this.Key.ToCharArray()[0].ToString());
        }

        /// <summary>
        /// Get the key group of the key - only for touch notes
        /// </summary>
        /// <value>Key group of the touch note</value>
        public string KeyGroup
        {
            get
            {
                string result = "";
                if (this.Key.ToCharArray().Count() > 1)
                {
                    result = this.Key.ToCharArray()[1].ToString();
                }
                return result;
            }
        }

        /// <summary>
        /// Get the number value of End Key
        /// </summary>
        /// <value>Number value of Key 0-7, exclude key group</value>
        public int EndKeyNum
        {
            get
            {
                int result = 0;
                if (!this.EndKey.Equals(""))
                {
                    result = int.Parse(this.EndKey.ToCharArray()[0].ToString());
                }
                return result;
            }
        }

        /// <summary>
        /// Judges if two notes are of same time
        /// </summary>
        /// <param name="x">Comparing note</param>
        /// <returns>True if BAR=BAR & TICK=TICK</returns>
        public bool IsOfSameTime(Note x)
        {
            return this.Bar == x.Bar && this.Tick == x.Tick;
        }

        /// <summary>
        /// Return this.SpecificType
        /// </summary>
        /// <returns>string of specific genre (specific type of Tap, Slide, etc.)</returns>
        public abstract string NoteSpecificGenre { get; }

        /// <summary>
        /// Return this.noteGenre
        /// </summary>
        /// <returns>string of note genre (general category of TAP, SLIDE and HOLD)</returns>
        public abstract string NoteGenre { get; }

        /// <summary>
        /// Return if this is a true note
        /// </summary>
        /// <returns>True if is TAP,HOLD or SLIDE, false otherwise</returns>
        public abstract bool IsNote { get; }

        public abstract bool CheckValidity();

        public abstract string Compose(int format);

        public abstract Note NewInstance();

        public int CompareTo(Object? obj)
        {
            int result = 0;

            Note another = obj as Note ?? throw new NullReferenceException("Note is not defined");

            //else if (this.NoteSpecificType().Equals("SLIDE")&&(this.NoteSpecificType().Equals("TAP")|| this.NoteSpecificType().Equals("HOLD")) && this.Tick == another.Tick && this.bar == another.Bar)
            //{
            //    result = -1;
            //}
            //else if (this.NoteSpecificType().Equals("SLIDE_START") && (another.NoteSpecificType().Equals("TAP") || another.NoteSpecificType().Equals("HOLD")) && this.Tick == another.Tick && this.bar == another.Bar)
            //{
            //    Console.WriteLine("STAR AND TAP");
            //    result = 1;
            //    Console.WriteLine(this.NoteSpecificType() + ".compareTo(" + another.NoteSpecificType() + ") is" + result);
            //    //Console.ReadKey();
            //}
            //if (this.Bar==another.Bar&&this.Tick==another.Tick)
            //{
            //    if (this.NoteGenre().Equals("BPM"))
            //    {
            //        result = -1;
            //    }
            //    else if (this.NoteGenre().Equals("MEASURE"))
            //    {
            //        result = 1;
            //    }
            //    else if ((this.NoteSpecificType().Equals("TAP")|| this.NoteSpecificType().Equals("HOLD"))&&another.NoteSpecificType().Equals("SLIDE_START"))
            //    {
            //        result= -1;
            //    }
            //}
            //else
            //{
            //    if (this.bar != another.Bar)
            //    {
            //        result = this.bar.CompareTo(another.Bar);
            //        //Console.WriteLine("this.compareTo(another) is" + result);
            //        //Console.ReadKey();
            //    }
            //    else result = this.Tick.CompareTo(another.Tick);
            //}
            if (this.Bar != another.Bar)
            {
                result = this.Bar.CompareTo(another.Bar);
            }
            else if (this.Bar == another.Bar && (this.Tick != another.Tick))
            {
                result = this.Tick.CompareTo(another.Tick);
            }
            else
            {
                if (this.NoteSpecificGenre.Equals("BPM"))
                {
                    result = -1;
                }
                //else if (this.NoteSpecificType().Equals("SLIDE")&&another.NoteSpecificType().Equals("SLIDE_START")&&this.Key.Equals(another.Key))
                //{
                //    result = 1;
                //}
                else result = 0;
            }
            return result;
        }

        public bool Equals(Note? other)
        {
            bool result = false;
            if (other != null &&
            this.NoteType.Equals(other.NoteType) &&
            this.Key.Equals(other.Key) &&
            this.EndKey.Equals(other.EndKey) &&
            this.Bar == other.Bar &&
            this.Tick == other.Tick &&
            this.LastLength == other.LastLength &&
            this.BPM == other.BPM)
            {
                result = true;
            }
            return result;
        }

        public virtual void Flip(string method)
        {
            if (this.Key != null && !this.Key.Equals("") && !(this.Key.Count() > 1 && this.Key.ToCharArray()[1] == 'C'))
            {
                #region FlipConditions
                switch (method)
                {
                    case "Clockwise90":
                        switch (this.KeyNum)
                        {
                            case 0:
                                this.Key = "2" + this.KeyGroup;
                                break;
                            case 1:
                                this.Key = "3" + this.KeyGroup;
                                break;
                            case 2:
                                this.Key = "4" + this.KeyGroup;
                                break;
                            case 3:
                                this.Key = "5" + this.KeyGroup;
                                break;
                            case 4:
                                this.Key = "6" + this.KeyGroup;
                                break;
                            case 5:
                                this.Key = "7" + this.KeyGroup;
                                break;
                            case 6:
                                this.Key = "0" + this.KeyGroup;
                                break;
                            case 7:
                                this.Key = "1" + this.KeyGroup;
                                break;
                        }
                        switch (this.EndKeyNum)
                        {
                            case 0:
                                this.EndKey = "2";
                                break;
                            case 1:
                                this.EndKey = "3";
                                break;
                            case 2:
                                this.EndKey = "4";
                                break;
                            case 3:
                                this.EndKey = "5";
                                break;
                            case 4:
                                this.EndKey = "6";
                                break;
                            case 5:
                                this.EndKey = "7";
                                break;
                            case 6:
                                this.EndKey = "0";
                                break;
                            case 7:
                                this.EndKey = "1";
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Clockwise180":
                        switch (this.KeyNum)
                        {
                            case 0:
                                this.Key = "4" + this.KeyGroup;
                                break;
                            case 1:
                                this.Key = "5" + this.KeyGroup;
                                break;
                            case 2:
                                this.Key = "6" + this.KeyGroup;
                                break;
                            case 3:
                                this.Key = "7" + this.KeyGroup;
                                break;
                            case 4:
                                this.Key = "0" + this.KeyGroup;
                                break;
                            case 5:
                                this.Key = "1" + this.KeyGroup;
                                break;
                            case 6:
                                this.Key = "2" + this.KeyGroup;
                                break;
                            case 7:
                                this.Key = "3" + this.KeyGroup;
                                break;
                        }
                        switch (this.EndKeyNum)
                        {
                            case 0:
                                this.EndKey = "4";
                                break;
                            case 1:
                                this.EndKey = "5";
                                break;
                            case 2:
                                this.EndKey = "6";
                                break;
                            case 3:
                                this.EndKey = "7";
                                break;
                            case 4:
                                this.EndKey = "0";
                                break;
                            case 5:
                                this.EndKey = "1";
                                break;
                            case 6:
                                this.EndKey = "2";
                                break;
                            case 7:
                                this.EndKey = "3";
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Counterclockwise90":
                        switch (this.KeyNum)
                        {
                            case 0:
                                this.Key = "6" + this.KeyGroup;
                                break;
                            case 1:
                                this.Key = "7" + this.KeyGroup;
                                break;
                            case 2:
                                this.Key = "0" + this.KeyGroup;
                                break;
                            case 3:
                                this.Key = "1" + this.KeyGroup;
                                break;
                            case 4:
                                this.Key = "2" + this.KeyGroup;
                                break;
                            case 5:
                                this.Key = "3" + this.KeyGroup;
                                break;
                            case 6:
                                this.Key = "4" + this.KeyGroup;
                                break;
                            case 7:
                                this.Key = "5" + this.KeyGroup;
                                break;
                        }
                        switch (this.EndKeyNum)
                        {
                            case 0:
                                this.EndKey = "6";
                                break;
                            case 1:
                                this.EndKey = "7";
                                break;
                            case 2:
                                this.EndKey = "0";
                                break;
                            case 3:
                                this.EndKey = "1";
                                break;
                            case 4:
                                this.EndKey = "2";
                                break;
                            case 5:
                                this.EndKey = "3";
                                break;
                            case 6:
                                this.EndKey = "4";
                                break;
                            case 7:
                                this.EndKey = "5";
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Counterclockwise180":
                        switch (this.KeyNum)
                        {
                            case 0:
                                this.Key = "4" + this.KeyGroup;
                                break;
                            case 1:
                                this.Key = "5" + this.KeyGroup;
                                break;
                            case 2:
                                this.Key = "6" + this.KeyGroup;
                                break;
                            case 3:
                                this.Key = "7" + this.KeyGroup;
                                break;
                            case 4:
                                this.Key = "0" + this.KeyGroup;
                                break;
                            case 5:
                                this.Key = "1" + this.KeyGroup;
                                break;
                            case 6:
                                this.Key = "2" + this.KeyGroup;
                                break;
                            case 7:
                                this.Key = "3" + this.KeyGroup;
                                break;
                        }
                        switch (this.EndKeyNum)
                        {
                            case 0:
                                this.EndKey = "4";
                                break;
                            case 1:
                                this.EndKey = "5";
                                break;
                            case 2:
                                this.EndKey = "6";
                                break;
                            case 3:
                                this.EndKey = "7";
                                break;
                            case 4:
                                this.EndKey = "0";
                                break;
                            case 5:
                                this.EndKey = "1";
                                break;
                            case 6:
                                this.EndKey = "2";
                                break;
                            case 7:
                                this.EndKey = "3";
                                break;
                            default:
                                break;
                        }
                        break;
                    case "UpSideDown":
                        if (this.NoteType.Equals("TTP") && (this.KeyGroup.Equals("E") || this.KeyGroup.Equals("D")))
                        {
                            switch (this.KeyNum)
                            {
                                case 0:
                                    this.Key = "4" + this.KeyGroup;
                                    break;
                                case 1:
                                    this.Key = "3" + this.KeyGroup;
                                    break;
                                case 2:
                                    break;
                                case 3:
                                    this.Key = "1" + this.KeyGroup;
                                    break;
                                case 4:
                                    this.Key = "0" + this.KeyGroup;
                                    break;
                                case 5:
                                    this.Key = "7" + this.KeyGroup;
                                    break;
                                case 6:
                                    break;
                                case 7:
                                    this.Key = "5" + this.KeyGroup;
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.KeyNum)
                            {
                                case 0:
                                    this.Key = "3" + this.KeyGroup;
                                    break;
                                case 1:
                                    this.Key = "2" + this.KeyGroup;
                                    break;
                                case 2:
                                    this.Key = "1" + this.KeyGroup;
                                    break;
                                case 3:
                                    this.Key = "0" + this.KeyGroup;
                                    break;
                                case 4:
                                    this.Key = "7" + this.KeyGroup;
                                    break;
                                case 5:
                                    this.Key = "6" + this.KeyGroup;
                                    break;
                                case 6:
                                    this.Key = "5" + this.KeyGroup;
                                    break;
                                case 7:
                                    this.Key = "4" + this.KeyGroup;
                                    break;
                            }
                        }
                        switch (this.EndKeyNum)
                        {
                            case 0:
                                this.EndKey = "3";
                                break;
                            case 1:
                                this.EndKey = "2";
                                break;
                            case 2:
                                this.EndKey = "1";
                                break;
                            case 3:
                                this.EndKey = "0";
                                break;
                            case 4:
                                this.EndKey = "7";
                                break;
                            case 5:
                                this.EndKey = "6";
                                break;
                            case 6:
                                this.EndKey = "5";
                                break;
                            case 7:
                                this.EndKey = "4";
                                break;
                            default:
                                break;
                        }
                        if (this.NoteGenre.Equals("SLIDE"))
                        {
                            switch (this.NoteType)
                            {
                                case "SCL":
                                    this.NoteType = "SCR";
                                    break;
                                case "SCR":
                                    this.NoteType = "SCL";
                                    break;
                                case "SUL":
                                    this.NoteType = "SUR";
                                    break;
                                case "SUR":
                                    this.NoteType = "SUL";
                                    break;
                                case "SLL":
                                    this.NoteType = "SLR";
                                    break;
                                case "SLR":
                                    this.NoteType = "SLL";
                                    break;
                                case "SXL":
                                    this.NoteType = "SXR";
                                    break;
                                case "SXR":
                                    this.NoteType = "SXL";
                                    break;
                                case "SSL":
                                    this.NoteType = "SSR";
                                    break;
                                case "SSR":
                                    this.NoteType = "SSL";
                                    break;
                            }
                        }
                        break;
                    case "LeftToRight":
                        if (this.NoteType.Equals("TTP") && (this.KeyGroup.Equals("E") || this.KeyGroup.Equals("D")))
                        {
                            switch (this.KeyNum)
                            {
                                case 0:
                                    break;
                                case 1:
                                    this.Key = "7" + this.KeyGroup;
                                    break;
                                case 2:
                                    this.Key = "6" + this.KeyGroup;
                                    break;
                                case 3:
                                    this.Key = "5" + this.KeyGroup;
                                    break;
                                case 4:
                                    break;
                                case 5:
                                    this.Key = "3" + this.KeyGroup;
                                    break;
                                case 6:
                                    this.Key = "2" + this.KeyGroup;
                                    break;
                                case 7:
                                    this.Key = "1" + this.KeyGroup;
                                    break;
                            }
                        }
                        else
                        {
                            switch (this.KeyNum)
                            {
                                case 0:
                                    this.Key = "7" + this.KeyGroup;
                                    break;
                                case 1:
                                    this.Key = "6" + this.KeyGroup;
                                    break;
                                case 2:
                                    this.Key = "5" + this.KeyGroup;
                                    break;
                                case 3:
                                    this.Key = "4" + this.KeyGroup;
                                    break;
                                case 4:
                                    this.Key = "3" + this.KeyGroup;
                                    break;
                                case 5:
                                    this.Key = "2" + this.KeyGroup;
                                    break;
                                case 6:
                                    this.Key = "1" + this.KeyGroup;
                                    break;
                                case 7:
                                    this.Key = "0" + this.KeyGroup;
                                    break;
                            }
                        }
                        switch (this.EndKeyNum)
                        {
                            case 0:
                                this.EndKey = "7";
                                break;
                            case 1:
                                this.EndKey = "6";
                                break;
                            case 2:
                                this.EndKey = "5";
                                break;
                            case 3:
                                this.EndKey = "4";
                                break;
                            case 4:
                                this.EndKey = "3";
                                break;
                            case 5:
                                this.EndKey = "2";
                                break;
                            case 6:
                                this.EndKey = "1";
                                break;
                            case 7:
                                this.EndKey = "0";
                                break;
                            default:
                                break;
                        }
                        if (this.NoteGenre.Equals("SLIDE"))
                        {
                            switch (this.NoteType)
                            {
                                case "SCL":
                                    this.NoteType = "SCR";
                                    break;
                                case "SCR":
                                    this.NoteType = "SCL";
                                    break;
                                case "SUL":
                                    this.NoteType = "SUR";
                                    break;
                                case "SUR":
                                    this.NoteType = "SUL";
                                    break;
                                case "SLL":
                                    this.NoteType = "SLR";
                                    break;
                                case "SLR":
                                    this.NoteType = "SLL";
                                    break;
                                case "SXL":
                                    this.NoteType = "SXR";
                                    break;
                                case "SXR":
                                    this.NoteType = "SXL";
                                    break;
                                case "SSL":
                                    this.NoteType = "SSR";
                                    break;
                                case "SSR":
                                    this.NoteType = "SSL";
                                    break;
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException("METHOD SPECIFIED INVALID. EXPECT: Clockwise90, Clockwise180, Counterclockwise90, Counterclockwise180, UpSideDown, LeftToRight");
                }
                #endregion
            }
            this.Update();
        }

        public virtual bool Update()
        {
            // Console.WriteLine("This note has BPM note number of " + this.BPMChangeNotes.Count());
            bool result = false;
            this.TickStamp = this.Bar * 384 + this.Tick;
            while (this.Tick >= 384)
            {
                this.Tick -= 384;
                this.Bar++;
            }
            // string noteInformation = "This note is "+this.NoteType+", in Tick "+ this.TickStamp+", ";
            //this.TickTimeStamp = this.GetTimeStamp(this.TickStamp);
            this.WaitTickStamp = this.TickStamp + this.WaitLength;
            //this.WaitTimeStamp = this.GetTimeStamp(this.WaitTickStamp);
            this.LastTickStamp = this.WaitTickStamp + this.LastLength;
            //this.LastTimeStamp = this.GetTimeStamp(this.LastTickStamp);
            if (!(this.NoteType.Equals("SLIDE") || this.NoteType.Equals("HOLD")))
            {
                result = true;
            }
            else if (this.CalculatedLastTime > 0 && this.CalculatedWaitTime > 0)
            {
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Replace this.BPMChangeNotes from change table given
        /// </summary>
        /// <param name="changeTable">Change table contains BPM notes</param>
        public void ReplaceBPMChanges(BPMChanges changeTable)
        {
            this.BPMChangeNotes = new List<BPMChange>();
            this.BPMChangeNotes.AddRange(changeTable.ChangeNotes);
        }

        /// <summary>
        /// Replace this.BPMChangeNotes from change table given
        /// </summary>
        /// <param name="changeTable">Change table contains BPM notes</param>
        public void ReplaceBPMChanges(List<BPMChange> changeTable)
        {
            this.BPMChangeNotes = new List<BPMChange>();
            this.BPMChangeNotes.AddRange(changeTable);
        }

        /// <summary>
        /// Generate appropriate length for hold and slide.
        /// </summary>
        /// <param name="length">Last Time</param>
        /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
        public string GenerateAppropriateLength(int length)
        {
            string result = "";
            const int definition = 384;
            int divisor = GCD(definition, length);
            int quaver = definition / divisor, beat = length / divisor;
            result = "[" + quaver.ToString() + ":" + beat.ToString() + "]";
            return result;
        }

        /// <summary>
        /// Return GCD of A and B.
        /// </summary>
        /// <param name="a">A</param>
        /// <param name="b">B</param>
        /// <returns>GCD of A and B</returns>
        static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        /// <summary>
        /// Generate appropriate length for hold and slide.
        /// </summary>
        /// <param name="length">Last Time</param>
        /// <param name="BPM">BPM</param>
        /// <returns>[Definition:Length]=[Quaver:Beat]</returns>
        public string GenerateAppropriateLength(int length, double BPM)
        {
            string result = "";
            double duration = Math.Round(this.LastTimeStamp - this.WaitTimeStamp, 4);
            switch (this.NoteGenre)
            {
                case "SLIDE":
                    double sustain = Math.Round(this.WaitTimeStamp - this.TickTimeStamp,4);                 
                    result = "[" + sustain + "##" + duration + "]";
                    break;
                case "HOLD":
                    double startTime = Math.Round(startTime = this.TickTimeStamp,4);
                    result = "[" + startTime + "##" + duration + "]";
                    break;
            }
            return result;
        }

        /// <summary>
        /// Get BPM Time Tick unit of BPM
        /// </summary>
        /// <param name="BPM">BPM to calculate</param>
        /// <returns>BPM Tick Unit of BPM</returns>
        public static double GetBPMTimeUnit(double BPM)
        {
            double result = 60 / BPM * 4 / 384;
            return result;
        }

        public double GetTimeStamp(int overallTick)
        {
            double result = 0.0;
            if (overallTick != 0)
            {
                int maximumBPMIndex = 0;
                for (int i = 0; i < this.BPMChangeNotes.Count; i++)
                {
                    if (this.BPMChangeNotes[i].TickStamp <= overallTick)
                    {
                        maximumBPMIndex = i;
                    }
                }
                if (maximumBPMIndex == 0)
                {
                    result = GetBPMTimeUnit(this.BPMChangeNotes[0].BPM) * overallTick;
                }
                else
                {
                    for (int i = 1; i <= maximumBPMIndex; i++)
                    {
                        double previousTickTimeUnit = GetBPMTimeUnit(this.BPMChangeNotes[i - 1].BPM);
                        result += (this.BPMChangeNotes[i].TickStamp - this.BPMChangeNotes[i - 1].TickStamp) * previousTickTimeUnit;
                    }
                    double TickTimeUnit = GetBPMTimeUnit(this.BPMChangeNotes[maximumBPMIndex].BPM);
                    result += (overallTick - this.BPMChangeNotes[maximumBPMIndex].TickStamp) * TickTimeUnit;
                }
            } //A serious improvement is needed for this method
            return result;
        }

        public override bool Equals(object? obj)
        {
            bool result = (this == null && obj == null) || (this!=null && obj!=null);
            if (result && obj!=null)
            {
                Note localNote = (Note)obj;
                result = (this.TickStamp == localNote.TickStamp) && this.Compose(1).Equals(localNote.Compose(1));
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
