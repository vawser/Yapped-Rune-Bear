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
    public partial class FilterSettings : Form
    {
        public FilterSettings()
        {
            InitializeComponent();

            textbox_Filter_CommandDelimiter.Text = Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter;
            textbox_Filter_SectionDelimiter.Text = Yapped_Rune_Bear.Properties.Settings.Default.Filter_SectionDelimiter;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            string command_delimiter = textbox_Filter_CommandDelimiter.Text;
            string section_delimiter = textbox_Filter_SectionDelimiter.Text;

            if (command_delimiter == "")
                command_delimiter = ":";

            if (section_delimiter == "")
                section_delimiter = "~";

            Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter = command_delimiter;
            Yapped_Rune_Bear.Properties.Settings.Default.Filter_SectionDelimiter = section_delimiter;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
