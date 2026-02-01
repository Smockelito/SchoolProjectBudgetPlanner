using Microsoft.EntityFrameworkCore;
using ProjektuppgiftBudgetplanerare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektuppgiftBudgetplanerare.Data
{
    public class BudgetDbContext : DbContext
    {
        public DbSet<DataTransaction> Transactions { get; set; }
        public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DataTransaction>()
                .Property(t => t.Amount)
                .HasPrecision(18, 2);
        }
    }
}
