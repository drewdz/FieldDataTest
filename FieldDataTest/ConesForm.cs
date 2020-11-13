using FieldDataTest.Bindings;
using FieldDataTest.ViewModels;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FieldDataTest
{
    public partial class ConesForm : BaseForm<ConesViewModel>
    {
        #region Fields

        private DateTime _LastChange = DateTime.Now;

        #endregion Fields

        #region Constructors

        public ConesForm()
        {
            InitializeComponent();
            var set = BindingSet<BaseForm<ConesViewModel>, ConesViewModel>.Create(this, ViewModel);

            this.Bind(BindingBuilder.BindOn(Cone1).For("Text").To("Cone1").Build<ConesForm, ConesViewModel>());
            this.Bind(BindingBuilder.BindOn(Cone2).For("Text").To("Cone2").Build<ConesForm, ConesViewModel>());
            this.Bind(BindingBuilder.BindOn(Cone3).For("Text").To("Cone3").Build<ConesForm, ConesViewModel>());
            this.Bind(BindingBuilder.BindOn(Cone4).For("Text").To("Cone4").Build<ConesForm, ConesViewModel>());
        }

        #endregion Constructors

        #region Lifecycle

        #endregion

        #region Event Handlers

        private void Ok_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private async void OnTextChanged(object sender, EventArgs e)
        {
            _LastChange = DateTime.Now;
            await Task.Delay(700);
            if (DateTime.Now.Subtract(_LastChange).TotalMilliseconds < 700) return;
            if (sender == Cone1)
            {
                ViewModel.Cone1 = Cone1.Text;
            }
            else if (sender == Cone2)
            {
                ViewModel.Cone2 = Cone2.Text;
            }
            else if (sender == Cone3)
            {
                ViewModel.Cone3 = Cone3.Text;
            }
            else if (sender == Cone4)
            {
                ViewModel.Cone4 = Cone4.Text;
            }
        }

        #endregion Event Handlers
    }
}
