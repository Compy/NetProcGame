using System;

namespace NetProc.Pdb
{
    public interface IPDLED
    {
        uint BoardAddress { get; }
        void WriteAddress(uint addr);
        void WriteColor(uint index, uint color);
        void WriteFadeColor(uint index, uint color);
        void WriteFadeTime(uint time);
    }

    public class PDLED : IPDLED
    {
        const byte PROC_OUTPUT_MODULE = 3;
        const int PROC_PDB_BUS_ADDRESS = 0xC00;
        private readonly uint baseRegAddress;
        private readonly IProcDevice proc;
        private uint fadeTime;
        public PDLED(IProcDevice proc, uint boardAddress)
        {
            this.proc = proc;
            this.BoardAddress = boardAddress;
            baseRegAddress = 0x01000000 | (boardAddress & 0x3F) << 16;
            fadeTime = 0;
            Console.WriteLine($"Created pdled, board address {boardAddress}");
        }

        public uint BoardAddress { get; private set; }
        public void WriteAddress(uint addr)
        {
            uint data = baseRegAddress | (addr % 0xFF);
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
        }

        public void WriteColor(uint index, uint color)
        {
            WriteAddress(index);
            var data = baseRegAddress | (1 << 8) | (color & 0xFF);
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
        }

        public void WriteFadeColor(uint index, uint color)
        {
            WriteAddress(index);
            var data = baseRegAddress | (2 << 8) | (color & 0xFF);
            proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
        }

        public void WriteFadeTime(uint time)
        {
            if(time != fadeTime)
            {
                fadeTime = time;
                var data = baseRegAddress | (3 << 8) | (time & 0xFF);
                proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
                data = baseRegAddress | (4 << 8) | (time >> 0xFF) & 0xFF;
                proc.WriteData(PROC_OUTPUT_MODULE, PROC_PDB_BUS_ADDRESS, ref data);
            }
        }
    }
}
