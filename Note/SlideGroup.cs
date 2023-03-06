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
        }

        public SlideGroup(Note inTake) : base(inTake)
        {
            this.internalSlides = new()
            {
                (Slide)inTake
            };
        }

        public SlideGroup(List<Slide> slideCandidate)
        {
            this.internalSlides = new();
            this.internalSlides.AddRange(slideCandidate);
        }

        public override string Compose(int format)
        {
            return base.Compose(format);
        }
    }
}
