using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.pdb
{
    public class Switch
    {
        private int sw_number;
        public Switch(string number_str)
        {
			sw_number = Int32.Parse (number_str);
        }

        public int proc_num()
        {
            return sw_number;
        }

        public int parse_matrix_num(string num)
        {
            string[] cr_list = num.Split('/');
            return (32 + Int32.Parse(cr_list[0]) * 16 + Int32.Parse(cr_list[1]));
        }
    }
}
