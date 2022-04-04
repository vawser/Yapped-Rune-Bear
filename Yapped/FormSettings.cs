using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace Yapped
{
    public partial class FormSettings : Form
    {
        public FormSettings()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            checkbox_CondenseParamView.Checked = Properties.Settings.Default.ShowCommonParamsOnly;
            checkbox_VerifyRowDeletion.Checked = Properties.Settings.Default.VerifyRowDeletion;
            checkbox_IncludeHeader.Checked = Properties.Settings.Default.IncludeHeaderInCSV;
            checkbox_IncludeRowNames.Checked = Properties.Settings.Default.IncludeRowNameInCSV;
            textbox_ignore_list.Text = Properties.Settings.Default.ParamsToIgnoreDuringSave;
            textbox_CSV_Delimiter.Text = Properties.Settings.Default.ExportDelimiter;
            textbox_ProjectName.Text = Properties.Settings.Default.ProjectName;
            textbox_TextEditor.Text = Properties.Settings.Default.TextEditorPath.ToString();
            textEditorPath.FileName = Properties.Settings.Default.TextEditorPath;
            checkbox_SuppressConfirmations.Checked = Properties.Settings.Default.ShowConfirmationMessages;
            checkbox_UseTextEditor.Checked = Properties.Settings.Default.UseTextEditor;
            checkbox_SaveNoEncryption.Checked = Properties.Settings.Default.SaveWithoutEncryption;
            checkbox_ExportUniqueOnly.Checked = Properties.Settings.Default.ExportUniqueOnly;
        }
        
        private void btnCreate_Click(object sender, EventArgs e)
        {
            // Don't allow nothing as a delimiter
            if (textbox_CSV_Delimiter.Text == "")
            {
                textbox_CSV_Delimiter.Text = ",";
            }
            
            Properties.Settings.Default.ShowCommonParamsOnly = checkbox_CondenseParamView.Checked;
            Properties.Settings.Default.VerifyRowDeletion = checkbox_VerifyRowDeletion.Checked;
            Properties.Settings.Default.IncludeHeaderInCSV = checkbox_IncludeHeader.Checked;
            Properties.Settings.Default.IncludeRowNameInCSV = checkbox_IncludeRowNames.Checked;
            Properties.Settings.Default.ParamsToIgnoreDuringSave = textbox_ignore_list.Text;
            Properties.Settings.Default.ExportDelimiter = textbox_CSV_Delimiter.Text;
            Properties.Settings.Default.ProjectName = textbox_ProjectName.Text;
            Properties.Settings.Default.TextEditorPath = textEditorPath.FileName;
            Properties.Settings.Default.ShowConfirmationMessages = checkbox_SuppressConfirmations.Checked;
            Properties.Settings.Default.UseTextEditor = checkbox_UseTextEditor.Checked;
            Properties.Settings.Default.SaveWithoutEncryption = checkbox_SaveNoEncryption.Checked;
            Properties.Settings.Default.ExportUniqueOnly = checkbox_ExportUniqueOnly.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void FormSettings_Load(object sender, EventArgs e)
        {

        }

        private void button_SelectTextEditor_Click(object sender, EventArgs e)
        {
            textEditorPath.FileName = "";

            if (textEditorPath.ShowDialog() == DialogResult.OK)
            {
                Properties.Settings.Default.TextEditorPath = textEditorPath.FileName;
            }

            textbox_TextEditor.Text = Properties.Settings.Default.TextEditorPath.ToString();
        }

        private void button_LogParamSizes_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Settings_LogParamSize = true;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void checkbox_CondenseParamView_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.ChangedCommonParamView = true;
        }
    }
}
