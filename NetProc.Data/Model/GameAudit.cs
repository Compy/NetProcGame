using System.Collections.Generic;

namespace NetProc.Data.Model
{
    public class GameAudit
    {
        public int Id { get; set; }
        public ICollection<GameLog> GameLogs { get; set; }
    }
}
