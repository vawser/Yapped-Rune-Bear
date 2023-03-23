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
    public partial class InterfaceSettings : Form
    {


        public InterfaceSettings()
        {
            InitializeComponent();

            Color int_color = Color.FromArgb(
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Int_R, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Int_G, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Int_B);

            Color float_color = Color.FromArgb(
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Float_R, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Float_G, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Float_B);

            Color bool_color = Color.FromArgb(
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Bool_R, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Bool_G, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Bool_B);

            Color string_color = Color.FromArgb(
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_String_R, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_String_G, 
                Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_String_B);

            colorDialog_FieldValue_Int.Color = int_color;
            colorDialog_FieldValue_Float.Color = float_color;
            colorDialog_FieldValue_Bool.Color = bool_color;
            colorDialog_FieldValue_String.Color = string_color;

            textBox_FieldColor_Int.BackColor = colorDialog_FieldValue_Int.Color;
            textBox_FieldColor_Float.BackColor = colorDialog_FieldValue_Float.Color;
            textBox_FieldColor_Bool.BackColor = colorDialog_FieldValue_Bool.Color;
            textBox_FieldColor_String.BackColor = colorDialog_FieldValue_String.Color;

            checkbox_DisplayEnums.Checked = Yapped_Rune_Bear.Properties.Settings.Default.ShowEnums;
            checkbox_EnumValueInName.Checked = Yapped_Rune_Bear.Properties.Settings.Default.ShowEnumValueInName;
            checkbox_FieldDescriptions.Checked = Yapped_Rune_Bear.Properties.Settings.Default.ShowFieldDescriptions;
            checkbox_BooleanEnumToggle.Checked = Yapped_Rune_Bear.Properties.Settings.Default.DisplayBooleanEnumAsCheckbox;
            checkbox_customizableEnumToggle.Checked = Yapped_Rune_Bear.Properties.Settings.Default.DisableEnumForCustomTypes;
        }

        private void button_FieldColor_int_Click(object sender, EventArgs e)
        {
            colorDialog_FieldValue_Int.AllowFullOpen = true;
            colorDialog_FieldValue_Int.ShowHelp = true;
            colorDialog_FieldValue_Int.Color = textBox_FieldColor_Int.BackColor;

            if (colorDialog_FieldValue_Int.ShowDialog() == DialogResult.OK)
                textBox_FieldColor_Int.BackColor = colorDialog_FieldValue_Int.Color;
        }

        private void button_FieldColor_Float_Click(object sender, EventArgs e)
        {
            colorDialog_FieldValue_Float.AllowFullOpen = true;
            colorDialog_FieldValue_Float.ShowHelp = true;
            colorDialog_FieldValue_Float.Color = textBox_FieldColor_Float.BackColor;

            if (colorDialog_FieldValue_Float.ShowDialog() == DialogResult.OK)
                textBox_FieldColor_Float.BackColor = colorDialog_FieldValue_Float.Color;
        }

        private void button_FieldColor_Bool_Click(object sender, EventArgs e)
        {
            colorDialog_FieldValue_Bool.AllowFullOpen = true;
            colorDialog_FieldValue_Bool.ShowHelp = true;
            colorDialog_FieldValue_Bool.Color = textBox_FieldColor_Bool.BackColor;

            if (colorDialog_FieldValue_Bool.ShowDialog() == DialogResult.OK)
                textBox_FieldColor_Bool.BackColor = colorDialog_FieldValue_Bool.Color;
        }

        private void button_FieldColor_String_Click(object sender, EventArgs e)
        {
            colorDialog_FieldValue_String.AllowFullOpen = true;
            colorDialog_FieldValue_String.ShowHelp = true;
            colorDialog_FieldValue_String.Color = textBox_FieldColor_String.BackColor;

            if (colorDialog_FieldValue_String.ShowDialog() == DialogResult.OK)
                textBox_FieldColor_String.BackColor = colorDialog_FieldValue_String.Color;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Int_R = colorDialog_FieldValue_Int.Color.R;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Int_G = colorDialog_FieldValue_Int.Color.G;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Int_B = colorDialog_FieldValue_Int.Color.B;

            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Float_R = colorDialog_FieldValue_Float.Color.R;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Float_G = colorDialog_FieldValue_Float.Color.G;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Float_B = colorDialog_FieldValue_Float.Color.B;

            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Bool_R = colorDialog_FieldValue_Bool.Color.R;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Bool_G = colorDialog_FieldValue_Bool.Color.G;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_Bool_B = colorDialog_FieldValue_Bool.Color.B;

            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_String_R = colorDialog_FieldValue_String.Color.R;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_String_G = colorDialog_FieldValue_String.Color.G;
            Yapped_Rune_Bear.Properties.Settings.Default.FieldColor_String_B = colorDialog_FieldValue_String.Color.B;

            Yapped_Rune_Bear.Properties.Settings.Default.ShowEnums = checkbox_DisplayEnums.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.ShowEnumValueInName = checkbox_EnumValueInName.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.ShowFieldDescriptions = checkbox_FieldDescriptions.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.DisplayBooleanEnumAsCheckbox = checkbox_BooleanEnumToggle.Checked;
            Yapped_Rune_Bear.Properties.Settings.Default.DisableEnumForCustomTypes = checkbox_customizableEnumToggle.Checked;

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
