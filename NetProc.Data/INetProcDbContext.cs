using Microsoft.EntityFrameworkCore;
using NetProc;
using NetProc.Data.Model;

namespace NetProc.Data
{
    public interface INetProcDbContext
    {
        DbSet<Audit> Audits { get; set; }
        DbSet<GameAudit> GameAudit { get; set; }
        DbSet<Machine> Machine { get; set; }
        DbSet<Player> Players { get; set; }
        DbSet<CoilConfigFileEntry> Coils { get; set; }
        DbSet<LampConfigFileEntry> Lamps { get; set; }
        DbSet<LedConfigFileEntry> Leds { get; set; }
        DbSet<SwitchConfigFileEntry> Switches { get; set; }
        DbSet<Setting> Settings { get; set; }

        /// <summary>
        /// Creates a P-ROC / P3-ROC configuration from tables in database
        /// </summary>
        /// <returns></returns>
        MachineConfiguration GetMachineConfiguration();
    }
}
