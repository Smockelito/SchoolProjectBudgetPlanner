using ProjektuppgiftBudgetplanerare.Command;
using ProjektuppgiftBudgetplanerare.Models;
using ProjektuppgiftBudgetplanerare.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektuppgiftBudgetplanerare.ViewModels
{
    public class TransactionsViewModel : BaseViewModel
    {
        //--Kopplat till konstruktor--
        private readonly ITransactionRepository repository;
        public DelegateCommand AddCommand { get; private set; }
        public DelegateCommand DeleteCommand { get; private set; }
        public DelegateCommand PrevMonthCommand { get; }
        public DelegateCommand NextMonthCommand { get; }
        public DelegateCommand ToggleTypeCommand { get; }
        public string TransactionTypeText { get => NewTransaction.IsIncome ? "Inkomst" : "Utgift"; }
        public Array RecurrenceValues => Enum.GetValues(typeof(TransactionRecurrence));


        public TransactionsViewModel(ITransactionRepository repo)
        {
            this.repository = repo;

            SelectedMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            PrevMonthCommand = new DelegateCommand(_ => SelectedMonth = SelectedMonth.AddMonths(-1));
            NextMonthCommand = new DelegateCommand(_ => SelectedMonth = SelectedMonth.AddMonths(1));
            ToggleTypeCommand = new DelegateCommand(_ =>
            {
                NewTransaction.IsIncome = !NewTransaction.IsIncome;
                RaisePropertyChanged(nameof(NewTransaction));
                RaisePropertyChanged(nameof(TransactionTypeText));
            });

            AddCommand = new DelegateCommand(AddTransaction);
            DeleteCommand = new DelegateCommand(DeleteTransaction, CanDelete);

            NewTransaction = new DataTransaction { Date = DateTime.Today, Recurrence = TransactionRecurrence.OneTime, IsIncome = false };
        }

        //--"Propfull" för transaktioner (hämta alla, hämta en, skapa ny och visa återkommande)--
        private ObservableCollection<DataTransaction> transactions = new();
        public ObservableCollection<DataTransaction> Transactions
        {
            get { return transactions; }
            set
            {
                transactions = value;
                RaisePropertyChanged();
            }
        }

        public DataTransaction? selectedTransaction;
        public DataTransaction? SelectedTransaction
        {
            get { return selectedTransaction; }
            set
            {
                selectedTransaction = value;
                RaisePropertyChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        private DataTransaction newTransaction = null!;
        public DataTransaction NewTransaction
        {
            get { return newTransaction; }
            set
            {
                newTransaction = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<TransactionOccurence> monthTransactions = new();
        public ObservableCollection<TransactionOccurence> MonthTransactions
        {
            get => monthTransactions;
            set
            {
                monthTransactions = value;
                RaisePropertyChanged();
            }
        }


        //--"Propfull" att att styra månad--
        private DateTime selectedMonth;
        public DateTime SelectedMonth
        {
            get { return selectedMonth; }
            set
            {
                selectedMonth = new DateTime(value.Year, value.Month, 1);
                RaisePropertyChanged();
                RecalculateTotals();
            }
        }

        //--"Propfull" för olika summeringar--
        private decimal monthIncome;
        public decimal MonthIncome
        {
            get { return monthIncome; }
            private set
            {
                monthIncome = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MonthRemaining));
            }
        }

        private decimal monthExpenses;
        public decimal MonthExpenses
        {
            get { return monthExpenses; }
            private set
            {
                monthExpenses = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(MonthRemaining));
            }
        }

        public decimal MonthRemaining => MonthIncome - MonthExpenses;

        private decimal annualIncome;
        public decimal AnnualIncome
        {
            get { return annualIncome; }
            private set
            {
                annualIncome = value;
                RaisePropertyChanged();
            }
        }


        //--Metoder kopplat till CRUD--
        public async Task LoadAsync()
        {
            var list = await repository.GetAllAsync();
            Transactions = new ObservableCollection<DataTransaction>(list);
            RecalculateTotals();
        }

        private async void AddTransaction(object? parameter)
        {
            if (string.IsNullOrWhiteSpace(NewTransaction.Category)) NewTransaction.Category = "Övrigt";
            if (NewTransaction.Amount <= 0) return;

            var saved = await repository.AddAsync(NewTransaction);
            Transactions.Insert(0, saved);

            NewTransaction = new DataTransaction { Date = DateTime.Today, Recurrence = TransactionRecurrence.OneTime, IsIncome = false };
            RaisePropertyChanged(nameof(TransactionTypeText));

            RecalculateTotals();
        }

        private async void DeleteTransaction(object? parameter)
        {
            if (SelectedTransaction is null) return;

            var tx = SelectedTransaction;
            await repository.DeleteAsync(tx);

            Transactions.Remove(tx);
            SelectedTransaction = null;

            RecalculateTotals();
        }

        //--Metod kopplat till beräkningar--
        private void RecalculateTotals()
        {
            var monthStart = SelectedMonth;
            var monthEnd = monthStart.AddMonths(1);

            var inMonth = Transactions.Where(t => t.Recurrence == TransactionRecurrence.OneTime && t.Date >= monthStart && t.Date < monthEnd)
                .Select(t => new TransactionOccurence
                {
                    SourceId = t.Id,
                    Category = t.Category,
                    Amount = t.Amount,
                    IsIncome = t.IsIncome,
                    Date = t.Date,
                    Recurrence = t.Recurrence,
                    Description = t.Description
                });

            var recurrenceInMonth = Transactions.Where(t => t.Recurrence == TransactionRecurrence.Monthly && t.Date < monthEnd)
                .Select(t => new TransactionOccurence
                {
                    SourceId = t.Id,
                    Category = t.Category,
                    Amount = t.Amount,
                    IsIncome = t.IsIncome,
                    Date = new DateTime(monthStart.Year, monthStart.Month, Math.Min(t.Date.Day, DateTime.DaysInMonth(monthStart.Year, monthStart.Month))),
                    Recurrence = t.Recurrence,
                    Description = t.Description
                });

            var combined = inMonth
                .Concat(recurrenceInMonth)
                .OrderByDescending(t => t.IsIncome)
                .ThenBy(t => t.Category)
                .ToList();
            MonthTransactions = new ObservableCollection<TransactionOccurence>(combined);

            MonthIncome = MonthTransactions.Where(t => t.IsIncome).Sum(t => t.Amount);
            MonthExpenses = MonthTransactions.Where(t => !t.IsIncome).Sum(t => t.Amount);

            var monthlyRecurringIncome = Transactions
                .Where(t => t.IsIncome && t.Recurrence == TransactionRecurrence.Monthly)
                .Sum(t => t.Amount);

            AnnualIncome = monthlyRecurringIncome * 12;
        }

        //--Övriga metoder--
        private bool CanDelete(object? parameter) => SelectedTransaction is not null;
    }
}
