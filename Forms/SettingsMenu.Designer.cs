namespace Yapped.Forms
{
    partial class SettingsMenu
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsMenu));
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkbox_VerifyRowDeletion = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip5 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip6 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip7 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip8 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip9 = new System.Windows.Forms.ToolTip(this.components);
            this.checkbox_SaveNoEncryption = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textbox_ProjectName = new System.Windows.Forms.TextBox();
            this.button_SelectTextEditor = new System.Windows.Forms.Button();
            this.textbox_TextEditor = new System.Windows.Forms.TextBox();
            this.textEditorPath = new System.Windows.Forms.OpenFileDialog();
            this.checkbox_SuppressConfirmations = new System.Windows.Forms.CheckBox();
            this.checkbox_UseTextEditor = new System.Windows.Forms.CheckBox();
            this.groupbox_General = new System.Windows.Forms.GroupBox();
            this.groupbox_Workflow = new System.Windows.Forms.GroupBox();
            this.secondaryDataPath = new System.Windows.Forms.OpenFileDialog();
            this.groupbox_General.SuspendLayout();
            this.groupbox_Workflow.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(13, 253);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSettings.TabIndex = 2;
            this.btnSaveSettings.Text = "Save";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(188, 253);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // checkbox_VerifyRowDeletion
            // 
            this.checkbox_VerifyRowDeletion.AutoSize = true;
            this.checkbox_VerifyRowDeletion.Location = new System.Drawing.Point(6, 84);
            this.checkbox_VerifyRowDeletion.Name = "checkbox_VerifyRowDeletion";
            this.checkbox_VerifyRowDeletion.Size = new System.Drawing.Size(119, 17);
            this.checkbox_VerifyRowDeletion.TabIndex = 10;
            this.checkbox_VerifyRowDeletion.Text = "Verify Row Deletion";
            this.toolTip4.SetToolTip(this.checkbox_VerifyRowDeletion, "Toggle warning before row deletion.");
            this.checkbox_VerifyRowDeletion.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "Project Name";
            this.toolTip7.SetToolTip(this.label1, "Define a project name. This is used to isolate mod-specific names so the original" +
        " Name files are not overwritten.");
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // checkbox_SaveNoEncryption
            // 
            this.checkbox_SaveNoEncryption.AutoSize = true;
            this.checkbox_SaveNoEncryption.Location = new System.Drawing.Point(6, 61);
            this.checkbox_SaveNoEncryption.Name = "checkbox_SaveNoEncryption";
            this.checkbox_SaveNoEncryption.Size = new System.Drawing.Size(141, 17);
            this.checkbox_SaveNoEncryption.TabIndex = 26;
            this.checkbox_SaveNoEncryption.Text = "Save without Encryption";
            this.checkbox_SaveNoEncryption.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Current Text Editor";
            // 
            // textbox_ProjectName
            // 
            this.textbox_ProjectName.Location = new System.Drawing.Point(6, 35);
            this.textbox_ProjectName.Name = "textbox_ProjectName";
            this.textbox_ProjectName.Size = new System.Drawing.Size(238, 20);
            this.textbox_ProjectName.TabIndex = 18;
            // 
            // button_SelectTextEditor
            // 
            this.button_SelectTextEditor.Location = new System.Drawing.Point(140, 33);
            this.button_SelectTextEditor.Name = "button_SelectTextEditor";
            this.button_SelectTextEditor.Size = new System.Drawing.Size(104, 23);
            this.button_SelectTextEditor.TabIndex = 22;
            this.button_SelectTextEditor.Text = "Select Text Editor";
            this.button_SelectTextEditor.UseVisualStyleBackColor = true;
            this.button_SelectTextEditor.Click += new System.EventHandler(this.button_SelectTextEditor_Click);
            // 
            // textbox_TextEditor
            // 
            this.textbox_TextEditor.Location = new System.Drawing.Point(6, 35);
            this.textbox_TextEditor.Name = "textbox_TextEditor";
            this.textbox_TextEditor.Size = new System.Drawing.Size(128, 20);
            this.textbox_TextEditor.TabIndex = 23;
            // 
            // textEditorPath
            // 
            this.textEditorPath.Filter = ".exe|*";
            // 
            // checkbox_SuppressConfirmations
            // 
            this.checkbox_SuppressConfirmations.AutoSize = true;
            this.checkbox_SuppressConfirmations.Location = new System.Drawing.Point(6, 61);
            this.checkbox_SuppressConfirmations.Name = "checkbox_SuppressConfirmations";
            this.checkbox_SuppressConfirmations.Size = new System.Drawing.Size(182, 17);
            this.checkbox_SuppressConfirmations.TabIndex = 25;
            this.checkbox_SuppressConfirmations.Text = "Suppress Confirmation Messages";
            this.checkbox_SuppressConfirmations.UseVisualStyleBackColor = true;
            // 
            // checkbox_UseTextEditor
            // 
            this.checkbox_UseTextEditor.AutoSize = true;
            this.checkbox_UseTextEditor.Location = new System.Drawing.Point(6, 84);
            this.checkbox_UseTextEditor.Name = "checkbox_UseTextEditor";
            this.checkbox_UseTextEditor.Size = new System.Drawing.Size(169, 17);
            this.checkbox_UseTextEditor.TabIndex = 26;
            this.checkbox_UseTextEditor.Text = "Automatically open output files";
            this.checkbox_UseTextEditor.UseVisualStyleBackColor = true;
            // 
            // groupbox_General
            // 
            this.groupbox_General.Controls.Add(this.checkbox_SaveNoEncryption);
            this.groupbox_General.Controls.Add(this.checkbox_VerifyRowDeletion);
            this.groupbox_General.Controls.Add(this.textbox_ProjectName);
            this.groupbox_General.Controls.Add(this.label1);
            this.groupbox_General.Location = new System.Drawing.Point(12, 12);
            this.groupbox_General.Name = "groupbox_General";
            this.groupbox_General.Size = new System.Drawing.Size(250, 115);
            this.groupbox_General.TabIndex = 30;
            this.groupbox_General.TabStop = false;
            this.groupbox_General.Text = "General";
            // 
            // groupbox_Workflow
            // 
            this.groupbox_Workflow.Controls.Add(this.checkbox_SuppressConfirmations);
            this.groupbox_Workflow.Controls.Add(this.checkbox_UseTextEditor);
            this.groupbox_Workflow.Controls.Add(this.button_SelectTextEditor);
            this.groupbox_Workflow.Controls.Add(this.label4);
            this.groupbox_Workflow.Controls.Add(this.textbox_TextEditor);
            this.groupbox_Workflow.Location = new System.Drawing.Point(13, 133);
            this.groupbox_Workflow.Name = "groupbox_Workflow";
            this.groupbox_Workflow.Size = new System.Drawing.Size(250, 114);
            this.groupbox_Workflow.TabIndex = 32;
            this.groupbox_Workflow.TabStop = false;
            this.groupbox_Workflow.Text = "Workflow";
            // 
            // secondaryDataPath
            // 
            this.secondaryDataPath.Filter = ".bdt|.bin|All files|*";
            // 
            // SettingsMenu
            // 
            this.AcceptButton = this.btnSaveSettings;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(275, 289);
            this.Controls.Add(this.groupbox_Workflow);
            this.Controls.Add(this.groupbox_General);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.groupbox_General.ResumeLayout(false);
            this.groupbox_General.PerformLayout();
            this.groupbox_Workflow.ResumeLayout(false);
            this.groupbox_Workflow.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox checkbox_VerifyRowDeletion;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.ToolTip toolTip4;
        private System.Windows.Forms.ToolTip toolTip5;
        private System.Windows.Forms.ToolTip toolTip6;
        private System.Windows.Forms.ToolTip toolTip7;
        private System.Windows.Forms.ToolTip toolTip8;
        private System.Windows.Forms.ToolTip toolTip9;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textbox_ProjectName;
        private System.Windows.Forms.Button button_SelectTextEditor;
        private System.Windows.Forms.TextBox textbox_TextEditor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog textEditorPath;
        private System.Windows.Forms.CheckBox checkbox_SuppressConfirmations;
        private System.Windows.Forms.CheckBox checkbox_UseTextEditor;
        private System.Windows.Forms.GroupBox groupbox_General;
        private System.Windows.Forms.GroupBox groupbox_Workflow;
        private System.Windows.Forms.CheckBox checkbox_SaveNoEncryption;
        private System.Windows.Forms.OpenFileDialog secondaryDataPath;
    }
}