﻿namespace MaiLib;

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
        ChangeNotes = [];
        for (int i = 0; i < bar.Count; i++)
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
        ChangeNotes = [];
        Update();
    }

    /// <summary>
    ///     Construct BPMChanges with existing one
    /// </summary>
    /// <param name="takenIn"></param>
    public BPMChanges(BPMChanges takenIn)
    {
        ChangeNotes = [.. takenIn.ChangeNotes];
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
                string? result = "BPM_DEF" + "\t";
                for (int x = 0; x < 4; x++)
                {
                    result = result + $"{ChangeNotes[x].BPM:F3}";
                    result += "\t";
                }

                return result + "\n";
            }
            else
            {
                int times = 0;
                string? result = "BPM_DEF" + "\t";
                foreach (BPMChange? x in ChangeNotes)
                {
                    result += $"{x.BPM:F3}";
                    result += "\t";
                    times++;
                }

                while (times < 4)
                {
                    result += $"{ChangeNotes[0].BPM:F3}";
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
        List<BPMChange> adjusted = [];
        Note lastNote = new Rest();
        foreach (BPMChange? x in ChangeNotes.Where(x =>
                     !(x.Bar == lastNote.Bar && x.Tick == lastNote.Tick && x.BPM == lastNote.BPM)))
        {
            adjusted.Add(x);
            lastNote = x;
        }

        // Console.WriteLine(adjusted.Count);
        ChangeNotes = [.. adjusted];
        if (ChangeNotes.Count != adjusted.Count) throw new Exception("Adjusted BPM Note number not matching");
    }

    /// <summary>
    ///     See if the BPMChange is valid
    /// </summary>
    /// <returns>True if valid, false elsewise</returns>
    public bool CheckValidity()
    {
        bool result = true;
        return result;
    }

    /// <summary>
    ///     Compose BPMChanges in beginning of MA2
    /// </summary>
    /// <returns></returns>
    public string Compose()
    {
        string? result = "";
        for (int i = 0; i < ChangeNotes.Count; i++)
            result += "BPM" + "\t" + ChangeNotes[i].Bar + "\t" + ChangeNotes[i].Tick + "\t" + ChangeNotes[i].BPM + "\n";
        //result += "BPM" + "\t" + bar[i] + "\t" + tick[i] + "\t" + String.Format("{0:F3}", bpm[i])+"\n";
        return result;
    }
}