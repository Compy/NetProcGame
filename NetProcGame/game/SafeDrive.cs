﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.game
{
    /// <summary>
    /// Queued thread-safe coil drive entry
    /// </summary>
    public class SafeCoilDrive
    {
        public string coil_name = "";
        public bool pulse = false;
        public ushort pulse_time = 30;
        public bool disable = false;
    }
}
