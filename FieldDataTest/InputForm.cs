using System.Windows.Forms;

namespace FieldDataTest
{
    public partial class InputForm : Form
    {
        #region Constructors

        public InputForm()
        {
            InitializeComponent();
        }

        public InputForm(string title, string prompt, string data)
            : this()
        {
            Text = title;
            Prompt.Text = prompt;
            Result = data;
            Data.Text = data;
        }

        #endregion Constructors

        #region Properties

        public string Result { get; set; }

        #endregion Properties

        #region Event Handlers

        private void Ok_Click(object sender, System.EventArgs e)
        {
            Result = Data.Text;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void Cancel_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion Event Handlers
    }
}
