namespace MaiLib;

using static MaiLib.NoteEnum;
using static MaiLib.ChartEnum;

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
        SlideStart = x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START ? x : null;
        InternalSlides = new List<Slide>();
        if (x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_EACH)
        {
            SlideEachSet? candidate =
                x as SlideEachSet ?? throw new InvalidOperationException("This is not a SLIDE EACH");
            InternalSlides.AddRange(candidate.InternalSlides);
        }
        else if (x.NoteSpecificGenre is NoteSpecificGenre.SLIDE)
        {
            Slide? candidate = x as Slide ?? throw new InvalidOperationException("This is not a SLIDE");
            InternalSlides.Add(candidate);
        }
        else if (x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_GROUP)
        {
            SlideGroup? candidate = x as SlideGroup ?? throw new InvalidOperationException("This is not a SLIDE GROUP");
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
            NoteType = NoteType.SLIDE_EACH;
            Bar = bar;
            Tick = startTime;
            WaitLength = InternalSlides.Last().WaitLength;
            LastLength = InternalSlides.Last().LastLength;
            // Delayed = WaitLength != 96;
        }

        Update();
    }

    #endregion

    public Note? SlideStart { get; set; }
    public override NoteSpecificGenre NoteSpecificGenre => NoteSpecificGenre.SLIDE_EACH;

    public override NoteGenre NoteGenre => NoteGenre.SLIDE;

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
        if (x.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START && (InternalSlides.Count == 0 ||
                                                                     (InternalSlides.Count > 0 &&
                                                                      InternalSlides.First().Key.Equals(x.Key) &&
                                                                      x.IsOfSameTime(InternalSlides.First()))))
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
        foreach (Slide? candidate in x) AddCandidateNote(candidate);
    }

    public bool TryAddCandidateNote(Tap x)
    {
        bool result = false;
        if (FirstIdentifier != null && x.Key.Equals(FirstIdentifier.Key) && FirstIdentifier.IsOfSameTime(x))
        {
            result = true;
            AddCandidateNote(x);
        }

        return result;
    }

    public bool TryAddCandidateNote(Slide x)
    {
        bool result = false;
        if (FirstIdentifier != null && x.Key.Equals(FirstIdentifier.Key) && FirstIdentifier.IsOfSameTime(x))
        {
            result = true;
            AddCandidateNote(x);
        }

        return result;
    }

    public bool TryAddCandidateNote(List<Slide> x)
    {
        bool result = false;
        foreach (Slide? candidate in x)
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
        bool result = SlideStart == null || SlideStart.NoteSpecificGenre is NoteSpecificGenre.SLIDE_START;

        if (SlideStart == null && InternalSlides == null) result = false;
        else if (SlideStart != null && InternalSlides.Count > 0 &&
                 !InternalSlides.First().IsOfSameTime(SlideStart)) result = false;
        else if (InternalSlides.Count > 0)
            foreach (Slide? x in InternalSlides)
            {
                Note? referencingNote = SlideStart == null ? InternalSlides.First() : SlideStart;
                result = result && x.IsOfSameTime(referencingNote);
            }

        return result;
    }

    public override void Flip(FlipMethod method)
    {
        if (SlideStart != null) SlideStart.Flip(method);
        for (int i = 0; i < InternalSlides.Count; i++) InternalSlides[i].Flip(method);
        Update();
    }

    public bool ContainsSlide(Note slide)
    {
        return InternalSlides.Contains(slide);
    }

    public override string Compose(ChartVersion format)
    {
        string? result = "";
        string? separateSymbol = "";
        switch (format)
        {
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
                separateSymbol = "*";
                if (InternalSlides.Count == 0 && SlideStart != null) result += SlideStart.Compose(format) + "$";
                else if (InternalSlides.Count > 0 && SlideStart == null)
                    result +=
                        new Tap(InternalSlides.First()) { NoteSpecialState = SpecialState.Normal }.Compose(format) +
                        "?";
                else if (SlideStart != null) result += SlideStart.Compose(format);
                for (int i = 0; i < InternalSlides.Count; i++)
                    if (i < InternalSlides.Count - 1)
                        result += InternalSlides[i].Compose(format) + separateSymbol;
                    else result += InternalSlides[i].Compose(format);
                break;
            case ChartVersion.Ma2_103:
            case ChartVersion.Ma2_104:
                if (SlideStart != null) SlideStart.Compose(format);
                foreach (Slide x in InternalSlides)
                {
                    x.Compose(format);
                }

                break;
            default:
                if (InternalSlides.Count == 0 && SlideStart != null) result += SlideStart.Compose(format) + "\n";
                else if (InternalSlides.Count > 0 && SlideStart == null)
                    result +=
                        new Tap(InternalSlides.First()) { NoteSpecialState = SpecialState.Normal }.Compose(format) +
                        "\n";
                else if (SlideStart != null) result += SlideStart.Compose(format);
                separateSymbol = "\n";
                for (int i = 0; i < InternalSlides.Count; i++)
                    if (i < InternalSlides.Count - 1)
                        result += InternalSlides[i].Compose(format) + separateSymbol;
                    else result += InternalSlides[i].Compose(format);
                break;
        }

        return result;
    }

    public override Note NewInstance()
    {
        return new SlideEachSet(this);
    }
}