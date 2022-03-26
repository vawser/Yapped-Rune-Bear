using System;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormNewRow : Form
    {
        public long ResultID;

        public string ResultName;

        public FormNewRow(string prompt)
        {
            InitializeComponent();
            Text = prompt;
            DialogResult = DialogResult.Cancel;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            ResultID = (long)nudID.Value;
            ResultName = txtName.Text.Length > 0 ? txtName.Text : null;
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void nudID_Enter(object sender, EventArgs e)
        {
            nudID.Select(0, nudID.Text.Length);
        }
    }
}
