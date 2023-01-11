using System;

namespace NetProc.Data.Model
{
    public class Audit
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public AuditType Type { get; set; }
        public string Info { get; set; }
    }

    [Flags]
    public enum AuditType
    {
        Standard,
        Game,
        Earnings
    }
}
