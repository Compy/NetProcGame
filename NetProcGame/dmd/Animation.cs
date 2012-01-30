using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using NetProcGame.tools;

namespace NetProcGame.dmd
{
    /// <summary>
    /// An ordered collection of Frame objects
    /// </summary>
    public class Animation
    {
        /// <summary>
        /// Width of each of the animation frames in dots
        /// </summary>
        public uint width = 0;

        /// <summary>
        /// Height of each of the animation frames in dots
        /// </summary>
        public uint height = 0;

        /// <summary>
        /// Ordered collection of Frame objects
        /// </summary>
        public List<Frame> frames;

        public Animation()
        {
            frames = new List<Frame>();
        }

        /// <summary>
        /// Loads the given file from disk. The native animation format is the 'dmd-format' which
        /// can be created using the dmdconvert tool.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="allow_cache"></param>
        public Animation load(string filename, bool allow_cache = true)
        {
            double t0 = Time.GetTime();
            // Load the file from disk
            if (filename.EndsWith(".dmd"))
            {
                // Load in from DMD file
                this.populate_from_dmd_file(filename);
            }
            else
            {
                // Load from other image formats (TODO)
            }

            return this;
        }

        /// <summary>
        /// Saves the animation as a .dmd file in the given filename
        /// </summary>
        public void save(string filename)
        {
            if (this.width == 0 || this.height == 0)
                throw new Exception("Width and height must be set on an animation before it can be saved.");

            this.save_to_dmd_file(filename);
        }

        public void populate_from_dmd_file(string filename)
        {
            BinaryReader br = new BinaryReader(File.Open(filename, FileMode.Open));
            long file_length = br.BaseStream.Length;
            br.BaseStream.Seek(4, SeekOrigin.Begin); // Skip over the 4 byte DMD header
            int frame_count = br.ReadInt32();
            this.width = (uint)br.ReadInt32();
            this.height = (uint)br.ReadInt32();

            if (file_length != 16 + this.width * this.height * frame_count)
                throw new Exception("File size inconsistent with header information. Old or incompatible file format?");

            for (int frame_index = 0; frame_index < frame_count; frame_index++)
            {
                byte[] frame = br.ReadBytes((int)(this.width * this.height));
                Frame new_frame = new Frame(this.width, this.height);
                new_frame.set_data(frame);
                this.frames.Add(new_frame);
            }
        }

        public void save_to_dmd_file(string filename)
        {
            BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Create));
            bw.Write(0x00646D64); // 4 byte DMD header
            bw.Write(this.frames.Count); // Frame count
            bw.Write((int)this.width); // Animation width
            bw.Write((int)this.height); // Animation height
            foreach (Frame f in this.frames)
                bw.Write(f.get_data());

            bw.Close();
        }
    }
}
