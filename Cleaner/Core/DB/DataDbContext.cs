using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cleaner.Core.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cleaner.Core.DB
{
    public class DataDbContext : DbContext
    {
        private readonly ILogger<DataDbContext> _logger;
        private readonly AppSettings _appSettings;

        public virtual DbSet<BaseWords> BaseWords { get; set; }
        public virtual DbSet<Words> Words { get; set; }

        public DataDbContext(DbContextOptions<DataDbContext> options, ILogger<DataDbContext> logger, IOptions<AppSettings> appSettings) : base(options)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            ChangeTracker.StateChanged += OnStateChanged;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //var c = configuration.GetConnectionString("DefaultConnection");
            //optionsBuilder.UseMySql(_appSettings.ConnectionStrings.DefaultConnection);
        }

        private void OnStateChanged(object sender, EntityStateChangedEventArgs e)
        {
            foreach (var entry in e.Entry.Properties.Where(x=>x.IsModified))
            {
                _logger.LogInformation(string.Format("{0,-10} : {1,10} => {2}", entry.Metadata.Name.Trim(), "\"" + entry.OriginalValue.ToString().Trim() + "\"", "\"" + entry.CurrentValue.ToString().Trim() + "\""));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseWords>(entity =>
            {
                entity.ToTable("basewords");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Words>(entity =>
            {
                entity.ToTable("words");
                entity.HasKey(x => x.Id);
            });
        }
    }
}
