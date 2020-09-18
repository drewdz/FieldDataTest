using System;
using System.Windows.Forms;

namespace FieldDataTest
{
    public partial class ConesForm : Form
    {
        #region Constructors

        public ConesForm()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Fields

        public string Id1 { get; set; } = Guid.NewGuid().ToString();

        public string Id2 { get; set; } = Guid.NewGuid().ToString();

        public string Id3 { get; set; } = Guid.NewGuid().ToString();

        public string Id4 { get; set; } = Guid.NewGuid().ToString();

        #endregion Fields

        #region Event Handlers

        private void Ok_Click(object sender, System.EventArgs e)
        {
            Id1 = Cone1.Text;
            Id2 = Cone2.Text;
            Id3 = Cone3.Text;
            Id4 = Cone4.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void ConesForm_Load(object sender, EventArgs e)
        {
            Cone1.Text = Id1;
            Cone2.Text = Id2;
            Cone3.Text = Id3;
            Cone4.Text = Id4;
        }

        #endregion Event Handlers
    }
}
