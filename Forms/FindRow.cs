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
    public partial class FindRow : Form
    {
        public string ResultPattern;

        public FindRow(string prompt)
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
