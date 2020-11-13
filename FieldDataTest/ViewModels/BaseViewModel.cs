using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FieldDataTest.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Operations

        public virtual void Initialize() { }

        public virtual void ViewLoaded() { }

        protected void SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = "")
        {
            target = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Operations
    }
}
