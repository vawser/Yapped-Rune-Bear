namespace Yapped
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.ofdRegulation = new System.Windows.Forms.OpenFileDialog();
            this.importedRegulation = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.restoreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripComboBoxGame = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.exploreToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickfolderNamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickfolderLayoutMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickfolderDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickfolderFieldValueMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quickfolderReferenceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gotoRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findNextFieldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.massImportDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.massExportDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.importNamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportNamesMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.fieldExporterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.valueReferenceFinderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rowReferenceFinderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.WorkflowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fieldAdjusterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.affinityGeneratorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.dgvParams = new System.Windows.Forms.DataGridView();
            this.dgvParamsParamCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvRows = new System.Windows.Forms.DataGridView();
            this.dgvRowsIDCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvRowsNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCells = new System.Windows.Forms.DataGridView();
            this.dgvCellsTypeCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCellsNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvCellsValueCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.processLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.fbdExport = new System.Windows.Forms.FolderBrowserDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRows)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCells)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ofdRegulation
            // 
            this.ofdRegulation.FileName = "gameparam.parambnd.dcx";
            this.ofdRegulation.Filter = "Regulation or parambnd|*";
            // 
            // importedRegulation
            // 
            this.importedRegulation.FileName = "regulation.bin";
            this.importedRegulation.Filter = "Regulation or parambnd|*";
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.ToolStripMenuItem,
            this.WorkflowToolStripMenuItem,
            this.settingsMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(977, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.restoreToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.toolStripSeparator5,
            this.toolStripComboBoxGame,
            this.toolStripSeparator2,
            this.exploreToolStripMenuItem,
            this.quickfolderNamesMenuItem,
            this.quickfolderLayoutMenuItem,
            this.quickfolderDataMenuItem,
            this.quickfolderFieldValueMenuItem,
            this.quickfolderReferenceMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.openToolStripMenuItem.Text = "&Open";
            this.openToolStripMenuItem.ToolTipText = "Browse for a regulation file to edit.";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.saveToolStripMenuItem.Text = "&Save";
            this.saveToolStripMenuItem.ToolTipText = "Save changes to the regulation file.";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_Click);
            // 
            // restoreToolStripMenuItem
            // 
            this.restoreToolStripMenuItem.Name = "restoreToolStripMenuItem";
            this.restoreToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.restoreToolStripMenuItem.Text = "Restore";
            this.restoreToolStripMenuItem.ToolTipText = "Restore the regulation file backup.";
            this.restoreToolStripMenuItem.Click += new System.EventHandler(this.RestoreToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.ToolTipText = "Export an encrypted Data0.bdt to decrypted parambnds.";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.ExportToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(195, 6);
            // 
            // toolStripComboBoxGame
            // 
            this.toolStripComboBoxGame.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolStripComboBoxGame.Name = "toolStripComboBoxGame";
            this.toolStripComboBoxGame.Size = new System.Drawing.Size(121, 23);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(195, 6);
            // 
            // exploreToolStripMenuItem
            // 
            this.exploreToolStripMenuItem.Name = "exploreToolStripMenuItem";
            this.exploreToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.exploreToolStripMenuItem.Text = "Show Regulation File";
            this.exploreToolStripMenuItem.ToolTipText = "Open the regulation file directory in Explorer.";
            this.exploreToolStripMenuItem.Click += new System.EventHandler(this.ExploreToolStripMenuItem_Click);
            // 
            // quickfolderNamesMenuItem
            // 
            this.quickfolderNamesMenuItem.Name = "quickfolderNamesMenuItem";
            this.quickfolderNamesMenuItem.Size = new System.Drawing.Size(198, 22);
            this.quickfolderNamesMenuItem.Text = "Show Names Folder";
            this.quickfolderNamesMenuItem.Click += new System.EventHandler(this.quickfolderNamesMenuItem_Click);
            // 
            // quickfolderLayoutMenuItem
            // 
            this.quickfolderLayoutMenuItem.Name = "quickfolderLayoutMenuItem";
            this.quickfolderLayoutMenuItem.Size = new System.Drawing.Size(198, 22);
            this.quickfolderLayoutMenuItem.Text = "Show Layout Folder";
            this.quickfolderLayoutMenuItem.Click += new System.EventHandler(this.quickfolderLayoutMenuItem_Click);
            // 
            // quickfolderDataMenuItem
            // 
            this.quickfolderDataMenuItem.Name = "quickfolderDataMenuItem";
            this.quickfolderDataMenuItem.Size = new System.Drawing.Size(198, 22);
            this.quickfolderDataMenuItem.Text = "Show Data Folder";
            this.quickfolderDataMenuItem.Click += new System.EventHandler(this.quickfolderDataMenuItem_Click);
            // 
            // quickfolderFieldValueMenuItem
            // 
            this.quickfolderFieldValueMenuItem.Name = "quickfolderFieldValueMenuItem";
            this.quickfolderFieldValueMenuItem.Size = new System.Drawing.Size(198, 22);
            this.quickfolderFieldValueMenuItem.Text = "Show Field Value Folder";
            this.quickfolderFieldValueMenuItem.Click += new System.EventHandler(this.quickfolderFieldValueMenuItem_Click);
            // 
            // quickfolderReferenceMenuItem
            // 
            this.quickfolderReferenceMenuItem.Name = "quickfolderReferenceMenuItem";
            this.quickfolderReferenceMenuItem.Size = new System.Drawing.Size(198, 22);
            this.quickfolderReferenceMenuItem.Text = "Show Reference Folder";
            this.quickfolderReferenceMenuItem.Click += new System.EventHandler(this.quickfolderReferenceMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addRowToolStripMenuItem,
            this.duplicateRowToolStripMenuItem,
            this.deleteRowToolStripMenuItem,
            this.findRowToolStripMenuItem,
            this.findNextRowToolStripMenuItem,
            this.gotoRowToolStripMenuItem,
            this.findFieldToolStripMenuItem,
            this.findNextFieldToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // addRowToolStripMenuItem
            // 
            this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
            this.addRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.addRowToolStripMenuItem.Text = "Add Row";
            this.addRowToolStripMenuItem.ToolTipText = "Add a new row to the active param.";
            this.addRowToolStripMenuItem.Click += new System.EventHandler(this.AddRowToolStripMenuItem_Click);
            // 
            // duplicateRowToolStripMenuItem
            // 
            this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
            this.duplicateRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.N)));
            this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
            this.duplicateRowToolStripMenuItem.ToolTipText = "Create a new row with values identical to the selected one";
            this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.DuplicateRowToolStripMenuItem_Click);
            // 
            // deleteRowToolStripMenuItem
            // 
            this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
            this.deleteRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.deleteRowToolStripMenuItem.Text = "Delete Row";
            this.deleteRowToolStripMenuItem.ToolTipText = "Delete the currently selected row";
            this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.DeleteRowToolStripMenuItem_Click);
            // 
            // findRowToolStripMenuItem
            // 
            this.findRowToolStripMenuItem.Name = "findRowToolStripMenuItem";
            this.findRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.findRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findRowToolStripMenuItem.Text = "&Find Row";
            this.findRowToolStripMenuItem.ToolTipText = "Search for a row with a matching name";
            this.findRowToolStripMenuItem.Click += new System.EventHandler(this.FindRowToolStripMenuItem_Click);
            // 
            // findNextRowToolStripMenuItem
            // 
            this.findNextRowToolStripMenuItem.Name = "findNextRowToolStripMenuItem";
            this.findNextRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.K)));
            this.findNextRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findNextRowToolStripMenuItem.Text = "Find Next Row";
            this.findNextRowToolStripMenuItem.ToolTipText = "Search again with the previous pattern";
            this.findNextRowToolStripMenuItem.Click += new System.EventHandler(this.FindNextRowToolStripMenuItem_Click);
            // 
            // gotoRowToolStripMenuItem
            // 
            this.gotoRowToolStripMenuItem.Name = "gotoRowToolStripMenuItem";
            this.gotoRowToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.gotoRowToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.gotoRowToolStripMenuItem.Text = "&Goto Row";
            this.gotoRowToolStripMenuItem.ToolTipText = "Go to a row with a certain ID";
            this.gotoRowToolStripMenuItem.Click += new System.EventHandler(this.GotoRowToolStripMenuItem_Click);
            // 
            // findFieldToolStripMenuItem
            // 
            this.findFieldToolStripMenuItem.Name = "findFieldToolStripMenuItem";
            this.findFieldToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.F)));
            this.findFieldToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findFieldToolStripMenuItem.Text = "Find Field";
            this.findFieldToolStripMenuItem.ToolTipText = "Search for a field with a matching name";
            this.findFieldToolStripMenuItem.Click += new System.EventHandler(this.FindFieldToolStripMenuItem_Click);
            // 
            // findNextFieldToolStripMenuItem
            // 
            this.findNextFieldToolStripMenuItem.Name = "findNextFieldToolStripMenuItem";
            this.findNextFieldToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F3)));
            this.findNextFieldToolStripMenuItem.Size = new System.Drawing.Size(225, 22);
            this.findNextFieldToolStripMenuItem.Text = "Find Next Field";
            this.findNextFieldToolStripMenuItem.ToolTipText = "Search again with the previous pattern";
            this.findNextFieldToolStripMenuItem.Click += new System.EventHandler(this.FindNextFieldToolStripMenuItem_Click);
            // 
            // ToolStripMenuItem
            // 
            this.ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importDataMenuItem,
            this.exportDataMenuItem,
            this.toolStripSeparator7,
            this.massImportDataMenuItem,
            this.massExportDataMenuItem,
            this.toolStripSeparator8,
            this.importNamesMenuItem,
            this.exportNamesMenuItem,
            this.toolStripSeparator9,
            this.fieldExporterMenuItem,
            this.valueReferenceFinderMenuItem,
            this.rowReferenceFinderMenuItem});
            this.ToolStripMenuItem.Name = "ToolStripMenuItem";
            this.ToolStripMenuItem.Size = new System.Drawing.Size(73, 20);
            this.ToolStripMenuItem.Text = "Data Tools";
            // 
            // importDataMenuItem
            // 
            this.importDataMenuItem.AutoToolTip = true;
            this.importDataMenuItem.Name = "importDataMenuItem";
            this.importDataMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.importDataMenuItem.Size = new System.Drawing.Size(212, 22);
            this.importDataMenuItem.Text = "Import Data";
            this.importDataMenuItem.ToolTipText = "For the currently selected param, \r\nimport param data from a CSV file of the same" +
    " name.\r\n\r\nData files are found in .\\<gametype>\\Data\\";
            this.importDataMenuItem.Click += new System.EventHandler(this.importDataMenuItem_Click);
            // 
            // exportDataMenuItem
            // 
            this.exportDataMenuItem.AutoToolTip = true;
            this.exportDataMenuItem.Name = "exportDataMenuItem";
            this.exportDataMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F12;
            this.exportDataMenuItem.Size = new System.Drawing.Size(212, 22);
            this.exportDataMenuItem.Text = "Export Data";
            this.exportDataMenuItem.ToolTipText = "For the currently selected param, \r\nexport param data into a CSV file of the same" +
    " name.\r\n\r\nData files are found in .\\<gametype>\\Data\\\r\n";
            this.exportDataMenuItem.Click += new System.EventHandler(this.exportDataMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(209, 6);
            // 
            // massImportDataMenuItem
            // 
            this.massImportDataMenuItem.AutoToolTip = true;
            this.massImportDataMenuItem.Name = "massImportDataMenuItem";
            this.massImportDataMenuItem.Size = new System.Drawing.Size(212, 22);
            this.massImportDataMenuItem.Text = "Mass Import Data";
            this.massImportDataMenuItem.ToolTipText = "For all params, import param data from \r\na CSV file of the same name.\r\n\r\nData fil" +
    "es are found in .\\<gametype>\\Data\\\r\n";
            this.massImportDataMenuItem.Click += new System.EventHandler(this.massImportDataMenuItem_Click);
            // 
            // massExportDataMenuItem
            // 
            this.massExportDataMenuItem.AutoToolTip = true;
            this.massExportDataMenuItem.Name = "massExportDataMenuItem";
            this.massExportDataMenuItem.Size = new System.Drawing.Size(212, 22);
            this.massExportDataMenuItem.Text = "Mass Export Data";
            this.massExportDataMenuItem.ToolTipText = "For all params, export param data into a CSV file \r\nof the same name.\r\n\r\nData fil" +
    "es are found in .\\<gametype>\\Data\\\r\n";
            this.massExportDataMenuItem.Click += new System.EventHandler(this.massExportDataMenuItem_Click);
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(209, 6);
            // 
            // importNamesMenuItem
            // 
            this.importNamesMenuItem.AutoToolTip = true;
            this.importNamesMenuItem.Name = "importNamesMenuItem";
            this.importNamesMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.importNamesMenuItem.Size = new System.Drawing.Size(212, 22);
            this.importNamesMenuItem.Text = "Import Names";
            this.importNamesMenuItem.ToolTipText = "Import row names from text files.\r\n\r\nNames can be found in .\\<gametype>\\Names\\";
            this.importNamesMenuItem.Click += new System.EventHandler(this.importNamesMenuItem_Click);
            // 
            // exportNamesMenuItem
            // 
            this.exportNamesMenuItem.AutoToolTip = true;
            this.exportNamesMenuItem.Name = "exportNamesMenuItem";
            this.exportNamesMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F8;
            this.exportNamesMenuItem.Size = new System.Drawing.Size(212, 22);
            this.exportNamesMenuItem.Text = "Export Names";
            this.exportNamesMenuItem.ToolTipText = "Export row names into text files.\r\n\r\nNames can be found in .\\<gametype>\\Names\\\r\n";
            this.exportNamesMenuItem.Click += new System.EventHandler(this.exportNamesMenuItem_Click);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(209, 6);
            // 
            // fieldExporterMenuItem
            // 
            this.fieldExporterMenuItem.AutoToolTip = true;
            this.fieldExporterMenuItem.Name = "fieldExporterMenuItem";
            this.fieldExporterMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.fieldExporterMenuItem.Size = new System.Drawing.Size(212, 22);
            this.fieldExporterMenuItem.Text = "Field Exporter";
            this.fieldExporterMenuItem.ToolTipText = "Export the specific field values for a field.";
            this.fieldExporterMenuItem.Click += new System.EventHandler(this.fieldExporterMenuItem_Click);
            // 
            // valueReferenceFinderMenuItem
            // 
            this.valueReferenceFinderMenuItem.AutoToolTip = true;
            this.valueReferenceFinderMenuItem.Name = "valueReferenceFinderMenuItem";
            this.valueReferenceFinderMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F3;
            this.valueReferenceFinderMenuItem.Size = new System.Drawing.Size(212, 22);
            this.valueReferenceFinderMenuItem.Text = "Value Reference Finder";
            this.valueReferenceFinderMenuItem.ToolTipText = "Find all references to a field value.";
            this.valueReferenceFinderMenuItem.Click += new System.EventHandler(this.valueReferenceFinderMenuItem_Click);
            // 
            // rowReferenceFinderMenuItem
            // 
            this.rowReferenceFinderMenuItem.AutoToolTip = true;
            this.rowReferenceFinderMenuItem.Name = "rowReferenceFinderMenuItem";
            this.rowReferenceFinderMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.rowReferenceFinderMenuItem.Size = new System.Drawing.Size(212, 22);
            this.rowReferenceFinderMenuItem.Text = "Row Reference Finder";
            this.rowReferenceFinderMenuItem.ToolTipText = "Find all references to a row ID.";
            this.rowReferenceFinderMenuItem.Click += new System.EventHandler(this.rowReferenceFinderMenuItem_Click);
            // 
            // WorkflowToolStripMenuItem
            // 
            this.WorkflowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fieldAdjusterMenuItem,
            this.toolStripSeparator3,
            this.affinityGeneratorMenuItem});
            this.WorkflowToolStripMenuItem.Name = "WorkflowToolStripMenuItem";
            this.WorkflowToolStripMenuItem.Size = new System.Drawing.Size(100, 20);
            this.WorkflowToolStripMenuItem.Text = "Workflow Tools";
            // 
            // fieldAdjusterMenuItem
            // 
            this.fieldAdjusterMenuItem.AutoToolTip = true;
            this.fieldAdjusterMenuItem.Name = "fieldAdjusterMenuItem";
            this.fieldAdjusterMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F2;
            this.fieldAdjusterMenuItem.Size = new System.Drawing.Size(213, 22);
            this.fieldAdjusterMenuItem.Text = "Field Adjuster";
            this.fieldAdjusterMenuItem.ToolTipText = "Use this to quickly apply a field edit to multiple rows.\r\nContains various tools " +
    "to filter which rows are edited.";
            this.fieldAdjusterMenuItem.Click += new System.EventHandler(this.fieldAdjusterMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(210, 6);
            // 
            // affinityGeneratorMenuItem
            // 
            this.affinityGeneratorMenuItem.AutoToolTip = true;
            this.affinityGeneratorMenuItem.Name = "affinityGeneratorMenuItem";
            this.affinityGeneratorMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.affinityGeneratorMenuItem.Size = new System.Drawing.Size(213, 22);
            this.affinityGeneratorMenuItem.Text = "Affinity Generator";
            this.affinityGeneratorMenuItem.ToolTipText = "Use this to quickly generate rows for weapons\r\nthat lack affinities.\r\n\r\nYou can c" +
    "hange the configuration applied by\r\nediting the text files in:\r\n.\\<gametype>\\Con" +
    "figuration\\AffinityGenerator\\";
            this.affinityGeneratorMenuItem.Click += new System.EventHandler(this.affinityGeneratorMenuItem_Click);
            // 
            // settingsMenuItem
            // 
            this.settingsMenuItem.AutoToolTip = true;
            this.settingsMenuItem.Name = "settingsMenuItem";
            this.settingsMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsMenuItem.Text = "Settings";
            this.settingsMenuItem.ToolTipText = "Configure the settings used by Yapped for various features.";
            this.settingsMenuItem.Click += new System.EventHandler(this.settingsMenuItem_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 24);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.dgvParams);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
            this.splitContainer2.Size = new System.Drawing.Size(977, 580);
            this.splitContainer2.SplitterDistance = 249;
            this.splitContainer2.TabIndex = 2;
            // 
            // dgvParams
            // 
            this.dgvParams.AllowUserToAddRows = false;
            this.dgvParams.AllowUserToDeleteRows = false;
            this.dgvParams.AllowUserToResizeColumns = false;
            this.dgvParams.AllowUserToResizeRows = false;
            this.dgvParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvParamsParamCol});
            this.dgvParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvParams.Location = new System.Drawing.Point(0, 0);
            this.dgvParams.MultiSelect = false;
            this.dgvParams.Name = "dgvParams";
            this.dgvParams.RowHeadersVisible = false;
            this.dgvParams.RowHeadersWidth = 51;
            this.dgvParams.Size = new System.Drawing.Size(249, 580);
            this.dgvParams.TabIndex = 0;
            this.dgvParams.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvParams_CellContentClick);
            this.dgvParams.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.DgvParams_CellToolTipTextNeeded);
            this.dgvParams.SelectionChanged += new System.EventHandler(this.DgvParams_SelectionChanged);
            // 
            // dgvParamsParamCol
            // 
            this.dgvParamsParamCol.DataPropertyName = "Name";
            this.dgvParamsParamCol.HeaderText = "Param";
            this.dgvParamsParamCol.MinimumWidth = 6;
            this.dgvParamsParamCol.Name = "dgvParamsParamCol";
            this.dgvParamsParamCol.ReadOnly = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.dgvRows);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dgvCells);
            this.splitContainer1.Size = new System.Drawing.Size(724, 580);
            this.splitContainer1.SplitterDistance = 242;
            this.splitContainer1.TabIndex = 7;
            // 
            // dgvRows
            // 
            this.dgvRows.AllowUserToAddRows = false;
            this.dgvRows.AllowUserToDeleteRows = false;
            this.dgvRows.AllowUserToResizeColumns = false;
            this.dgvRows.AllowUserToResizeRows = false;
            this.dgvRows.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRows.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvRowsIDCol,
            this.dgvRowsNameCol});
            this.dgvRows.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRows.Location = new System.Drawing.Point(0, 0);
            this.dgvRows.MultiSelect = false;
            this.dgvRows.Name = "dgvRows";
            this.dgvRows.RowHeadersVisible = false;
            this.dgvRows.RowHeadersWidth = 51;
            this.dgvRows.Size = new System.Drawing.Size(242, 580);
            this.dgvRows.TabIndex = 1;
            this.dgvRows.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DgvRows_CellValidating);
            this.dgvRows.SelectionChanged += new System.EventHandler(this.DgvRows_SelectionChanged);
            // 
            // dgvRowsIDCol
            // 
            this.dgvRowsIDCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvRowsIDCol.DataPropertyName = "ID";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dgvRowsIDCol.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRowsIDCol.HeaderText = "Row";
            this.dgvRowsIDCol.MinimumWidth = 6;
            this.dgvRowsIDCol.Name = "dgvRowsIDCol";
            this.dgvRowsIDCol.Width = 54;
            // 
            // dgvRowsNameCol
            // 
            this.dgvRowsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvRowsNameCol.DataPropertyName = "Name";
            this.dgvRowsNameCol.HeaderText = "Name";
            this.dgvRowsNameCol.MinimumWidth = 6;
            this.dgvRowsNameCol.Name = "dgvRowsNameCol";
            // 
            // dgvCells
            // 
            this.dgvCells.AllowUserToAddRows = false;
            this.dgvCells.AllowUserToDeleteRows = false;
            this.dgvCells.AllowUserToResizeColumns = false;
            this.dgvCells.AllowUserToResizeRows = false;
            this.dgvCells.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCells.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvCellsTypeCol,
            this.dgvCellsNameCol,
            this.dgvCellsValueCol});
            this.dgvCells.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvCells.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvCells.Location = new System.Drawing.Point(0, 0);
            this.dgvCells.Name = "dgvCells";
            this.dgvCells.RowHeadersVisible = false;
            this.dgvCells.RowHeadersWidth = 51;
            this.dgvCells.Size = new System.Drawing.Size(478, 580);
            this.dgvCells.TabIndex = 2;
            this.dgvCells.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.DgvCells_CellFormatting);
            this.dgvCells.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.DgvCells_CellParsing);
            this.dgvCells.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.DgvCells_CellToolTipTextNeeded);
            this.dgvCells.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.DgvCells_CellValidating);
            this.dgvCells.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.DgvCells_DataBindingComplete);
            this.dgvCells.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.DgvCells_DataError);
            // 
            // dgvCellsTypeCol
            // 
            this.dgvCellsTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvCellsTypeCol.DataPropertyName = "Type";
            this.dgvCellsTypeCol.HeaderText = "Type";
            this.dgvCellsTypeCol.MinimumWidth = 6;
            this.dgvCellsTypeCol.Name = "dgvCellsTypeCol";
            this.dgvCellsTypeCol.ReadOnly = true;
            this.dgvCellsTypeCol.Width = 56;
            // 
            // dgvCellsNameCol
            // 
            this.dgvCellsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvCellsNameCol.DataPropertyName = "Name";
            this.dgvCellsNameCol.HeaderText = "Name";
            this.dgvCellsNameCol.MinimumWidth = 6;
            this.dgvCellsNameCol.Name = "dgvCellsNameCol";
            this.dgvCellsNameCol.ReadOnly = true;
            this.dgvCellsNameCol.Width = 60;
            // 
            // dgvCellsValueCol
            // 
            this.dgvCellsValueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvCellsValueCol.DataPropertyName = "Value";
            this.dgvCellsValueCol.HeaderText = "Value";
            this.dgvCellsValueCol.MinimumWidth = 6;
            this.dgvCellsValueCol.Name = "dgvCellsValueCol";
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.processLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 604);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(977, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 9;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // processLabel
            // 
            this.processLabel.Name = "processLabel";
            this.processLabel.Size = new System.Drawing.Size(103, 17);
            this.processLabel.Text = "No active process.";
            // 
            // fbdExport
            // 
            this.fbdExport.Description = "Choose the folder to export parambnds to";
            this.fbdExport.RootFolder = System.Environment.SpecialFolder.MyComputer;
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(977, 626);
            this.Controls.Add(this.splitContainer2);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormMain";
            this.Text = "Yapped <version>";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvParams)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRows)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCells)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvParams;
        private System.Windows.Forms.DataGridView dgvRows;
        private System.Windows.Forms.DataGridView dgvCells;
        private System.Windows.Forms.OpenFileDialog ofdRegulation;
        private System.Windows.Forms.OpenFileDialog importedRegulation;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem restoreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exploreToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteRowToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvParamsParamCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvRowsIDCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvRowsNameCol;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCellsTypeCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCellsNameCol;
        private System.Windows.Forms.DataGridViewTextBoxColumn dgvCellsValueCol;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem findRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem gotoRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findFieldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findNextFieldToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel processLabel;
        private System.Windows.Forms.ToolStripMenuItem duplicateRowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog fbdExport;
        private System.Windows.Forms.ToolStripComboBox toolStripComboBoxGame;
        private System.Windows.Forms.ToolStripMenuItem WorkflowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fieldAdjusterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickfolderFieldValueMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickfolderDataMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickfolderLayoutMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quickfolderNamesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem quickfolderReferenceMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importDataMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportDataMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem massImportDataMenuItem;
        private System.Windows.Forms.ToolStripMenuItem massExportDataMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem importNamesMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportNamesMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.ToolStripMenuItem fieldExporterMenuItem;
        private System.Windows.Forms.ToolStripMenuItem valueReferenceFinderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rowReferenceFinderMenuItem;
        private System.Windows.Forms.ToolStripMenuItem affinityGeneratorMenuItem;
    }
}

