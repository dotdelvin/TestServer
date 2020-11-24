using System;
using TestServer.World;

namespace TestServer.Events
{
    /// <summary>
    ///     Provides data for the <see cref="Player.Resumed" /> event.
    /// </summary>
    public class ResumeEventArgs : EventArgs
    {
        /// <summary>
        ///     Gets the time that the player was paused.
        /// </summary>
        public int Time { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ResumeEventArgs" /> class.
        /// </summary>
        /// <param name="time">The time that the player was paused.</param>
        public ResumeEventArgs(int time)
        {
            Time = time;
        }
    }
}