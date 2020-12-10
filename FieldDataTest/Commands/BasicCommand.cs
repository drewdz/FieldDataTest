using DataFactory.Model;
using System;
using System.Windows.Input;

namespace FieldDataTest.Commands
{
    public class BasicCommand : NotifyBase, ICommand
    {
        #region Constants

        protected const int COOLOFF_TIME = 700;

        #endregion Constants

        #region Events

        public event EventHandler CanExecuteChanged;

        #endregion Events

        #region Fields

        protected DateTimeOffset _Tapped = DateTimeOffset.MinValue;

        #endregion Fields

        #region Constructors

        public BasicCommand(Action<object> action)
        {
            TapAction = action;
        }

        #endregion Constructors

        #region Properties

        private bool _Enabled = true;
        public bool Enabled
        {
            get => _Enabled; 
            set => SetProperty(ref _Enabled, value); 
        }

        private Action<object> _TapAction = null;
        public Action<object> TapAction
        {
            get => _TapAction; 
            set => SetProperty(ref _TapAction, value); 
        }

        #endregion Properties

        #region Operations

        public bool CanExecute(object parameter)
        {
            return Enabled;
        }

        public void Execute(object parameter)
        {
            try
            {
                if (DateTimeOffset.Now.Subtract(_Tapped).TotalMilliseconds <= COOLOFF_TIME) return;
                _Tapped = DateTimeOffset.Now;
                Enabled = false;
                TapAction?.Invoke(parameter);
            }
            finally
            {
                Enabled = true;
            }
        }

        #endregion Operations
    }
}
