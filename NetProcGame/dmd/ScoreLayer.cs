﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame.tools;
using NetProcGame.game;
using NetProcGame.modes;

namespace NetProcGame.dmd
{
    public class ScoreLayer : GroupedLayer
    {
        public ScoreDisplay mode;

        public ScoreLayer(int width, int height, ScoreDisplay mode)
            : base(width, height)
        {
            this.mode = mode;
        }

        public override Frame next_frame()
        {
            this.mode.update_layer();
            return base.next_frame();
        }
    }
}
