using System;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormReferenceFinder : Form
    {
        public FormReferenceFinder()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        public string GetReferenceText()
        {
            return this.textbox_referenceText.Text;
        }
    }
}
