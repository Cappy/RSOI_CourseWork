using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace UsersAPIService.Models
{
    public partial class UsersContext : DbContext
    {
        public UsersContext()
        {
        }

        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Users> Users { get; set; }

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
            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("users", "p460741_rsoi");

                entity.HasIndex(e => e.Email)
                    .HasName("email")
                    .IsUnique();

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasMaxLength(16);

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("date");

                entity.Property(e => e.DateOfBirth)
                    .HasColumnName("date_of_birth")
                    .HasColumnType("date");

                entity.Property(e => e.Email)
                    .HasColumnName("email")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.IsAdmin)
                    .HasColumnName("is_admin")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.IsRentlord)
                    .HasColumnName("is_rentlord")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Name)
                    .HasColumnName("name")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PasswordHash)
                    .HasColumnName("password_hash")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.PasswordSalt)
                    .HasColumnName("password_salt")
                    .HasColumnType("varchar(255)");

                entity.Property(e => e.Surname)
                    .HasColumnName("surname")
                    .HasColumnType("varchar(255)");
            });
        }
    }
}
