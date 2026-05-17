using DreamAnalyzer2.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DreamAnalyzer2.Infrastructure.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<Dream> Dreams { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<DreamAnalysis> DreamAnalyses { get; set; }
        public DbSet<DreamSymbol> DreamSymbols { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Dreams)
                .WithOne()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Dream>()
                .HasOne<DreamAnalysis>()
                .WithOne()
                .HasForeignKey<DreamAnalysis>(a => a.DreamId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DreamAnalysis>()
                .HasMany(a => a.Symbols)
                .WithMany()
                .UsingEntity(s => s.ToTable("AnalysisSymbols"));

            modelBuilder.Entity<DreamAnalysis>()
                .OwnsOne(a => a.Mood, mood =>
                {
                    mood.Property(m => m.Name).HasColumnName("MoodName");
                    mood.Property(m => m.ColorHex).HasColumnName("MoodColorHex");
                    mood.Property(m => m.Description).HasColumnName("MoodDescription");
                });
        }
    }
}
