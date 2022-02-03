using NetProc.Dmd;

namespace NetProcGame.Modes
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
