using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.pdb
{
    public class Coil
    {
        private PDBConfig pdb;
        public string coil_type = "";
        public byte bank_num;
        public byte output_num;
        public byte board_num;

        public Coil(PDBConfig pdb, string number_str)
        {
            this.pdb = pdb;
            string upper_str = number_str.ToUpper();

            if (this.is_direct_coil(upper_str))
            {
                this.coil_type = "dedicated";
                this.bank_num = (byte)((Int32.Parse(number_str.Substring(1)) - 1) / 8);
                this.output_num = (byte)(Int32.Parse(number_str.Substring(1)));
            }
            else if (this.is_pdb_coil(number_str))
            {
                this.coil_type = "pdb";
                PDBFunctions.PDBAddress addr = PDBFunctions.decode_pdb_address(number_str, this.pdb.aliases.ToArray());
                this.board_num = addr.board;
                this.bank_num = addr.bank;
                this.output_num = addr.output;
            }
            
        }

        public int bank()
        {
            if (this.coil_type == "dedicated")
                return this.bank_num;
            else if (this.coil_type == "pdb")
                return this.board_num * 2 + this.bank_num;
            else
                return -1;
        }

        public int output()
        {
            return this.output_num;
        }

        public bool is_direct_coil(string str)
        {
            int testNum;
            if (str.Length < 2 || str.Length > 3) return false;
            if (str[0] != 'C') return false;
            if (Int32.TryParse(str.Substring(1), out testNum)) return false;
            return true;
        }

        public bool is_pdb_coil(string str)
        {
            return PDBFunctions.is_pdb_address(str, this.pdb.aliases.ToArray());
        }
    }
}
