using Microsoft.EntityFrameworkCore;
using MTJR.HardwareMonitor.Configuration;
using MTJR.HardwareMonitor.Model;

namespace MTJR.HardwareMonitor.Data
{
    /// <summary>
    /// Data context to manage the database
    /// </summary>
    public class DataContext:DbContext
    {
        /// <summary>
        /// Table for <see cref="Server"/>
        /// </summary>
        public DbSet<Server> Servers { get; set; }

        /// <summary>
        /// Table for <see cref="GuiConfiguration"/>
        /// </summary>
        public DbSet<GuiConfiguration> GuiConfigurations { get; set; }


        /// <summary>
        /// Table for <see cref="StateTypeConfiguration"/>
        /// </summary>
        public DbSet<StateTypeConfiguration> StateTypeConfiguration { get; set; }

        /// <summary>
        /// Default constructor for EntityFrameWork
        /// </summary>
        public DataContext() { }

        /// <summary>
        /// Constructor for migration management
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        /// <summary>
        /// Overrides the <see cref="OnConfiguring"/> to use PostgresSql
        /// </summary>
        /// <param name="optionsBuilder"></param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }
    }
}
