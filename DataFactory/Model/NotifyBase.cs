using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DataFactory.Model
{
    public abstract class NotifyBase : INotifyPropertyChanged
    {
        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Operations

        protected void SetProperty<T>(ref T target, T value, [CallerMemberName] string propertyName = "")
        {
            target = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion Operations
    }
}
