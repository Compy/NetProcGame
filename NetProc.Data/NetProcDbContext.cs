using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetProc;
using NetProc.Data.Config;
using NetProc.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetProc.Data
{

    public class NetProcDbContext : DbContext, INetProcDbContext
    {
        public DbSet<Audit> Audits { get; set; }
        public DbSet<CoilConfigFileEntry> Coils { get; set; }
        public DbSet<GameAudit> GameAudit { get; set; }
        public DbSet<LampConfigFileEntry> Lamps { get; set; }
        public DbSet<LedConfigFileEntry> Leds { get; set; }
        public DbSet<Machine> Machine { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Setting> Settings { get; set; }
        public DbSet<SwitchConfigFileEntry> Switches { get; set; }
        public MachineConfiguration GetMachineConfiguration()
        {
            var machine = Machine.Find(1);
            var mc = new MachineConfiguration()
            {
                PRSwitches = Switches.AsNoTracking()
                 .Select(x => new SwitchConfigFileEntry()
                 {
                     Name = x.Name,
                     Number = x.Number,
                     SearchReset = x.SearchReset,
                     SearchStop = x.SearchStop,
                     Tags = x.Tags,
                     Type = x.Type
                 }).ToList(),

                PRCoils = Coils.AsNoTracking()
                 .Select(x => new CoilConfigFileEntry()
                 {
                     Bus = x.Bus,
                     Name = x.Name,
                     Number = x.Number,
                     Polarity = x.Polarity,
                     PulseTime = x.PulseTime,
                     Search = x.Search,
                     Tags = x.Tags
                 }).ToList(),

                PRLamps = Lamps.AsNoTracking()
                .Select(x => new LampConfigFileEntry()
                {
                    Bus = x.Bus,
                    Name = x.Name,
                    Number = x.Number,
                    Polarity = x.Polarity,
                    Tags = x.Tags
                }).ToList(),

                PRLeds = Leds.AsNoTracking()
                .Select(x => new LampConfigFileEntry()
                {
                    Bus = x.Bus,
                    Name = x.Name,
                    Number = x.Number,
                    Polarity = x.Polarity,
                    Tags = x.Tags
                }).ToList(),

                PRGame = new GameConfigFileEntry()
                {
                    DisplayMonitor = machine.DisplayMonitor,
                    MachineType = machine.MachineType,
                    NumBalls = machine.NumBalls
                },
                //todo: PRBumpers = switches and coils when disabling flippers
            };

            //setup ball search reset and stop switches
            Dictionary<string, string> resets = new Dictionary<string, string>();
            foreach (var item in mc.PRSwitches.Where(x => !string.IsNullOrWhiteSpace(x.SearchReset))
                .Select(x => new { x.Name, x.SearchReset }))
            {
                resets.Add(item.Name, item.SearchReset);
            }

            Dictionary<string, string> stops = new Dictionary<string, string>();
            foreach (var item in mc.PRSwitches.Where(x => !string.IsNullOrWhiteSpace(x.SearchStop))
                .Select(x => new { x.Name, x.SearchStop }))
            {
                resets.Add(item.Name, item.SearchStop);
            }

            mc.PRBallSearch = new BallSearchConfigFileEntry()
            {
                PulseCoils = mc.PRCoils.Where(x => x.Search > 0)?.Select(x => x.Name).ToList(),
                StopSwitches = stops,
                ResetSwitches = resets,
            };

            return mc;
        }

        /// <summary>
        /// Initializes the database default values for machine items
        /// </summary>
        /// <param name="isMachinePdb"></param>
        public void InitializeDatabase(bool isMachinePdb)
        {
            try
            {
                //Database.EnsureCreated();
                //Database.Migrate();

                string initFile = string.Empty;

                if (isMachinePdb)
                    initFile = Path.Combine(Directory.GetCurrentDirectory(), "sql/init_p3roc.sql");
                else
                    initFile = Path.Combine(Directory.GetCurrentDirectory(), "sql/init_proc.sql");

                if (File.Exists(initFile))
                {
                    var sql = File.ReadAllText(initFile);
                    Database.ExecuteSqlRaw(sql);
                }
                else
                    throw new FileNotFoundException($"{initFile} sql file not found");
            }
            catch
            {
                throw;
            }
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(CoilConfiguration).Assembly);
        }
    }
}
