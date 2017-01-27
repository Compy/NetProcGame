using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace NetProcGame.dmd
{
    public class FontManager
    {
        public static FontManager instance;

        private Dictionary<string, Font> _font_cache;
        public List<string> font_paths;

        public FontManager(string path)
        {
            instance = this;
            _font_cache = new Dictionary<string, Font>();
            if (!path.EndsWith(@"/")) path = path + @"/";
            font_paths = new List<string>();
            font_paths.Add(path);
        }

        /// <summary>
        /// Searches the font path for a font file of the given name and returns an instance of the
        /// Font class for the given font if it exists.
        /// </summary>
        public Font font_named(string name)
        {
            if (_font_cache.ContainsKey(name))
                return _font_cache[name];

            foreach (string _font_path in font_paths)
            {

                if (File.Exists(_font_path + name))
                {
                    Font font = new Font(_font_path + name);
                    _font_cache.Add(name, font);
                    return font;
                }
                else if (File.Exists(_font_path + name + ".dmd"))
                {
                    Font font = new Font(_font_path + name + ".dmd");
                    _font_cache.Add(name, font);
                    return font;
                }
            }
            throw new Exception("Font named " + name + " not found. Paths = " + get_font_paths());
        }

        private string get_font_paths()
        {
            string result = "";
            foreach (string path in font_paths)
            {
                result += path + "\n";
            }
            return result;
        }
    }
}
