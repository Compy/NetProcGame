using NetProcGame.Game;
using NetProcGame.Tools;
using System.Collections.Generic;
using System.IO;

namespace NetProcGame.Lamps
{
    /// <summary>
    /// Manages loading and playing a lamp show consisting of several lamps (or other drivers),
    /// each of which is a track (LampShowTrack)
    /// </summary>
    public class LampShow
    {

        public IGameController game;
        public List<LampShowTrack> tracks;
        private double t0 = 0;
        private double last_time;

        public LampShow(IGameController game)
        {
            this.game = game;
            this.reset();
        }

        /// <summary>
        /// Clears out all of the tracks in this lamp show
        /// </summary>
        public void reset()
        {
            this.tracks = new List<LampShowTrack>();
            this.t0 = 0;
            this.last_time = -.5;
        }

        /// <summary>
        /// Reads lines from the given file to create tracks within the lamp show. A lamp show
        /// generally consists of several lines of text, one for each driver, spaced so as to show a 
        /// textual representation of the lamp activity over time
        /// </summary>
        /// <param name="filename"></param>
        public void load(string filename)
        {
            StreamReader file = new StreamReader(filename);
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if (line[0] != '#')
                    this.tracks.Add(new LampShowTrack(line));
            }
            file.Close();
        }

        /// <summary>
        /// Instructs the lamp show to advance based on the system clock and update the drivers
        /// associated with its tracks
        /// </summary>
        public void tick()
        {
            if (this.t0 == 0)
                this.t0 = Time.GetTime();
            double new_time = (Time.GetTime() - this.t0);
            double time_diff = new_time - this.last_time;
            if (time_diff > 0.500)
            {
                this.last_time = new_time;
                for (int i = 0; i < tracks.Count; i++)
                {
                    if (tracks[i].driver == null) // Lazily set drivers
                        tracks[i].resolve_driver_with_game(this.game);
                    uint sch = tracks[i].next_schedule();
                    tracks[i].driver.Schedule(sch, 1, true);
                }
            }
        }

        /// <summary>
        /// Restart the show from the beginning
        /// </summary>
        public void restart()
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                tracks[i].restart();
            }
        }

        /// <summary>
        /// True if each of the tracks has completed
        /// </summary>
        /// <returns></returns>
        public bool is_complete()
        {
            for (int i = 0; i < tracks.Count; i++)
            {
                if (!tracks[i].is_complete())
                    return false;
            }
            return true;
        }
    }
}
