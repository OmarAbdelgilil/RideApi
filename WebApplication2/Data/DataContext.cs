using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication1.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
        public DbSet<Passanger> Passanger { get; set; }
        public DbSet<Driver> Driver { get; set; }
        public DbSet<Credentials> Credentials { get; set; }
        public DbSet<Rides> Rides { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Driver entity
            modelBuilder.Entity<Driver>()
                .HasOne(d => d.Credentials)
                .WithOne()
                .HasForeignKey<Driver>(d => d.Email)
                .OnDelete(DeleteBehavior.Cascade); // Specify cascade behavior

            modelBuilder.Entity<Driver>()
                .HasMany(d => d.Rides)
                .WithOne(r => r.Driver)
                .HasForeignKey(r => r.DriverEmail)
                .OnDelete(DeleteBehavior.Cascade); // Specify cascade behavior

            // Configure Passanger entity
          // Specify cascade behavior

            modelBuilder.Entity<Passanger>()
                .HasMany(p => p.Rides)
                .WithOne(r => r.Passanger)
                .HasForeignKey(r => r.PassangerEmail)
                .OnDelete(DeleteBehavior.Cascade); // Specify cascade behavior

            // Configure Rides entity
            modelBuilder.Entity<Rides>()
                .HasOne(r => r.Driver)
                .WithMany(d => d.Rides)
                .HasForeignKey(r => r.DriverEmail)
                .OnDelete(DeleteBehavior.Restrict); // Specify cascade behavior

            modelBuilder.Entity<Rides>()
                .HasOne(r => r.Passanger)
                .WithMany(p => p.Rides)
                .HasForeignKey(r => r.PassangerEmail)
                .OnDelete(DeleteBehavior.Restrict); // Specify cascade behavior

            // Additional configurations
            modelBuilder.Entity<Rides>()
                .Property(r => r.Id)
                .ValueGeneratedOnAdd();

            // If you have other configurations, you can add them here.

            base.OnModelCreating(modelBuilder);
        }
    }
}
