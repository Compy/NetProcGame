using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.dmd
{
    /// <summary>
    /// An ordered collection of Frame objects
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Width of each of the animation frames in dots
        /// </summary>
        public uint width = 0;

        /// <summary>
        /// Height of each of the animation frames in dots
        /// </summary>
        public uint height = 0;

        /// <summary>
        /// Ordered collection of Frame objects
        /// </summary>
        public List<Frame> frames;

        public Animation()
        {
            frames = new List<Frame>();
        }

    }
}
