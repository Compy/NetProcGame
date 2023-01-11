using System;

namespace NetProc.Pdb
{
    public class Lamp
    {
        public string lamp_type;
        private byte banknum = 0;
        private byte output;
        private PDBConfig pdb;
        private byte sink_banknum;
        private byte sink_boardnum;
        private byte sink_outputnum;
        private byte source_banknum;
        private byte source_boardnum;
        private byte source_outputnum;

        public Lamp(PDBConfig pdb, string number_str)
        {
            this.pdb = pdb;
            string upper_str = number_str.ToUpper();
            if (IsDirectLamp(upper_str))
            {
                this.lamp_type = "dedicated";
                this.output = (byte)(Int32.Parse(number_str.Substring(1)));
            }
            else if (IsPDBLamp(number_str))
            {
                this.lamp_type = "pdb";
                string[] addr_parts = PDBFunctions.SplitMatrixAddressParts(number_str);
                string source_addr, sink_addr;
                source_addr = addr_parts[0];
                sink_addr = addr_parts[1];


                PDBAddress addr = PDBFunctions.DecodePdbAddress(source_addr, pdb.aliases.ToArray());
                source_boardnum = addr.board;
                source_banknum = addr.bank;
                source_outputnum = addr.output;

                addr = PDBFunctions.DecodePdbAddress(sink_addr, pdb.aliases.ToArray());
                sink_boardnum = addr.board;
                sink_banknum = addr.bank;
                sink_outputnum = addr.output;
            }
            else
            {
                lamp_type = "unknown";
            }
            
        }

        public byte DedicatedBank() => this.banknum;

        public byte DedicatedOutput() => this.output;

        public bool IsDirectLamp(string str)
        {
            int testNum;
            if (str.Length < 2 || str.Length > 3) return false;
            if (str[0] != 'L') return false;
            if (!Int32.TryParse(str.Substring(1), out testNum)) return false;
            return true;
        }

        public bool IsPDBLamp(string str)
        {
            string[] _params = PDBFunctions.SplitMatrixAddressParts(str);
            if (_params.Length != 2) return false;
            foreach (string addr in _params)
            {
                if (!PDBFunctions.IsPdbAddress(addr, this.pdb.aliases.ToArray()))
                {
                    Console.WriteLine("Not PDB address! " + addr);
                    return false;
                }
            }
            return true;
        }

        public byte SinkBank() => (byte)(sink_boardnum * 2 + sink_banknum);
        public byte SinkBoard() => this.sink_boardnum;
        public byte SinkOutput() => sink_outputnum;
        public byte SourceBank() => (byte)(source_boardnum * 2 + source_banknum);
        public byte SourceBoard() => this.source_boardnum;
        public byte SourceOutput() => source_outputnum;
    }
}
