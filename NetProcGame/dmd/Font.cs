using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NetProcGame.tools;

namespace NetProcGame.dmd
{
    /// <summary>
    /// Variable width bitmap font.
    /// 
    /// Fonts can be loaded manually, using the load() method or with the font_named() utility
    /// function which supports searching a font path.
    /// </summary>
    public class Font
    {
        /// <summary>
        /// Array of dot widths for each character, 0-indexed from 'space'.
        /// This array is populated by the load() method
        /// </summary>
        public List<uint> char_widths = null;

        /// <summary>
        /// Number of dots to adjust the horizontal position between characters, in addition to the last chars width.
        /// </summary>
        public uint tracking = 0;

        /// <summary>
        /// Composite operation used by draw() when calling DMDBuffer.copy_rect()
        /// </summary>
        public DMDBlendMode composite_op = DMDBlendMode.DMDBlendModeCopy;

        private Animation _anim;
        private uint char_size = 0;
        public Frame bitmap = null;

        public Font(string filename = "")
        {
            this._anim = new Animation();
            this.char_size = 0;
            this.bitmap = null;
            if (filename != "")
            {
                this.load(filename);
            }
        }

        /// <summary>
        /// Loads a font from a dmd file. Fonts are stored in .dmd files with frame 0 containing
        /// the bitmap data and frame 1 containing the character widths. 96 characters (32..127, ASCII 
        /// printables) are stored in a 10x10 grid starting with space (' ') in the upper left at 0,0
        /// The character widths are stored in the second frame within the raw bitmap data in bytes 0-95
        /// </summary>
        /// <param name="filename"></param>
        public void load(string filename)
        {
            this.char_widths = new List<uint>();
            this._anim.load(filename, false);

            if (this._anim.width != this._anim.height)
                throw new Exception("Width != Height");

            if (this._anim.frames.Count == 1)
            {
                // We allow 1 frame for handmade fonts.
                // This is so that they can be loaded as a basic bitmap, have their char widths modified
                // then be saved (TODO)
                this._anim.frames.Add(new Frame(this._anim.width, this._anim.height));
            }
            else if (this._anim.frames.Count != 2)
            {
                throw new Exception("Expected 2 frames, got " + this._anim.frames.Count.ToString());
            }
            this.char_size = (this._anim.width / 10);
            this.bitmap = this._anim.frames[0];
            for (uint i = 0; i < 96; i++)
            {
                this.char_widths.Add(this._anim.frames[1].get_dot(i % _anim.width, i / _anim.width));
            }
        }

        /// <summary>
        /// Save the font to the given path
        /// </summary>
        public void save(string filename)
        {
            Animation result = new Animation();
            result.width = this._anim.width;
            result.height = this._anim.height;
            result.frames.Add(this.bitmap);
            result.frames.Add(new Frame(result.width, result.height));
            for (uint i = 0; i < 96; i++)
            {
                result.frames[1].set_dot(i % _anim.width, i / _anim.width, (byte)this.char_widths[(int)i]);
            }
        }

        /// <summary>
        /// Uses this fonts characters to draw the given string at the given position
        /// </summary>
        public void draw(Frame frame, string text, uint x, uint y)
        {
            Console.WriteLine(String.Format("draw() start at {0}", Time.GetTime()));
            foreach (char ch in text)
            {
                uint char_offset = (uint)ch - (uint)' ';
                if (char_offset < 0 || char_offset >= 96)
                    continue;

                uint char_x = this.char_size * (char_offset % 10);
                uint char_y = this.char_size * (char_offset / 10);
                uint width = this.char_widths[(int)char_offset];
                Frame.copy_rect(frame, x, y, this.bitmap, char_x, char_y, width, this.char_size, this.composite_op);
                x += width + this.tracking;
            }
            Console.WriteLine(String.Format("draw() end at {0}", Time.GetTime()));
            Console.WriteLine("font.draw() called");
        }

        /// <summary>
        /// Returns a tuple of the width and height of this text as rendered with this font.
        /// </summary>
        public Pair<uint, uint> size(string text)
        {
            uint x = 0;
            uint char_offset = 0;
            foreach (char ch in text)
            {
                char_offset = (uint)ch - (uint)' ';
                if (char_offset < 0 || char_offset >= 96)
                    continue;

                uint width = this.char_widths[(int)char_offset];
                x += width + this.tracking;
            }
            return new Pair<uint, uint>(x, this.char_size);
        }
    }
}
