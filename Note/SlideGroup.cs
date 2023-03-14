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
        }

        public override void Flip(string method)
        {
            foreach (Slide x in this.internalSlides)
                x.Flip(method);
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
    }
}
