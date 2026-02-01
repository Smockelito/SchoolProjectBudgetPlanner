
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjektuppgiftBudgetplanerare.Command
{
    public class DelegateCommand : ICommand
    {
        protected readonly Action<object?> execute;
        protected readonly Func<object?, bool>? canExecute;

        public event EventHandler? CanExecuteChanged;

        public DelegateCommand(Action<object?> execute, Func<object?, bool>? canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object? parameter) => canExecute?.Invoke(parameter) ?? true;

        public void Execute(object? parameter) => execute(parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
