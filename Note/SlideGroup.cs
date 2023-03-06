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
            this.NoteType = "CNS";
            this.NoteSpecialState = SpecialState.Normal;
        }

        public SlideGroup(Note inTake) : base(inTake)
        {
            this.internalSlides = new()
            {
                (Slide)inTake
            };
            this.NoteType = "CNS";
            this.NoteSpecialState = SpecialState.Normal;
        }

        public SlideGroup(List<Slide> slideCandidate)
        {
            this.internalSlides = new();
            this.internalSlides.AddRange(slideCandidate);
            this.NoteType = "STR";
            this.NoteSpecialState = SpecialState.Normal;
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

        public override string Compose(int format)
        {
            string result = "";
            if (format == 0)
            {

            }
            else
            {
                
            }
            return result;
        }
    }
}
