namespace Yapped.Dump
{
    partial class FormMain
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
            System.Windows.Forms.Label lblRegulation;
            System.Windows.Forms.Label lblOutput;
            System.Windows.Forms.Label label3;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.txtRegulation = new System.Windows.Forms.TextBox();
            this.txtOutput = new System.Windows.Forms.TextBox();
            this.btnBrowseRegulation = new System.Windows.Forms.Button();
            this.btnExploreRegulation = new System.Windows.Forms.Button();
            this.btnExploreOutput = new System.Windows.Forms.Button();
            this.btnBrowseOutput = new System.Windows.Forms.Button();
            this.btnDump = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            lblRegulation = new System.Windows.Forms.Label();
            lblOutput = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblRegulation
            // 
            lblRegulation.AutoSize = true;
            lblRegulation.Location = new System.Drawing.Point(12, 9);
            lblRegulation.Name = "lblRegulation";
            lblRegulation.Size = new System.Drawing.Size(58, 13);
            lblRegulation.TabIndex = 0;
            lblRegulation.Text = "Regulation";
            // 
            // lblOutput
            // 
            lblOutput.AutoSize = true;
            lblOutput.Location = new System.Drawing.Point(12, 48);
            lblOutput.Name = "lblOutput";
            lblOutput.Size = new System.Drawing.Size(84, 13);
            lblOutput.TabIndex = 1;
            lblOutput.Text = "Output Directory";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(12, 114);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(37, 13);
            label3.TabIndex = 8;
            label3.Text = "Status";
            // 
            // txtRegulation
            // 
            this.txtRegulation.Location = new System.Drawing.Point(12, 25);
            this.txtRegulation.Name = "txtRegulation";
            this.txtRegulation.Size = new System.Drawing.Size(614, 20);
            this.txtRegulation.TabIndex = 2;
            this.txtRegulation.Text = "C:\\Users\\Joseph\\Downloads\\gameparam.parambnd.dcx";
            // 
            // txtOutput
            // 
            this.txtOutput.Location = new System.Drawing.Point(12, 64);
            this.txtOutput.Name = "txtOutput";
            this.txtOutput.Size = new System.Drawing.Size(614, 20);
            this.txtOutput.TabIndex = 3;
            // 
            // btnBrowseRegulation
            // 
            this.btnBrowseRegulation.Location = new System.Drawing.Point(632, 23);
            this.btnBrowseRegulation.Name = "btnBrowseRegulation";
            this.btnBrowseRegulation.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseRegulation.TabIndex = 4;
            this.btnBrowseRegulation.Text = "Browse";
            this.btnBrowseRegulation.UseVisualStyleBackColor = true;
            // 
            // btnExploreRegulation
            // 
            this.btnExploreRegulation.Location = new System.Drawing.Point(713, 23);
            this.btnExploreRegulation.Name = "btnExploreRegulation";
            this.btnExploreRegulation.Size = new System.Drawing.Size(75, 23);
            this.btnExploreRegulation.TabIndex = 5;
            this.btnExploreRegulation.Text = "Explore";
            this.btnExploreRegulation.UseVisualStyleBackColor = true;
            // 
            // btnExploreOutput
            // 
            this.btnExploreOutput.Location = new System.Drawing.Point(713, 62);
            this.btnExploreOutput.Name = "btnExploreOutput";
            this.btnExploreOutput.Size = new System.Drawing.Size(75, 23);
            this.btnExploreOutput.TabIndex = 7;
            this.btnExploreOutput.Text = "Explore";
            this.btnExploreOutput.UseVisualStyleBackColor = true;
            // 
            // btnBrowseOutput
            // 
            this.btnBrowseOutput.Location = new System.Drawing.Point(632, 62);
            this.btnBrowseOutput.Name = "btnBrowseOutput";
            this.btnBrowseOutput.Size = new System.Drawing.Size(75, 23);
            this.btnBrowseOutput.TabIndex = 6;
            this.btnBrowseOutput.Text = "Browse";
            this.btnBrowseOutput.UseVisualStyleBackColor = true;
            // 
            // btnDump
            // 
            this.btnDump.Location = new System.Drawing.Point(713, 101);
            this.btnDump.Name = "btnDump";
            this.btnDump.Size = new System.Drawing.Size(75, 23);
            this.btnDump.TabIndex = 10;
            this.btnDump.Text = "Dump";
            this.btnDump.UseVisualStyleBackColor = true;
            this.btnDump.Click += new System.EventHandler(this.btnDump_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(12, 130);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(776, 308);
            this.txtStatus.TabIndex = 11;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.btnDump);
            this.Controls.Add(label3);
            this.Controls.Add(this.btnExploreOutput);
            this.Controls.Add(this.btnBrowseOutput);
            this.Controls.Add(this.btnExploreRegulation);
            this.Controls.Add(this.btnBrowseRegulation);
            this.Controls.Add(this.txtOutput);
            this.Controls.Add(this.txtRegulation);
            this.Controls.Add(lblOutput);
            this.Controls.Add(lblRegulation);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Yapped.Dump <version>";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtRegulation;
        private System.Windows.Forms.TextBox txtOutput;
        private System.Windows.Forms.Button btnBrowseRegulation;
        private System.Windows.Forms.Button btnExploreRegulation;
        private System.Windows.Forms.Button btnExploreOutput;
        private System.Windows.Forms.Button btnBrowseOutput;
        private System.Windows.Forms.Button btnDump;
        private System.Windows.Forms.TextBox txtStatus;
    }
}

