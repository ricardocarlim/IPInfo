using api.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Reflection.Emit;

namespace api.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public DbSet<Country> Countries { get; set; }
        public DbSet<CountryReport> CountryReports { get; set; }
        public DbSet<IPAddress> IPAddresses { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<CountryReport>(entity =>
            {
                entity.HasNoKey(); 
            });

            builder.Entity<Country>()
           .HasKey(c => c.Id);

            builder.Entity<IPAddress>()
                .HasKey(ip => ip.Id);

            builder.Entity<IPAddress>()
                .HasIndex(ip => ip.IP)
                .IsUnique();
           
        }       
    }
}
