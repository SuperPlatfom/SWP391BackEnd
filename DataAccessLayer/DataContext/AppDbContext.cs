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

        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<CoOwnershipGroup> CoOwnershipGroups { get; set; }
        public DbSet<GroupMember> GroupMembers { get; set; }
        public DbSet<GroupInvite> GroupInvites { get; set; } = null!;
        public DbSet<ContractTemplate> ContractTemplates { get; set; }
        public DbSet<ContractClause> ContractClauses { get; set; }
        public DbSet<ContractVariable> ContractVariables { get; set; }
        public DbSet<EContract> EContracts { get; set; }
        public DbSet<EContractSigner> EContractSigners { get; set; }

        public DbSet<EContractMemberShare> EContractMemberShares { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<UsageSession> UsageSessions { get; set; }
        public DbSet<TripEvent> TripEvents { get; set; }
        public DbSet<UsageQuota> UsageQuotas { get; set; }
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


            // Account 1–N CoOwnershipGroup (CreatedBy)
            modelBuilder.Entity<CoOwnershipGroup>()
                .HasOne(g => g.CreatedByAccount)
                .WithMany(a => a.CreatedGroups)
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            // Account 1–N GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.UserAccount)
                .WithMany(a => a.GroupMemberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // CoOwnershipGroup 1–N GroupMember
            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // CoOwnershipGroup 1–N Vehicle
            modelBuilder.Entity<Vehicle>()
                .HasOne(v => v.Group)
                .WithMany(g => g.Vehicles)
                .HasForeignKey(v => v.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractClause>()
                .HasOne(c => c.Template)
                .WithMany(t => t.Clauses)
                .HasForeignKey(c => c.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ContractVariable>()
                .HasOne(v => v.Template)
                .WithMany(t => t.Variables)
                .HasForeignKey(v => v.TemplateId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EContractSigner>()
                .HasOne(s => s.Contract)
                .WithMany(c => c.Signers)
                .HasForeignKey(s => s.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EContract>()
                .HasOne(c => c.Group)
                .WithMany(g => g.Contracts)
                .HasForeignKey(c => c.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EContract>()
                .HasOne(c => c.Template)
                .WithMany(t => t.Contracts)
                .HasForeignKey(c => c.TemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EContract>()
                .HasOne(c => c.Vehicle)
                .WithMany(v => v.Contracts)
                .HasForeignKey(c => c.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EContract>()
                .HasOne(c => c.CreatedByAccount)
                .WithMany(a => a.CreatedContracts)
                .HasForeignKey(c => c.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EContractSigner>()
                .HasOne(s => s.User)
                .WithMany(a => a.SignedContracts)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EContractMemberShare>()
                .HasOne(ms => ms.User)
                .WithMany(a => a.OwnershipShares)
                .HasForeignKey(ms => ms.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<EContractMemberShare>()
                .HasOne(ms => ms.Contract)
                .WithMany(c => c.MemberShares)
                .HasForeignKey(ms => ms.ContractId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
