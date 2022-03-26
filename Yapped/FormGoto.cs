using System;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormGoto : Form
    {
        public long ResultID;

        public FormGoto()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;
        }

        private void btnGoto_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ResultID = (long)nudID.Value;
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
