using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StarEvents.Models;

namespace StarEvents.Data
{
    // This is the SINGLE, CORRECT definition of the class
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        // SINGLE, CORRECT constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Venue> Venues { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<EventCategory> EventCategories { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<BookingPromotion> BookingPromotions { get; set; }

        // SINGLE, CORRECT OnModelCreating method
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure decimal precision
            builder.Entity<Event>()
                .Property(e => e.TicketPrice)
                .HasPrecision(10, 2);

            builder.Entity<Booking>()
                .Property(b => b.UnitPrice)
                .HasPrecision(10, 2);

            builder.Entity<Booking>()
                .Property(b => b.TotalAmount)
                .HasPrecision(10, 2);

            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            builder.Entity<Payment>()
                .Property(p => p.RefundAmount)
                .HasPrecision(10, 2); // This line is now valid since you added the property to Payment model

            builder.Entity<Promotion>()
                .Property(p => p.DiscountValue)
                .HasPrecision(10, 2);

            builder.Entity<Promotion>()
                .Property(p => p.MinimumPurchase)
                .HasPrecision(10, 2);

            builder.Entity<Promotion>()
                .Property(p => p.MaximumDiscount)
                .HasPrecision(10, 2);

            builder.Entity<BookingPromotion>()
                .Property(bp => bp.DiscountApplied)
                .HasPrecision(10, 2);

            // Configure relationships
            builder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Venue>()
                .HasOne(v => v.CreatedByUser)
                .WithMany(u => u.CreatedVenues)
                .HasForeignKey(v => v.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);


            builder.Entity<Event>()
                .Ignore(e => e.UserId);

            builder.Entity<Venue>()
                .Ignore(v => v.UserId);

            // Unique constraints
            builder.Entity<Venue>()
                .HasIndex(v => v.VenueName)
                .IsUnique();

            builder.Entity<EventCategory>()
                .HasIndex(ec => ec.CategoryName)
                .IsUnique();

            builder.Entity<Promotion>()
                .HasIndex(p => p.PromotionCode)
                .IsUnique();

            builder.Entity<Booking>()
                .HasIndex(b => b.BookingReference)
                .IsUnique();

            builder.Entity<Ticket>()
                .HasIndex(t => t.TicketNumber)
                .IsUnique();

            builder.Entity<Payment>()
                .HasIndex(p => p.PaymentReference)
                .IsUnique();
        }
    }
}