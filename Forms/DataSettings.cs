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
    public partial class DataSettings : Form
    {
        public DataSettings()
        {
            InitializeComponent();

            checkbox_IncludeHeader.Checked = Yapped_Rune_Bear.Properties.Settings.Default.IncludeHeaderInCSV;
            checkbox_IncludeRowNames.Checked = Yapped_Rune_Bear.Properties.Settings.Default.IncludeRowNameInCSV;
            checkbox_ExportUniqueOnly.Checked = Yapped_Rune_Bear.Properties.Settings.Default.ExportUniqueOnly;
            checkbox_UnfurledCSVExport.Checked = Yapped_Rune_Bear.Properties.Settings.Default.VerboseCSVExport;
            textbox_CSV_Delimiter.Text = Yapped_Rune_Bear.Properties.Settings.Default.ExportDelimiter;
            checkbox_EnableFieldValidation.Checked = Yapped_Rune_Bear.Properties.Settings.Default.EnableFieldValidation;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            // Don't allow nothing as a delimiter
            if (textbox_CSV_Delimiter.Text == "")
            {
                textbox_CSV_Delimiter.Text = ";";
                MessageBox.Show("CSV Delimiter cannot be blank. It has been reset to ;", "Settings", MessageBoxButtons.OK);
            }

            Yapped_Rune_Bear.Properties.Settings.Default.ExportDelimiter = textbox_CSV_Delimiter.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.IncludeHeaderInCSV = checkbox_IncludeHeader.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.IncludeRowNameInCSV = checkbox_IncludeRowNames.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.ExportUniqueOnly = checkbox_ExportUniqueOnly.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.VerboseCSVExport = checkbox_UnfurledCSVExport.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.EnableFieldValidation = checkbox_EnableFieldValidation.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
