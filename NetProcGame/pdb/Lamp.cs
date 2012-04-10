using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.pdb
{
    public class Lamp
    {
        private PDBConfig pdb;
        public string lamp_type;

        private byte output;
        private byte banknum;
        private byte source_boardnum;
        private byte source_banknum;
        private byte source_outputnum;
        private byte sink_boardnum;
        private byte sink_banknum;
        private byte sink_outputnum;

        public Lamp(PDBConfig pdb, string number_str)
        {
            this.pdb = pdb;
            string upper_str = number_str.ToUpper();
            if (is_direct_lamp(upper_str))
            {
                this.lamp_type = "dedicated";
                this.output = (byte)(Int32.Parse(number_str.Substring(1)));
            }
            else if (is_pdb_lamp(number_str))
            {
                this.lamp_type = "pdb";
                string[] addr_parts = split_matrix_addr_parts(number_str);
                string source_addr, sink_addr;
                source_addr = addr_parts[0];
                sink_addr = addr_parts[1];


                PDBFunctions.PDBAddress addr = PDBFunctions.decode_pdb_address(source_addr, pdb.aliases.ToArray());
                source_boardnum = addr.board;
                source_banknum = addr.bank;
                source_outputnum = addr.output;

                addr = PDBFunctions.decode_pdb_address(sink_addr, pdb.aliases.ToArray());
                sink_boardnum = addr.board;
                sink_banknum = addr.bank;
                sink_outputnum = addr.output;
            }
            else
            {
                lamp_type = "unknown";
            }
            
        }

        public byte source_board()
        {
            return this.source_boardnum;
        }

        public byte sink_board()
        {
            return this.sink_boardnum;
        }

        public byte source_bank()
        {
            return (byte)(source_boardnum * 2 + source_banknum);
        }

        public byte sink_bank()
        {
            return (byte)(sink_boardnum * 2 + sink_banknum);
        }

        public byte source_output()
        {
            return source_outputnum;
        }

        public byte sink_output()
        {
            return sink_outputnum;
        }

        public byte dedicated_bank()
        {
            return this.banknum;
        }

        public byte dedicated_output()
        {
            return this.output;
        }

        public bool is_direct_lamp(string str)
        {
            int testNum;
            if (str.Length < 2 || str.Length > 3) return false;
            if (str[0] != 'L') return false;
            if (!Int32.TryParse(str.Substring(1), out testNum)) return false;
            return true;
        }

        public string[] split_matrix_addr_parts(string str)
        {
            string[] addrs = str.Split(':');
            Array.Reverse(addrs);
            if (addrs.Length != 2) return new string[] { };
            List<string> addrs_out = new List<string>();

            foreach (string addr in addrs)
            {
                string[] bits = addr.Split('-');
                if (bits.Length == 1)
                    addrs_out.Add(addr);
                else
                {
                    string joinedString = "";
                    for (int i = 1; i < bits.Length; i++)
                    {
                        if (joinedString != "") joinedString += "-" + bits[i];
                        else joinedString = bits[i];
                    }
                    addrs_out.Add(joinedString);
                }
            }
            return addrs_out.ToArray();
        }

        public bool is_pdb_lamp(string str)
        {
            string[] _params = split_matrix_addr_parts(str);
            if (_params.Length != 2) return false;
            foreach (string addr in _params)
            {
                if (!PDBFunctions.is_pdb_address(addr, this.pdb.aliases.ToArray()))
                {
                    Console.WriteLine("Not PDB address! " + addr);
                    return false;
                }
            }
            return true;
        }
    }
}
