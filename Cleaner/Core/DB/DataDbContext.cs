using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cleaner.Core.DB.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cleaner.Core.DB
{
    public class DataDbContext : DbContext
    {
        //private readonly ValueConverter _nullableStringConverter = new ValueConverter<string, string>(v => v == null ? "" : v, v => v);
        private readonly ILogger<DataDbContext> _logger;
        private readonly AppSettings _appSettings;

        public virtual DbSet<Universal> Universals { get; set; }
        public virtual DbSet<Ui_Translations> Ui_Translations { get; set; }
        public virtual DbSet<Statistic> Statistics { get; set; }
        public virtual DbSet<Abbreviations> Abbreviations { get; set; }
        public virtual DbSet<Basewordexamples> Basewordexamples { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Functions> Functions { get; set; }
        public virtual DbSet<Grammar> Grammars { get; set; }
        public virtual DbSet<Connections> Connections { get; set; }
        public virtual DbSet<LanguageTranslations> LanguageTranslations { get; set; }
        public virtual DbSet<Languages> Languages { get; set; }
        public virtual DbSet<Characters> Characters { get; set; }
        public virtual DbSet<BaseWords> BaseWords { get; set; }
        public virtual DbSet<Words> Words { get; set; }

        public DataDbContext(DbContextOptions<DataDbContext> options, ILogger<DataDbContext> logger, IOptions<AppSettings> appSettings) : base(options)
        {
            _logger = logger;
            _appSettings = appSettings.Value;

            //ChangeTracker.StateChanged += OnStateChanged;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
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

            modelBuilder.Entity<Universal>(entity =>
            {
                entity.ToTable("universal");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Ui_Translations>(entity =>
            {
                entity.ToTable("ui_translations");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Abbreviations>(entity =>
            {
                entity.ToTable("abbreviations");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Basewordexamples>(entity =>
            {
                entity.ToTable("basewordexamples");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("feedback");
            });

            modelBuilder.Entity<Functions>(entity =>
            {
                entity.ToTable("functions");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Statistic>(entity =>
            {
                entity.ToTable("statistic");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Grammar>(entity =>
            {
                entity.ToTable("grammar");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Connections>(entity =>
            {
                entity.ToTable("connections");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.BaseWord).WithMany(x => x.WordLinks).HasForeignKey(x => x.BaseWordId);
                entity.HasOne(x => x.Word).WithMany(x => x.BaseWordLinks).HasForeignKey(x => x.WordId);

            });

            modelBuilder.Entity<Languages>(entity =>
            {
                entity.ToTable("languages");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<LanguageTranslations>(entity =>
            {
                entity.ToTable("languageTranslations");
                entity.HasKey(x => x.Id);
            });

            modelBuilder.Entity<Characters>(entity =>
            {
                entity.ToTable("characters");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Language).WithMany(x => x.Characters).HasForeignKey(x => x.LangId);
                
            });

            modelBuilder.Entity<BaseWords>(entity =>
            {
                entity.ToTable("basewords");
                entity.HasKey(x => x.Id);

                entity.HasOne(x => x.Language).WithMany(x => x.BaseWords).HasForeignKey(x => x.LangId);
            });

            modelBuilder.Entity<Words>(entity =>
            {
                entity.ToTable("words");
                entity.HasKey(x => x.Id);
            });
        }
    }
}
