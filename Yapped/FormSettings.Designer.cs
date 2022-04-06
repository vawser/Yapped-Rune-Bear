namespace Yapped
{
    partial class FormSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSettings));
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.checkbox_IncludeHeader = new System.Windows.Forms.CheckBox();
            this.checkbox_IncludeRowNames = new System.Windows.Forms.CheckBox();
            this.label_CSV_Delimiter = new System.Windows.Forms.Label();
            this.textbox_CSV_Delimiter = new System.Windows.Forms.TextBox();
            this.checkbox_VerifyRowDeletion = new System.Windows.Forms.CheckBox();
            this.checkbox_CondenseParamView = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip2 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip3 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip4 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip5 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip6 = new System.Windows.Forms.ToolTip(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.toolTip7 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip8 = new System.Windows.Forms.ToolTip(this.components);
            this.toolTip9 = new System.Windows.Forms.ToolTip(this.components);
            this.checkbox_SaveNoEncryption = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textbox_ignore_list = new System.Windows.Forms.TextBox();
            this.textbox_ProjectName = new System.Windows.Forms.TextBox();
            this.button_SelectTextEditor = new System.Windows.Forms.Button();
            this.textbox_TextEditor = new System.Windows.Forms.TextBox();
            this.textEditorPath = new System.Windows.Forms.OpenFileDialog();
            this.checkbox_SuppressConfirmations = new System.Windows.Forms.CheckBox();
            this.checkbox_UseTextEditor = new System.Windows.Forms.CheckBox();
            this.button_LogParamSizes = new System.Windows.Forms.Button();
            this.groupbox_General = new System.Windows.Forms.GroupBox();
            this.groupbox_Data = new System.Windows.Forms.GroupBox();
            this.groupbox_Workflow = new System.Windows.Forms.GroupBox();
            this.groupbox_Saving = new System.Windows.Forms.GroupBox();
            this.groupbox_Actions = new System.Windows.Forms.GroupBox();
            this.checkbox_ExportUniqueOnly = new System.Windows.Forms.CheckBox();
            this.groupbox_General.SuspendLayout();
            this.groupbox_Data.SuspendLayout();
            this.groupbox_Workflow.SuspendLayout();
            this.groupbox_Saving.SuspendLayout();
            this.groupbox_Actions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(454, 286);
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
            this.btnCancel.Location = new System.Drawing.Point(578, 286);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // checkbox_IncludeHeader
            // 
            this.checkbox_IncludeHeader.AutoSize = true;
            this.checkbox_IncludeHeader.Location = new System.Drawing.Point(9, 42);
            this.checkbox_IncludeHeader.Name = "checkbox_IncludeHeader";
            this.checkbox_IncludeHeader.Size = new System.Drawing.Size(124, 17);
            this.checkbox_IncludeHeader.TabIndex = 5;
            this.checkbox_IncludeHeader.Text = "Include Header Row";
            this.toolTip1.SetToolTip(this.checkbox_IncludeHeader, "Toggle whether Data and Field Value Export include a header row.");
            this.checkbox_IncludeHeader.UseVisualStyleBackColor = true;
            // 
            // checkbox_IncludeRowNames
            // 
            this.checkbox_IncludeRowNames.AutoSize = true;
            this.checkbox_IncludeRowNames.Location = new System.Drawing.Point(9, 19);
            this.checkbox_IncludeRowNames.Name = "checkbox_IncludeRowNames";
            this.checkbox_IncludeRowNames.Size = new System.Drawing.Size(122, 17);
            this.checkbox_IncludeRowNames.TabIndex = 6;
            this.checkbox_IncludeRowNames.Text = "Include Row Names";
            this.toolTip2.SetToolTip(this.checkbox_IncludeRowNames, "Toggle whether Data and Field Value Export include row names.");
            this.checkbox_IncludeRowNames.UseVisualStyleBackColor = true;
            // 
            // label_CSV_Delimiter
            // 
            this.label_CSV_Delimiter.AutoSize = true;
            this.label_CSV_Delimiter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_CSV_Delimiter.Location = new System.Drawing.Point(6, 65);
            this.label_CSV_Delimiter.Name = "label_CSV_Delimiter";
            this.label_CSV_Delimiter.Size = new System.Drawing.Size(71, 13);
            this.label_CSV_Delimiter.TabIndex = 7;
            this.label_CSV_Delimiter.Text = "CSV Delimiter";
            this.toolTip3.SetToolTip(this.label_CSV_Delimiter, "Set the CSV delimiter to use for Data and Field Value Export.");
            // 
            // textbox_CSV_Delimiter
            // 
            this.textbox_CSV_Delimiter.Location = new System.Drawing.Point(6, 81);
            this.textbox_CSV_Delimiter.Name = "textbox_CSV_Delimiter";
            this.textbox_CSV_Delimiter.Size = new System.Drawing.Size(200, 20);
            this.textbox_CSV_Delimiter.TabIndex = 8;
            // 
            // checkbox_VerifyRowDeletion
            // 
            this.checkbox_VerifyRowDeletion.AutoSize = true;
            this.checkbox_VerifyRowDeletion.Location = new System.Drawing.Point(9, 19);
            this.checkbox_VerifyRowDeletion.Name = "checkbox_VerifyRowDeletion";
            this.checkbox_VerifyRowDeletion.Size = new System.Drawing.Size(119, 17);
            this.checkbox_VerifyRowDeletion.TabIndex = 10;
            this.checkbox_VerifyRowDeletion.Text = "Verify Row Deletion";
            this.toolTip4.SetToolTip(this.checkbox_VerifyRowDeletion, "Toggle warning before row deletion.");
            this.checkbox_VerifyRowDeletion.UseVisualStyleBackColor = true;
            // 
            // checkbox_CondenseParamView
            // 
            this.checkbox_CondenseParamView.AutoSize = true;
            this.checkbox_CondenseParamView.Location = new System.Drawing.Point(9, 42);
            this.checkbox_CondenseParamView.Name = "checkbox_CondenseParamView";
            this.checkbox_CondenseParamView.Size = new System.Drawing.Size(133, 17);
            this.checkbox_CondenseParamView.TabIndex = 12;
            this.checkbox_CondenseParamView.Text = "Condense Param View";
            this.toolTip5.SetToolTip(this.checkbox_CondenseParamView, "Show only params that are commonly used");
            this.checkbox_CondenseParamView.UseVisualStyleBackColor = true;
            this.checkbox_CondenseParamView.CheckedChanged += new System.EventHandler(this.checkbox_CondenseParamView_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(147, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Params to Ignore during Save";
            this.toolTip6.SetToolTip(this.label2, "Define names of Param Files to ignore during the Save Process.\nThey will be saved" +
        " as is, without any layouts applied to them.");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 65);
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
            this.checkbox_SaveNoEncryption.Location = new System.Drawing.Point(9, 19);
            this.checkbox_SaveNoEncryption.Name = "checkbox_SaveNoEncryption";
            this.checkbox_SaveNoEncryption.Size = new System.Drawing.Size(141, 17);
            this.checkbox_SaveNoEncryption.TabIndex = 26;
            this.checkbox_SaveNoEncryption.Text = "Save without Encryption";
            this.toolTip8.SetToolTip(this.checkbox_SaveNoEncryption, resources.GetString("checkbox_SaveNoEncryption.ToolTip"));
            this.checkbox_SaveNoEncryption.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Current Text Editor";
            // 
            // textbox_ignore_list
            // 
            this.textbox_ignore_list.Location = new System.Drawing.Point(6, 55);
            this.textbox_ignore_list.Name = "textbox_ignore_list";
            this.textbox_ignore_list.Size = new System.Drawing.Size(418, 20);
            this.textbox_ignore_list.TabIndex = 15;
            // 
            // textbox_ProjectName
            // 
            this.textbox_ProjectName.Location = new System.Drawing.Point(6, 81);
            this.textbox_ProjectName.Name = "textbox_ProjectName";
            this.textbox_ProjectName.Size = new System.Drawing.Size(196, 20);
            this.textbox_ProjectName.TabIndex = 18;
            // 
            // button_SelectTextEditor
            // 
            this.button_SelectTextEditor.Location = new System.Drawing.Point(298, 49);
            this.button_SelectTextEditor.Name = "button_SelectTextEditor";
            this.button_SelectTextEditor.Size = new System.Drawing.Size(126, 23);
            this.button_SelectTextEditor.TabIndex = 22;
            this.button_SelectTextEditor.Text = "Select Text Editor";
            this.button_SelectTextEditor.UseVisualStyleBackColor = true;
            this.button_SelectTextEditor.Click += new System.EventHandler(this.button_SelectTextEditor_Click);
            // 
            // textbox_TextEditor
            // 
            this.textbox_TextEditor.Location = new System.Drawing.Point(6, 78);
            this.textbox_TextEditor.Name = "textbox_TextEditor";
            this.textbox_TextEditor.Size = new System.Drawing.Size(418, 20);
            this.textbox_TextEditor.TabIndex = 23;
            // 
            // textEditorPath
            // 
            this.textEditorPath.Filter = ".exe|*";
            // 
            // checkbox_SuppressConfirmations
            // 
            this.checkbox_SuppressConfirmations.AutoSize = true;
            this.checkbox_SuppressConfirmations.Location = new System.Drawing.Point(9, 19);
            this.checkbox_SuppressConfirmations.Name = "checkbox_SuppressConfirmations";
            this.checkbox_SuppressConfirmations.Size = new System.Drawing.Size(182, 17);
            this.checkbox_SuppressConfirmations.TabIndex = 25;
            this.checkbox_SuppressConfirmations.Text = "Suppress Confirmation Messages";
            this.checkbox_SuppressConfirmations.UseVisualStyleBackColor = true;
            // 
            // checkbox_UseTextEditor
            // 
            this.checkbox_UseTextEditor.AutoSize = true;
            this.checkbox_UseTextEditor.Location = new System.Drawing.Point(9, 42);
            this.checkbox_UseTextEditor.Name = "checkbox_UseTextEditor";
            this.checkbox_UseTextEditor.Size = new System.Drawing.Size(169, 17);
            this.checkbox_UseTextEditor.TabIndex = 26;
            this.checkbox_UseTextEditor.Text = "Automatically open output files";
            this.checkbox_UseTextEditor.UseVisualStyleBackColor = true;
            // 
            // button_LogParamSizes
            // 
            this.button_LogParamSizes.Location = new System.Drawing.Point(6, 19);
            this.button_LogParamSizes.Name = "button_LogParamSizes";
            this.button_LogParamSizes.Size = new System.Drawing.Size(189, 23);
            this.button_LogParamSizes.TabIndex = 27;
            this.button_LogParamSizes.Text = "Log Param Sizes";
            this.button_LogParamSizes.UseVisualStyleBackColor = true;
            this.button_LogParamSizes.Click += new System.EventHandler(this.button_LogParamSizes_Click);
            // 
            // groupbox_General
            // 
            this.groupbox_General.Controls.Add(this.checkbox_VerifyRowDeletion);
            this.groupbox_General.Controls.Add(this.checkbox_CondenseParamView);
            this.groupbox_General.Controls.Add(this.textbox_ProjectName);
            this.groupbox_General.Controls.Add(this.label1);
            this.groupbox_General.Location = new System.Drawing.Point(12, 12);
            this.groupbox_General.Name = "groupbox_General";
            this.groupbox_General.Size = new System.Drawing.Size(212, 107);
            this.groupbox_General.TabIndex = 30;
            this.groupbox_General.TabStop = false;
            this.groupbox_General.Text = "General";
            // 
            // groupbox_Data
            // 
            this.groupbox_Data.Controls.Add(this.checkbox_ExportUniqueOnly);
            this.groupbox_Data.Controls.Add(this.checkbox_IncludeRowNames);
            this.groupbox_Data.Controls.Add(this.checkbox_IncludeHeader);
            this.groupbox_Data.Controls.Add(this.label_CSV_Delimiter);
            this.groupbox_Data.Controls.Add(this.textbox_CSV_Delimiter);
            this.groupbox_Data.Location = new System.Drawing.Point(230, 12);
            this.groupbox_Data.Name = "groupbox_Data";
            this.groupbox_Data.Size = new System.Drawing.Size(423, 107);
            this.groupbox_Data.TabIndex = 31;
            this.groupbox_Data.TabStop = false;
            this.groupbox_Data.Text = "Data";
            // 
            // groupbox_Workflow
            // 
            this.groupbox_Workflow.Controls.Add(this.checkbox_SuppressConfirmations);
            this.groupbox_Workflow.Controls.Add(this.checkbox_UseTextEditor);
            this.groupbox_Workflow.Controls.Add(this.button_SelectTextEditor);
            this.groupbox_Workflow.Controls.Add(this.label4);
            this.groupbox_Workflow.Controls.Add(this.textbox_TextEditor);
            this.groupbox_Workflow.Location = new System.Drawing.Point(12, 125);
            this.groupbox_Workflow.Name = "groupbox_Workflow";
            this.groupbox_Workflow.Size = new System.Drawing.Size(430, 103);
            this.groupbox_Workflow.TabIndex = 32;
            this.groupbox_Workflow.TabStop = false;
            this.groupbox_Workflow.Text = "Workflow";
            // 
            // groupbox_Saving
            // 
            this.groupbox_Saving.Controls.Add(this.checkbox_SaveNoEncryption);
            this.groupbox_Saving.Controls.Add(this.label2);
            this.groupbox_Saving.Controls.Add(this.textbox_ignore_list);
            this.groupbox_Saving.Location = new System.Drawing.Point(12, 234);
            this.groupbox_Saving.Name = "groupbox_Saving";
            this.groupbox_Saving.Size = new System.Drawing.Size(430, 81);
            this.groupbox_Saving.TabIndex = 33;
            this.groupbox_Saving.TabStop = false;
            this.groupbox_Saving.Text = "Saving";
            // 
            // groupbox_Actions
            // 
            this.groupbox_Actions.Controls.Add(this.button_LogParamSizes);
            this.groupbox_Actions.Location = new System.Drawing.Point(448, 125);
            this.groupbox_Actions.Name = "groupbox_Actions";
            this.groupbox_Actions.Size = new System.Drawing.Size(205, 155);
            this.groupbox_Actions.TabIndex = 34;
            this.groupbox_Actions.TabStop = false;
            this.groupbox_Actions.Text = "Actions";
            // 
            // checkbox_ExportUniqueOnly
            // 
            this.checkbox_ExportUniqueOnly.AutoSize = true;
            this.checkbox_ExportUniqueOnly.Location = new System.Drawing.Point(208, 19);
            this.checkbox_ExportUniqueOnly.Name = "checkbox_ExportUniqueOnly";
            this.checkbox_ExportUniqueOnly.Size = new System.Drawing.Size(152, 17);
            this.checkbox_ExportUniqueOnly.TabIndex = 9;
            this.checkbox_ExportUniqueOnly.Text = "Export Unique Values Only";
            this.toolTip9.SetToolTip(this.checkbox_ExportUniqueOnly, "Field Exporter will only include each unique field value once. Useful for making a list of possible field values.");
            this.checkbox_ExportUniqueOnly.UseVisualStyleBackColor = true;
            // 
            // FormSettings
            // 
            this.AcceptButton = this.btnSaveSettings;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(661, 319);
            this.Controls.Add(this.groupbox_Actions);
            this.Controls.Add(this.groupbox_Saving);
            this.Controls.Add(this.groupbox_Workflow);
            this.Controls.Add(this.groupbox_Data);
            this.Controls.Add(this.groupbox_General);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FormSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Settings";
            this.Load += new System.EventHandler(this.FormSettings_Load);
            this.groupbox_General.ResumeLayout(false);
            this.groupbox_General.PerformLayout();
            this.groupbox_Data.ResumeLayout(false);
            this.groupbox_Data.PerformLayout();
            this.groupbox_Workflow.ResumeLayout(false);
            this.groupbox_Workflow.PerformLayout();
            this.groupbox_Saving.ResumeLayout(false);
            this.groupbox_Saving.PerformLayout();
            this.groupbox_Actions.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox checkbox_IncludeHeader;
        private System.Windows.Forms.CheckBox checkbox_IncludeRowNames;
        private System.Windows.Forms.Label label_CSV_Delimiter;
        private System.Windows.Forms.TextBox textbox_CSV_Delimiter;
        private System.Windows.Forms.CheckBox checkbox_VerifyRowDeletion;
        private System.Windows.Forms.CheckBox checkbox_CondenseParamView;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ToolTip toolTip2;
        private System.Windows.Forms.ToolTip toolTip3;
        private System.Windows.Forms.ToolTip toolTip4;
        private System.Windows.Forms.ToolTip toolTip5;
        private System.Windows.Forms.ToolTip toolTip6;
        private System.Windows.Forms.ToolTip toolTip7;
        private System.Windows.Forms.ToolTip toolTip8;
        private System.Windows.Forms.ToolTip toolTip9;
        private System.Windows.Forms.TextBox textbox_ignore_list;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textbox_ProjectName;
        private System.Windows.Forms.Button button_SelectTextEditor;
        private System.Windows.Forms.TextBox textbox_TextEditor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.OpenFileDialog textEditorPath;
        private System.Windows.Forms.CheckBox checkbox_SuppressConfirmations;
        private System.Windows.Forms.CheckBox checkbox_UseTextEditor;
        private System.Windows.Forms.Button button_LogParamSizes;
        private System.Windows.Forms.GroupBox groupbox_General;
        private System.Windows.Forms.GroupBox groupbox_Data;
        private System.Windows.Forms.GroupBox groupbox_Workflow;
        private System.Windows.Forms.GroupBox groupbox_Saving;
        private System.Windows.Forms.CheckBox checkbox_SaveNoEncryption;
        private System.Windows.Forms.GroupBox groupbox_Actions;
        private System.Windows.Forms.CheckBox checkbox_ExportUniqueOnly;
    }
}