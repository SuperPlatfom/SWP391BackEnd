using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataContext
{
    public class AppDbContext : DbContext
    {
        

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
            public DbSet<Vehicle>Vehicles {  get; set; }
        
    }
}
