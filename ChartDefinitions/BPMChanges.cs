using System.Dynamic;
using System.Globalization;

namespace MaiLib
{
    public class BPMChanges
    {
        public List<BPMChange> ChangeNotes { get; private set; }

        /// <summary>
        /// Construct with changes listed
        /// </summary>
        /// <param name="bar">Bar which contains changes</param>
        /// <param name="tick">Tick in bar contains changes</param>
        /// <param name="bpm">Specified BPM changes</param>
        public BPMChanges(List<int> bar, List<int> tick, List<double> bpm)
        {
            this.ChangeNotes = new List<BPMChange>();
            for (int i = 0; i < bar.Count; i++)
            {
                BPMChange candidate = new(bar[i], tick[i], bpm[i]);
                this.ChangeNotes.Add(candidate);
            }
            this.Update();
        }

        /// <summary>
        /// Construct empty BPMChange List
        /// </summary>
        public BPMChanges()
        {
            this.ChangeNotes = new List<BPMChange>();
            this.Update();
        }

        /// <summary>
        /// Construct BPMChanges with existing one
        /// </summary>
        /// <param name="takenIn"></param>
        public BPMChanges(BPMChanges takenIn)
        {
            this.ChangeNotes = new List<BPMChange>();
            foreach (BPMChange candidate in takenIn.ChangeNotes)
            {
                this.ChangeNotes.Add(candidate);
            }
        }

        

        /// <summary>
        /// Add BPMChange to change notes
        /// </summary>
        /// <param name="takeIn"></param>
        public void Add(BPMChange takeIn)
        {
            this.ChangeNotes.Add(takeIn);
            this.Update();
        }

        /// <summary>
        /// Compose change notes according to BPMChanges
        /// </summary>
        public void Update()
        {
            List<BPMChange> adjusted = new();
            Note lastNote = new Rest();
            foreach (BPMChange x in this.ChangeNotes)
            {
                if (!(x.Bar == lastNote.Bar && x.Tick == lastNote.Tick && x.BPM == lastNote.BPM))
                {
                    adjusted.Add(x);
                    lastNote = x;
                }
            }
            // Console.WriteLine(adjusted.Count);
            this.ChangeNotes = new List<BPMChange>();
            foreach(BPMChange x in adjusted)
            {
                this.ChangeNotes.Add(x);
            }
            if (this.ChangeNotes.Count!=adjusted.Count)
            {
                throw new Exception("Adjusted BPM Note number not matching");
            }
        }

        /// <summary>
        /// Returns first definitions
        /// </summary>
        public string InitialChange
        {
            get
            {
                if (ChangeNotes.Count > 4)
                {
                    string result = "BPM_DEF" + "\t";
                    for (int x = 0; x < 4; x++)
                    {
                        result = result + String.Format("{0:F3}", this.ChangeNotes[x].BPM);
                        result += "\t";
                    }
                    return result + "\n";
                }
                else
                {
                    int times = 0;
                    string result = "BPM_DEF" + "\t";
                    foreach (BPMChange x in ChangeNotes)
                    {
                        result += String.Format("{0:F3}", x.BPM);
                        result += "\t";
                        times++;
                    }
                    while (times < 4)
                    {
                        result += String.Format("{0:F3}", this.ChangeNotes[0].BPM);
                        result += "\t";
                        times++;
                    }
                    return result + "\n";
                }
            }
        }

        /// <summary>
        /// See if the BPMChange is valid
        /// </summary>
        /// <returns>True if valid, false elsewise</returns>
        public bool CheckValidity()
        {
            bool result = true;
            return result;
        }

        /// <summary>
        /// Compose BPMChanges in beginning of MA2
        /// </summary>
        /// <returns></returns>
        public string Compose()
        {
            string result = "";
            for (int i = 0; i < ChangeNotes.Count; i++)
            {
                result += "BPM" + "\t" + this.ChangeNotes[i].Bar + "\t" + this.ChangeNotes[i].Tick + "\t" + this.ChangeNotes[i].BPM + "\n";
                //result += "BPM" + "\t" + bar[i] + "\t" + tick[i] + "\t" + String.Format("{0:F3}", bpm[i])+"\n";
            }
            return result;
        }
    }
}
