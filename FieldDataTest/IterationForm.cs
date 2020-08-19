using System.Windows.Forms;

namespace FieldDataTest
{
    public partial class IterationForm : Form
    {
        #region Constructors

        public IterationForm()
        {
            InitializeComponent();
        }

        #endregion Constructors

        #region Properties

        public int Iterations { get; set; } = 1;

        #endregion Properties

        #region Event Handlers

        private void IterationForm_Load(object sender, System.EventArgs e)
        {
            IterationCount.Value = Iterations;
        }

        private void OkButton_Click(object sender, System.EventArgs e)
        {
            Iterations = (int)IterationCount.Value;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CancelButton_Click(object sender, System.EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        #endregion Event Handlers
    }
}
