namespace Yapped.Forms
{
    partial class FilterSettings
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
            this.group_Filter = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textbox_Filter_CommandDelimiter = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textbox_Filter_SectionDelimiter = new System.Windows.Forms.TextBox();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.group_Filter.SuspendLayout();
            this.SuspendLayout();
            // 
            // group_Filter
            // 
            this.group_Filter.Controls.Add(this.textbox_Filter_SectionDelimiter);
            this.group_Filter.Controls.Add(this.label2);
            this.group_Filter.Controls.Add(this.textbox_Filter_CommandDelimiter);
            this.group_Filter.Controls.Add(this.label1);
            this.group_Filter.Location = new System.Drawing.Point(12, 12);
            this.group_Filter.Name = "group_Filter";
            this.group_Filter.Size = new System.Drawing.Size(250, 112);
            this.group_Filter.TabIndex = 35;
            this.group_Filter.TabStop = false;
            this.group_Filter.Text = "Filter ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(122, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filter Command Delimiter";
            // 
            // textbox_Filter_CommandDelimiter
            // 
            this.textbox_Filter_CommandDelimiter.Location = new System.Drawing.Point(9, 37);
            this.textbox_Filter_CommandDelimiter.Name = "textbox_Filter_CommandDelimiter";
            this.textbox_Filter_CommandDelimiter.Size = new System.Drawing.Size(235, 20);
            this.textbox_Filter_CommandDelimiter.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Filter Section Delimiter";
            // 
            // textbox_Filter_SectionDelimiter
            // 
            this.textbox_Filter_SectionDelimiter.Location = new System.Drawing.Point(9, 80);
            this.textbox_Filter_SectionDelimiter.Name = "textbox_Filter_SectionDelimiter";
            this.textbox_Filter_SectionDelimiter.Size = new System.Drawing.Size(235, 20);
            this.textbox_Filter_SectionDelimiter.TabIndex = 3;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(12, 130);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(75, 23);
            this.btnSaveSettings.TabIndex = 36;
            this.btnSaveSettings.Text = "Save";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(187, 130);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 37;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FilterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 167);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.group_Filter);
            this.Name = "FilterSettings";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Filter Settings";
            this.group_Filter.ResumeLayout(false);
            this.group_Filter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox group_Filter;
        private System.Windows.Forms.TextBox textbox_Filter_SectionDelimiter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textbox_Filter_CommandDelimiter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.Button btnCancel;
    }
}