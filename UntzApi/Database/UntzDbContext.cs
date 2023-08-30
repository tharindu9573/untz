using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Untz.Database.Models;
using UntzApi.Database.Models;

namespace Untz.Database
{
    public class UntzDbContext : IdentityDbContext<UntzUser>
    {
        public UntzDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Event> Events { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<MainEvent> MainEvents { get; set; }
        public DbSet<GuestUser> GuestUsers { get; set; }
        public DbSet<TicketPurchase> TicketPurchases { get; set; }
        public DbSet<PaymentMethod> PaymentMethods { get; set; }
        public DbSet<Recipt> Recipts { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<PaymentStatus> PaymentStatuses { get; set; }
        public DbSet<EventSubscription> EventSubscriptions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<TicketPurchase>().HasOne(_ => _.GuestUser).WithMany().OnDelete(DeleteBehavior.Cascade);
            builder.Entity<TicketPurchase>().HasOne(_ => _.User).WithMany().OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(builder);
        }
    }
}
