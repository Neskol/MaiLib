﻿namespace MaiLib
{
    /// <summary>
    /// Provide interface and basic functions for Notes
    /// </summary>
    interface INote
    {
        /// <summary>
        /// Convert note to string for writing
        /// </summary>
        /// <param name="format">0 if Simai, 1 if Ma2</param>
        string Compose(int format);

        /// <summary>
        /// See if current note has all information needed
        /// </summary>
        /// <returns>True if qualified, false otherwise</returns>
        bool CheckValidity();

        /// <summary>
        /// Updates this note instance.
        /// </summary>
        /// <returns>True if Calculated Times is defined, false otherwise</returns>
        bool Update();

        /// <summary>
        /// Flip the note according to the method specified
        /// </summary>
        /// <param name="method">UpSideDown, LeftToRight, Clockwise90/180, Counterclockwise90/180</param>
        void Flip(string method);

        /// <summary>
        /// Give time stamp given overall tick
        /// </summary>
        /// <param name="overallTick">Note.Bar*384+Note.Tick</param>
        /// <returns>Appropriate time stamp in seconds</returns>
        double GetTimeStamp(int overallTick);
    }
}
