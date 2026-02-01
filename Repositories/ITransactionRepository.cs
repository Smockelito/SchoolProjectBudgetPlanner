using ProjektuppgiftBudgetplanerare.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektuppgiftBudgetplanerare.Repositories
{
    public interface ITransactionRepository
    {
        Task<List<DataTransaction>> GetAllAsync();
        Task<DataTransaction> AddAsync(DataTransaction transaction);
        Task DeleteAsync(DataTransaction transaction);
    }
}
