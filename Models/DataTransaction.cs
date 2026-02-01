using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektuppgiftBudgetplanerare.Models
{
    public class DataTransaction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public bool IsIncome { get; set; }
        public DateTime Date { get; set; } = DateTime.Today;
        public TransactionRecurrence Recurrence { get; set; }
        public string Description { get; set; } = "";
    }

    public enum TransactionRecurrence
    {
        OneTime,
        Monthly,
        Yearly
    }
}
