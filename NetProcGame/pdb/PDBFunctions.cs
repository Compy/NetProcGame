using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.pdb
{
    public class PDBFunctions
    {
        public struct PDBAddress
        {
            public int board;
            public int bank;
            public int output;
        }

        public static bool is_pdb_address(string addr, DriverAlias[] aliases = null)
        {
            try
            {
                decode_pdb_address(addr, aliases);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static PDBAddress decode_pdb_address(string addr, DriverAlias[] aliases = null)
        {
            if (aliases == null) aliases = new DriverAlias[] { };
            string[] _params;
            PDBAddress address = new PDBAddress();

            foreach (DriverAlias alias in aliases)
            {
                if (alias.match(addr).Success)
                {
                    addr = alias.decode(addr);
                    break;
                }
            }

            if (addr.Contains('-'))
            {
                _params = addr.Split('-');
                Array.Reverse(_params);

                if (_params.Length != 3)
                    throw new ArgumentOutOfRangeException("PDB address must have 3 components");

                address.board = Int32.Parse(_params[0].Substring(1));
                address.bank = Int32.Parse(_params[1].Substring(1));
                address.output = Int32.Parse(_params[2]);
                return address;
            }
            else if (addr.Contains('/'))
            {
                _params = addr.Split('/');
                Array.Reverse(_params);

                if (_params.Length != 3)
                    throw new ArgumentOutOfRangeException("PDB address must have 3 components");

                address.board = Int32.Parse(_params[0]);
                address.bank = Int32.Parse(_params[1]);
                address.output = Int32.Parse(_params[2]);
                return address;
            }
            else
                throw new ArgumentException("PDB address delimiter (- or /) not found");
        }
    }
}
