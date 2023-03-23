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
    public partial class NewRow : Form
    {
        public int ResultID;

        public string ResultName;

        public NewRow(string prompt)
        {
            InitializeComponent();
            Text = prompt;
            DialogResult = DialogResult.Cancel;
            textbox_RepeatCount.Text = Yapped_Rune_Bear.Properties.Settings.Default.NewRow_RepeatCount.ToString();
            textbox_StepValue.Text = Yapped_Rune_Bear.Properties.Settings.Default.NewRow_StepValue.ToString();
        }

        public NewRow(string prompt, int row_id, string row_name)
        {
            InitializeComponent();
            Text = prompt;
            nudID.Text = row_id.ToString();
            txtName.Text = row_name.ToString();
            DialogResult = DialogResult.Cancel;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            ResultID = (int)nudID.Value;
            ResultName = txtName.Text.Length > 0 ? txtName.Text : null;

            Yapped_Rune_Bear.Properties.Settings.Default.NewRow_RepeatCount = Convert.ToInt32(textbox_RepeatCount.Text);
            Yapped_Rune_Bear.Properties.Settings.Default.NewRow_StepValue = Convert.ToInt32(textbox_StepValue.Text);

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
