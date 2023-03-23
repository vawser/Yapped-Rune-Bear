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
    public partial class FieldAdjuster : Form
    {
        public FieldAdjuster()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            if(Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RetainFieldText)
            {
                textbox_FieldMatch.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMatch;
                textbox_RowRange.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RowRange;
                textbox_RowPartialMatch.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RowPartialMatch;
                textbox_FieldMinimum.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMinimum;
                textbox_FieldMaximum.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMaximum;
                textbox_FieldExclusion.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldExclusion;
                textbox_FieldInclusion.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldInclusion;
                textbox_Formula.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_Formula;
                textbox_ValueMin.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_ValueMin;
                textbox_ValueMax.Text = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_ValueMax;
                checkbox_RetainFieldText.Checked = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RetainFieldText;
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
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMatch = textbox_FieldMatch.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RowRange = textbox_RowRange.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RowPartialMatch = textbox_RowPartialMatch.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMinimum = textbox_FieldMinimum.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMaximum = textbox_FieldMaximum.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldExclusion = textbox_FieldExclusion.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldInclusion = textbox_FieldInclusion.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_Formula = textbox_Formula.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_ValueMin = textbox_ValueMin.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_ValueMax = textbox_ValueMax.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RetainFieldText = checkbox_RetainFieldText.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
