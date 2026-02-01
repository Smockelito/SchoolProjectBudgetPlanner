using Microsoft.EntityFrameworkCore;
using ProjektuppgiftBudgetplanerare.Data;
using ProjektuppgiftBudgetplanerare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektuppgiftBudgetplanerare.Repositories
{
    internal class TransactionRepository : ITransactionRepository
    {
        private readonly BudgetDbContext dbContext;

        public TransactionRepository(BudgetDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<DataTransaction> AddAsync(DataTransaction transaction)
        {
            dbContext.Add(transaction);
            await dbContext.SaveChangesAsync();
            return transaction;
        }

        public async Task DeleteAsync(DataTransaction transaction)
        {
            dbContext.Transactions.Remove(transaction);
            await dbContext.SaveChangesAsync();
        }

        public Task<List<DataTransaction>> GetAllAsync() => dbContext.Transactions
                                                            .OrderByDescending(t => t.Date)
                                                            .ToListAsync();
    }
}
