using System.Collections.Specialized;
using System.ComponentModel;

namespace FieldDataTest.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {
        void Initialize();

        void ViewLoaded();
    }
}
