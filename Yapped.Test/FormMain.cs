using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using CellType = SoulsFormats.PARAM.CellType;

namespace Yapped.Test
{
    public partial class FormMain : Form
    {
        private static Properties.Settings settings = Properties.Settings.Default;

        private string regulationPath;
        private List<ParamFile> paramFiles;
        private Dictionary<string, PARAM.Layout> layouts;

        public FormMain()
        {
            InitializeComponent();

            dgvParams.AutoGenerateColumns = false;
            dgvRows.AutoGenerateColumns = false;
            dgvCells.AutoGenerateColumns = false;
            dgvLayout.AutoGenerateColumns = false;
            dgvLayoutTypeCol.DataSource = Enum.GetValues(typeof(CellType));

            layouts = new Dictionary<string, PARAM.Layout>();
            foreach (string path in Directory.GetFiles("Layouts", "*.xml"))
            {
                string format = Path.GetFileNameWithoutExtension(path);
                layouts[format] = PARAM.Layout.ReadXMLFile(path);
            }
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Text = "Yapped.Test " + Application.ProductVersion;
            Location = settings.WindowLocation;
            if (settings.WindowSize.Width >= MinimumSize.Width && settings.WindowSize.Height >= MinimumSize.Height)
                Size = settings.WindowSize;
            if (settings.WindowMaximized)
                WindowState = FormWindowState.Maximized;

            regulationPath = settings.RegulationPath;
            splitContainer1.SplitterDistance = settings.SplitterDistance1;
            splitContainer2.SplitterDistance = settings.SplitterDistance2;
            splitContainer3.SplitterDistance = settings.SplitterDistance3;

            LoadRegulation(regulationPath);
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            settings.WindowMaximized = WindowState == FormWindowState.Maximized;
            if (WindowState == FormWindowState.Normal)
            {
                settings.WindowLocation = Location;
                settings.WindowSize = Size;
            }
            else
            {
                settings.WindowLocation = RestoreBounds.Location;
                settings.WindowSize = RestoreBounds.Size;
            }

            settings.RegulationPath = regulationPath;
            settings.SplitterDistance1 = splitContainer1.SplitterDistance;
            settings.SplitterDistance2 = splitContainer2.SplitterDistance;
            settings.SplitterDistance3 = splitContainer3.SplitterDistance;
        }

        private void LoadRegulation(string path)
        {
            BND4 bnd;
            try
            {
                bnd = SFUtil.DecryptDS3Regulation(path);
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load regulation file:\r\n\r\n{path}\r\n\r\n{ex}");
                return;
            }

            paramFiles = new List<ParamFile>();
            foreach (BinderFile file in bnd.Files)
            {
                if (Path.GetExtension(file.Name) == ".param")
                {
                    try
                    {
                        PARAM param = PARAM.Read(file.Bytes);
                        paramFiles.Add(new ParamFile(Path.GetFileNameWithoutExtension(file.Name), param, layouts));
                    }
                    catch
                    {

                    }
                }
            }

            paramFiles.Sort((p1, p2) => p1.Name.CompareTo(p2.Name));
            dgvParams.DataSource = paramFiles;
        }

        private static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public class ParamFile
        {
            public string Name { get; set; }
            public PARAM.Layout Layout;
            public PARAM Param;
            public List<PARAM.Row> Rows { get; set; }

            public ParamFile(string name, PARAM param, Dictionary<string, PARAM.Layout> layouts)
            {
                Name = name;
                Param = param;
                string format = Param.ID;
                if (!layouts.ContainsKey(format))
                    layouts[format] = new PARAM.Layout();

                try
                {
                    Layout = layouts[format];
                    Param.SetLayout(Layout);
                    Rows = Param.Rows;
                }
                catch (Exception ex)
                {
                    Rows = new List<PARAM.Row>();
                    ShowError($"Error in layout {format}, please try again.\r\n\r\n{ex}");
                }
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private void dgvParams_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvParams.SelectedCells.Count > 0)
            {
                ParamFile paramFile = (ParamFile)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                dgvRows.DataSource = paramFile;
                dgvRows.DataMember = "Rows";

                dgvLayout.DataSource = layouts[paramFile.Param.ID];

                if (paramFile.Rows.Count == 0 || paramFile.Param.DetectedSize == paramFile.Layout.Size)
                    warningToolStripMenuItem.Visible = false;
                else
                {
                    warningToolStripMenuItem.Text = $"Layout size (0x{paramFile.Layout.Size:X}) does not equal param size (0x{paramFile.Param.DetectedSize:X})";
                    warningToolStripMenuItem.Visible = true;
                }
            }
        }

        private void dgvRows_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRows.SelectedCells.Count > 0)
            {
                PARAM.Row row = (PARAM.Row)dgvRows.SelectedCells[0].OwningRow.DataBoundItem;
                dgvCells.DataSource = row;
                dgvCells.DataMember = "Cells";
            }
        }

        private void dgvCells_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            PARAM.Cell cell = (PARAM.Cell)dgvCells.Rows[e.RowIndex].DataBoundItem;
            if (e.ColumnIndex == 1)
            {
                if (cell.Type == CellType.x8)
                    e.Value = $"0x{e.Value:X2}";
                if (cell.Type == CellType.x16)
                    e.Value = $"0x{e.Value:X4}";
                if (cell.Type == CellType.x32)
                    e.Value = $"0x{e.Value:X8}";
            }
        }

        private void dgvLayout_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            PARAM.Layout.Entry entry = (PARAM.Layout.Entry)dgvLayout.Rows[e.RowIndex].DataBoundItem;

            // Type
            if (e.ColumnIndex == 0)
            {

            }
            // Default
            else if (e.ColumnIndex == 3)
            {
                try
                {
                    if (entry.Type != CellType.dummy8)
                        PARAM.Layout.ParseParamValue(entry.Type, e.FormattedValue.ToString());
                }
                catch
                {
                    e.Cancel = true;
                    ShowError($"Invalid value for type \"{entry.Type}\".");
                }
            }
        }

        private void dgvLayout_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            PARAM.Layout.Entry entry = (PARAM.Layout.Entry)dgvLayout.Rows[e.RowIndex].DataBoundItem;
            DataGridViewCell cell = dgvLayout.Rows[e.RowIndex].Cells[e.ColumnIndex];

            // Size
            if (e.ColumnIndex == 2)
            {
                cell.ReadOnly = !entry.IsVariableSize;
            }
            // Default
            else if (e.ColumnIndex == 3)
            {
                cell.ReadOnly = entry.Type == CellType.dummy8;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                ofdRegulation.InitialDirectory = Path.GetDirectoryName(regulationPath);
            }
            catch { }

            if (ofdRegulation.ShowDialog() == DialogResult.OK)
            {
                regulationPath = ofdRegulation.FileName;
                LoadRegulation(regulationPath);
            }
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParamFile selectedParam = (ParamFile)dgvRows.DataSource;
            PARAM.Row selectedRow = (PARAM.Row)dgvCells.DataSource;

            LoadRegulation(regulationPath);

            dgvParams.ClearSelection();
            foreach (DataGridViewRow row in dgvParams.Rows)
                if (((ParamFile)row.DataBoundItem).Name == selectedParam.Name)
                    row.Cells[0].Selected = true;

            dgvRows.ClearSelection();
            foreach (DataGridViewRow row in dgvRows.Rows)
                if (((PARAM.Row)row.DataBoundItem).ID == selectedRow.ID)
                    row.Cells[0].Selected = true;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParamFile paramFile = (ParamFile)dgvRows.DataSource;
            string format = paramFile.Param.ID;
            Directory.CreateDirectory("Layouts");
            layouts[format].Write($"Layouts\\{format}.xml");
        }

        private void addEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var layout = (PARAM.Layout)dgvLayout.DataSource;
            var entry = new PARAM.Layout.Entry(CellType.u8, "name", 0);
            if (dgvLayout.SelectedCells.Count == 0)
                layout.Add(entry);
            else
                layout.Insert(dgvLayout.SelectedCells[dgvLayout.SelectedCells.Count - 1].RowIndex + 1, entry);

            dgvLayout.DataSource = null;
            dgvLayout.DataSource = layout;
        }

        private void deleteEntriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvLayout.SelectedCells.Count > 0)
            {
                var layout = (PARAM.Layout)dgvLayout.DataSource;
                int rangeStart = layout.Count;
                int rangeEnd = 0;
                foreach (DataGridViewCell cell in dgvLayout.SelectedCells)
                {
                    if (cell.RowIndex < rangeStart)
                        rangeStart = cell.RowIndex;
                    if (cell.RowIndex > rangeEnd)
                        rangeEnd = cell.RowIndex;
                }
                layout.RemoveRange(rangeStart, rangeEnd - rangeStart + 1);

                dgvLayout.DataSource = null;
                dgvLayout.DataSource = layout;
            }
        }
    }
}
