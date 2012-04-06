using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetProcGame.pdb
{
    public class Switch
    {
        private string sw_type;
        private int sw_number;
        public Switch(string number_str)
        {
            string upper_str = number_str.ToUpper();
            if (upper_str.StartsWith("SD"))
            {
                sw_type = "dedicated";
            }
            else if (upper_str.Contains('/'))
            {
                sw_type = "matrix";
                sw_number = this.parse_matrix_num(upper_str);
            }
            else
            {
                sw_type = "proc";
                sw_number = Int32.Parse(number_str);
            }
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
