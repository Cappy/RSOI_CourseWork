using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AdsAPIService.Models
{
    public partial class AdsContext : DbContext
    {
        public AdsContext()
        {
        }

        public AdsContext(DbContextOptions<AdsContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ads> Ads { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=46.254.21.136;port=3306;user=p460741_pavel;password=2M8p8B0c;database=p460741_rsoi");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ads>(entity =>
            {
                entity.HasKey(e => e.Adid);

                entity.ToTable("ads");

                entity.HasIndex(e => e.Userid)
                    .HasName("userid");

                entity.Property(e => e.Adid)
                    .HasColumnName("adid")
                    .HasMaxLength(16);
                entity.Property(e => e.City)
                     .HasColumnName("city")
                     .HasColumnType("varchar(255)");
                entity.Property(e => e.Adress)
                    .HasColumnName("adress")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Bathrooms)
                    .HasColumnName("bathrooms")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Bedrooms)
                    .HasColumnName("bedrooms")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Beds)
                    .HasColumnName("beds")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Caption)
                    .HasColumnName("caption")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("date");

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Price)
                    .HasColumnName("price")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Type)
                    .HasColumnName("type")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasMaxLength(16);

                entity.Property(e => e.WhatRented)
                    .HasColumnName("what_rented")
                    .HasColumnType("varchar(255)");
            });
        }
    }
}
