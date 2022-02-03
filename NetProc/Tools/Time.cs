using System;

namespace NetProc.Tools
{
    public class Time
    {
        /// <summary>
        /// Get the current unix timestamp
        /// </summary>
        /// <returns>Number of seconds since the unix epoch</returns>
        public static double GetTime()
        {
            TimeSpan ts = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return ts.TotalSeconds;
        }
    }
}
