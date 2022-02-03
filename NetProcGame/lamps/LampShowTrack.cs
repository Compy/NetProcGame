using NetProc;
using NetProcGame.Game;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NetProcGame.Lamps
{
    public class LampShowTrack
    {
        /// <summary>
        /// Name of this track which corresponds to a driver
        /// </summary>
        public string name = "";

        /// <summary>
        /// Sequence of 32-bit schedule values
        /// </summary>
        public List<uint> schedules;

        /// <summary>
        /// Index into the schedules list
        /// </summary>
        public int current_index = 0;

        /// <summary>
        /// The driver corresponding to this track
        /// </summary>
        public IDriver driver = null;

        private uint static31shift;

        public LampShowTrack(string line)
        {
            this.static31shift = BitConverter.ToUInt32(BitConverter.GetBytes(1 << 31), 0);
            this.load_from_line(line);
        }

        public void load_from_line(string line)
        {
            Regex line_re = new Regex(@"(?<name>\S+)\s*\| (?<data>.*)$");
            Match m = line_re.Match(line);

            if (m == null)
                throw new ArgumentException("Regexp didnt match on the track line: " + line);

            this.name = m.Groups["name"].Value;
            string data = m.Groups["data"].Value + new string(' ', 32);
            data = LampshowUtils.expand_line(data);

            uint bits = 0;
            int bit_count = 0;
            bool ignore_first = true;
            this.schedules = new List<uint>();
            foreach (char ch in data)
            {
                bits >>= 1;
                bit_count++;

                if (ch != ' ')
                    bits |= static31shift;
                if (bit_count % 16 == 0)
                {
                    if (!ignore_first)
                        this.schedules.Add(bits);
                    ignore_first = false;
                }
            }
        }

        public void resolve_driver_with_game(IGameController game)
        {
            if (name.StartsWith("coil:"))
                this.driver = game.Coils[name.Substring(5)];
            else if (name.StartsWith("lamp:"))
                this.driver = game.Lamps[name.Substring(5)];
            else
                this.driver = game.Lamps[name];
        }

        /// <summary>
        /// Clears the contents of this track
        /// </summary>
        public void reset()
        {
            this.schedules.Clear();
            this.current_index = 0;
        }

        /// <summary>
        /// Restarts this track at the beginning
        /// </summary>
        public void restart()
        {
            this.current_index = 0;
        }

        public uint next_schedule()
        {
            if (this.is_complete()) return 0;
            this.current_index++;
            return (uint)this.schedules[this.current_index - 1];
        }

        public bool is_complete()
        {
            return this.current_index >= this.schedules.Count;
        }

    }
}
