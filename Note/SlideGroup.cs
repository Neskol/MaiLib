using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MaiLib
{
    public class SlideGroup : Slide
    {
        private List<Slide> internalSlides;

        public int SlideCount { get => this == null ? 0 : this.internalSlides.Count;}

        public override string NoteSpecificGenre => "SLIDE_GROUP";

        public List<Slide> InternalSlides { get => this.internalSlides; }

        public Slide FirstSlide { get => this.internalSlides.First(); }
        public Slide LastSlide { get => this.internalSlides.Last(); }

        public SlideGroup()
        {
            this.internalSlides = new();
            this.NoteSpecialState = SpecialState.Normal;
            this.Update();
        }

        public SlideGroup(Note inTake) : base(inTake)
        {
            this.internalSlides = new()
            {
                (Slide)inTake
            };
            this.NoteSpecialState = SpecialState.Normal;
            this.Update();
        }

        public SlideGroup(List<Slide> slideCandidate)
        {
            this.internalSlides = new();
            this.internalSlides.AddRange(slideCandidate);
            this.NoteSpecialState = SpecialState.Normal;
            this.Update();
        }

        public void AddConnectingSlide(Slide candidate)
        {
            this.internalSlides.Add(candidate);
            this.Update();
        }

        public override void Flip(string method)
        {
            foreach (Slide x in this.internalSlides)
                x.Flip(method);
            this.Update();
        }

        public bool ContainsSlide(Note slide)
        {
            return this.internalSlides.Contains(slide);
        }

        /// <summary>
        /// By default this does not compose festival format - compose all internal slide in <code>this</code>. Also, since this contradicts with the ma2 note ordering, this method cannot compose in ma2 format.
        /// </summary>
        /// <param name="format">0 if simai, 1 if ma2</param>
        /// <returns>the composed simai slide group</returns>
        public override string Compose(int format)
        {
            string result = "";
            if (format == 0)
            {
                foreach (Slide x in this.internalSlides)
                {
                    // Note localSlideStart = x.SlideStart != null ? x.SlideStart : new Tap("NST", x.Bar, x.Tick, x.Key);
                    result += x.Compose(format);
                }
            }
            else
            {
                Console.WriteLine("Invalid slide group located at bar " + this.Bar + " tick " + this.Tick);
                throw new InvalidOperationException("MA2 IS NOT COMPATIBLE WITH SLIDE GROUP");
            }
            return result;
        }

        public override bool Equals(object? obj)
        {
            bool result = (this == null && obj == null) || (this != null && obj != null);
            if (result && obj!=null) for (int i = 0; i < this.internalSlides.Count; i++)
            {
                SlideGroup localGroup = (SlideGroup)(obj);
                result = result && this.internalSlides[i].Equals(localGroup.internalSlides[i]);
            }
            return result;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Update()
        {
            bool result = false;
            
            if (this.SlideCount>0&&this.Key!=null)
            {
                while (this.Tick >= 384)
                {
                    this.Tick -= 384;
                    this.Bar++;
                }
                // string noteInformation = "This note is "+this.NoteType+", in tick "+ this.tickStamp+", ";
                //this.tickTimeStamp = this.GetTimeStamp(this.tickStamp);
                int totalWaitLength = 0;
                int totalLastLength = 0;
                foreach (Slide x in this.internalSlides)
                {
                    totalWaitLength += x.WaitLength;
                    totalLastLength += x.LastLength;
                }
                this.WaitTickStamp = this.TickStamp + totalWaitLength;
                //this.waitTimeStamp = this.GetTimeStamp(this.waitTickStamp);
                this.LastTickStamp = this.WaitTickStamp + totalLastLength;
                //this.lastTimeStamp = this.GetTimeStamp(this.lastTickStamp);
                if (this.CalculatedLastTime > 0 && this.CalculatedWaitTime > 0)
                {
                    result = true;
                }
            }
            if (this.SlideCount>0&&(this.Key == null ||this.Key !=this.internalSlides[0].Key))
            {
                Note inTake = this.internalSlides[0];
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
            
            return result;
        }
    }
}
