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
    public partial class GoToRow : Form
    {
        public long ResultID;

        public GoToRow()
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
