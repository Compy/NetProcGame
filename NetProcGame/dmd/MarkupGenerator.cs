using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetProcGame.dmd;

namespace NetProcGame.dmd
{
    /// <summary>
    /// Renders a Frame object for given text-based markup.
    /// 
    /// The markup format presently uses three markup tokens:
    /// '#' for headlines and '[' and ']' for plain text. The markup tokens indicate
    /// justification. Lines with no markup or a leading '#' or '[' will be left-justified.
    /// Lines with a training '#' or ']' will be right justified. Lines with both will be centered.
    /// 
    /// The width and min-height are specified with instantiation.
    /// 
    /// Fonts can be adjusted by assigning 'font_plain' and 'font_bold' member values.
    /// </summary>
    public class MarkupGenerator
    {
        public Font font_plain = null;
        public Font font_bold = null;

        public Frame frame;
        public int width, min_height;

        public MarkupGenerator(int width = 128, int min_height = 32)
        {
            this.width = width;
            this.min_height = min_height;
            this.frame = null;
            this.font_plain = FontManager.instance.font_named("Font07x5.dmd");
            this.font_bold = FontManager.instance.font_named("Font09Bx7.dmd");
        }

        /// <summary>
        /// Returns a Frame with the given markup rendered within it.
        /// 
        /// The frame width is fixed, but the height will be adjusted to fit the contents while respecting
        /// min_height.
        /// 
        /// The Y offset can be configured by supplying 'y_offset'
        /// </summary>
        public Frame frame_for_markup(string markup, int y_offset = 0)
        {
            string[] lines = markup.Split('\n');
            foreach (bool draw in new bool[] { false, true })
            {
                int y = y_offset;
                foreach (string line in lines)
                {
                    if (line.StartsWith("#") && line.EndsWith("#")) // Centered headline
                        y = this.draw_text(y, line.Substring(1, line.Length - 1), font_bold, FontJustify.Center, draw);
                    else if (line.StartsWith("#")) // Left justified headline
                        y = this.draw_text(y, line.Substring(1), font_bold, FontJustify.Left, draw);
                    else if (line.EndsWith("#")) // Right justified headline
                        y = this.draw_text(y, line.Substring(0, line.Length - 1), font_bold, FontJustify.Right, draw);
                    else if (line.StartsWith("[") && line.EndsWith("]")) // Centered text
                        y = this.draw_text(y, line.Substring(1, line.Length - 1), font_plain, FontJustify.Center, draw);
                    else if (line.EndsWith("]")) // Right justified text
                        y = this.draw_text(y, line.Substring(0, line.Length - 1), font_plain, FontJustify.Right, draw);
                    else if (line.StartsWith("[")) // Left justified text
                        y = this.draw_text(y, line.Substring(1), font_plain, FontJustify.Left, draw);
                    else // Left justified but nothing to clip off
                        y = this.draw_text(y, line, font_plain, FontJustify.Left, draw);
                }
                if (!draw)
                    this.frame = new Frame(this.width, Math.Max(this.min_height, y));
            }
            return this.frame;
        }

        private int draw_text(int y, string text, Font font, FontJustify justify, bool draw)
        {
            if (GetMaxValueInList(font.char_widths) * text.Length > this.width)
            {
                // We need to do some word wrapping
                string line = "";
                int w = 0;
                foreach (char ch in text)
                {
                    line += ch;
                    w += font.size(ch.ToString()).First;
                    if (w > this.width)
                    {
                        // Too much! We need to back-track for the last space. If possible...
                        int idx = line.LastIndexOf(' ');
                        if (idx == -1)
                        {
                            // No space, we'll have to break before this char and continue
                            y = this.draw_line(y, line.Substring(0, line.Length - 1), font, justify, draw);
                            line = ch.ToString();
                        }
                        else
                        {
                            // We found a space!
                            y = this.draw_line(y, line.Substring(0, idx), font, justify, draw);
                            line = line.Substring(idx + 1, line.Length - idx);
                        }
                        // Recalculate w
                        w = font.size(line).First;
                    }
                }
                if (line.Length > 0) // Left-over text we have to draw
                    y = this.draw_line(y, line, font, justify, draw);
                return y;
            }
            else
                return this.draw_line(y, text, font, justify, draw);
        }

        /// <summary>
        /// Draw a line without concern for word wrapping
        /// </summary>
        private int draw_line(int y, string text, Font font, FontJustify justify, bool draw)
        {
            int w = 0;
            if (draw)
            {
                int x = 0; // TODO: x should be set based on justify
                if (justify != FontJustify.Left)
                {
                    w = font.size(text).First;
                    if (justify == FontJustify.Center)
                        x = (this.frame.width - w) / 2;
                    else
                        x = (this.frame.width - w);
                }
                font.draw(this.frame, text, x, y);
            }
            y += font.char_size;
            return y;
        }

        private int GetMaxValueInList(List<int> list)
        {
            int[] listArr = list.ToArray();
            Array.Sort(listArr);
            return listArr[listArr.Length - 1];
        }
    }
}
