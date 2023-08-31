namespace MaiLib;

public class BPMChanges
{
    #region Constructors
    /// <summary>
    ///     Construct with changes listed
    /// </summary>
    /// <param name="bar">Bar which contains changes</param>
    /// <param name="tick">Tick in bar contains changes</param>
    /// <param name="bpm">Specified BPM changes</param>
    public BPMChanges(List<int> bar, List<int> tick, List<double> bpm)
    {
        ChangeNotes = new List<BPMChange>();
        for (var i = 0; i < bar.Count; i++)
        {
            BPMChange candidate = new(bar[i], tick[i], bpm[i]);
            ChangeNotes.Add(candidate);
        }

        Update();
    }

    /// <summary>
    ///     Construct empty BPMChange List
    /// </summary>
    public BPMChanges()
    {
        ChangeNotes = new List<BPMChange>();
        Update();
    }

    /// <summary>
    ///     Construct BPMChanges with existing one
    /// </summary>
    /// <param name="takenIn"></param>
    public BPMChanges(BPMChanges takenIn)
    {
        ChangeNotes = new List<BPMChange>();
        foreach (var candidate in takenIn.ChangeNotes) ChangeNotes.Add(candidate);
    }
    #endregion

    public List<BPMChange> ChangeNotes { get; private set; }

    /// <summary>
    ///     Returns first definitions
    /// </summary>
    public string InitialChange
    {
        get
        {
            if (ChangeNotes.Count > 4)
            {
                var result = "BPM_DEF" + "\t";
                for (var x = 0; x < 4; x++)
                {
                    result = result + string.Format("{0:F3}", ChangeNotes[x].BPM);
                    result += "\t";
                }

                return result + "\n";
            }
            else
            {
                var times = 0;
                var result = "BPM_DEF" + "\t";
                foreach (var x in ChangeNotes)
                {
                    result += string.Format("{0:F3}", x.BPM);
                    result += "\t";
                    times++;
                }

                while (times < 4)
                {
                    result += string.Format("{0:F3}", ChangeNotes[0].BPM);
                    result += "\t";
                    times++;
                }

                return result + "\n";
            }
        }
    }


    /// <summary>
    ///     Add BPMChange to change notes
    /// </summary>
    /// <param name="takeIn"></param>
    public void Add(BPMChange takeIn)
    {
        ChangeNotes.Add(takeIn);
        Update();
    }

    /// <summary>
    ///     Compose change notes according to BPMChanges
    /// </summary>
    public void Update()
    {
        List<BPMChange> adjusted = new();
        Note lastNote = new Rest();
        foreach (var x in ChangeNotes)
            if (!(x.Bar == lastNote.Bar && x.Tick == lastNote.Tick && x.BPM == lastNote.BPM))
            {
                adjusted.Add(x);
                lastNote = x;
            }

        // Console.WriteLine(adjusted.Count);
        ChangeNotes = new List<BPMChange>();
        foreach (var x in adjusted) ChangeNotes.Add(x);
        if (ChangeNotes.Count != adjusted.Count) throw new Exception("Adjusted BPM Note number not matching");
    }

    /// <summary>
    ///     See if the BPMChange is valid
    /// </summary>
    /// <returns>True if valid, false elsewise</returns>
    public bool CheckValidity()
    {
        var result = true;
        return result;
    }

    /// <summary>
    ///     Compose BPMChanges in beginning of MA2
    /// </summary>
    /// <returns></returns>
    public string Compose()
    {
        var result = "";
        for (var i = 0; i < ChangeNotes.Count; i++)
            result += "BPM" + "\t" + ChangeNotes[i].Bar + "\t" + ChangeNotes[i].Tick + "\t" + ChangeNotes[i].BPM + "\n";
        //result += "BPM" + "\t" + bar[i] + "\t" + tick[i] + "\t" + String.Format("{0:F3}", bpm[i])+"\n";
        return result;
    }
}