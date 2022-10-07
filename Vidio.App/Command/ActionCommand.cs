using System;
using System.Windows.Input;

namespace Videio.App.Command
{
    public class ActionCommand : ICommand
    {
        private readonly Action selectAction;

        public event EventHandler CanExecuteChanged;

        public ActionCommand(Action selectAction)
        {
            this.selectAction = selectAction;
        }

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            this.selectAction?.Invoke();
        }
    }
}
