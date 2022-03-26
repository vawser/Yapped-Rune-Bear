using System;
using System.Windows.Forms;

namespace Yapped
{
    public partial class FormFieldAdjuster : Form
    {

        public FormFieldAdjuster()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            if(Properties.Settings.Default.FieldAdjuster_RetainFieldText)
            {
                textbox_FieldMatch.Text = Properties.Settings.Default.FieldAdjuster_FieldMatch;
                textbox_RowRange.Text = Properties.Settings.Default.FieldAdjuster_RowRange;
                textbox_RowPartialMatch.Text = Properties.Settings.Default.FieldAdjuster_RowPartialMatch;
                textbox_FieldMinimum.Text = Properties.Settings.Default.FieldAdjuster_FieldMinimum;
                textbox_FieldMaximum.Text = Properties.Settings.Default.FieldAdjuster_FieldMaximum;
                textbox_FieldExclusion.Text = Properties.Settings.Default.FieldAdjuster_FieldExclusion;
                textbox_FieldInclusion.Text = Properties.Settings.Default.FieldAdjuster_FieldInclusion;
                textbox_Formula.Text = Properties.Settings.Default.FieldAdjuster_Formula;
                textbox_ValueMin.Text = Properties.Settings.Default.FieldAdjuster_ValueMin;
                textbox_ValueMax.Text = Properties.Settings.Default.FieldAdjuster_ValueMax;
                checkbox_RetainFieldText.Checked = Properties.Settings.Default.FieldAdjuster_RetainFieldText;
            }
            else
            {
                textbox_FieldMatch.Text = "";
                textbox_RowRange.Text = "";
                textbox_RowPartialMatch.Text = "";
                textbox_FieldMinimum.Text = "";
                textbox_FieldMaximum.Text = "";
                textbox_FieldExclusion.Text = "";
                textbox_FieldInclusion.Text = "";
                textbox_Formula.Text = "";
                textbox_ValueMin.Text = "";
                textbox_ValueMax.Text = "";
            }
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.FieldAdjuster_FieldMatch = textbox_FieldMatch.Text;
            Properties.Settings.Default.FieldAdjuster_RowRange = textbox_RowRange.Text;
            Properties.Settings.Default.FieldAdjuster_RowPartialMatch = textbox_RowPartialMatch.Text;
            Properties.Settings.Default.FieldAdjuster_FieldMinimum = textbox_FieldMinimum.Text;
            Properties.Settings.Default.FieldAdjuster_FieldMaximum = textbox_FieldMaximum.Text;
            Properties.Settings.Default.FieldAdjuster_FieldExclusion = textbox_FieldExclusion.Text;
            Properties.Settings.Default.FieldAdjuster_FieldInclusion = textbox_FieldInclusion.Text;
            Properties.Settings.Default.FieldAdjuster_Formula = textbox_Formula.Text;
            Properties.Settings.Default.FieldAdjuster_ValueMin = textbox_ValueMin.Text;
            Properties.Settings.Default.FieldAdjuster_ValueMax = textbox_ValueMax.Text;
            Properties.Settings.Default.FieldAdjuster_RetainFieldText = checkbox_RetainFieldText.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
