using Microsoft.EntityFrameworkCore;
using BusinessObject.Models;
using Microsoft.Extensions.Configuration;

namespace DataAccessLayer.DataContext
{
    public class AppDbContext : DbContext
    {
   

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Console.WriteLine(" AppDbContext constructed successfully.");
        }

   
        public DbSet<Account> Accounts { get; set; }

        public DbSet<CitizenIdentityCard> CitizenIdentityCards { get; set; }

        public DbSet<Role> Roles { get; set; }
    


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
                if (!string.IsNullOrEmpty(envConn))
                {
                    Console.WriteLine(" Using environment connection string");
                    optionsBuilder.UseNpgsql(envConn);
                }
                else
                {
                    Console.WriteLine("📁 Using local appsettings.json connection string");
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .Build();
                    var localConn = configuration.GetConnectionString("DefaultConnection");
                    if (!string.IsNullOrEmpty(localConn))
                        optionsBuilder.UseNpgsql(localConn);
                    else
                        Console.WriteLine(" No connection string found at all!");
                }
            }
        }
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
