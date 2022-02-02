using NetProcGame.Dmd;
using NetProcGame.Tools;

namespace NetProcGame.Dmd
{
    /// <summary>
    /// Pans a frame about on a 128x32 buffer, bouncing when it reaches the boundaries
    /// </summary>
    public class PanningLayer : Layer
    {
        public Frame buffer;
        public Frame frame;
        public Pair<int, int> origin;
        public Pair<int, int> original_origin;
        public Pair<int, int> translate;
        public bool bounce;
        public int tick;
        public PanningLayer(int width, int height, Frame frame, Pair<int, int> origin, Pair<int, int> translate, bool bounce = true)
        {
            this.buffer = new Frame(width, height);
            this.frame = frame;
            this.origin = origin;
            this.original_origin = origin;
            this.translate = translate;
            this.bounce = bounce;
            this.tick = 0;

            // make sure the translate value doesnt cause us to do any strange movements
            if (width == frame.width)
                this.translate = new Pair<int, int>(0, this.translate.Second);
            if (height == frame.height)
                this.translate = new Pair<int, int>(this.translate.First, 0);
        }

        public override void reset()
        {
            this.origin = this.original_origin;
        }

        public override Frame next_frame()
        {
            this.tick += 1;

            if ((this.tick % 6) != 0) return this.buffer;

            Frame.copy_rect(this.buffer, 0, 0, this.frame, this.origin.First, this.origin.Second, this.buffer.width, this.buffer.height);
            if (this.bounce && (this.origin.First + this.buffer.width + this.translate.First > this.frame.width) ||
                this.origin.First + this.translate.First < 0)
            {
                this.translate = new Pair<int, int>(this.translate.First * -1, this.translate.Second);
            }

            if (this.bounce && (this.origin.Second + this.buffer.height + this.translate.Second > this.frame.height) ||
                (this.origin.Second + this.translate.Second < 0))
            {
                this.translate = new Pair<int, int>(this.translate.First, this.translate.Second * -1);
            }

            this.origin = new Pair<int, int>(this.origin.First + this.translate.First, this.origin.Second + this.translate.Second);
            return this.buffer;
        }
    }
}
