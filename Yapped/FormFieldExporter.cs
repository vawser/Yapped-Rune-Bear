using System;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormFieldExporter : Form
    {
        public FormFieldExporter()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            checkbox_RetainFieldText.Checked = Properties.Settings.Default.FieldExporter_RetainFieldText;


            if (checkbox_RetainFieldText.Checked)
            {
                textbox_FieldMatch.Text = Properties.Settings.Default.FieldExporter_FieldMatch;
                textbox_FieldMinimum.Text = Properties.Settings.Default.FieldExporter_FieldMinimum;
                textbox_FieldMaximum.Text = Properties.Settings.Default.FieldExporter_FieldMaximum;
                textbox_FieldExclusions.Text = Properties.Settings.Default.FieldExporter_FieldExclusion;
                textbox_FieldInclusions.Text = Properties.Settings.Default.FieldExporter_FieldInclusion;
            }
            else
            {
                textbox_FieldMatch.Text = "";
                textbox_FieldMinimum.Text = "";
                textbox_FieldMaximum.Text = "";
                textbox_FieldExclusions.Text = "";
                textbox_FieldInclusions.Text = "";
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FieldExporter_FieldMatch = textbox_FieldMatch.Text;
            Properties.Settings.Default.FieldExporter_FieldMinimum = textbox_FieldMinimum.Text;
            Properties.Settings.Default.FieldExporter_FieldMaximum = textbox_FieldMaximum.Text;
            Properties.Settings.Default.FieldExporter_FieldExclusion = textbox_FieldExclusions.Text;
            Properties.Settings.Default.FieldExporter_FieldInclusion = textbox_FieldInclusions.Text;
            Properties.Settings.Default.FieldExporter_RetainFieldText = checkbox_RetainFieldText.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
