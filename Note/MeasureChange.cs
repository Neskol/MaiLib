namespace MaiLib
{
    /// <summary>
    /// Defines measure change note that indicates a measure change in bar.
    /// </summary>
    public class MeasureChange : Note
    {
        private int quaver;
        private int beat;

        /// <summary>
        /// Construct Empty
        /// </summary>
        public MeasureChange()
        {
            this.Bar = 0;
            this.Tick = 0;
            this.quaver = 4;
            this.beat = 4;
            this.Update();
        }

        /// <summary>
        /// Construct BPMChange with given bar, tick, BPM
        /// </summary>
        /// <param name="bar">Bar</param>
        /// <param name="tick">Tick</param>
        /// <param name="Quaver">Quaver</param>
        public MeasureChange(int bar, int tick, int quaver, int beat)
        {
            this.Bar = bar;
            this.Tick = tick;
            this.quaver = quaver;
            this.beat = beat;
            this.Update();
        }

        /// <summary>
        /// Construct measureChange from another takeIn
        /// </summary>
        /// <param name="takeIn">Another measure change note</param>
        public MeasureChange(MeasureChange takeIn)
        {
            this.Bar = takeIn.Bar;
            this.Tick = takeIn.Tick;
            this.quaver = takeIn.Quaver;
            this.Update();
        }

        /// <summary>
        /// Return this.quaver
        /// </summary>
        /// <value>Quaver</value>
        public int Quaver
        {
            get { return this.quaver; }
        }

        /// <summary>
        /// Return this.beat
        /// </summary>
        /// <value>Quaver</value>
        public int Beat
        {
            get { return this.Beat; }
        }

        public override bool CheckValidity()
        {
            return this.quaver > 0;
        }

        public override string Compose(int format)
        {
            string result = "";
            if (format == 0)
            {
                result += "{" + this.Quaver + "}";
                //result += "{" + this.Quaver+"_"+this.Tick + "}";
            }
            else if (format == 1)
            {
                result += "MET\t"+this.Quaver+"\t"+this.Beat+"\n";
                //result += "{" + this.Quaver+"_"+this.Tick + "}";
            }
            return result;
        }

        public override Note NewInstance()
        {
            Note result = new MeasureChange(this);
            return result;
        }

        public override string NoteGenre => "MEASURE";

        public override bool IsNote => false;

        public override string NoteSpecificGenre => "MEASURE";
    }
}
