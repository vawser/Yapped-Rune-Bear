using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Yapped.Forms
{
    public partial class RowReferenceSearch : Form
    {
        public RowReferenceSearch()
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
