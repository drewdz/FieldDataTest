using DataFactory.Model;

namespace FieldDataTest.ViewModels
{
    public abstract class BaseViewModel : NotifyBase
    {
        #region Operations

        public virtual void Initialize() { }

        public virtual void ViewLoaded() { }

        #endregion Operations
    }
}
