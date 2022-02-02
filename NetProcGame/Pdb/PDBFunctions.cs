using System;
using System.Collections.Generic;
using System.Linq;

namespace NetProcGame.Pdb
{
    public class PDBFunctions
    {
        /// <summary>
        /// Returns True if the given address is a valid PDB address.
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="aliases"></param>
        /// <returns></returns>
        public static bool IsPdbAddress(string addr, DriverAlias[] aliases = null)
        {
            try
            {
                DecodePdbAddress(addr, aliases);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsPDBLamp(string str, DriverAlias[] aliases = null)
        {
            string[] _params = SplitMatrixAddressParts(str);
            if (_params.Length != 2) return false;
            foreach (string addr in _params)
            {
                if (!IsPdbAddress(addr, aliases))
                {
                    Console.WriteLine("Not PDB address! " + addr);
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Decodes Ax-By-z or x/y/z into PDB address, bank number, and output number. <para/>
        /// Raises exception if it is not a PDB address, otherwise returns a PDBAddress - (addr, bank, number).
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="aliases"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static PDBAddress DecodePdbAddress(string addr, DriverAlias[] aliases = null)
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

                if (_params.Length != 3)
                    throw new ArgumentOutOfRangeException("PDB address must have 3 components");

                address.board = (byte)Int32.Parse(_params[0].Substring(1));
                address.bank = (byte)Int32.Parse(_params[1].Substring(1));
                address.output = (byte)Int32.Parse(_params[2]);
                return address;
            }
            else if (addr.Contains('/'))
            {
                _params = addr.Split('/');
                Array.Reverse(_params);

                if (_params.Length != 3)
                    throw new ArgumentOutOfRangeException("PDB address must have 3 components");

                address.board = (byte)Int32.Parse(_params[0]);
                address.bank = (byte)Int32.Parse(_params[1]);
                address.output = (byte)Int32.Parse(_params[2]);
                return address;
            }
            else
                throw new ArgumentException("PDB address delimiter (- or /) not found");
        }

        /// <summary>
        /// Input is of form C-Ax-By-z:R-Ax-By-z  or  C-x/y/z:R-x/y/z  or  aliasX:aliasY <para/>
        /// We want to return only the address part: Ax-By-z, x/y/z, or aliasX.  That is, remove the two character prefix if present.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string[] SplitMatrixAddressParts(string str)
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
    }
}
