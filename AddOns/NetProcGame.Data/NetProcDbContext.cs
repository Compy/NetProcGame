using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NetProcGame.Data.Model;
using System;
using System.IO;

namespace NetProcGame.Data
{
    public class NetProcDbContext : DbContext
    {
        public DbSet<Audit> Audits { get; set; }
        public DbSet<GameAudit> GameAudit { get; set; }
        public DbSet<Machine> Machine { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<PRCoil> PRCoils { get; set; }
        public DbSet<PRLamp> PRLamps { get; set; }
        public DbSet<PRSwitch> PRSwitches { get; set; }
        public DbSet<Setting> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            
            optionsBuilder.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
        }

        public void InitializeDatabase()
        {
            return;
            Database.Migrate();
            try
            {
                var initFile = Path.Combine(Directory.GetCurrentDirectory(), "sql/init.sql");
                if (File.Exists(initFile))
                {
                    var sql = File.ReadAllText(initFile);
                    Database.ExecuteSqlRaw(sql);
                }
                else
                    throw new FileNotFoundException("./sql/init.sql file not found");
            }
            catch
            {
                throw;
            }
        }
    }
}
