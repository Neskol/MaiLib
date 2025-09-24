namespace MaiLib;

using static NoteEnum;
using static ChartEnum;

public class SlideGroup : Slide
{
    #region Constructors

    public SlideGroup()
    {
        InternalSlides = [];
        NoteSpecialState = SpecialState.Normal;
        Update();
    }

    public SlideGroup(Note inTake) : base(inTake)
    {
        InternalSlides =
        [
            (Slide)inTake
        ];
        NoteSpecialState = inTake.NoteSpecialState;
        Update();
    }

    public SlideGroup(List<Slide> slideCandidate)
    {
        InternalSlides = [.. slideCandidate];
        NoteSpecialState = slideCandidate.First().NoteSpecialState;
        Update();
    }

    #endregion

    public int SlideCount => InternalSlides.Count;

    public override NoteSpecificGenre NoteSpecificGenre => NoteSpecificGenre.SLIDE_GROUP;

    public List<Slide> InternalSlides { get; }

    public Slide FirstSlide => InternalSlides.First();
    public Slide LastSlide => InternalSlides.Last();

    public override int LastLength => InternalSlides.Sum(slide => slide.LastLength);

    public void AddConnectingSlide(Slide candidate)
    {
        InternalSlides.Add(candidate);
        Update();
    }

    public override void Flip(FlipMethod method)
    {
        foreach (Slide? x in InternalSlides)
            x.Flip(method);
        Update();
    }

    public bool ContainsSlide(Note slide)
    {
        return InternalSlides.Contains(slide);
    }

    /// <summary>
    ///     By default this does not compose festival format - compose all internal slide in <code>this</code>. Also, since
    ///     this contradicts with the ma2 note ordering, this method cannot compose in ma2 format.
    /// </summary>
    /// <param name="format">0 if simai, 1 if ma2</param>
    /// <returns>the composed simai slide group</returns>
    public override string Compose(ChartVersion format)
    {
        string? result = "";

        // foreach (Slide? x in InternalSlides)
        //     result += x.Compose(format);

        switch (format)
        {
            case ChartVersion.Ma2_103:
            case ChartVersion.Ma2_104:
            case ChartVersion.Ma2_105:
                foreach (Slide? x in InternalSlides) result += x.Compose(format);

                break;
            case ChartVersion.Simai:
            case ChartVersion.SimaiFes:
                if (InternalSlides.Any(slide =>
                        slide.NoteSpecialState is SpecialState.ConnectingSlide && slide.WaitLength is not 0))
                {
                    foreach (Slide? x in InternalSlides)
                        result += x.Compose(format);
                }
                else
                {
                    Update();
                    foreach (Slide? x in InternalSlides) result += ComposeWithoutLengthInSimai(x);
                    // Console.WriteLine("The internal last length is {0}",x.LastLength);
                    // Console.WriteLine("Current last length is {0}",this.LastLength);
                    if (TickBPMDisagree || Delayed)
                        result += GenerateAppropriateLength(LastLength, BPM);
                    else result += GenerateAppropriateLength(LastLength);
                    // Console.WriteLine("Recalculating Last Length: {0}", this.InternalSlides.Sum(slide => slide.LastLength));
                    // Console.WriteLine("Calculated last time is: {0}", GenerateAppropriateLength(this.InternalSlides.Sum(slide => slide.LastLength),BPM));
                }

                if (FirstSlide.NoteSpecialState == SpecialState.Break) result += "b";
                break;
        }

        return result;
    }

    public override bool Equals(object? obj)
    {
        bool result = (this == null && obj == null) || (this != null && obj != null);
        if (result && obj != null)
            for (int i = 0; i < InternalSlides.Count; i++)
            {
                SlideGroup? localGroup = (SlideGroup)obj;
                result = result && InternalSlides[i].Equals(localGroup.InternalSlides[i]);
            }

        return result;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override bool Update()
    {
        // base.Update();
        if (InternalSlides.Any(slide => slide.NoteSpecialState is SpecialState.Break))
            NoteSpecialState = SpecialState.Break;
        if (InternalSlides.Count > 0) InternalSlides.First().NoteSpecialState = NoteSpecialState;
        bool result = false;
        if (SlideCount > 0 && InternalSlides.Last().LastLength == 0)
            throw new InvalidOperationException("THE LAST SLIDE IN THIS GROUP DOES NOT HAVE LAST TIME ASSIGNED");
        if (SlideCount > 0 && Key != null)
        {
            Tick = FirstSlide.Tick;
            Bar = FirstSlide.Bar;
            BPM = FirstSlide.BPM;
            WaitLength = FirstSlide.WaitLength;
            foreach (Slide? x in InternalSlides)
                if (x.LastLength == 0)
                    x.LastLength = LastSlide.LastLength;

            while (Tick >= Definition)
            {
                Tick -= Definition;
                Bar++;
            }

            // string noteInformation = "This note is "+this.NoteType+", in tick "+ this.tickStamp+", ";
            //this.tickTimeStamp = this.GetTimeStamp(this.tickStamp);
            int totalWaitLength = 0;
            int totalLastLength = 0;
            foreach (Slide? x in InternalSlides)
            {
                totalWaitLength += x.WaitLength;
                totalLastLength += x.LastLength;
            }

            WaitLength = totalWaitLength;
            LastLength = totalLastLength;

            WaitTimeStamp = GetTimeStamp(WaitTickStamp);
            CalculatedWaitTime = WaitTimeStamp - TickTimeStamp;
            LastTimeStamp = GetTimeStamp(LastTickStamp);
            CalculatedLastTime = LastTimeStamp - TickTimeStamp;
            // FixedLastLength =
            //     (int)double.Round(CalculatedLastTime /
            //                       GetBPMTimeUnit(Chart.GetBPMByTick(TickStamp), Definition));

            // WaitTickStamp = TickStamp + totalWaitLength;
            // //this.waitTimeStamp = this.GetTimeStamp(this.waitTickStamp);
            // LastTickStamp = WaitTickStamp + totalLastLength;
            // //this.lastTimeStamp = this.GetTimeStamp(this.lastTickStamp);
            if (CalculatedLastTime > 0 && CalculatedWaitTime > 0) result = true;
        }

        if (SlideCount > 0 && (Key == null || Key != InternalSlides[0].Key))
        {
            Note inTake = InternalSlides[0];
            inTake.CopyOver(this);
        }

        return result;
    }
}