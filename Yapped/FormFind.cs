using System;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormFind : Form
    {
        public string ResultPattern;

        public FormFind(string prompt)
        {
            InitializeComponent();
            Text = prompt;
            DialogResult = DialogResult.Cancel;
        }

        private void btnFind_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            ResultPattern = txtPattern.Text;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
