using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MaiLib
{
    public class SlideGroup : Slide
    {
        private List<Slide> internalSlides;

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
            this.NoteVersion = Version.Festival;
            this.Update();
        }

        public SlideGroup(List<Slide> slideCandidate)
        {
            this.internalSlides = new();
            this.internalSlides.AddRange(slideCandidate);
            this.NoteSpecialState = SpecialState.Normal;
            this.NoteVersion = Version.Festival;
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
                    Note localSlideStart = x.SlideStart != null ? x.SlideStart : new Tap("NST", x.Bar, x.Tick, x.Key);
                    result += x == internalSlides.Last() ? localSlideStart.Compose(format) + x.Compose(format) : x.Compose(format) + "/";
                }
            }
            else
            {
                Console.WriteLine("Invalid slide group located at bar "+this.Bar+" tick "+this.Tick);
                throw new InvalidOperationException("MA2 IS NOT COMPATIBLE WITH SLIDE GROUP");
            }
            return result;
        }

        /// <summary>
        /// Compose all internal slide in <code>this</code>. Also, since this contradicts with the ma2 note ordering, this method cannot compose in ma2 format.
        /// </summary>
        /// <param name="format">0 if simai, 1 if ma2</param>
        /// <returns>the composed simai slide group</returns>
        public override string Compose(int format, Chart.CompatibleProperty chartProperty)
        {
            string result = "";
            if (chartProperty == Chart.CompatibleProperty.Festival)
            {

            }
            else return this.Compose(format);
            return result;
        }
    }
}
