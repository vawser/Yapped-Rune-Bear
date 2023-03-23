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
    public partial class SettingsMenu : Form
    {
        public SettingsMenu()
        {
            InitializeComponent();
            DialogResult = DialogResult.Cancel;

            textbox_ProjectName.Text = Yapped_Rune_Bear.Properties.Settings.Default.ProjectName;

            textbox_TextEditor.Text = Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath.ToString();
            textEditorPath.FileName = Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath;

            checkbox_VerifyRowDeletion.Checked = Yapped_Rune_Bear.Properties.Settings.Default.VerifyRowDeletion;
            checkbox_SuppressConfirmations.Checked = Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages;
            checkbox_UseTextEditor.Checked = Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor;
            checkbox_SaveNoEncryption.Checked = Yapped_Rune_Bear.Properties.Settings.Default.SaveWithoutEncryption;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            // Don't allow nothing as a project name
            if (textbox_ProjectName.Text == "")
            {
                textbox_ProjectName.Text = "ExampleMod";
                MessageBox.Show("Project Name cannot be blank. It has been reset to ExampleMod", "Settings", MessageBoxButtons.OK);
            }

            Yapped_Rune_Bear.Properties.Settings.Default.VerifyRowDeletion = checkbox_VerifyRowDeletion.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.ProjectName = textbox_ProjectName.Text;
            Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath = textEditorPath.FileName;
            Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages = checkbox_SuppressConfirmations.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor = checkbox_UseTextEditor.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.SaveWithoutEncryption = checkbox_SaveNoEncryption.Checked;

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
                Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath = textEditorPath.FileName;
            }

            textbox_TextEditor.Text = Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath.ToString();
        }
    }
}
