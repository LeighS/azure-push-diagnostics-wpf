using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotificationHubs.Diagnostics.Commands
{
    public class SimpleDelegateCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;
        public SimpleDelegateCommand(Func<Task> execute)
        {
            _execute = (o) => execute();
        }
        public SimpleDelegateCommand(Action execute)
        {
            _execute = (o) => execute();
        }
        public SimpleDelegateCommand(Action<object> execute)
        {
            _execute = execute;
        }
        public SimpleDelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            if (_canExecute == null)
                return true;
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;
    }
}
