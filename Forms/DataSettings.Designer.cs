namespace Yapped.Forms
{
    partial class DataSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSettings));
            this.groupbox_Data = new System.Windows.Forms.GroupBox();
            this.checkbox_UnfurledCSVExport = new System.Windows.Forms.CheckBox();
            this.checkbox_ExportUniqueOnly = new System.Windows.Forms.CheckBox();
            this.checkbox_IncludeRowNames = new System.Windows.Forms.CheckBox();
            this.checkbox_IncludeHeader = new System.Windows.Forms.CheckBox();
            this.label_CSV_Delimiter = new System.Windows.Forms.Label();
            this.textbox_CSV_Delimiter = new System.Windows.Forms.TextBox();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.checkbox_EnableFieldValidation = new System.Windows.Forms.CheckBox();
            this.groupbox_Data.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupbox_Data
            // 
            this.groupbox_Data.Controls.Add(this.checkbox_UnfurledCSVExport);
            this.groupbox_Data.Controls.Add(this.checkbox_ExportUniqueOnly);
            this.groupbox_Data.Controls.Add(this.checkbox_IncludeRowNames);
            this.groupbox_Data.Controls.Add(this.checkbox_IncludeHeader);
            this.groupbox_Data.Controls.Add(this.label_CSV_Delimiter);
            this.groupbox_Data.Controls.Add(this.textbox_CSV_Delimiter);
            this.groupbox_Data.Location = new System.Drawing.Point(12, 53);
            this.groupbox_Data.Name = "groupbox_Data";
            this.groupbox_Data.Size = new System.Drawing.Size(255, 153);
            this.groupbox_Data.TabIndex = 32;
            this.groupbox_Data.TabStop = false;
            this.groupbox_Data.Text = "Data Export";
            // 
            // checkbox_UnfurledCSVExport
            // 
            this.checkbox_UnfurledCSVExport.AutoSize = true;
            this.checkbox_UnfurledCSVExport.Location = new System.Drawing.Point(6, 127);
            this.checkbox_UnfurledCSVExport.Name = "checkbox_UnfurledCSVExport";
            this.checkbox_UnfurledCSVExport.Size = new System.Drawing.Size(169, 17);
            this.checkbox_UnfurledCSVExport.TabIndex = 10;
            this.checkbox_UnfurledCSVExport.Text = "Unfurl Output in Exported CSV";
            this.checkbox_UnfurledCSVExport.UseVisualStyleBackColor = true;
            // 
            // checkbox_ExportUniqueOnly
            // 
            this.checkbox_ExportUniqueOnly.AutoSize = true;
            this.checkbox_ExportUniqueOnly.Location = new System.Drawing.Point(6, 104);
            this.checkbox_ExportUniqueOnly.Name = "checkbox_ExportUniqueOnly";
            this.checkbox_ExportUniqueOnly.Size = new System.Drawing.Size(154, 17);
            this.checkbox_ExportUniqueOnly.TabIndex = 9;
            this.checkbox_ExportUniqueOnly.Text = "Collate Unique Values Only";
            this.checkbox_ExportUniqueOnly.UseVisualStyleBackColor = true;
            // 
            // checkbox_IncludeRowNames
            // 
            this.checkbox_IncludeRowNames.AutoSize = true;
            this.checkbox_IncludeRowNames.Location = new System.Drawing.Point(6, 61);
            this.checkbox_IncludeRowNames.Name = "checkbox_IncludeRowNames";
            this.checkbox_IncludeRowNames.Size = new System.Drawing.Size(202, 17);
            this.checkbox_IncludeRowNames.TabIndex = 6;
            this.checkbox_IncludeRowNames.Text = "Include Row Names in Exported CSV";
            this.checkbox_IncludeRowNames.UseVisualStyleBackColor = true;
            // 
            // checkbox_IncludeHeader
            // 
            this.checkbox_IncludeHeader.AutoSize = true;
            this.checkbox_IncludeHeader.Location = new System.Drawing.Point(6, 82);
            this.checkbox_IncludeHeader.Name = "checkbox_IncludeHeader";
            this.checkbox_IncludeHeader.Size = new System.Drawing.Size(204, 17);
            this.checkbox_IncludeHeader.TabIndex = 5;
            this.checkbox_IncludeHeader.Text = "Include Header Row in Exported CSV";
            this.checkbox_IncludeHeader.UseVisualStyleBackColor = true;
            // 
            // label_CSV_Delimiter
            // 
            this.label_CSV_Delimiter.AutoSize = true;
            this.label_CSV_Delimiter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_CSV_Delimiter.Location = new System.Drawing.Point(6, 19);
            this.label_CSV_Delimiter.Name = "label_CSV_Delimiter";
            this.label_CSV_Delimiter.Size = new System.Drawing.Size(71, 13);
            this.label_CSV_Delimiter.TabIndex = 7;
            this.label_CSV_Delimiter.Text = "CSV Delimiter";
            // 
            // textbox_CSV_Delimiter
            // 
            this.textbox_CSV_Delimiter.Location = new System.Drawing.Point(6, 35);
            this.textbox_CSV_Delimiter.Name = "textbox_CSV_Delimiter";
            this.textbox_CSV_Delimiter.Size = new System.Drawing.Size(238, 20);
            this.textbox_CSV_Delimiter.TabIndex = 8;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(12, 212);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSettings.TabIndex = 33;
            this.btnSaveSettings.Text = "Save";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(192, 212);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 34;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkbox_EnableFieldValidation);
            this.groupBox1.Location = new System.Drawing.Point(12, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(255, 41);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Yapped";
            // 
            // checkbox_EnableFieldValidation
            // 
            this.checkbox_EnableFieldValidation.AutoSize = true;
            this.checkbox_EnableFieldValidation.Location = new System.Drawing.Point(6, 19);
            this.checkbox_EnableFieldValidation.Name = "checkbox_EnableFieldValidation";
            this.checkbox_EnableFieldValidation.Size = new System.Drawing.Size(133, 17);
            this.checkbox_EnableFieldValidation.TabIndex = 6;
            this.checkbox_EnableFieldValidation.Text = "Enable Field Validation";
            this.checkbox_EnableFieldValidation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkbox_EnableFieldValidation.UseVisualStyleBackColor = true;
            // 
            // DataSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 244);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.groupbox_Data);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DataSettings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Data Settings";
            this.groupbox_Data.ResumeLayout(false);
            this.groupbox_Data.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupbox_Data;
        private System.Windows.Forms.CheckBox checkbox_UnfurledCSVExport;
        private System.Windows.Forms.CheckBox checkbox_ExportUniqueOnly;
        private System.Windows.Forms.CheckBox checkbox_IncludeRowNames;
        private System.Windows.Forms.CheckBox checkbox_IncludeHeader;
        private System.Windows.Forms.Label label_CSV_Delimiter;
        private System.Windows.Forms.TextBox textbox_CSV_Delimiter;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkbox_EnableFieldValidation;
    }
}