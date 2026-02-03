using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektuppgiftBudgetplanerare.Models
{
    public class TransactionOccurence
    {
        public int SourceId { get; set; }
        public string Category { get; set; } = "";
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
        public DateTime Date { get; set; }
        public TransactionRecurrence Recurrence { get; set; }
        public string Description { get; set; } = "";
    }
}
