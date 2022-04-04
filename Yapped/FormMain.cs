using Semver;
using SoulsFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using CellType = SoulsFormats.PARAM.CellType;
using GameType = Yapped.GameMode.GameType;
using System.Globalization;

namespace Yapped
{
    public partial class FormMain : Form
    {
        private static Properties.Settings settings = Properties.Settings.Default;

        private string regulationPath;
        private IBinder regulation;
        private bool encrypted;
        private BindingSource rowSource;
        private Dictionary<string, (int Row, int Cell)> dgvIndices;
        private string lastFindRowPattern, lastFindFieldPattern;

        private string[] common_param_list = { "AtkParam_Npc", "AtkParam_Pc", "AttackElementCorrectParam", "BaseChrSelectMenuParam", "BehaviorParam", "BehaviorParam_PC", "BuddyParam", "BuddyStoneParam", "Bullet", "BulletCreateLimitParam", "CalcCorrectGraph", "CharaInitParam", "CharMakeMenuListItemParam", "EquipMtrlSetParam", "EquipParamAccessory", "EquipParamCustomWeapon", "EquipParamGem", "EquipParamGoods", "EquipParamProtector", "EquipParamWeapon", "GameAreaParam", "ItemLotParam_enemy", "ItemLotParam_map", "LockCamParam", "Magic", "NpcParam", "NpcThinkParam", "ObjActParam", "PhantomParam", "ReinforceParamProtector", "ReinforceParamWeapon", "ResistCorrectParam", "RideParam", "ShopLineupParam", "ShopLineupParam_Recipe", "SpEffectParam", "SpEffectSetParam", "SpEffectVfxParam", "SwordArtsParam", "TalkParam", "ThrowParam", "ToughnessParam" };

        public FormMain()
        {
            InitializeComponent();
            regulation = null;
            rowSource = new BindingSource();
            dgvIndices = new Dictionary<string, (int Row, int Cell)>();
            dgvRows.DataSource = rowSource;
            dgvParams.AutoGenerateColumns = false;
            dgvRows.AutoGenerateColumns = false;
            dgvCells.AutoGenerateColumns = false;
            lastFindRowPattern = "";

            // Enforce culture so exported CSV are consistent between users
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            Text = "Yapped - Rune Bear Edition";

            Location = settings.WindowLocation;
            if (settings.WindowSize.Width >= MinimumSize.Width && settings.WindowSize.Height >= MinimumSize.Height)
                Size = settings.WindowSize;
            if (settings.WindowMaximized)
                WindowState = FormWindowState.Maximized;

            toolStripComboBoxGame.ComboBox.DisplayMember = "Name";
            toolStripComboBoxGame.Items.AddRange(GameMode.Modes);
            var game = (GameType)Enum.Parse(typeof(GameType), settings.GameType);
            toolStripComboBoxGame.SelectedIndex = Array.FindIndex(GameMode.Modes, m => m.Game == game);
            if (toolStripComboBoxGame.SelectedIndex == -1)
                toolStripComboBoxGame.SelectedIndex = 0;

            // Settings
            regulationPath = settings.RegulationPath;
            splitContainer2.SplitterDistance = settings.SplitterDistance2;
            splitContainer1.SplitterDistance = settings.SplitterDistance1;

            LoadParams();

            foreach (Match match in Regex.Matches(settings.DGVIndices, @"[^,]+"))
            {
                string[] components = match.Value.Split(':');
                dgvIndices[components[0]] = (int.Parse(components[1]), int.Parse(components[2]));
            }

            if (settings.SelectedParam >= dgvParams.Rows.Count)
                settings.SelectedParam = 0;

            if (dgvParams.Rows.Count > 0)
            {
                dgvParams.ClearSelection();
                dgvParams.Rows[settings.SelectedParam].Selected = true;
                dgvParams.CurrentCell = dgvParams.SelectedCells[0];
            }

            ToggleCommonParamVisibility(true);

            processLabel.Text = "No active process.";
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

            settings.GameType = ((GameMode)toolStripComboBoxGame.SelectedItem).Game.ToString();
            settings.RegulationPath = regulationPath;
            settings.SplitterDistance2 = splitContainer2.SplitterDistance;
            settings.SplitterDistance1 = splitContainer1.SplitterDistance;

            if (dgvParams.SelectedCells.Count > 0)
                settings.SelectedParam = dgvParams.SelectedCells[0].RowIndex;

            // Force saving the dgv indices
            dgvParams.ClearSelection();

            var components = new List<string>();
            foreach (string key in dgvIndices.Keys)
                components.Add($"{key}:{dgvIndices[key].Row}:{dgvIndices[key].Cell}");
            settings.DGVIndices = string.Join(",", components);
        }

        private void LoadParams()
        {
            string resDir = GetResRoot();
            Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
            Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, true);

            if (result == null)
            {
                exportToolStripMenuItem.Enabled = false;
            }
            else
            {
                encrypted = result.Encrypted;
                regulation = result.ParamBND;
                exportToolStripMenuItem.Enabled = encrypted;

                foreach (ParamWrapper wrapper in result.ParamWrappers)
                {
                    if (!dgvIndices.ContainsKey(wrapper.Name))
                        dgvIndices[wrapper.Name] = (0, 0);
                }

                dgvParams.DataSource = result.ParamWrappers;

                foreach (DataGridViewRow row in dgvParams.Rows)
                {
                    var wrapper = (ParamWrapper)row.DataBoundItem;

                    if (wrapper.Error)
                        row.Cells[0].Style.BackColor = Color.Pink;
                }
            }
        }

        #region File Menu
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ofdRegulation.FileName = regulationPath;
            if (ofdRegulation.ShowDialog() == DialogResult.OK)
            {
                regulationPath = ofdRegulation.FileName;
                LoadParams();
            }

            ToggleCommonParamVisibility(false);
        }

        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveParams(".bak");
        }

        private void SaveParams(String backup_format)
        {
            foreach (BinderFile file in regulation.Files)
            {
                foreach (DataGridViewRow paramRow in dgvParams.Rows)
                {
                    ParamWrapper paramFile = (ParamWrapper)paramRow.DataBoundItem;

                    if (Path.GetFileNameWithoutExtension(file.Name) == paramFile.Name)
                    {
                        if (settings.ParamsToIgnoreDuringSave != "")
                        {
                            String[] param_name_list = settings.ParamsToIgnoreDuringSave.Split(settings.ExportDelimiter.ToCharArray());

                            foreach (String param_name in param_name_list)
                            {
                                if (paramFile.Name != param_name)
                                {
                                    file.Bytes = paramFile.Param.Write();
                                }
                            }
                        }
                        else
                        {
                            file.Bytes = paramFile.Param.Write();
                        }
                    }
                }
            }

            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            if (!File.Exists(regulationPath + backup_format))
                File.Copy(regulationPath, regulationPath + backup_format);

            // Save with default encryption unless Save without Encryption is toggled.
            if (encrypted && !Properties.Settings.Default.SaveWithoutEncryption)
            {
                if (gameMode.Game == GameType.DarkSouls2)
                    Util.EncryptDS2Regulation(regulationPath, regulation as BND4);
                else if (gameMode.Game == GameType.DarkSouls3)
                    SFUtil.EncryptDS3Regulation(regulationPath, regulation as BND4);
                else if (gameMode.Game == GameType.EldenRing)
                    SFUtil.EncryptERRegulation(regulationPath, regulation as BND4);
                else
                    Util.ShowError("Encryption is not valid for this file.");
            }
            else
            {
                if (regulation is BND3 bnd3)
                    bnd3.Write(regulationPath);
                else if (regulation is BND4 bnd4)
                {
                    bnd4.Write(regulationPath);
                }
            }
            SystemSounds.Asterisk.Play();
        }

        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(regulationPath + ".bak"))
            {
                DialogResult choice = MessageBox.Show("Are you sure you want to restore the backup?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (choice == DialogResult.Yes)
                {
                    try
                    {
                        File.Copy(regulationPath + ".bak", regulationPath, true);
                        LoadParams();
                        SystemSounds.Asterisk.Play();
                    }
                    catch (Exception ex)
                    {
                        Util.ShowError($"Failed to restore backup\r\n\r\n{regulationPath}.bak\r\n\r\n{ex}");
                    }
                }
            }
            else
            {
                Util.ShowError($"There is no backup to restore at:\r\n\r\n{regulationPath}.bak");
            }
        }

        private void ExploreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.GetDirectoryName(regulationPath));
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var bnd4 = regulation as BND4;
            fbdExport.SelectedPath = Path.GetDirectoryName(regulationPath);
            if (fbdExport.ShowDialog() == DialogResult.OK)
            {
                BND4 paramBND = new BND4
                {
                    BigEndian = false,
                    Compression = DCX.Type.SekiroDFLT,
                    Extended = 0x04,
                    Flag1 = false,
                    Flag2 = false,
                    Format = Binder.Format.x74,
                    Timestamp = bnd4.Timestamp,
                    Unicode = true,
                    Files = regulation.Files.Where(f => f.Name.EndsWith(".param")).ToList()
                };

                BND4 stayBND = new BND4
                {
                    BigEndian = false,
                    Compression = DCX.Type.SekiroDFLT,
                    Extended = 0x04,
                    Flag1 = false,
                    Flag2 = false,
                    Format = Binder.Format.x74,
                    Timestamp = bnd4.Timestamp,
                    Unicode = true,
                    Files = regulation.Files.Where(f => f.Name.EndsWith(".stayparam")).ToList()
                };

                string dir = fbdExport.SelectedPath;
                try
                {
                    paramBND.Write($@"{dir}\gameparam.parambnd.dcx");
                    stayBND.Write($@"{dir}\stayparam.parambnd.dcx");
                }
                catch (Exception ex)
                {
                    Util.ShowError($"Failed to write exported parambnds.\r\n\r\n{ex}");
                }
            }
        }
        #endregion

        #region Edit Menu
        private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateRow("Add a new row...");
        }

        private void DuplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRows.SelectedCells.Count == 0)
            {
                Util.ShowError("You can't duplicate a row without one selected!");
                return;
            }

            int index = dgvRows.SelectedCells[0].RowIndex;
            ParamWrapper wrapper = (ParamWrapper)rowSource.DataSource;
            PARAM.Row oldRow = wrapper.Rows[index];
            PARAM.Row newRow;
            if ((newRow = CreateRow("Duplicate a row...")) != null)
            {
                for (int i = 0; i < oldRow.Cells.Count; i++)
                {
                    newRow.Cells[i].Value = oldRow.Cells[i].Value;
                }
            }
        }

        private PARAM.Row CreateRow(string prompt)
        {
            if (rowSource.DataSource == null)
            {
                Util.ShowError("You can't create a row with no param selected!");
                return null;
            }

            PARAM.Row result = null;
            var newRowForm = new FormNewRow(prompt);
            if (newRowForm.ShowDialog() == DialogResult.OK)
            {
                long id = newRowForm.ResultID;
                string name = newRowForm.ResultName;
                ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;
                if (paramWrapper.Rows.Any(row => row.ID == id))
                {
                    Util.ShowError($"A row with this ID already exists: {id}");
                }
                else
                {
                    result = new PARAM.Row(id, name, paramWrapper.Layout);
                    rowSource.Add(result);
                    paramWrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                    int index = paramWrapper.Rows.FindIndex(row => ReferenceEquals(row, result));
                    int displayedRows = dgvRows.DisplayedRowCount(false);
                    dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                    dgvRows.ClearSelection();
                    dgvRows.Rows[index].Selected = true;
                    dgvRows.Refresh();
                }
            }
            return result;
        }

        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (dgvRows.SelectedCells.Count > 0)
            {
                DialogResult choice = DialogResult.Yes;
                if (settings.VerifyRowDeletion == true)
                    choice = MessageBox.Show("Are you sure you want to delete this row?",
                        "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (choice == DialogResult.Yes)
                {
                    int rowIndex = dgvRows.SelectedCells[0].RowIndex;
                    rowSource.RemoveAt(rowIndex);

                    // If you remove a row it automatically selects the next one, but if you remove the last row
                    // it doesn't automatically select the previous one
                    if (rowIndex == dgvRows.RowCount)
                    {
                        if (dgvRows.RowCount > 0)
                            dgvRows.Rows[dgvRows.RowCount - 1].Selected = true;
                        else
                            dgvCells.DataSource = null;
                    }
                }
            }
        }

        private void ImportNamesMenuItem_Click(object sender, EventArgs e)
        {
            bool replace = MessageBox.Show("If a row already has a name, would you like to skip it?\r\n" +
                "Click Yes to skip existing names.\r\nClick No to replace existing names.",
                "Importing Names", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            string namesDir = $@"{GetResRoot()}\Names";

            if (settings.ProjectName != "")
            {
                namesDir = namesDir + "\\" + settings.ProjectName;

                bool exists = System.IO.Directory.Exists(namesDir);

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(namesDir);
                }
            }

            List<ParamWrapper> paramFiles = (List<ParamWrapper>)dgvParams.DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                if (File.Exists($@"{namesDir}\{paramFile.Name}.txt"))
                {
                    var names = new Dictionary<long, string>();
                    string nameStr = File.ReadAllText($@"{namesDir}\{paramFile.Name}.txt");
                    foreach (string line in Regex.Split(nameStr, @"\s*[\r\n]+\s*"))
                    {
                        if (line.Length > 0)
                        {
                            Match match = Regex.Match(line, @"^(\d+) (.+)$");
                            long id = long.Parse(match.Groups[1].Value);
                            string name = match.Groups[2].Value;
                            names[id] = name;
                        }
                    }

                    foreach (PARAM.Row row in paramFile.Param.Rows)
                    {
                        if (names.ContainsKey(row.ID))
                        {
                            if (replace || row.Name == null || row.Name == "")
                                row.Name = names[row.ID];
                        }
                    }
                }
            }

            dgvRows.Refresh();
        }

        private void ExportNamesMenuItem_Click(object sender, EventArgs e)
        {
            string namesDir = $@"{GetResRoot()}\Names";

            if (settings.ProjectName != "")
            {
                namesDir = namesDir + "\\" + settings.ProjectName;

                bool exists = System.IO.Directory.Exists(namesDir);

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(namesDir);
                }
            }

            List<ParamWrapper> paramFiles = (List<ParamWrapper>)dgvParams.DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                StringBuilder sb = new StringBuilder();
                foreach (PARAM.Row row in paramFile.Param.Rows)
                {
                    string name = (row.Name ?? "").Trim();
                    if (name != "")
                    {
                        sb.AppendLine($"{row.ID} {name}");
                    }
                }

                try
                {
                    File.WriteAllText($@"{namesDir}\{paramFile.Name}.txt", sb.ToString());
                }
                catch (Exception ex)
                {
                    Util.ShowError($"Failed to write name file: {paramFile.Name}.txt\r\n\r\n{ex}");
                    break;
                }
            }

            if (!Properties.Settings.Default.ShowConfirmationMessages)
                MessageBox.Show("Names exported!", "Export Names", MessageBoxButtons.OK);
        }

        private void ImportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow paramRow = dgvParams.CurrentRow;
            ParamWrapper wrapper = ((ParamWrapper)paramRow.DataBoundItem);

            ImportParamData(wrapper, false);

            // Update visuals
            dgvRows.Refresh();
            dgvCells.Refresh();

            processLabel.Text = "No active process.";
        }

        private void ExportDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataGridViewRow paramRow = dgvParams.CurrentRow;
            ParamWrapper wrapper = ((ParamWrapper)paramRow.DataBoundItem);

            ExportParamData(wrapper, false);

            processLabel.Text = "No active process.";
        }

        private void FindRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var findForm = new FormFind("Find row with name...");
            if (findForm.ShowDialog() == DialogResult.OK)
            {
                FindRow(findForm.ResultPattern);
            }
        }

        private void FindNextRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindRow(lastFindRowPattern);
        }

        private void FindRow(string pattern)
        {
            if (rowSource.DataSource == null)
            {
                Util.ShowError("You can't search for a row when there are no rows!");
                return;
            }

            int startIndex = dgvRows.SelectedCells.Count > 0 ? dgvRows.SelectedCells[0].RowIndex + 1 : 0;
            List<PARAM.Row> rows = ((ParamWrapper)rowSource.DataSource).Rows;
            int index = -1;

            for (int i = 0; i < rows.Count; i++)
            {
                if ((rows[(startIndex + i) % rows.Count].Name ?? "").ToLower().Contains(pattern.ToLower()))
                {
                    index = (startIndex + i) % rows.Count;
                    break;
                }
            }

            if (index != -1)
            {
                int displayedRows = dgvRows.DisplayedRowCount(false);
                dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                dgvRows.ClearSelection();
                dgvRows.Rows[index].Selected = true;
                lastFindRowPattern = pattern;
            }
            else
            {
                Util.ShowError($"No row found matching: {pattern}");
            }
        }

        private void GotoRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var gotoForm = new FormGoto();
            if (gotoForm.ShowDialog() == DialogResult.OK)
            {
                if (rowSource.DataSource == null)
                {
                    Util.ShowError("You can't goto a row when there are no rows!");
                    return;
                }

                long id = gotoForm.ResultID;
                List<PARAM.Row> rows = ((ParamWrapper)rowSource.DataSource).Rows;
                int index = rows.FindIndex(row => row.ID == id);

                if (index != -1)
                {
                    int displayedRows = dgvRows.DisplayedRowCount(false);
                    dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                    dgvRows.ClearSelection();
                    dgvRows.Rows[index].Selected = true;
                }
                else
                {
                    Util.ShowError($"Row ID not found: {id}");
                }
            }
        }

        private void FindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var findForm = new FormFind("Find field with name...");
            if (findForm.ShowDialog() == DialogResult.OK)
            {
                FindField(findForm.ResultPattern);
            }
        }

        private void FindNextFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FindField(lastFindFieldPattern);
        }

        private void FindField(string pattern)
        {
            if (dgvCells.DataSource == null)
            {
                Util.ShowError("You can't search for a field when there are no fields!");
                return;
            }

            int startIndex = dgvCells.SelectedCells.Count > 0 ? dgvCells.SelectedCells[0].RowIndex + 1 : 0;
            var cells = (PARAM.Cell[])dgvCells.DataSource;
            int index = -1;

            for (int i = 0; i < cells.Length; i++)
            {
                if ((cells[(startIndex + i) % cells.Length].Name ?? "").ToLower().Contains(pattern.ToLower()))
                {
                    index = (startIndex + i) % cells.Length;
                    break;
                }
            }

            if (index != -1)
            {
                int displayedRows = dgvCells.DisplayedRowCount(false);
                dgvCells.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                dgvCells.ClearSelection();
                dgvCells.Rows[index].Selected = true;
                lastFindFieldPattern = pattern;
            }
            else
            {
                Util.ShowError($"No field found matching: {pattern}");
            }
        }

        // Field Adjuster
        private void fieldAdjusterMenuItem_Click(object sender, EventArgs e)
        {
            var field_adjuster = new FormFieldAdjuster();

            if (field_adjuster.ShowDialog() == DialogResult.OK)
            {
                processLabel.Text = "Field Adjuster in operation.";

                // Save current state before applying the edits
                SaveParams(".bak");

                ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;

                string adjustment_file_path = $@"{GetResRoot()}\Logs\FieldAdjustment_" + paramWrapper.Name + ".log";
                char delimiter = settings.ExportDelimiter.ToCharArray()[0];

                // Selection
                string field_match = Properties.Settings.Default.FieldAdjuster_FieldMatch;
                string row_range = Properties.Settings.Default.FieldAdjuster_RowRange;
                string row_partial_match = Properties.Settings.Default.FieldAdjuster_RowPartialMatch;
                string field_minimum = Properties.Settings.Default.FieldAdjuster_FieldMinimum;
                string field_maximum = Properties.Settings.Default.FieldAdjuster_FieldMaximum;
                string field_exclusions = Properties.Settings.Default.FieldAdjuster_FieldExclusion;
                string field_inclusions = Properties.Settings.Default.FieldAdjuster_FieldInclusion;

                // Output
                string field_formula = Properties.Settings.Default.FieldAdjuster_Formula;
                string output_max = Properties.Settings.Default.FieldAdjuster_ValueMax;
                string output_min = Properties.Settings.Default.FieldAdjuster_ValueMin;

                // No field specified
                if (field_match == "")
                {
                    MessageBox.Show("You did not specify a target field.", "Field Adjuster", MessageBoxButtons.OK);
                    return;
                }

                // No field change specified
                if (field_formula == "")
                {
                    MessageBox.Show("You did not specify a field formula.", "Field Adjuster", MessageBoxButtons.OK);
                    return;
                }

                // Order of Checks:
                // 1: Row Range
                // 2: Partial Match
                // 3: Field Minimum
                // 4: Field Maximum
                // 5: Field Inclusion
                // 6: Field Exclusion

                StreamWriter output_file = new StreamWriter(adjustment_file_path);

                // Rows
                foreach (PARAM.Row row in paramWrapper.Rows)
                {
                    // Row Range
                    if (row_range != "")
                    {
                        try
                        {
                            string[] row_range_array = row_range.Split(delimiter);

                            if(row_range_array.Length != 2)
                            {
                                MessageBox.Show("Row range invalid.", "Field Adjuster", MessageBoxButtons.OK);
                                output_file.Close();
                                return;
                            }
                            
                            // Has one count, i.e is a valid row ID
                            if (Regex.Matches(row_range_array[0], ".*[0-9].*").Count == 1)
                            {
                                // Row ID is below Row Range Start, skip it
                                if (int.Parse(row.ID.ToString()) < int.Parse(row_range_array[0].ToString()))
                                    continue;
                            }

                            // Has one count, i.e is a valid row ID
                            if (Regex.Matches(row_range_array[1], ".*[0-9].*").Count == 1)
                            {
                                // Row ID is above Row Range End, skip it
                                if (int.Parse(row.ID.ToString()) > int.Parse(row_range_array[1].ToString()))
                                    continue;
                            }
                        }
                        catch
                        {
                            MessageBox.Show("Row range invalid.", "Field Adjuster", MessageBoxButtons.OK);
                            output_file.Close();
                            return;
                        }
                    }

                    // Partial Match
                    if(row_partial_match != "")
                    {
                        var substring_length = row_partial_match.Length;

                        var row_id_string = row.ID.ToString();
                        var row_substring = row_id_string.Substring(row_id_string.Length - substring_length);

                        // If row ID does not contain the substring at the end, skip it
                        if (row_substring != row_partial_match)
                            continue;
                    }

                    // Cells
                    foreach (PARAM.Cell cell in row.Cells)
                    {
                        PARAM.CellType type = cell.Type;

                        string name = cell.Name;
                        string value = cell.Value.ToString();
                        bool isMatchedField = false;
                        bool isBoolType = false;

                        if(type == CellType.b8 || type == CellType.b16 || type == CellType.b32)
                            isBoolType = true;

                        // Check if field name matches
                        if (type != PARAM.CellType.dummy8)
                        {
                            if (field_match == name)
                            {
                                isMatchedField = true;

                                // Minimum
                                if (field_minimum != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        if (Convert.ToSByte(value) < Convert.ToSByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        if (Convert.ToByte(value) < Convert.ToByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        if (Convert.ToInt16(value) < Convert.ToInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        if (Convert.ToUInt16(value) < Convert.ToUInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        if (Convert.ToInt32(value) < Convert.ToInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        if (Convert.ToUInt32(value) < Convert.ToUInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        if (Convert.ToSingle(value) < Convert.ToSingle(field_minimum))
                                            isMatchedField = false;
                                    }
                                }

                                // Maximum
                                if (field_maximum != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        if (Convert.ToSByte(value) > Convert.ToSByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        if (Convert.ToByte(value) > Convert.ToByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        if (Convert.ToInt16(value) > Convert.ToInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        if (Convert.ToUInt16(value) > Convert.ToUInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        if (Convert.ToInt32(value) > Convert.ToInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        if (Convert.ToUInt32(value) > Convert.ToUInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        if (Convert.ToSingle(value) > Convert.ToSingle(field_maximum))
                                            isMatchedField = false;
                                    }
                                }

                                // Inclusion
                                if (field_inclusions != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        Single[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Single.Parse(s));

                                        foreach (Single array_value in temp_array)
                                        {
                                            if (Convert.ToSingle(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                }

                                // Check exclusions
                                if (field_exclusions != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        Single[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Single.Parse(s));

                                        foreach (Single array_value in temp_array)
                                        {
                                            if (Convert.ToSingle(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                }

                                if (isMatchedField)
                                {
                                    output_file.WriteLine("Row: " + row.ID.ToString());
                                    output_file.WriteLine("- Field " + cell.Name.ToString());
                                    output_file.WriteLine("- Old Value " + cell.Value.ToString());

                                    if (type == PARAM.CellType.b8 || type == PARAM.CellType.b16 || type == PARAM.CellType.b32)
                                    {

                                        // Only write to the bool if the string is True or False
                                        if(field_formula == "True" || field_formula == "False")
                                            cell.Value = Boolean.Parse(field_formula);
                                    }
                                    else
                                    {
                                        double result = 0;

                                        // Only use the formula stuff if X is present
                                        if (field_formula.Contains("x"))
                                        {
                                            // Replace x with th e current cell value
                                            string cell_formula = field_formula.Replace("x", cell.Value.ToString());
                                            // Evaluate the formula
                                            StringToFormula stf = new StringToFormula();
                                            result = stf.Eval(cell_formula);

                                            if (output_max != "")
                                            {
                                                if (result > double.Parse(output_max))
                                                {
                                                    result = double.Parse(output_max);
                                                }
                                            }

                                            if (output_min != "")
                                            {
                                                if (result < double.Parse(output_min))
                                                {
                                                    result = double.Parse(output_min);
                                                }
                                            }
                                        }
                                        // Otherwise just set the value to the entered value
                                        else
                                        {
                                            result = Double.Parse(field_formula);
                                        }

                                        if (type == CellType.s8)
                                            cell.Value = Convert.ToSByte(result);
                                        else if (type == CellType.u8 || type == CellType.x8)
                                            cell.Value = Convert.ToByte(result);
                                        else if (type == CellType.s16)
                                            cell.Value = Convert.ToInt16(result);
                                        else if (type == CellType.u16 || type == CellType.x16)
                                            cell.Value = Convert.ToUInt16(result);
                                        else if (type == CellType.s32)
                                            cell.Value = Convert.ToInt32(result);
                                        else if (type == CellType.u32 || type == CellType.x32)
                                            cell.Value = Convert.ToUInt32(result);
                                        else if (type == CellType.f32)
                                            cell.Value = Convert.ToSingle(result);
                                    }

                                    output_file.WriteLine("- New Value " + cell.Value.ToString());
                                    output_file.WriteLine("");
                                }
                            }
                        }
                    }
                }

                output_file.Close();

                if (Properties.Settings.Default.UseTextEditor && Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + adjustment_file_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }

                }

                if (!Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Field Adjustment complete.", "Field Adjuster", MessageBoxButtons.OK);

                processLabel.Text = "No active process.";
            }
        }

        /// Row Reference Finder
        private void rowReferenceFinderMenuItem_Click(object sender, EventArgs e)
        {
            var newFormReferenceFinder = new FormReferenceFinder();
            string reference_text;

            if (newFormReferenceFinder.ShowDialog() == DialogResult.OK)
            {
                processLabel.Text = "Row Reference Finder in operation.";

                reference_text = newFormReferenceFinder.GetReferenceText();

                if (reference_text == "")
                {
                    MessageBox.Show("You did not specify a value.", "Reference Finder", MessageBoxButtons.OK);
                    return;
                }

                string reference_file_path = $@"{GetResRoot()}\Logs\RowReference.log";
                string resDir = GetResRoot();
                Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
                Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
                var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
                LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, true);

                StreamWriter output_file = new StreamWriter(reference_file_path);

                foreach (var param in result.ParamWrappers)
                {
                    foreach(var row in param.Rows)
                    {
                        foreach (PARAM.Cell cell in row.Cells)
                        {
                            // Does cell value match reference_text, if so we have a reference to output
                            if (CheckFieldReference(reference_text, cell.Value.ToString()))
                            {
                                output_file.WriteLine(param.Name.ToString()); // Param Name

                                if (row.Name != null)
                                    output_file.WriteLine("  - Row Name: " + row.Name.ToString()); // Row Name

                                if (cell.Name != null)
                                    output_file.WriteLine("  - Cell Name: " + cell.Name.ToString()); // Cell Name

                                output_file.WriteLine("  - Row ID: " + row.ID.ToString()); // Row ID of Cell
                                output_file.WriteLine("  - Reference Value: " + reference_text); // Reference Text

                                output_file.WriteLine("");
                            }
                        }
                    }
                }

                output_file.Close();

                if (Properties.Settings.Default.UseTextEditor && Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + reference_file_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }

                }

                if (!Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("References exported to " + reference_file_path, "Reference Finder", MessageBoxButtons.OK);

                processLabel.Text = "No active process.";
            }
        }
        
        // Value Reference Finder
        private void valueReferenceFinderMenuITem_Click(object sender, EventArgs e)
        {
            var newFormReferenceFinder = new FormReferenceFinder();
            string reference_text;

            if (newFormReferenceFinder.ShowDialog() == DialogResult.OK)
            {
                processLabel.Text = "Value Reference Finder in operation.";

                reference_text = newFormReferenceFinder.GetReferenceText();

                if (reference_text == "")
                {
                    MessageBox.Show("You did not specify a value.", "Reference Finder", MessageBoxButtons.OK);
                    return;
                }

                string reference_file_path = $@"{GetResRoot()}\Logs\ValueReference.log";
                string resDir = GetResRoot();
                Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
                Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
                var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
                LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, true);

                StreamWriter output_file = new StreamWriter(reference_file_path);

                foreach (var param in result.ParamWrappers)
                {
                    foreach (var row in param.Rows)
                    {
                        // Does row ID match reference_text, if so we have a reference to output
                        if (CheckFieldReference(reference_text, row.ID.ToString()))
                        {
                            output_file.WriteLine(param.Name.ToString()); // Param Name

                            if (row.Name != null)
                                output_file.WriteLine("  - Row Name: " + row.Name.ToString()); // Row Name

                            output_file.WriteLine("  - Row ID: " + row.ID.ToString()); // Row ID
                            output_file.WriteLine("  - Reference Value: " + reference_text); // Reference Text

                            output_file.WriteLine("");
                        }
                    }
                }

                output_file.Close();

                if (Properties.Settings.Default.UseTextEditor && Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + reference_file_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }

                }

                if (!Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("References exported to " + reference_file_path, "Reference Finder", MessageBoxButtons.OK);

                processLabel.Text = "No active process.";
            }
        }

        private bool CheckFieldReference(string value_1, string value_2)
        {
            int number;

            // Field Value
            bool field_value_success = Int32.TryParse(value_1, out number);

            if (field_value_success)
            {
                // Row ID
                bool row_id_success = Int32.TryParse(value_2, out number);

                if (row_id_success)
                {
                    if (Int32.Parse(value_1).Equals(Int32.Parse(value_2)))
                        return true;
                    else
                        return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        // Field Exporter
        private void fieldExporterMenuItem_Click(object sender, EventArgs e)
        {
            var field_exporter = new FormFieldExporter();

            if (field_exporter.ShowDialog() == DialogResult.OK)
            {
                processLabel.Text = "Field Exporter in operation.";

                ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;

                string field_data_path = $@"{GetResRoot()}\Logs\FieldValue_" + paramWrapper.Name + ".log";
                char delimiter = settings.ExportDelimiter.ToCharArray()[0];

                string field_match = Properties.Settings.Default.FieldExporter_FieldMatch;
                string field_minimum = Properties.Settings.Default.FieldExporter_FieldMinimum;
                string field_maximum = Properties.Settings.Default.FieldExporter_FieldMaximum;
                string field_exclusions = Properties.Settings.Default.FieldExporter_FieldExclusion;
                string field_inclusions = Properties.Settings.Default.FieldExporter_FieldInclusion;

                // No field specified
                if (field_match == "")
                {
                    MessageBox.Show("You did not specify any field names.", "Field Exporter", MessageBoxButtons.OK);
                    return;
                }
                
                // Create file
                if (File.Exists(field_data_path))
                {
                    string message = $@"{field_data_path} exists. Overwrite?";
                    DialogResult answer = MessageBox.Show(message, "Export Field Values", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                    if (answer == DialogResult.No)
                        return;
                }
                else
                {
                    // Add file if it doesn't yet exist
                    if (!System.IO.File.Exists(field_data_path))
                    {
                        using (System.IO.FileStream fs = System.IO.File.Create(field_data_path))
                        {
                            for (byte i = 0; i < 100; i++)
                            {
                                fs.WriteByte(i);
                            }
                        }
                    }
                }

                // Write field value output
                StreamWriter output_file = new StreamWriter(field_data_path);

                string header_line = "";

                // Header
                if (settings.IncludeHeaderInCSV)
                {
                    header_line = "Row ID" + settings.ExportDelimiter + "Row Name" + settings.ExportDelimiter + field_match;

                    output_file.WriteLine(header_line);
                }

                // Order of Checks:
                // 1. Row ID Minimum/Maximum
                // 2. Field Name
                // 3. Field Inclusions
                // 4. Field Exclusions

                List<String> unique_list = new List<String>();

                // Field Data
                foreach (PARAM.Row row in paramWrapper.Rows)
                {
                    string row_line = row.ID + settings.ExportDelimiter;

                    // Add row name
                    if (settings.IncludeRowNameInCSV == true)
                    {
                        string row_name = row.Name;

                        row_line = row_line + row_name + settings.ExportDelimiter;
                    }

                    bool isValidRow = false;

                    foreach (PARAM.Cell cell in row.Cells)
                    {
                        PARAM.CellType type = cell.Type;

                        string name = cell.Name;
                        string value = cell.Value.ToString();
                        bool isMatchedField = false;
                        bool isBoolType = false;

                        if (type == CellType.b8 || type == CellType.b16 || type == CellType.b32)
                            isBoolType = true;

                        // Check if field name matches
                        if (type != PARAM.CellType.dummy8)
                        {
                            if (field_match == name)
                            {
                                isMatchedField = true;

                                // Minimum
                                if (field_minimum != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        if (Convert.ToSByte(value) < Convert.ToSByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        if (Convert.ToByte(value) < Convert.ToByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        if (Convert.ToInt16(value) < Convert.ToInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        if (Convert.ToUInt16(value) < Convert.ToUInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        if (Convert.ToInt32(value) < Convert.ToInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        if (Convert.ToUInt32(value) < Convert.ToUInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        if (Convert.ToSingle(value) < Convert.ToSingle(field_minimum))
                                            isMatchedField = false;
                                    }
                                }

                                // Maximum
                                if (field_maximum != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        if (Convert.ToSByte(value) > Convert.ToSByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        if (Convert.ToByte(value) > Convert.ToByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        if (Convert.ToInt16(value) > Convert.ToInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        if (Convert.ToUInt16(value) > Convert.ToUInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        if (Convert.ToInt32(value) > Convert.ToInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        if (Convert.ToUInt32(value) > Convert.ToUInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        if (Convert.ToSingle(value) > Convert.ToSingle(field_maximum))
                                            isMatchedField = false;
                                    }
                                }

                                // Inclusion
                                if (field_inclusions != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        Single[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Single.Parse(s));

                                        foreach (Single array_value in temp_array)
                                        {
                                            if (Convert.ToSingle(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                }

                                // Check exclusions
                                if (field_exclusions != "" && !isBoolType)
                                {
                                    if (type == CellType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u8 || type == CellType.x8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u16 || type == CellType.x16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.u32 || type == CellType.x32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == CellType.f32)
                                    {
                                        Single[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Single.Parse(s));

                                        foreach (Single array_value in temp_array)
                                        {
                                            if (Convert.ToSingle(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                }

                                if (settings.ExportUniqueOnly)
                                {
                                    Console.WriteLine("Current value: " + value);

                                    if(unique_list.Contains(value))
                                    {
                                        Console.WriteLine("Unqiue value matches current, invalidating field addition");
                                        isMatchedField = false;
                                    }

                                    if (!unique_list.Contains(value))
                                    {
                                        Console.WriteLine("Added new value to Unique List");
                                        unique_list.Add(value);
                                    }
                                }

                                if (isMatchedField)
                                {
                                    isValidRow = true;

                                    if (type == CellType.s8)
                                        row_line = row_line + Convert.ToSByte(value);
                                    else if (type == CellType.u8 || type == CellType.x8)
                                        row_line = row_line + Convert.ToByte(value);
                                    else if (type == CellType.s16)
                                        row_line = row_line + Convert.ToInt16(value);
                                    else if (type == CellType.u16 || type == CellType.x16)
                                        row_line = row_line + Convert.ToUInt16(value);
                                    else if (type == CellType.s32)
                                        row_line = row_line + Convert.ToInt32(value);
                                    else if (type == CellType.u32 || type == CellType.x32)
                                        row_line = row_line + Convert.ToUInt32(value);
                                    else if (type == CellType.f32)
                                        row_line = row_line + Convert.ToSingle(value);
                                    else if (type == PARAM.CellType.b8 || type == PARAM.CellType.b16 || type == PARAM.CellType.b32)
                                        row_line = row_line + Convert.ToBoolean(value);
                                    else if (type == CellType.fixstr || type == CellType.fixstrW)
                                        row_line = row_line + value;
                                }
                            }
                        }
                    }

                    // Only write if the row actually results in a valid field output 
                    if(isValidRow)
                        output_file.WriteLine(row_line);
                }

                output_file.Close();

                if(Properties.Settings.Default.UseTextEditor && Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + field_data_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }
                    
                }

                if (!Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Field values exported to " + field_data_path, "Field Exporter", MessageBoxButtons.OK);

                processLabel.Text = "No active process.";
            }
        }
        #endregion
        
        #region dgvParams
        private void DgvParams_SelectionChanged(object sender, EventArgs e)
        {
            if (rowSource.DataSource != null)
            {
                ParamWrapper paramFile = (ParamWrapper)rowSource.DataSource;
                (int Row, int Cell) indices = (0, 0);

                if (dgvRows.SelectedCells.Count > 0)
                    indices.Row = dgvRows.SelectedCells[0].RowIndex;
                else if (dgvRows.FirstDisplayedScrollingRowIndex >= 0)
                    indices.Row = dgvRows.FirstDisplayedScrollingRowIndex;

                if (dgvCells.FirstDisplayedScrollingRowIndex >= 0)
                    indices.Cell = dgvCells.FirstDisplayedScrollingRowIndex;

                dgvIndices[paramFile.Name] = indices;
            }

            rowSource.DataSource = null;
            dgvCells.DataSource = null;
            if (dgvParams.SelectedCells.Count > 0)
            {
                ParamWrapper paramFile = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                // Yes, I need to set this again every time because it gets cleared out when you null the DataSource for some stupid reason
                rowSource.DataMember = "Rows";
                rowSource.DataSource = paramFile;
                (int Row, int Cell) indices = dgvIndices[paramFile.Name];

                if (indices.Row >= dgvRows.RowCount)
                    indices.Row = dgvRows.RowCount - 1;

                if (indices.Row < 0)
                    indices.Row = 0;

                dgvIndices[paramFile.Name] = indices;
                dgvRows.ClearSelection();
                if (dgvRows.RowCount > 0)
                {
                    dgvRows.FirstDisplayedScrollingRowIndex = indices.Row;
                    dgvRows.Rows[indices.Row].Selected = true;
                }
            }
        }

        private void DgvParams_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var paramWrapper = (ParamWrapper)dgvParams.Rows[e.RowIndex].DataBoundItem;
                e.ToolTipText = paramWrapper.Description;
            }
        }
        #endregion

        #region dgvRows
        private void DgvRows_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvRows.SelectedCells.Count > 0)
            {
                ParamWrapper paramFile = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                (int Row, int Cell) indices = dgvIndices[paramFile.Name];
                if (dgvCells.FirstDisplayedScrollingRowIndex >= 0)
                    indices.Cell = dgvCells.FirstDisplayedScrollingRowIndex;

                PARAM.Row row = (PARAM.Row)dgvRows.SelectedCells[0].OwningRow.DataBoundItem;
                dgvCells.DataSource = row.Cells.Where(cell => cell.Type != CellType.dummy8).ToArray();

                if (indices.Cell >= dgvCells.RowCount)
                    indices.Cell = dgvCells.RowCount - 1;

                if (indices.Cell < 0)
                    indices.Cell = 0;

                dgvIndices[paramFile.Name] = indices;
                if (dgvCells.RowCount > 0)
                    dgvCells.FirstDisplayedScrollingRowIndex = indices.Cell;
            }
        }

        private void DgvRows_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // ID
            if (e.ColumnIndex == 0)
            {
                bool parsed = int.TryParse((string)e.FormattedValue, out int value);
                if (!parsed || value < 0)
                {
                    Util.ShowError("Row ID must be a positive integer.\r\nEnter a valid number or press Esc to cancel.");
                    e.Cancel = true;
                }
            }
        }
        #endregion

        #region dgvCells
        private void DgvCells_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgvCells.Rows)
            {
                var cell = (PARAM.Cell)row.DataBoundItem;
                if (cell.Enum != null)
                {
                    var paramWrapper = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                    PARAM.Layout layout = paramWrapper.Layout;
                    PARAM.Enum pnum = layout.Enums[cell.Enum];
                    if (pnum.Any(v => v.Value.Equals(cell.Value)))
                    {
                        row.Cells[2] = new DataGridViewComboBoxCell
                        {
                            DataSource = pnum,
                            DisplayMember = "Name",
                            ValueMember = "Value",
                            ValueType = cell.Value.GetType()
                        };
                    }
                }
                else if (cell.Type == CellType.b8 || cell.Type == CellType.b16 || cell.Type == CellType.b32)
                {
                    row.Cells[2] = new DataGridViewCheckBoxCell();
                }
                else
                {
                    row.Cells[2].ValueType = cell.Value.GetType();
                }
            }
        }

        private void DgvCells_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            DataGridViewRow row = dgvCells.Rows[e.RowIndex];
            if (!(row.Cells[2] is DataGridViewComboBoxCell))
            {
                var cell = (PARAM.Cell)row.DataBoundItem;
                if (cell.Type == CellType.x8)
                {
                    e.Value = $"0x{e.Value:X2}";
                    e.FormattingApplied = true;
                }
                else if (cell.Type == CellType.x16)
                {
                    e.Value = $"0x{e.Value:X4}";
                    e.FormattingApplied = true;
                }
                else if (cell.Type == CellType.x32)
                {
                    e.Value = $"0x{e.Value:X8}";
                    e.FormattingApplied = true;
                }
            }
        }

        private void DgvCells_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            DataGridViewRow row = dgvCells.Rows[e.RowIndex];
            try
            {
                if (!(row.Cells[2] is DataGridViewComboBoxCell))
                {
                    var cell = (PARAM.Cell)row.DataBoundItem;
                    if (cell.Type == CellType.x8)
                        Convert.ToByte((string)e.FormattedValue, 16);
                    else if (cell.Type == CellType.x16)
                        Convert.ToUInt16((string)e.FormattedValue, 16);
                    else if (cell.Type == CellType.x32)
                        Convert.ToUInt32((string)e.FormattedValue, 16);
                }
            }
            catch
            {
                e.Cancel = true;
                dgvCells.EditingPanel.BackColor = Color.Pink;
                dgvCells.EditingControl.BackColor = Color.Pink;
                SystemSounds.Hand.Play();
            }
        }

        private void DgvCells_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (e.ColumnIndex != 2)
                return;

            DataGridViewRow row = dgvCells.Rows[e.RowIndex];
            if (!(row.Cells[2] is DataGridViewComboBoxCell))
            {
                var cell = (PARAM.Cell)row.DataBoundItem;
                if (cell.Type == CellType.x8)
                {
                    e.Value = Convert.ToByte((string)e.Value, 16);
                    e.ParsingApplied = true;
                }
                else if (cell.Type == CellType.x16)
                {
                    e.Value = Convert.ToUInt16((string)e.Value, 16);
                    e.ParsingApplied = true;
                }
                else if (cell.Type == CellType.x32)
                {
                    e.Value = Convert.ToUInt32((string)e.Value, 16);
                    e.ParsingApplied = true;
                }
            }
        }

        private void DgvCells_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
            if (dgvCells.EditingPanel != null)
            {
                dgvCells.EditingPanel.BackColor = Color.Pink;
                dgvCells.EditingControl.BackColor = Color.Pink;
            }
            SystemSounds.Hand.Play();
        }

        private void DgvCells_CellToolTipTextNeeded(object sender, DataGridViewCellToolTipTextNeededEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 1)
            {
                var cell = (PARAM.Cell)dgvCells.Rows[e.RowIndex].DataBoundItem;
                e.ToolTipText = cell.Description;
            }
        }
        #endregion

        private void dgvParams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            var newFormSettings = new FormSettings();

            if (newFormSettings.ShowDialog() == DialogResult.OK)
            {
                if(!Properties.Settings.Default.ShowConfirmationMessages && !Properties.Settings.Default.Settings_LogParamSize)
                    MessageBox.Show("Settings changed.", "Settings", MessageBoxButtons.OK);
            }

            ToggleCommonParamVisibility(false);

            if(Properties.Settings.Default.Settings_LogParamSize)
            {
                Properties.Settings.Default.Settings_LogParamSize = false;

                string resDir = GetResRoot();
                string param_size_path = $@"{resDir}\Logs\ParamSizes.log";
                Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
                Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
                var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
                LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, true);

                if (result != null)
                {
                    encrypted = result.Encrypted;
                    regulation = result.ParamBND;
                    exportToolStripMenuItem.Enabled = encrypted;

                    StreamWriter output_file = new StreamWriter(param_size_path);

                    foreach (ParamWrapper wrapper in result.ParamWrappers)
                    {
                        output_file.WriteLine(wrapper.Name.ToString());
                        output_file.WriteLine(wrapper.Param.DetectedSize.ToString());
                    }

                    output_file.Close();
                }

                if (Properties.Settings.Default.UseTextEditor && Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + param_size_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }
                }

                if (!Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Param sizes logged to " + param_size_path, "Field Exporter", MessageBoxButtons.OK);
            }
        }

        private void quickfolderFieldValueMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GetResRoot() + "//Field Values");
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void quickfolderDataMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GetResRoot() + "//Data");
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void quickfolderLayoutMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GetResRoot() + "//Layouts");
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void quickfolderNamesMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GetResRoot() + "//Names");
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void quickfolderReferenceMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start(GetResRoot() + "//Reference");
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }

        private void massImportDataMenuItem_Click(object sender, EventArgs e)
        {
            // Prompt user to confirm process, since this takes a while
            string message = $@"Mass Import will import data from CSV files for all params. Continue?";
            DialogResult answer = MessageBox.Show(message, "Mass Import", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (answer == DialogResult.No)
                return;

            processLabel.Text = "Mass Import of CSV to params has begun.";

            string resDir = GetResRoot();
            Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
            Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, true);

            if (result != null)
            {
                foreach (ParamWrapper wrapper in result.ParamWrappers)
                {
                    Console.WriteLine(wrapper.Name);
                    ImportParamData(wrapper, true);
                }

                MessageBox.Show($@"Mass data import complete!", "Mass Import");
            }

            // Update visuals
            dgvRows.Refresh();
            dgvCells.Refresh();

            processLabel.Text = "No active process.";
        }

        private void massExportDataMenuItem_Click(object sender, EventArgs e)
        {
            // Prompt user to confirm process, since this takes a while
            string message = $@"Mass Export will export all params to CSV. Continue?";
            DialogResult answer = MessageBox.Show(message, "Mass Export", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (answer == DialogResult.No)
                return;

            // Notify the user if row names are off
            if (!Properties.Settings.Default.IncludeRowNameInCSV)
            {
                message = $@"Row Names are currently not included. Continue?";
                answer = MessageBox.Show(message, "Mass Export", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (answer == DialogResult.No)
                    return;
            }

            processLabel.Text = "Mass Export of params to CSV has begun.";

            string resDir = GetResRoot();
            Dictionary<string, PARAM.Layout> layouts = Util.LoadLayouts($@"{resDir}\Layouts");
            Dictionary<string, ParamInfo> paramInfo = ParamInfo.ReadParamInfo($@"{resDir}\ParamInfo.xml");
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            LoadParamsResult result = Util.LoadParams(regulationPath, paramInfo, layouts, gameMode, true);

            if (result != null)
            {
                foreach (ParamWrapper wrapper in result.ParamWrappers)
                {
                    ExportParamData(wrapper, true);
                }

                MessageBox.Show($@"Mass data export complete!", "Data Export");
            }

            processLabel.Text = "";
        }

        private void ImportParamData(ParamWrapper wrapper, Boolean isSilent)
        {
            // NetworkParam has no fields, so return immediately
            if (wrapper.Name == "NetworkParam")
                return;

            string dataDir = $@"{GetResRoot()}\Data";

            if (settings.ProjectName != "")
            {
                dataDir = dataDir + "\\" + settings.ProjectName;

                bool exists = System.IO.Directory.Exists(dataDir);

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(dataDir);
                }
            }

            string paramName = wrapper.Name;
            string paramFile = $@"{paramName}.csv";
            string paramPath = $@"{dataDir}\{paramFile}";

            processLabel.Text = $@"Currently importing {paramFile}";

            if (!File.Exists(paramPath))
            {
                // Only show this for the individual import, otherwise silently ignore params with no CSV source
                if (!isSilent)
                    MessageBox.Show($@"{paramFile} does not exist.", "Import Data");

                return;
            }

            if (!isSilent)
            {
                string message = $@"Importing will overwrite {paramName} data. Continue?";
                DialogResult answer = MessageBox.Show(message, "Import Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (answer == DialogResult.No)
                    return;
            }
            
            StreamReader reader = null;

            try
            {
                reader = new StreamReader(File.OpenRead(paramPath));
            }
            catch (Exception ex)
            {
                Util.ShowError($"Failed to open {paramFile}.\r\n\r\n{ex}");
                return;
            }

            // ignore the headers
            _ = reader.ReadLine();

            wrapper.Rows.Clear();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values = line.Split(Properties.Settings.Default.ExportDelimiter.ToCharArray());

                long id = long.Parse(values[0]);

                // Default with no row names included
                string name = "";
                int cell_index = 1;

                // Adjust index and include name if row names are included in CSV.
                if (Properties.Settings.Default.IncludeRowNameInCSV)
                {
                    name = values[1];
                    cell_index = 2;
                }
                
                PARAM.Row newRow = null;
                
                newRow = new PARAM.Row(id, name, wrapper.Layout);
                wrapper.Rows.Add(newRow);

                // Sort rows by ID
                wrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                foreach(PARAM.Cell cell in newRow.Cells)
                {
                    var type = cell.Type;

                    if (type != CellType.dummy8)
                    {
                        var new_value = values[cell_index];

                        if (type == CellType.s8)
                            cell.Value = Convert.ToSByte(new_value);
                        else if (type == CellType.u8 || type == CellType.x8)
                            cell.Value = Convert.ToByte(new_value);
                        else if (type == CellType.s16)
                            cell.Value = Convert.ToInt16(new_value);
                        else if (type == CellType.u16 || type == CellType.x16)
                            cell.Value = Convert.ToUInt16(new_value);
                        else if (type == CellType.s32)
                            cell.Value = Convert.ToInt32(new_value);
                        else if (type == CellType.u32 || type == CellType.x32)
                            cell.Value = Convert.ToUInt32(new_value);
                        else if (type == CellType.f32)
                            cell.Value = Convert.ToSingle(new_value);
                        else if (type == CellType.b8)
                            cell.Value = Convert.ToBoolean(new_value);
                        else if (type == CellType.b16)
                            cell.Value = Convert.ToBoolean(new_value);
                        else if (type == CellType.b32)
                            cell.Value = Convert.ToBoolean(new_value);
                        else if (type == CellType.fixstr || type == CellType.fixstrW)
                            cell.Value = new_value.ToString();

                        cell_index = cell_index + 1;
                    }
                }
            }

            reader.Close();

            if (!Properties.Settings.Default.ShowConfirmationMessages && !isSilent)
                MessageBox.Show($@"{paramName} data import complete!", "Data Import");
        }

        private void ExportParamData(ParamWrapper wrapper, Boolean isSilent)
        {
            // NetworkParam has no fields, so return immediately
            if (wrapper.Name == "NetworkParam")
                return;

            string dataDir = $@"{GetResRoot()}\Data";

            if (settings.ProjectName != "")
            {
                dataDir = dataDir + "\\" + settings.ProjectName;

                bool exists = System.IO.Directory.Exists(dataDir);

                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(dataDir);
                }
            }

            string paramName = wrapper.Name;
            string paramFile = $@"{paramName}.csv";
            string paramPath = $@"{dataDir}\{paramFile}";

            processLabel.Text = $@"Currently exporting {paramFile}";

            if (File.Exists(paramPath) && !isSilent)
            {
                string message = $@"{paramFile} exists. Overwrite?";
                DialogResult answer = MessageBox.Show(message, "Export Data", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (answer == DialogResult.No)
                    return;
            }
            else
            {
                // Add file if it doesn't yet exist
                if (!System.IO.File.Exists(paramPath))
                {
                    using (System.IO.FileStream fs = System.IO.File.Create(paramPath))
                    {
                        for (byte i = 0; i < 100; i++)
                        {
                            fs.WriteByte(i);
                        }
                    }
                }
            }

            StreamWriter output_file = new StreamWriter(paramPath);

            string composed_line = "";

            // Header
            if (settings.IncludeHeaderInCSV)
            {
                PARAM.Row first_row = wrapper.Rows.ElementAt(0);

                if(Properties.Settings.Default.IncludeRowNameInCSV)
                    composed_line = composed_line + "Row ID" + settings.ExportDelimiter + "Row Name" + settings.ExportDelimiter;
                else
                    composed_line = composed_line + "Row ID" + settings.ExportDelimiter;

                foreach (PARAM.Cell cell in first_row.Cells)
                {
                    // Ignore the padding fields
                    if (cell.Type != CellType.dummy8)
                    {
                        composed_line = composed_line + cell.Name + settings.ExportDelimiter;
                    }
                }

                char[] charsToTrim = settings.ExportDelimiter.ToCharArray();

                composed_line = composed_line.TrimEnd(charsToTrim);

                output_file.WriteLine(composed_line);
            }

            // Cells

            foreach (PARAM.Row row in wrapper.Rows)
            {
                composed_line = row.ID + settings.ExportDelimiter;

                // Add row name
                if (settings.IncludeRowNameInCSV)
                {
                    string row_name = row.Name;

                    composed_line = composed_line + row_name + settings.ExportDelimiter;
                }

                foreach (PARAM.Cell cell in row.Cells)
                {
                    // Ignore the padding fields
                    if (cell.Type != CellType.dummy8)
                    {
                        // Special case: remove the Japanese text in the developer caption field since it causes issues on import.
                        if (paramName == "MenuPropertySpecParam")
                        {
                            if (cell.Type == CellType.fixstrW)
                            {
                                composed_line = composed_line + " " + settings.ExportDelimiter;
                            }
                            else
                                composed_line = composed_line + cell.Value.ToString() + settings.ExportDelimiter;
                        }
                        else
                            composed_line = composed_line + cell.Value.ToString() + settings.ExportDelimiter;
                    }
                }

                char[] charsToTrim = settings.ExportDelimiter.ToCharArray();

                composed_line = composed_line.TrimEnd(charsToTrim);

                output_file.WriteLine(composed_line);
            }

            output_file.Close();

            if (Properties.Settings.Default.UseTextEditor && Properties.Settings.Default.TextEditorPath != "" && !isSilent)
            {
                try
                {
                    Process.Start("\"" + Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + paramPath + "\"");
                }
                catch
                {
                    SystemSounds.Hand.Play();
                }

            }

            if (!Properties.Settings.Default.ShowConfirmationMessages && !isSilent)
                MessageBox.Show($@"{paramName} data export complete!", "Data Export");
        }

        private void ToggleCommonParamVisibility(Boolean initialLoad)
        {
            // Update dgvParams if this is set to true
            if (Properties.Settings.Default.ShowCommonParamsOnly)
            {
                // Force select of a common param when switching to the limited view, skip if initial load
                if (!initialLoad && Properties.Settings.Default.ChangedCommonParamView)
                {
                    dgvParams.ClearSelection();
                    dgvParams.Rows[44].Selected = true;
                    dgvParams.CurrentCell = dgvParams.SelectedCells[0];
                }

                foreach (DataGridViewRow row in dgvParams.Rows)
                {
                    var wrapper = (ParamWrapper)row.DataBoundItem;

                    if (common_param_list.Contains(wrapper.Name))
                        row.Visible = true;
                    else
                        row.Visible = false;
                }
            }
            else
            {
                foreach (DataGridViewRow row in dgvParams.Rows)
                {
                    row.Visible = true;
                }
            }
        }

        private string GetResRoot()
        {
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            return $@"res\{gameMode.Directory}";
        }
    }
}
