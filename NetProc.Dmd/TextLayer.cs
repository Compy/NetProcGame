using NetProc.Interface;
using NetProc.Tools;

namespace NetProc.Dmd
{
    public enum FontJustify
    {
        Left,
        Right,
        Center
    }
    /// <summary>
    /// A layer that displays text
    /// </summary>
    public class TextLayer : Layer
    {
        private Font font;
        private double started_at = -1;
        private int seconds = -1;
        public Frame frame = null;
        public Frame frame_old = null;
        private FontJustify justify = FontJustify.Left;
        private int blink_frames = -1;
        private int blink_frames_counter = 0;

        public TextLayer(int x, int y, Font font, FontJustify justify, bool opaque = false)
            : base(opaque)
        {
            this.set_target_position(x, y);
            this.font = font;
            this.started_at = -1;
            this.seconds = -1;
            this.frame = null;
            this.frame_old = null;
            this.justify = justify;
            this.blink_frames = -1;
            this.blink_frames_counter = 0;
        }

        /// <summary>
        /// Displays the given message for the given number of seconds
        /// </summary>
        public void set_text(string text, int seconds = -1, int blink_frames = -1)
        {
            this.started_at = -1;
            this.seconds = seconds;
            this.blink_frames = blink_frames;
            this.blink_frames_counter = this.blink_frames;

            if (text == "")
                this.frame = null;
            else
            {
                Pair<int, int> font_size = this.font.size(text);
                this.frame = new Frame(font_size.First, font_size.Second);
                this.font.draw(this.frame, text, 0, 0);
                if (this.justify == FontJustify.Left)
                {
                    this.target_x_offset = 0;
                    this.target_y_offset = 0;
                }
                else if (this.justify == FontJustify.Right)
                {
                    this.target_x_offset = (127 - font_size.First);
                    this.target_y_offset = 0;
                }
                else if (this.justify == FontJustify.Center)
                {
                    this.target_x_offset = (int)-(font_size.First / 2);
                    this.target_y_offset = 0;
                }
            }
        }

        public override IFrame next_frame()
        {
            if (this.started_at == -1)
                this.started_at = Time.GetTime();
            if ((this.seconds != -1) && (this.started_at + this.seconds < Time.GetTime()))
                this.frame = null;
            else if (this.blink_frames > 0)
            {
                if (this.blink_frames_counter == 0)
                {
                    this.blink_frames_counter = this.blink_frames;
                    if (this.frame == null)
                        this.frame = this.frame_old;
                    else
                    {
                        this.frame_old = this.frame;
                        this.frame = null;
                    }
                }
                else
                    this.blink_frames_counter--;
            }
            return this.frame;
        }

        public bool is_visible()
        {
            return this.frame != null;
        }
    }
}
