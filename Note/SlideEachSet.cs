using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Net.Cache;
using System.Reflection;
using System.Xml;

namespace MaiLib
{
    public class SlideEachSet : Note
    {
        public Note? SlideStart{ get; set; }
        public List<Slide> InternalSlides;
        public override string NoteSpecificGenre => "SLIDE_EACH";

        public override string NoteGenre => "SLIDE";

        public Note? FirstIdentifier {get { if (this.SlideStart != null) return this.SlideStart; else return this.InternalSlides.Count > 0 ? this.InternalSlides.First() :null; } }
        public Note? FirstSlide => this.InternalSlides.First();
        public Note? LastSlide => this.InternalSlides.Last();

        public override bool IsNote => true;

        public SlideEachSet() : base()
        {
            this.InternalSlides = new();
            this.Update();
        }

        public SlideEachSet(Note x):base(x)
        {
            this.SlideStart = x.NoteSpecificGenre.Equals("SLIDE_START") ? x : null;
            this.InternalSlides = new();
            if (x.NoteSpecificGenre.Equals("SLIDE_EACH"))
            {
                SlideEachSet candidate = x as SlideEachSet ?? throw new InvalidOperationException("This is not a SLIDE EACH");
                this.InternalSlides.AddRange(candidate.InternalSlides);
            }
            else if (x.NoteSpecificGenre.Equals("SLIDE"))
            {
                Slide candidate = x as Slide ?? throw new InvalidOperationException("This is not a SLIDE");
                this.InternalSlides.Add(candidate);
            }
            this.Update();
        }

        public SlideEachSet(int bar, int startTime, Note slideStart, List<Slide> internalSlides):base(slideStart)
        {
            this.SlideStart = slideStart;
            this.InternalSlides=new(internalSlides);
            if (internalSlides.Count>0)
            {
                this.EndKey = this.InternalSlides.Last().EndKey;
                this.NoteType = "SLIDE_EACH";
                this.Bar = bar;
                this.Tick = startTime;
                this.WaitLength = this.InternalSlides.Last().WaitLength;
                this.LastLength = this.InternalSlides.Last().LastLength;
                this.Delayed = this.WaitLength != 96;
            }
            this.Update();
        }

        public void AddCandidateNote(Tap x)
        {
            if (x.NoteSpecificGenre.Equals("SLIDE_START") && (this.InternalSlides.Count ==0 || (this.InternalSlides.Count >0 && this.InternalSlides.First().Key.Equals(x.Key)&&x.IsOfSameTime(this.InternalSlides.First()))))
            {
                this.SlideStart = x;
            }
            else throw new InvalidOperationException("THE INTAKE NOTE IS NOT VALID SLIDE START");
        }

        public void AddCandidateNote(Slide x)
        {
            if ((this.SlideStart==null && this.InternalSlides.Count == 0) || (this.SlideStart!= null && x.Key.Equals(this.SlideStart.Key)&& x.IsOfSameTime(this.SlideStart)) || (this.InternalSlides.Count > 0 && this.InternalSlides.First().Key.Equals(x.Key)&&x.IsOfSameTime(this.InternalSlides.First())))
            {
                this.InternalSlides.Add(x);
            }
            else throw new InvalidOperationException("THE INTAKE NOTE IS NOT VALID SLIDE OR THIS NOTE IS NOT VALID");
        }

        public void AddCandidateNote(List<Slide> x)
        {
            foreach (Slide candidate in x)
            {
                this.AddCandidateNote(candidate);
            }
        }

        public bool TryAddCandidateNote(Tap x)
        {
            bool result = false;
            if (this.FirstIdentifier != null && x.Key.Equals(this.FirstIdentifier.Key)&&this.FirstIdentifier.IsOfSameTime(x))
            {
                result = true;
                this.AddCandidateNote(x);
            }
            return result;
        }

        public bool TryAddCandidateNote(Slide x)
        {
            bool result = false;
            if (this.FirstIdentifier != null && x.Key.Equals(this.FirstIdentifier.Key) && this.FirstIdentifier.IsOfSameTime(x))
            {
                result = true;
                this.AddCandidateNote(x);
            }
            return result;
        }

        public bool TryAddCandidateNote(List<Slide> x)
        {
            bool result = false;
            foreach (Slide candidate in x)
            {
                if (this.FirstIdentifier != null && candidate.Key.Equals(this.FirstIdentifier.Key) && this.FirstIdentifier.IsOfSameTime(candidate))
                {
                    result = result || true;
                    this.AddCandidateNote(candidate);
                }
            }
            return result;
        }

        public override bool CheckValidity()
        {
            bool result = this.SlideStart == null || this.SlideStart.NoteSpecificGenre.Equals("SLIDE_START");

            if (this.SlideStart == null && this.InternalSlides == null) result = false;
            else if (this.SlideStart!=null && this.InternalSlides.Count>0 && !this.InternalSlides.First().IsOfSameTime(this.SlideStart)) result = false;
            else if (this.InternalSlides.Count>0) foreach(Slide x in this.InternalSlides)
            {
                Note referencingNote = this.SlideStart == null ? this.InternalSlides.First() : this.SlideStart;
                result = result && x.IsOfSameTime(referencingNote);
            }

            return result;
        }

        public override void Flip(string method)
        {
            if (this.SlideStart!=null) this.SlideStart.Flip(method);
            for (int i = 0; i < this.InternalSlides.Count;i++)
            {
                this.InternalSlides[i].Flip(method);
            }
            this.Update();
        }

        public bool ContainsSlide(Note slide)
        {
            return this.InternalSlides.Contains(slide);
        }

        public override string Compose(int format)
        {
            string result = "";
            string separateSymbol = "";
            switch (format)
            {
                case 0:
                    separateSymbol = "*";
                    if (this.InternalSlides.Count == 0 && this.SlideStart!=null) result += this.SlideStart.Compose(format) + "$";
                    else if (this.InternalSlides.Count>0 && this.SlideStart == null) result += new Tap(this.InternalSlides.First()).Compose(format) + "!";
                    else if (this.SlideStart != null) result += this.SlideStart.Compose(format);
                    break;
                case 1:
                default:
                    if (this.InternalSlides.Count == 0 && this.SlideStart != null) result += this.SlideStart.Compose(format) + "\n";
                    else if (this.InternalSlides.Count > 0 && this.SlideStart == null) result += new Tap(this.InternalSlides.First()).Compose(format)+"\n";
                    else if (this.SlideStart != null) result += this.SlideStart.Compose(format);
                    separateSymbol = "\n";
                    break;
            }

            for (int i = 0; i < this.InternalSlides.Count;i++)
            {
                if (i<this.InternalSlides.Count-1)
                {
                    result += this.InternalSlides[i].Compose(format) + separateSymbol;
                }
                else result += this.InternalSlides[i].Compose(format);
            }
            
            return result;
        }

        public override Note NewInstance()
        {
            return new SlideEachSet(this);
        }
    }
}