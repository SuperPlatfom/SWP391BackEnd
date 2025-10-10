using Microsoft.EntityFrameworkCore;
using BusinessObject.Models;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.DataContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext()
        {
        }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

   
        public DbSet<Account> Accounts { get; set; }

        public DbSet<CitizenIdentityCard> CitizenIdentityCards { get; set; }

        public DbSet<Role> Roles { get; set; }
    


        private static string? GetConnectionString()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true).Build();
            return configuration["ConnectionStrings:DefaultConnection"];
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(GetConnectionString());

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Account>()
                .HasOne(a => a.Role)
                .WithMany(r => r.Accounts)
                .HasForeignKey(a => a.RoleId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<CitizenIdentityCard>()
                .HasOne(c => c.Account)
                .WithOne(a => a.CitizenIdentityCard)
                .HasForeignKey<CitizenIdentityCard>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

          
        }
    }
}
