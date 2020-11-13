using FieldDataTest.Bindings;
using FieldDataTest.ViewModels;
using System.Collections.Generic;
using System.Windows.Forms;

namespace FieldDataTest
{
    public class BaseForm<TViewModel> : Form, IBindable where TViewModel : BaseViewModel, new()
    {
        #region Delegates

        protected delegate void CrossDelegateMethod(string s);
        protected delegate void CrossDelegateMethod<T, U>(T target, U value);

        #endregion Delegates

        #region Fields

        #endregion Fields

        #region Constructors

        public BaseForm()
        {
            ViewModel = new TViewModel();
            ViewModel.Initialize();
            ViewModel.PropertyChanged += PropertyChanged;
            this.Load += FormLoad;
        }

        #endregion Constructors

        #region Properties

        public TViewModel ViewModel { get; set; }

        public Dictionary<string, Bindings.Binding> Bindings { get; set; } = new Dictionary<string, Bindings.Binding>();

        #endregion Properties

        #region Operations

        protected virtual void Initialize() { }

        protected override void Dispose(bool disposing)
        {
            ViewModel.PropertyChanged -= PropertyChanged;
            ViewModel = null;
            base.Dispose(disposing);
        }

        #endregion Operations

        #region Event Handlers

        protected virtual void FormLoad(object sender, System.EventArgs e)
        {
            ViewModel.ViewLoaded();
        }

        public void PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //  find the binding
            if (!Bindings.ContainsKey(e.PropertyName)) return;
            var binding = Bindings[e.PropertyName];
            //  find the view property
            var viewProperty = binding.Target.GetType().GetProperty(binding.ViewProperty);
            var vPropertyValue = viewProperty.GetValue(binding.Target);
            //  get the view model property
            var viewModelProperty = ViewModel.GetType().GetProperty(binding.ViewModelProperty);
            var vmPropertyValue = viewModelProperty.GetValue(ViewModel);
            //  avoid endless loops
            if (vPropertyValue == vmPropertyValue) return;
            viewProperty.SetValue(binding.Target, vmPropertyValue);
        }

        #endregion Event Handlers
    }
}
