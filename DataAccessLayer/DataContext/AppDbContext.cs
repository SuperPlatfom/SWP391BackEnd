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
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<ServiceRequestConfirmation> ServiceRequestConfirmations { get; set; }
        public DbSet<ServiceJob> ServiceJobs { get; set; }
        public DbSet<GroupExpense> GroupExpenses { get; set; }
        public DbSet<MemberInvoice> MemberInvoices { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PayOSTransaction> PayOSTransactions { get; set; }
        public DbSet<ServiceCenter> ServiceCenters { get; set; }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<VehicleRequest> VehicleRequests { get; set; }
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

            modelBuilder.Entity<CoOwnershipGroup>()
                .HasOne(g => g.CreatedByAccount)
                .WithMany(a => a.CreatedGroups)
                .HasForeignKey(g => g.CreatedBy)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.UserAccount)
                .WithMany(a => a.GroupMemberships)
                .HasForeignKey(gm => gm.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GroupMember>()
                .HasOne(gm => gm.Group)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

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

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Group)
                .WithMany(g => g.ServiceRequests)
                .HasForeignKey(sr => sr.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Vehicle)
                .WithMany(v => v.ServiceRequests)
                .HasForeignKey(sr => sr.VehicleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.CreatedByAccount)
                .WithMany(a => a.CreatedServiceRequests)
                .HasForeignKey(sr => sr.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.Technician)
                .WithMany(a => a.AssignedServiceRequests)
                .HasForeignKey(sr => sr.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<ServiceRequest>()
                .HasOne(sr => sr.ServiceCenter)
                .WithMany(sc => sc.ServiceRequests)
                .HasForeignKey(sr => sr.ServiceCenterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<GroupExpense>()
                .HasOne(ge => ge.ServiceRequest)
                .WithOne(sr => sr.GroupExpense)
                .HasForeignKey<GroupExpense>(ge => ge.ServiceRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRequestConfirmation>()
                .HasOne(c => c.Request)
                .WithMany(sr => sr.Confirmations)
                .HasForeignKey(c => c.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceRequestConfirmation>()
                .HasOne(c => c.User)
                .WithMany(a => a.ServiceRequestConfirmations)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceJob>()
                .HasOne(j => j.Request)
                .WithOne(r => r.Job)
                .HasForeignKey<ServiceJob>(j => j.RequestId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ServiceJob>()
                .HasOne(j => j.Technician)
                .WithMany(a => a.ServiceJobs)
                .HasForeignKey(j => j.TechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupExpense>()
                .HasOne(ge => ge.Group)
                .WithMany(g => g.GroupExpenses)
                .HasForeignKey(ge => ge.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberInvoice>()
                .HasOne(mi => mi.Expense)
                .WithMany(ge => ge.MemberInvoices)
                .HasForeignKey(mi => mi.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MemberInvoice>()
                .HasOne(mi => mi.Group)
                .WithMany(g => g.MemberInvoices)
                .HasForeignKey(mi => mi.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MemberInvoice>()
                .HasOne(mi => mi.User)
                .WithMany(a => a.MemberInvoices)
                .HasForeignKey(mi => mi.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.User)
                .WithMany(a => a.Payments)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Invoice)
                .WithMany(mi => mi.Payments)
                .HasForeignKey(p => p.InvoiceId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PayOSTransaction>()
                .HasOne(pt => pt.Payment)
                .WithOne(p => p.PayOSTransaction)
                .HasForeignKey<PayOSTransaction>(pt => pt.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<ServiceRequest>()
                .Property(sr => sr.Type)
                .HasConversion<string>()  
                .HasMaxLength(50);


        }
    }
}
