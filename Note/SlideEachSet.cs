namespace MaiLib;

public class SlideEachSet : Note
{
    public List<Slide> InternalSlides;

#region Constructors
    public SlideEachSet()
    {
        InternalSlides = new List<Slide>();
        Update();
    }

    public SlideEachSet(Note x) : base(x)
    {
        SlideStart = x.NoteSpecificGenre.Equals("SLIDE_START") ? x : null;
        InternalSlides = new List<Slide>();
        if (x.NoteSpecificGenre.Equals("SLIDE_EACH"))
        {
            var candidate = x as SlideEachSet ?? throw new InvalidOperationException("This is not a SLIDE EACH");
            InternalSlides.AddRange(candidate.InternalSlides);
        }
        else if (x.NoteSpecificGenre.Equals("SLIDE"))
        {
            var candidate = x as Slide ?? throw new InvalidOperationException("This is not a SLIDE");
            InternalSlides.Add(candidate);
        }

        Update();
    }

    public SlideEachSet(int bar, int startTime, Note slideStart, List<Slide> internalSlides) : base(slideStart)
    {
        SlideStart = slideStart;
        InternalSlides = new List<Slide>(internalSlides);
        if (internalSlides.Count > 0)
        {
            EndKey = InternalSlides.Last().EndKey;
            NoteType = "SLIDE_EACH";
            Bar = bar;
            Tick = startTime;
            WaitLength = InternalSlides.Last().WaitLength;
            LastLength = InternalSlides.Last().LastLength;
            Delayed = WaitLength != 96;
        }

        Update();
    }
    #endregion

    public Note? SlideStart { get; set; }
    public override string NoteSpecificGenre => "SLIDE_EACH";

    public override string NoteGenre => "SLIDE";

    public Note? FirstIdentifier
    {
        get
        {
            if (SlideStart != null) return SlideStart;
            return InternalSlides.Count > 0 ? InternalSlides.First() : null;
        }
    }

    public Note? FirstSlide => InternalSlides.First();
    public Note? LastSlide => InternalSlides.Last();

    public override bool IsNote => true;

    public void AddCandidateNote(Tap x)
    {
        if (x.NoteSpecificGenre.Equals("SLIDE_START") && (InternalSlides.Count == 0 || (InternalSlides.Count > 0 &&
                InternalSlides.First().Key.Equals(x.Key) && x.IsOfSameTime(InternalSlides.First()))))
            SlideStart = x;
        else throw new InvalidOperationException("THE INTAKE NOTE IS NOT VALID SLIDE START");
    }

    public void AddCandidateNote(Slide x)
    {
        if ((SlideStart == null && InternalSlides.Count == 0) ||
            (SlideStart != null && x.Key.Equals(SlideStart.Key) && x.IsOfSameTime(SlideStart)) ||
            (InternalSlides.Count > 0 && InternalSlides.First().Key.Equals(x.Key) &&
             x.IsOfSameTime(InternalSlides.First())))
            InternalSlides.Add(x);
        else throw new InvalidOperationException("THE INTAKE NOTE IS NOT VALID SLIDE OR THIS NOTE IS NOT VALID");
    }

    public void AddCandidateNote(List<Slide> x)
    {
        foreach (var candidate in x) AddCandidateNote(candidate);
    }

    public bool TryAddCandidateNote(Tap x)
    {
        var result = false;
        if (FirstIdentifier != null && x.Key.Equals(FirstIdentifier.Key) && FirstIdentifier.IsOfSameTime(x))
        {
            result = true;
            AddCandidateNote(x);
        }

        return result;
    }

    public bool TryAddCandidateNote(Slide x)
    {
        var result = false;
        if (FirstIdentifier != null && x.Key.Equals(FirstIdentifier.Key) && FirstIdentifier.IsOfSameTime(x))
        {
            result = true;
            AddCandidateNote(x);
        }

        return result;
    }

    public bool TryAddCandidateNote(List<Slide> x)
    {
        var result = false;
        foreach (var candidate in x)
            if (FirstIdentifier != null && candidate.Key.Equals(FirstIdentifier.Key) &&
                FirstIdentifier.IsOfSameTime(candidate))
            {
                result = result || true;
                AddCandidateNote(candidate);
            }

        return result;
    }

    public override bool CheckValidity()
    {
        var result = SlideStart == null || SlideStart.NoteSpecificGenre.Equals("SLIDE_START");

        if (SlideStart == null && InternalSlides == null) result = false;
        else if (SlideStart != null && InternalSlides.Count > 0 &&
                 !InternalSlides.First().IsOfSameTime(SlideStart)) result = false;
        else if (InternalSlides.Count > 0)
            foreach (var x in InternalSlides)
            {
                var referencingNote = SlideStart == null ? InternalSlides.First() : SlideStart;
                result = result && x.IsOfSameTime(referencingNote);
            }

        return result;
    }

    public override void Flip(string method)
    {
        if (SlideStart != null) SlideStart.Flip(method);
        for (var i = 0; i < InternalSlides.Count; i++) InternalSlides[i].Flip(method);
        Update();
    }

    public bool ContainsSlide(Note slide)
    {
        return InternalSlides.Contains(slide);
    }

    public override string Compose(int format)
    {
        var result = "";
        var separateSymbol = "";
        switch (format)
        {
            case 0:
                separateSymbol = "*";
                if (InternalSlides.Count == 0 && SlideStart != null) result += SlideStart.Compose(format) + "$";
                else if (InternalSlides.Count > 0 && SlideStart == null)
                    result += new Tap(InternalSlides.First()).Compose(format) + "!";
                else if (SlideStart != null) result += SlideStart.Compose(format);
                break;
            case 1:
                if( SlideStart != null) SlideStart.Compose(format);
                foreach (Slide x in InternalSlides)
                {
                    x.Compose(format);
                }
                break;
            default:
                if (InternalSlides.Count == 0 && SlideStart != null) result += SlideStart.Compose(format) + "\n";
                else if (InternalSlides.Count > 0 && SlideStart == null)
                    result += new Tap(InternalSlides.First()).Compose(format) + "\n";
                else if (SlideStart != null) result += SlideStart.Compose(format);
                separateSymbol = "\n";
                break;
        }

        for (var i = 0; i < InternalSlides.Count; i++)
            if (i < InternalSlides.Count - 1)
                result += InternalSlides[i].Compose(format) + separateSymbol;
            else result += InternalSlides[i].Compose(format);

        return result;
    }

    public override Note NewInstance()
    {
        return new SlideEachSet(this);
    }
}
