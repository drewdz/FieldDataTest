using FieldDataTest.Bindings;
using FieldDataTest.ViewModels;
using System.Diagnostics;
using System.Windows.Forms;

namespace FieldDataTest
{
    public partial class TunnelView : BaseForm<TunnelViewModel>
    {
        #region Constructors

        public TunnelView()
        {
            InitializeComponent();

            var set = BindingSet<TunnelView, TunnelViewModel>.Create(this, ViewModel);
            this.Bind(BindingBuilder.BindOn(Status).For("Text").To("State").Build<TunnelView, TunnelViewModel>());
            this.Bind(BindingBuilder.BindOn(Info).For("Text").To("Feedback").Build<TunnelView, TunnelViewModel>());

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        ~TunnelView()
        {
            if (ViewModel == null) return;
            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
            ViewModel.State = TunnelViewModel.STATE_STOP;
            ViewModel = null;
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Debug.WriteLine($"{nameof(TunnelView)}.{nameof(ViewModel_PropertyChanged)} - Property Changed: {e.PropertyName}");
        }

        #endregion Constructors

        #region Event Handlers

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            if ((ViewModel == null) || (ViewModel.CancelCommand == null)) return;
            if (ViewModel.CancelCommand.CanExecute(null)) ViewModel.CancelCommand.Execute(null);
        }

        private void Info_TextChanged(object sender, System.EventArgs e)
        {
            Info.Select(Info.Text.Length - 1, 0);
            Info.ScrollToCaret();
        }

        #endregion Event Handlers
    }
}
