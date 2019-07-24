using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace xLingua.Entities
{
    public class DataDbContext : DbContext
    {
        //string conn = "Server=192.168.254.202;port=3306;Database=xLingua;Uid=root;Pwd=gmbh123;CharSet=utf8;";
        //string conn = "server=172.20.20.21;port=3306;database=xLinguaCheck;uid=root;password=gmbh123!;CharSet=utf8;";
        string conn = "server=172.20.20.21;port=3306;database=xLingua;uid=root;password=gmbh123!;CharSet=utf8;";

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


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
            optionsBuilder.UseMySql(conn, b => {
                b.UnicodeCharSet(CharSet.Utf8mb4);
            });

            optionsBuilder.EnableDetailedErrors();
            optionsBuilder.EnableSensitiveDataLogging();
            optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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
