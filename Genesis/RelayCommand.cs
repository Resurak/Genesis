using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Genesis
{
    public class RelayCommand : ICommand
    {
        public RelayCommand() { }

        public event EventHandler CanExecuteChanged;

        private Action<object> executeMethod;
        private Predicate<object> CanExecuteMethod;

        public RelayCommand(Action<object> Execute, Predicate<object> CanExecute)
        {
            executeMethod = Execute;
            CanExecuteMethod = CanExecute;
        }

        public RelayCommand(Action<object> Execute) : this(Execute, null)
        {

        }

        public bool CanExecute(object parameter)
        {
            return (CanExecuteMethod == null) ? true : CanExecuteMethod.Invoke(parameter);
        }

        public void Execute(object parameter)
        {
            executeMethod.Invoke(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
