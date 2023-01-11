namespace NetProc.Pdb
{

    public class PDBSwitch
    {
        private int sw_number;

        public PDBSwitch(string number_str)
        {
            var upperStr = number_str.ToUpper();
            if (upperStr.StartsWith("SD"))
            {
                this.SwitchType = PdbSwitchType.dedicated;
                sw_number = int.Parse(upperStr.Substring(2));
            }
            else if (upperStr.Contains("/"))
            {
                this.SwitchType = PdbSwitchType.matrix;
                sw_number = ParseMatrixNum(upperStr);
            }
            else
            {
                this.SwitchType = PdbSwitchType.proc;
                sw_number = int.Parse(number_str);
            }
        }

        public PdbSwitchType SwitchType { get; }
        public int ProcNum() => sw_number;

        private int ParseMatrixNum(string upperStr)
        {
            var crList = upperStr.Split('/');
            return (32 + int.Parse(crList[0]) * 16 + int.Parse(crList[1]));
        }
    }
}
