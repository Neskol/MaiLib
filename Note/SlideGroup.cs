﻿namespace MaiLib;

public class SlideGroup : Slide
{
    public SlideGroup()
    {
        InternalSlides = new List<Slide>();
        NoteSpecialState = SpecialState.Normal;
        Update();
    }

    public SlideGroup(Note inTake) : base(inTake)
    {
        InternalSlides = new List<Slide>
        {
            (Slide)inTake
        };
        NoteSpecialState = SpecialState.Normal;
        Update();
    }

    public SlideGroup(List<Slide> slideCandidate)
    {
        InternalSlides = new List<Slide>();
        InternalSlides.AddRange(slideCandidate);
        NoteSpecialState = SpecialState.Normal;
        Update();
    }

    public int SlideCount => this == null ? 0 : InternalSlides.Count;

    public override string NoteSpecificGenre => "SLIDE_GROUP";

    public List<Slide> InternalSlides { get; }

    public Slide FirstSlide => InternalSlides.First();
    public Slide LastSlide => InternalSlides.Last();

    public void AddConnectingSlide(Slide candidate)
    {
        InternalSlides.Add(candidate);
        Update();
    }

    public override void Flip(string method)
    {
        foreach (var x in InternalSlides)
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
    public override string Compose(int format)
    {
        var result = "";
        if (format == 0)
        {
            foreach (var x in InternalSlides)
                // Note localSlideStart = x.SlideStart != null ? x.SlideStart : new Tap("NST", x.Bar, x.Tick, x.Key);
                result += x.Compose(format);
        }
        else
        {
            Console.WriteLine("Invalid slide group located at bar " + Bar + " tick " + Tick);
            throw new InvalidOperationException("MA2 IS NOT COMPATIBLE WITH SLIDE GROUP");
        }

        return result;
    }

    public override bool Equals(object? obj)
    {
        var result = (this == null && obj == null) || (this != null && obj != null);
        if (result && obj != null)
            for (var i = 0; i < InternalSlides.Count; i++)
            {
                var localGroup = (SlideGroup)obj;
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
        var result = false;
        if (SlideCount > 0 && InternalSlides.Last().LastLength == 0)
            throw new InvalidOperationException("THE LAST SLIDE IN THIS GROUP DOES NOT HAVE LAST TIME ASSIGNED");
        if (SlideCount > 0 && Key != null)
        {
            foreach (var x in InternalSlides)
                if (x.LastLength == 0)
                    x.LastLength = InternalSlides.Last().LastLength;

            while (Tick >= 384)
            {
                Tick -= 384;
                Bar++;
            }

            // string noteInformation = "This note is "+this.NoteType+", in tick "+ this.tickStamp+", ";
            //this.tickTimeStamp = this.GetTimeStamp(this.tickStamp);
            var totalWaitLength = 0;
            var totalLastLength = 0;
            foreach (var x in InternalSlides)
            {
                totalWaitLength += x.WaitLength;
                totalLastLength += x.LastLength;
            }

            WaitTickStamp = TickStamp + totalWaitLength;
            //this.waitTimeStamp = this.GetTimeStamp(this.waitTickStamp);
            LastTickStamp = WaitTickStamp + totalLastLength;
            //this.lastTimeStamp = this.GetTimeStamp(this.lastTickStamp);
            if (CalculatedLastTime > 0 && CalculatedWaitTime > 0) result = true;
        }

        if (SlideCount > 0 && (Key == null || Key != InternalSlides[0].Key))
        {
            Note inTake = InternalSlides[0];
            NoteType = inTake.NoteType;
            Key = inTake.Key;
            EndKey = inTake.EndKey;
            Bar = inTake.Bar;
            Tick = inTake.Tick;
            TickStamp = inTake.TickStamp;
            TimeStamp = inTake.TimeStamp;
            LastLength = inTake.LastLength;
            LastTickStamp = inTake.LastTickStamp;
            LastTimeStamp = inTake.LastTimeStamp;
            WaitLength = inTake.WaitLength;
            WaitTickStamp = inTake.WaitTickStamp;
            WaitTimeStamp = inTake.WaitTimeStamp;
            CalculatedLastTime = inTake.CalculatedLastTime;
            CalculatedLastTime = inTake.CalculatedLastTime;
            TickBPMDisagree = inTake.TickBPMDisagree;
            BPM = inTake.BPM;
            BPMChangeNotes = inTake.BPMChangeNotes;
        }

        return result;
    }
}