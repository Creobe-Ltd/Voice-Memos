using System;
using System.Windows.Input;

namespace Creobe.VoiceMemos.Core.Commands
{
    public class DelegateCommand : ICommand
    {
        #region Private Members

        Func<object, bool> _canExecute;
        Action<object> _executeAction;

        #endregion

        #region Constructors

        public DelegateCommand(Action<object> executeAction) :
            this(executeAction, null) { }

        public DelegateCommand(Action<object> executeAction, Func<object, bool> canExecute)
        {
            if (executeAction == null)
                throw new ArgumentNullException("executeAction");

            _executeAction = executeAction; 
            _canExecute = canExecute;
        }

        #endregion

        #region Private Methods

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, new EventArgs());
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            bool result = true;

            if (_canExecute != null)
                _canExecute(parameter);

            return result;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _executeAction(parameter);
        }

        #endregion
    }
}
