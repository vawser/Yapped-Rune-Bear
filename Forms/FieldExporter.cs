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
    public partial class FieldExporter : Form
    {
        public FieldExporter()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            checkbox_RetainFieldText.Checked = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_RetainFieldText;


            if (checkbox_RetainFieldText.Checked)
            {
                textbox_FieldMatch.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMatch;
                textbox_FieldMinimum.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMinimum;
                textbox_FieldMaximum.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMaximum;
                textbox_FieldExclusions.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldExclusion;
                textbox_FieldInclusions.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldInclusion;
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
            Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMatch = textbox_FieldMatch.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMinimum = textbox_FieldMinimum.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMaximum = textbox_FieldMaximum.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldExclusion = textbox_FieldExclusions.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldInclusion = textbox_FieldInclusions.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_RetainFieldText = checkbox_RetainFieldText.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
