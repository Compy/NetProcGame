using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NetProc.Game.Lamps
{
    public static class LampshowUtils
    {
        public static string make_pattern_of_length(int l)
        {
            string pattern = ".  .  . . .. .. ... ... .... .... ..... .....";
            if (l > pattern.Length) l = pattern.Length;
            string s = pattern.Substring(0, l);
            if (s.Length < l)
            {
                s += String.Concat(Enumerable.Repeat(".", l - s.Length));
            }
            return s;
        }
        public static string fade_in(int length)
        {
            return make_pattern_of_length(length);
        }
        public static string fade_out(int length)
        {
            string s = fade_in(length);
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        public static string fade_fade(int length)
        {
            string a = fade_in(length / 2);
            string b = fade_out(length / 2);
            if ((length % 2) == 0)
                return a.Substring(0, a.Length - 1) + " " + b;
            else
                return a + " " + b;
        }

        /// <summary>
        /// Expands special characters "<>[]" within 'str' and returns the dots-and-spaces representation.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string expand_line(string str)
        {
            str = Regex.Replace(str, @"(\[[\- ]*\])", delegate(Match match)
            {
                return new string('.', match.Groups[1].Length);
            });

            str = Regex.Replace(str, @"(\<[\- ]*\])", delegate(Match match)
            {
                string f = fade_in(match.Groups[1].Length);
                return f.Substring(0, f.Length - 1) + ".";
            });

            str = Regex.Replace(str, @"(\[[\- ]*\>)", delegate(Match match)
            {
                string f = fade_out(match.Groups[1].Length);
                return "." + f.Substring(1);
            });

            str = Regex.Replace(str, @"(\<[\- ]*\>)", delegate(Match match)
            {
                return fade_fade(match.Groups[1].Length);
            });

            return str;
        }
    }
}
