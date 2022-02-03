using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NetProc.Pdb
{
    public class DriverAlias
    {
        public Regex expr;
        string repl;
        public DriverAlias(string key, string value)
        {
            this.expr = new Regex(key);
            this.repl = value;
        }

        public MatchCollection matches(string addr)
        {
            return expr.Matches(addr);
        }

        public Match match(string addr)
        {
            return expr.Match(addr);
        }

        public string decode(string addr)
        {
            return expr.Replace(addr, repl);
        }
    }
}
