using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BookingsAPIService.Models
{
    public partial class BookingContext : DbContext
    {
        public BookingContext()
        {
        }

        public BookingContext(DbContextOptions<BookingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Booking> Booking { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySql("server=46.254.21.136;port=3306;user=p460741_pavel;password=2M8p8B0c;database=p460741_rsoi");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.ToTable("booking");

                entity.HasIndex(e => e.Adid)
                    .HasName("adid");

                entity.HasIndex(e => e.Userid)
                    .HasName("userid");

                entity.Property(e => e.Bookingid)
                    .HasColumnName("bookingid")
                    .HasMaxLength(16);

                entity.Property(e => e.Adid)
                    .HasColumnName("adid")
                    .HasMaxLength(16);

                entity.Property(e => e.ArrivalDate)
                    .HasColumnName("arrival_date")
                    .HasColumnType("date");

                entity.Property(e => e.BookedPrice)
                    .HasColumnName("booked_price")
                    .HasColumnType("int(11)");

                entity.Property(e => e.CreatedAt)
                    .HasColumnName("created_at")
                    .HasColumnType("date");

                entity.Property(e => e.DepartureDate)
                    .HasColumnName("departure_date")
                    .HasColumnType("date");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasMaxLength(16);
            });
        }
    }
}
