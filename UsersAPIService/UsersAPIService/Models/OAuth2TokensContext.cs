using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UsersAPIService.Models
{
    public partial class OAuth2TokensContext : DbContext
    {
        public OAuth2TokensContext()
        {
        }

        public OAuth2TokensContext(DbContextOptions<OAuth2TokensContext> options)
            : base(options)
        {
        }

        public virtual DbSet<OAuth2Tokens> OAuth2Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=46.254.21.136;port=3306;user=p460741_pavel;password=2M8p8B0c;database=p460741_lab");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OAuth2Tokens>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.ToTable("OAuth2Tokens");

                entity.Property(e => e.UserId)
                    .HasColumnName("UserID")
                    .HasColumnType("binary(16)")
                    .ValueGeneratedNever();

                entity.Property(e => e.IssuedAt).HasColumnType("datetime");

                entity.Property(e => e.Revoked).HasColumnType("int(16)");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnType("text");
            });
        }
    }
}

