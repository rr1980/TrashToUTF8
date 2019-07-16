using System;
using System.Collections.Generic;
using System.Text;
using Cleaner.Core.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Cleaner.Core.DB
{
    public class DataDbContext : DbContext
    {
        private readonly AppSettings _appSettings;

        public virtual DbSet<BaseWords> BaseWords { get; set; }

        public DataDbContext(DbContextOptions<DataDbContext> options, IOptions<AppSettings> appSettings) : base(options)
        {
            _appSettings = appSettings.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //var c = configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseMySQL(_appSettings.ConnectionStrings.DefaultConnection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BaseWords>(entity =>
            {
                entity.ToTable("basewords");
                entity.HasKey(x => x.Id);


            });
        }
    }
}
