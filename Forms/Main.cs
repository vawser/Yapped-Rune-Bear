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
using GameType = Yapped.Util.GameMode.GameType;
using System.Globalization;
using Yapped.Util;
using Yapped.Forms;
using Yapped.Tools;
using System.Reflection;
using System.Data;
using System.Threading;
using Binder = SoulsFormats.Binder;
using System.Threading.Tasks;

namespace Yapped
{
    public partial class Main : Form
    {
        private static Yapped_Rune_Bear.Properties.Settings settings = Yapped_Rune_Bear.Properties.Settings.Default;

        private bool InvalidationMode;
        private string regulationPath;
        private IBinder regulation;
        private IBinder secondary_regulation;
        private bool encrypted;
        private bool secondary_encrypted;
        private BindingSource rowSource;
        private Dictionary<string, (int Row, int Cell)> dgvIndices;
        private string lastFindRowPattern, lastFindFieldPattern;

        // Used for DataGridView column indexes
        //private int PARAM_NAME_COL = 0;

        private int ROW_ID_COL = 0;
        private int ROW_NAME_COL = 1;

        private int FIELD_PARAM_NAME_COL = 0;
        private int FIELD_EDITOR_NAME_COL = 1;
        private int FIELD_VALUE_COL = 2;
        private int FIELD_TYPE_COL = 3;

        private LoadParamsResult primary_result;
        private LoadParamsResult secondary_result;

        private List<PARAMDEF> paramdefs = new List<PARAMDEF>();

        private List<PARAMTDF> paramtdfs = new List<PARAMTDF>();
        private Dictionary<String, PARAMTDF> tdf_dict = new Dictionary<String, PARAMTDF>();

        private List<string> bool_type_tdfs = new List<String>();
        private List<string> custom_type_tdfs = new List<String>();

        public Main()
        {
            InitializeComponent();

            regulation = null;
            secondary_regulation = null;
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

        private void Main_Load(object sender, EventArgs e)
        {
            Text = "Yapped - Rune Bear Edition";

            // Filter tooltips
            toolTip_filterParams.SetToolTip(filter_Params.Control, filter_Params.ToolTipText);
            toolTip_filterRows.SetToolTip(filter_Rows.Control, filter_Rows.ToolTipText);
            toolTip_filterCells.SetToolTip(filter_Cells.Control, filter_Cells.ToolTipText);

            InvalidationMode = false;

            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dgvParams, new object[] { true });
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dgvRows, new object[] { true });
            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty, null, dgvCells, new object[] { true });

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

            if (settings.ProjectName == "")
                settings.ProjectName = "ExampleMod";

            // Settings
            regulationPath = settings.RegulationPath;
            splitContainer2.SplitterDistance = settings.SplitterDistance2;
            splitContainer1.SplitterDistance = settings.SplitterDistance1;
            secondaryFilePath.FileName = settings.SecondaryFilePath;
            Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode = false; // Disable this if it was left on from previous session

            BuildParamDefs();
            BuildParamTdfs();
            BuildBoolTypes();
            BuildCustomTypes();
            LoadParams(true);

            // Only load if an actual path has been set
            if (secondaryFilePath.FileName != "")
                LoadSecondaryParams(true);

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

        public string GetProjectDirectory(string subfolder)
        {
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            return $@"Projects\\{settings.ProjectName}\\{subfolder}\\{gameMode.Directory}";
        }

        public string GetParamdexDirectory(string subfolder)
        {
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            if(subfolder == "")
                return $@"Paramdex\\{gameMode.Directory}";
            else
                return $@"Paramdex\\{gameMode.Directory}\\{subfolder}";
        }

        #region Load Params
        private void LoadParams(bool isSilent)
        {
            primary_result = LoadParamResult(regulationPath, false);

            if (primary_result == null)
            {
                exportToolStripMenuItem.Enabled = false;
            }
            else
            {
                encrypted = primary_result.Encrypted;
                regulation = primary_result.ParamBND;
                exportToolStripMenuItem.Enabled = encrypted;

                foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
                {

                    if (!dgvIndices.ContainsKey(wrapper.Name))
                        dgvIndices[wrapper.Name] = (0, 0);
                }

                dgvParams.DataSource = primary_result.ParamWrappers;

                foreach (DataGridViewRow row in dgvParams.Rows)
                {
                    var wrapper = (ParamWrapper)row.DataBoundItem;

                    if (wrapper.Error)
                        row.Cells[0].Style.BackColor = Color.Pink;
                }

                if (!isSilent)
                {
                    if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                        MessageBox.Show("Primary file loaded.", "File Data", MessageBoxButtons.OK);
                }
            }
        }

        private void LoadSecondaryParams(bool isSilent)
        {
            secondary_result = LoadParamResult(Yapped_Rune_Bear.Properties.Settings.Default.SecondaryFilePath, true);

            if (secondary_result == null)
            {
                Utility.ShowError($"Failed to load secondary data file.");
            }
            else
            {
                secondary_encrypted = secondary_result.Encrypted;
                secondary_regulation = secondary_result.ParamBND;

                if (!isSilent)
                {
                    if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                        MessageBox.Show("Secondary file loaded.", "File Data", MessageBoxButtons.OK);
                }
            }
        }

        private void BuildParamDefs()
        {
            // Empty old version
            paramdefs.Clear();

            // Build list of paramdefs
            var paramdef_dir = GetParamdexDirectory("Defs");

            foreach (string path in Directory.GetFiles(paramdef_dir, "*.xml"))
            {
                string paramID = Path.GetFileNameWithoutExtension(path);

                try
                {
                    var paramdef = PARAMDEF.XmlDeserialize(path);
                    paramdefs.Add(paramdef);
                }
                catch (Exception ex)
                {
                    Utility.ShowError($"Failed to load layout {paramID}.txt\r\n\r\n{ex}");
                }
            }
        }
        private void BuildParamTdfs()
        {
            // Empty old version
            paramtdfs.Clear();
            tdf_dict.Clear();

            // Build enum dictionary
            var paramtdf_dir = GetParamdexDirectory("Tdfs");

            foreach (string path in Directory.GetFiles(paramtdf_dir, "*.tdf"))
            {
                string tdfID = Path.GetFileNameWithoutExtension(path);
                string tdfText = System.IO.File.ReadAllText(path);

                try
                {
                    var paramtdf = new PARAMTDF(tdfText);
                    paramtdfs.Add(paramtdf);
                }
                catch (Exception ex)
                {
                    Utility.ShowError($"Failed to load layout {tdfID}.txt\r\n\r\n{ex}");
                }
            }

            // Build tdf dict
            foreach (PARAMTDF paramtdf in paramtdfs)
            {
                string tdf_name = paramtdf.Name;

                try
                {
                    tdf_dict.Add(tdf_name, paramtdf);
                }
                catch (Exception ex)
                {
                    Utility.ShowError($"Failed to add TDF {tdf_name}.\r\n\r\n{ex}");
                }
            }
        }

        private void BuildBoolTypes()
        {
            // Empty old version
            bool_type_tdfs.Clear();

            // Build enum dictionary
            var boolean_type_dir = GetParamdexDirectory("Meta");
            var boolean_type_file = (boolean_type_dir + "\\bool_types.txt");

            if (!File.Exists(boolean_type_file))
            {
                bool_type_tdfs = null;
                return;
            }

            StreamReader reader = null;

            try
            {
                reader = new StreamReader(File.OpenRead(boolean_type_file));
            }
            catch (Exception ex)
            {
                Utility.ShowError($"Failed to open {boolean_type_file}.\r\n\r\n{ex}");
                return;
            }

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                bool_type_tdfs.Add(line);
            }
        }

        private void BuildCustomTypes()
        {
            // Empty old version
            custom_type_tdfs.Clear();

            // Build enum dictionary
            var custom_type_dir = GetParamdexDirectory("Meta");
            var custom_type_file = (custom_type_dir + "\\customizable_types.txt");

            if (!File.Exists(custom_type_file))
            {
                custom_type_tdfs = null;
                return;
            }

            StreamReader reader = null;

            try
            {
                reader = new StreamReader(File.OpenRead(custom_type_file));
            }
            catch (Exception ex)
            {
                Utility.ShowError($"Failed to open {custom_type_file}.\r\n\r\n{ex}");
                return;
            }

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                custom_type_tdfs.Add(line);
            }
        }

        private LoadParamsResult LoadParamResult(string target_path, bool isSecondary)
        {
            // Load regulation
            if (!File.Exists(target_path))
            {
                Utility.ShowError($"Parambnd not found:\r\n{target_path}\r\nPlease browse to the Data0.bdt or parambnd you would like to edit.");

                return null;
            }

            var result = new LoadParamsResult();
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            try
            {
                if (BND4.Is(target_path))
                {
                    result.ParamBND = BND4.Read(target_path);
                    result.Encrypted = false;
                }
                else if (BND3.Is(target_path))
                {
                    result.ParamBND = BND3.Read(target_path);
                    result.Encrypted = false;
                }
                else if (gameMode.Game == GameMode.GameType.DarkSouls2 || gameMode.Game == GameMode.GameType.DarkSouls2Scholar)
                {
                    result.ParamBND = Utility.DecryptDS2Regulation(target_path);
                    result.Encrypted = true;
                }
                else if (gameMode.Game == GameMode.GameType.DarkSouls3)
                {
                    result.ParamBND = SFUtil.DecryptDS3Regulation(target_path);
                    result.Encrypted = true;
                }
                else if (gameMode.Game == GameMode.GameType.EldenRing)
                {
                    result.ParamBND = SFUtil.DecryptERRegulation(target_path);
                    result.Encrypted = true;
                }
                else
                {
                    throw new FormatException("Unrecognized file format.");
                }
            }
            catch (DllNotFoundException ex) when (ex.Message.Contains("oo2core_6_win64.dll"))
            {
                Utility.ShowError($"In order to load Sekiro params, you must copy oo2core_6_win64.dll from Sekiro into Yapped's lib folder.");

                return null;
            }
            catch (Exception ex)
            {
                Utility.ShowError($"Failed to load parambnd:\r\n{target_path}\r\n\r\n{ex}");

                return null;
            }

            if (!isSecondary)
                processLabel.Text = target_path;

            result.ParamWrappers = new List<ParamWrapper>();

            foreach (BinderFile file in result.ParamBND.Files.Where(f => f.Name.EndsWith(".param")))
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);

                try
                {
                    PARAM param = PARAM.Read(file.Bytes);

                    if (param.ApplyParamdefCarefully(paramdefs))
                    {
                        var wrapper = new ParamWrapper(name, param, param.AppliedParamdef);
                        result.ParamWrappers.Add(wrapper);
                    }
                }
                catch (Exception ex)
                {
                    Utility.ShowError($"Failed to load param file: {name}.param\r\n\r\n{ex}");
                }
            }

            result.ParamWrappers.Sort();
            return result;
        }

        internal class LoadParamsResult
        {
            public bool Encrypted { get; set; }

            public IBinder ParamBND { get; set; }

            public List<ParamWrapper> ParamWrappers { get; set; }
        }
        #endregion

        #region View

        private void toggleFieldNameTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            if (!settings.CellView_ShowEditorNames)
            {
                settings.CellView_ShowEditorNames = true;

            }
            else
            {
                settings.CellView_ShowEditorNames = false;
            }
        }

        private void toggleFieldTypeVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            if (!settings.CellView_ShowTypes)
            {
                settings.CellView_ShowTypes = true;

            }
            else
            {
                settings.CellView_ShowTypes = false;
            }
        }
        #endregion

        #region Settings

        private void viewSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var newFormSettings = new SettingsMenu();

            if (newFormSettings.ShowDialog() == DialogResult.OK)
            {
                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Settings changed.", "Settings", MessageBoxButtons.OK);
            }

            // This is done automatically so the user can simply change the Project Name and have the folder structure already present.
            GenerateProjectDirectories(Yapped_Rune_Bear.Properties.Settings.Default.ProjectName);
        }
        private void logParamSizesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var log_dir = GetProjectDirectory("Logs");
            string param_size_path = $@"{log_dir}\ParamSizes.log";

            if (primary_result != null)
            {
                StreamWriter output_file = new StreamWriter(param_size_path);

                foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
                {
                    output_file.WriteLine(wrapper.Name.ToString());
                    output_file.WriteLine(wrapper.Param.DetectedSize.ToString());
                }

                output_file.Close();
            }

            if (Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor && Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath != "")
            {
                try
                {
                    Process.Start("\"" + Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + param_size_path + "\"");
                }
                catch
                {
                    SystemSounds.Hand.Play();
                }
            }

            if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                MessageBox.Show("Param sizes logged to " + param_size_path, "Field Exporter", MessageBoxButtons.OK);
        }
        #endregion

        #region File - Open 
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            dataFileDialog.FileName = regulationPath;

            if (dataFileDialog.ShowDialog() == DialogResult.OK)
            {
                regulationPath = dataFileDialog.FileName;

                // Rebuild defs and tdfs since gametype may have changed
                BuildParamDefs();
                BuildParamTdfs();
                BuildBoolTypes();
                BuildCustomTypes();

                LoadParams(false);
            }
        }
        #endregion

        #region File - Save
        private void SaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            SaveParams(".bak");

            if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                MessageBox.Show($"Params saved to {regulationPath}", "Save", MessageBoxButtons.OK);
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
                        try
                        {
                            file.Bytes = paramFile.Param.Write();
                        }
                        catch
                        {
                            MessageBox.Show($"Invalid data, failed to save {paramFile}. Data must be fixed before saving can complete.", "Save", MessageBoxButtons.OK);
                            return;
                        }
                    }
                }
            }

            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;
            if (!File.Exists(regulationPath + backup_format))
                File.Copy(regulationPath, regulationPath + backup_format);

            // Save with default encryption unless Save without Encryption is toggled.
            if (encrypted && !Yapped_Rune_Bear.Properties.Settings.Default.SaveWithoutEncryption)
            {
                if (gameMode.Game == GameType.DarkSouls2)
                    Utility.EncryptDS2Regulation(regulationPath, regulation as BND4);
                else if (gameMode.Game == GameType.DarkSouls3)
                    SFUtil.EncryptDS3Regulation(regulationPath, regulation as BND4);
                else if (gameMode.Game == GameType.EldenRing)
                    SFUtil.EncryptERRegulation(regulationPath, regulation as BND4);
                else
                    Utility.ShowError("Encryption is not valid for this file.");
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
        #endregion

        #region File - Restore
        private void RestoreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            if (File.Exists(regulationPath + ".bak"))
            {
                DialogResult choice = MessageBox.Show("Are you sure you want to restore the backup?",
                    "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (choice == DialogResult.Yes)
                {
                    try
                    {
                        File.Copy(regulationPath + ".bak", regulationPath, true);

                        BuildParamDefs();
                        BuildParamTdfs();
                        BuildBoolTypes();
                        BuildCustomTypes();
                        LoadParams(false);

                        SystemSounds.Asterisk.Play();
                    }
                    catch (Exception ex)
                    {
                        Utility.ShowError($"Failed to restore backup\r\n\r\n{regulationPath}.bak\r\n\r\n{ex}");
                    }
                }
            }
            else
            {
                Utility.ShowError($"There is no backup to restore at:\r\n\r\n{regulationPath}.bak");
            }
        }
        #endregion

        #region File - Expore
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
        #endregion

        #region File - Export
        private void ExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            var bnd4 = regulation as BND4;
            string dir = fbdExport.SelectedPath;
            fbdExport.SelectedPath = Path.GetDirectoryName(regulationPath);

            if (fbdExport.ShowDialog() == DialogResult.OK)
            {
                // Params
                BND4 paramBND = new BND4
                {
                    BigEndian = false,
                    Compression = DCX.Type.DCX_DFLT_10000_44_9,
                    Extended = 0x04,
                    Unk04 = false,
                    Unk05 = false,
                    Format = Binder.Format.Compression | Binder.Format.Flag6 | Binder.Format.LongOffsets | Binder.Format.Names1,
                    Unicode = true,
                    Files = regulation.Files.Where(f => f.Name.EndsWith(".param")).ToList()
                };

                try
                {
                    paramBND.Write($@"{dir}\gameparam.parambnd.dcx");
                }
                catch (Exception ex)
                {
                    Utility.ShowError($"Failed to write exported parambnds.\r\n\r\n{ex}");
                }

                // Stay Params
                if (gameMode.Game == GameMode.GameType.DarkSouls3)
                {
                    BND4 stayBND = new BND4
                    {
                        BigEndian = false,
                        Compression = DCX.Type.DCX_DFLT_10000_44_9,
                        Extended = 0x04,
                        Unk04 = false,
                        Unk05 = false,
                        Format = Binder.Format.Compression | Binder.Format.Flag6 | Binder.Format.LongOffsets | Binder.Format.Names1,
                        Unicode = true,
                        Files = regulation.Files.Where(f => f.Name.EndsWith(".stayparam")).ToList()
                    };

                    try
                    {
                        stayBND.Write($@"{dir}\stayparam.parambnd.dcx");
                    }
                    catch (Exception ex)
                    {
                        Utility.ShowError($"Failed to write exported parambnds.\r\n\r\n{ex}");
                    }
                }
            }
        }
        #endregion

        #region File - Resource Folder
        private void ProjectFolderMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start($@"Projects");
            }
            catch
            {
                SystemSounds.Hand.Play();
            }
        }
        #endregion

        #region Edit - Add Row
        private void AddRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            CreateRow("Add a new row...");
        }
        #endregion

        #region Edit - Duplicate Row
        private void DuplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            if (dgvRows.SelectedCells.Count == 0)
            {
                Utility.ShowError("You can't duplicate a row without one selected!");
                return;
            }

            int index = dgvRows.SelectedCells[0].RowIndex;
            ParamWrapper wrapper = (ParamWrapper)rowSource.DataSource;
            PARAM.Row oldRow = wrapper.Rows[index];
            PARAM.Row newRow;

            if (rowSource.DataSource == null)
            {
                Utility.ShowError("You can't create a row with no param selected!");
                return;
            }

            var newRowForm = new NewRow("Duplicate a row...");

            if (newRowForm.ShowDialog() == DialogResult.OK)
            {
                int base_id = newRowForm.ResultID;
                string name = newRowForm.ResultName;
                int repeat_count = Yapped_Rune_Bear.Properties.Settings.Default.NewRow_RepeatCount;
                int step_value = Yapped_Rune_Bear.Properties.Settings.Default.NewRow_StepValue;

                if (repeat_count == 0)
                    repeat_count = 1;

                if (step_value == 0)
                    step_value = 1;

                int current_id = base_id;

                for (int j = 0; j < repeat_count; j++)
                {
                    ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;
                    if (paramWrapper.Rows.Any(row => row.ID == current_id))
                    {
                        Utility.ShowError($"A row with this ID already exists: {current_id}");
                    }
                    else
                    {
                        newRow = new PARAM.Row(current_id, name, paramWrapper.AppliedParamDef);
                        rowSource.Add(newRow);
                        paramWrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                        int row_index = paramWrapper.Rows.FindIndex(row => ReferenceEquals(row, newRow));
                        int displayedRows = dgvRows.DisplayedRowCount(false);
                        dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, row_index - displayedRows / 2);
                        dgvRows.ClearSelection();
                        dgvRows.Rows[row_index].Selected = true;
                        dgvRows.Refresh();

                        for (int i = 0; i < oldRow.Cells.Count; i++)
                        {
                            newRow.Cells[i].Value = oldRow.Cells[i].Value;
                        }
                    }

                    current_id = current_id + step_value;
                }
            }
        }
        #endregion

        #region Edit - Create Row
        private PARAM.Row CreateRow(string prompt)
        {
            if (rowSource.DataSource == null)
            {
                Utility.ShowError("You can't create a row with no param selected!");
                return null;
            }

            PARAM.Row row_result = null;
            var newRowForm = new NewRow(prompt);
            if (newRowForm.ShowDialog() == DialogResult.OK)
            {
                int id = newRowForm.ResultID;
                string name = newRowForm.ResultName;
                ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;
                if (paramWrapper.Rows.Any(row => row.ID == id))
                {
                    Utility.ShowError($"A row with this ID already exists: {id}");
                }
                else
                {
                    row_result = new PARAM.Row(id, name, paramWrapper.AppliedParamDef);
                    rowSource.Add(row_result);
                    paramWrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                    int index = paramWrapper.Rows.FindIndex(row => ReferenceEquals(row, row_result));
                    int displayedRows = dgvRows.DisplayedRowCount(false);
                    dgvRows.FirstDisplayedScrollingRowIndex = Math.Max(0, index - displayedRows / 2);
                    dgvRows.ClearSelection();
                    dgvRows.Rows[index].Selected = true;
                    dgvRows.Refresh();
                }
            }
            return row_result;
        }
        #endregion

        #region Edit - Delete Row
        private void DeleteRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

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
        #endregion

        #region Edit - Find Row
        private void FindRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var findForm = new FindRow("Find row with name...");
            if (findForm.ShowDialog() == DialogResult.OK)
            {
                FindRow(findForm.ResultPattern);
            }
        }

        private void FindNextRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            FindRow(lastFindRowPattern);
        }

        private void FindRow(string pattern)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            if (rowSource.DataSource == null)
            {
                Utility.ShowError("You can't search for a row when there are no rows!");
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
                Utility.ShowError($"No row found matching: {pattern}");
            }
        }

        #endregion

        #region Edit - Go To Row
        private void GotoRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var gotoForm = new GoToRow();
            if (gotoForm.ShowDialog() == DialogResult.OK)
            {
                if (rowSource.DataSource == null)
                {
                    Utility.ShowError("You can't goto a row when there are no rows!");
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
                    Utility.ShowError($"Row ID not found: {id}");
                }
            }
        }

        #endregion

        #region Edit - Find Field
        private void FindFieldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var findForm = new FindField("Find field with name...");
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
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            if (dgvCells.DataSource == null)
            {
                Utility.ShowError("You can't search for a field when there are no fields!");
                return;
            }

            int startIndex = dgvCells.SelectedCells.Count > 0 ? dgvCells.SelectedCells[0].RowIndex + 1 : 0;
            var cells = (PARAM.Cell[])dgvCells.DataSource;
            int index = -1;

            for (int i = 0; i < cells.Length; i++)
            {
                if (Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowEditorNames)
                {
                    if ((cells[(startIndex + i) % cells.Length].Def.DisplayName.ToString() ?? "").ToLower().Contains(pattern.ToLower()))
                    {
                        index = (startIndex + i) % cells.Length;
                        break;
                    }
                }
                else
                {
                    if ((cells[(startIndex + i) % cells.Length].Def.InternalName.ToString() ?? "").ToLower().Contains(pattern.ToLower()))
                    {
                        index = (startIndex + i) % cells.Length;
                        break;
                    }
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
                Utility.ShowError($"No field found matching: {pattern}");
            }
        }

        #endregion

        #region Data Tools - Import Names
        private void importRowNames_Project_MenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            bool replace = MessageBox.Show("If a row already has a name, would you like to skip it?\r\n" +
                "Click Yes to skip existing names.\r\nClick No to replace existing names.",
                "Importing Names", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            var name_dir = GetProjectDirectory("Names");

            List<ParamWrapper> paramFiles = (List<ParamWrapper>)dgvParams.DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                if (File.Exists($@"{name_dir}\{paramFile.Name}.txt"))
                {
                    var names = new Dictionary<long, string>();
                    string nameStr = File.ReadAllText($@"{name_dir}\{paramFile.Name}.txt");
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
        private void importRowNames_Stock_MenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            bool replace = MessageBox.Show("If a row already has a name, would you like to skip it?\r\n" +
                "Click Yes to skip existing names.\r\nClick No to replace existing names.",
                "Importing Names", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

            var name_dir = GetParamdexDirectory("Names");

            List<ParamWrapper> paramFiles = (List<ParamWrapper>)dgvParams.DataSource;
            foreach (ParamWrapper paramFile in paramFiles)
            {
                if (File.Exists($@"{name_dir}\{paramFile.Name}.txt"))
                {
                    var names = new Dictionary<long, string>();
                    string nameStr = File.ReadAllText($@"{name_dir}\{paramFile.Name}.txt");
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
        #endregion

        #region Data Tools - Export Names
        private void exportRowNames_Project_MenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var name_dir = GetProjectDirectory("Names");

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
                    File.WriteAllText($@"{name_dir}\{paramFile.Name}.txt", sb.ToString());
                }
                catch (Exception ex)
                {
                    Utility.ShowError($"Failed to write name file: {paramFile.Name}.txt\r\n\r\n{ex}");
                    break;
                }
            }

            if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                MessageBox.Show("Names exported!", "Export Names", MessageBoxButtons.OK);
        }
        #endregion

        #region Data Tools - Import Data
        private void importDataMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            DataGridViewRow paramRow = dgvParams.CurrentRow;
            ParamWrapper wrapper = ((ParamWrapper)paramRow.DataBoundItem);

            ImportParamData(wrapper, false);
        }
        #endregion

        #region Data Tools - Export Data
        private void exportDataMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            DataGridViewRow paramRow = dgvParams.CurrentRow;
            ParamWrapper wrapper = ((ParamWrapper)paramRow.DataBoundItem);

            ExportParamData(wrapper, false);
        }

        #endregion

        #region Data Tools - Field Exporter
        private void fieldExporterMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var field_exporter = new FieldExporter();
            var log_dir = GetProjectDirectory("Logs");

            if (field_exporter.ShowDialog() == DialogResult.OK)
            {
                ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;

                string field_data_path = $@"{log_dir}\\FieldValue_" + paramWrapper.Name + ".log";
                char delimiter = settings.ExportDelimiter.ToCharArray()[0];

                string field_match = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMatch;
                string field_minimum = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMinimum;
                string field_maximum = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldMaximum;
                string field_exclusions = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldExclusion;
                string field_inclusions = Yapped_Rune_Bear.Properties.Settings.Default.FieldExporter_FieldInclusion;

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
                        PARAMDEF.DefType type = cell.Def.DisplayType;

                        string display_name = cell.Def.DisplayName;
                        string internal_name = cell.Def.InternalName;

                        string value = cell.Value.ToString();
                        bool isMatchedField = false;

                        // Check if field name matches
                        if (type != PARAMDEF.DefType.dummy8)
                        {
                            if ((Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowEditorNames && field_match == display_name) || (!Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowEditorNames && field_match == internal_name))
                            {
                                isMatchedField = true;

                                // Minimum
                                if (field_minimum != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        if (Convert.ToSByte(value) < Convert.ToSByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        if (Convert.ToByte(value) < Convert.ToByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        if (Convert.ToInt16(value) < Convert.ToInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        if (Convert.ToUInt16(value) < Convert.ToUInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        if (Convert.ToInt32(value) < Convert.ToInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        if (Convert.ToUInt32(value) < Convert.ToUInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
                                    {
                                        if (Convert.ToSingle(value) < Convert.ToSingle(field_minimum))
                                            isMatchedField = false;
                                    }
                                }

                                // Maximum
                                if (field_maximum != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        if (Convert.ToSByte(value) > Convert.ToSByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        if (Convert.ToByte(value) > Convert.ToByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        if (Convert.ToInt16(value) > Convert.ToInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        if (Convert.ToUInt16(value) > Convert.ToUInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        if (Convert.ToInt32(value) > Convert.ToInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        if (Convert.ToUInt32(value) > Convert.ToUInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
                                    {
                                        if (Convert.ToSingle(value) > Convert.ToSingle(field_maximum))
                                            isMatchedField = false;
                                    }
                                }

                                // Inclusion
                                if (field_inclusions != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
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
                                if (field_exclusions != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
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
                                    if (unique_list.Contains(value))
                                    {
                                        isMatchedField = false;
                                    }

                                    if (!unique_list.Contains(value))
                                    {
                                        unique_list.Add(value);
                                    }
                                }

                                if (isMatchedField)
                                {
                                    isValidRow = true;

                                    if (type == PARAMDEF.DefType.s8)
                                        row_line = row_line + Convert.ToSByte(value);
                                    else if (type == PARAMDEF.DefType.u8)
                                        row_line = row_line + Convert.ToByte(value);
                                    else if (type == PARAMDEF.DefType.s16)
                                        row_line = row_line + Convert.ToInt16(value);
                                    else if (type == PARAMDEF.DefType.u16)
                                        row_line = row_line + Convert.ToUInt16(value);
                                    else if (type == PARAMDEF.DefType.s32)
                                        row_line = row_line + Convert.ToInt32(value);
                                    else if (type == PARAMDEF.DefType.u32)
                                        row_line = row_line + Convert.ToUInt32(value);
                                    else if (type == PARAMDEF.DefType.f32)
                                        row_line = row_line + Convert.ToSingle(value);
                                    else if (type == PARAMDEF.DefType.fixstr || type == PARAMDEF.DefType.fixstrW)
                                        row_line = row_line + value;
                                }
                            }
                        }
                    }

                    // Only write if the row actually results in a valid field output 
                    if (isValidRow)
                        output_file.WriteLine(row_line);
                }

                output_file.Close();

                if (Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor && Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + field_data_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }

                }

                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Field values exported to " + field_data_path, "Field Exporter", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region Data Tools- Reference Finder
        private void rowReferenceFinderMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var newFormReferenceFinder = new RowReferenceSearch();
            string reference_text;
            var log_dir = GetProjectDirectory("Logs");

            if (newFormReferenceFinder.ShowDialog() == DialogResult.OK)
            {
                reference_text = newFormReferenceFinder.GetReferenceText();

                if (reference_text == "")
                {
                    MessageBox.Show("You did not specify a value.", "Reference Finder", MessageBoxButtons.OK);
                    return;
                }

                string reference_file_path = $@"{log_dir}\\RowReference.log";

                StreamWriter output_file = new StreamWriter(reference_file_path);

                foreach (var param in primary_result.ParamWrappers)
                {
                    foreach (var row in param.Rows)
                    {
                        foreach (PARAM.Cell cell in row.Cells)
                        {
                            // Does cell value match reference_text, if so we have a reference to output
                            if (CheckFieldReference(reference_text, cell.Value.ToString()))
                            {
                                output_file.WriteLine(param.Name.ToString()); // Param Name

                                if (row.Name != null)
                                    output_file.WriteLine("  - Row Name: " + row.Name.ToString()); // Row Name

                                if (cell.Def.ToString() != null)
                                    output_file.WriteLine("  - Cell Name: " + cell.Def.ToString()); // Cell Name

                                output_file.WriteLine("  - Row ID: " + row.ID.ToString()); // Row ID of Cell
                                output_file.WriteLine("  - Reference Value: " + reference_text); // Reference Text

                                output_file.WriteLine("");
                            }
                        }
                    }
                }

                output_file.Close();

                if (Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor && Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + reference_file_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }

                }

                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("References exported to " + reference_file_path, "Reference Finder", MessageBoxButtons.OK);
            }
        }

        // Value Reference Finder
        private void valueReferenceFinderMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var newFormReferenceFinder = new FieldReferenceSearch();
            string reference_text;
            var log_dir = GetProjectDirectory("Logs");

            if (newFormReferenceFinder.ShowDialog() == DialogResult.OK)
            {
                reference_text = newFormReferenceFinder.GetReferenceText();

                if (reference_text == "")
                {
                    MessageBox.Show("You did not specify a value.", "Reference Finder", MessageBoxButtons.OK);
                    return;
                }

                string reference_file_path = $@"{log_dir}\\ValueReference.log";

                StreamWriter output_file = new StreamWriter(reference_file_path);

                foreach (var param in primary_result.ParamWrappers)
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

                if (Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor && Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + reference_file_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }

                }

                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("References exported to " + reference_file_path, "Reference Finder", MessageBoxButtons.OK);
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

        #endregion

        #region Data Tools - Mass Import
        private void massImportDataMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            // Prompt user to confirm process, since this takes a while
            string message = $@"Mass Import will import data from CSV files for all params. Continue?";
            DialogResult answer = MessageBox.Show(message, "Mass Import", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (answer == DialogResult.No)
                return;

            if (primary_result != null)
            {
                foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
                {
                    ImportParamData(wrapper, true);
                }

                MessageBox.Show($@"Mass data import complete!", "Mass Import");
            }
        }

        #endregion

        #region Data Tools - Mass Export
        private void massExportDataMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            // Prompt user to confirm process, since this takes a while
            string message = $@"Mass Export will export all params to CSV. Continue?";
            DialogResult answer = MessageBox.Show(message, "Mass Export", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (answer == DialogResult.No)
                return;

            // Notify the user if row names are off
            if (!Yapped_Rune_Bear.Properties.Settings.Default.IncludeRowNameInCSV)
            {
                message = $@"Row Names are currently not included. Continue?";
                answer = MessageBox.Show(message, "Mass Export", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (answer == DialogResult.No)
                    return;
            }

            if (primary_result != null)
            {
                foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
                {
                    ExportParamData(wrapper, true);
                }

                MessageBox.Show($@"Mass data export complete!", "Data Export");
            }
        }

        #endregion

        #region Workflow Tools - Field Adjuster
        private void fieldAdjusterMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var field_adjuster = new FieldAdjuster();
            var log_dir = GetProjectDirectory("Logs");

            if (field_adjuster.ShowDialog() == DialogResult.OK)
            {
                // Save current state before applying the edits
                SaveParams(".bak");

                ParamWrapper paramWrapper = (ParamWrapper)rowSource.DataSource;

                string adjustment_file_path = $@"{log_dir}\\FieldAdjustment_" + paramWrapper.Name + ".log";
                char delimiter = settings.ExportDelimiter.ToCharArray()[0];

                // Selection
                string field_match = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMatch;
                string row_range = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RowRange;
                string row_partial_match = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_RowPartialMatch;
                string field_minimum = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMinimum;
                string field_maximum = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldMaximum;
                string field_exclusions = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldExclusion;
                string field_inclusions = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_FieldInclusion;

                // Output
                string field_formula = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_Formula;
                string output_max = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_ValueMax;
                string output_min = Yapped_Rune_Bear.Properties.Settings.Default.FieldAdjuster_ValueMin;

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

                            if (row_range_array.Length != 2)
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
                    if (row_partial_match != "")
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
                        PARAMDEF.DefType type = cell.Def.DisplayType;

                        string display_name = cell.Def.DisplayName;
                        string internal_name = cell.Def.InternalName;

                        string value = cell.Value.ToString();
                        bool isMatchedField = false;

                        // Check if field name matches
                        if (type != PARAMDEF.DefType.dummy8)
                        {
                            if ((Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowEditorNames && field_match == display_name) || (!Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowEditorNames && field_match == internal_name))
                            {
                                isMatchedField = true;

                                var exception_string = $@"Entered value: {value} is invalid for [{cell.Name}] .";

                                // Minimum
                                if (field_minimum != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        if (Convert.ToSByte(value) < Convert.ToSByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        if (Convert.ToByte(value) < Convert.ToByte(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        if (Convert.ToInt16(value) < Convert.ToInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        if (Convert.ToUInt16(value) < Convert.ToUInt16(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        if (Convert.ToInt32(value) < Convert.ToInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        if (Convert.ToUInt32(value) < Convert.ToUInt32(field_minimum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
                                    {
                                        if (Convert.ToSingle(value) < Convert.ToSingle(field_minimum))
                                            isMatchedField = false;
                                    }
                                }

                                // Maximum
                                if (field_maximum != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        if (Convert.ToSByte(value) > Convert.ToSByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        if (Convert.ToByte(value) > Convert.ToByte(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        if (Convert.ToInt16(value) > Convert.ToInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        if (Convert.ToUInt16(value) > Convert.ToUInt16(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        if (Convert.ToInt32(value) > Convert.ToInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        if (Convert.ToUInt32(value) > Convert.ToUInt32(field_maximum))
                                            isMatchedField = false;
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
                                    {
                                        if (Convert.ToSingle(value) > Convert.ToSingle(field_maximum))
                                            isMatchedField = false;
                                    }
                                }

                                // Inclusion
                                if (field_inclusions != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_inclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) != array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
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
                                if (field_exclusions != "")
                                {
                                    if (type == PARAMDEF.DefType.s8)
                                    {
                                        SByte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => SByte.Parse(s));

                                        foreach (SByte array_value in temp_array)
                                        {
                                            if (Convert.ToSByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u8)
                                    {
                                        Byte[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Byte.Parse(s));

                                        foreach (Byte array_value in temp_array)
                                        {
                                            if (Convert.ToByte(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s16)
                                    {
                                        Int16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u16)
                                    {
                                        UInt16[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt16.Parse(s));

                                        foreach (Int16 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt16(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.s32)
                                    {
                                        Int32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => Int32.Parse(s));

                                        foreach (Int32 array_value in temp_array)
                                        {
                                            if (Convert.ToInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.u32)
                                    {
                                        UInt32[] temp_array = Array.ConvertAll(field_exclusions.Split(delimiter), s => UInt32.Parse(s));

                                        foreach (UInt32 array_value in temp_array)
                                        {
                                            if (Convert.ToUInt32(value) == array_value)
                                                isMatchedField = false;
                                        }
                                    }
                                    else if (type == PARAMDEF.DefType.f32)
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
                                    output_file.WriteLine("- Field " + cell.Def.InternalName.ToString());
                                    output_file.WriteLine("- Old Value " + cell.Value.ToString());


                                    double field_result = 0;

                                    // Only use the formula stuff if X is present
                                    if (field_formula.Contains("x"))
                                    {
                                        // Replace x with th e current cell value
                                        string cell_formula = field_formula.Replace("x", cell.Value.ToString());
                                        // Evaluate the formula
                                        StringToFormula stf = new StringToFormula();
                                        field_result = stf.Eval(cell_formula);

                                        if (output_max != "")
                                        {
                                            if (field_result > double.Parse(output_max))
                                            {
                                                field_result = double.Parse(output_max);
                                            }
                                        }

                                        if (output_min != "")
                                        {
                                            if (field_result < double.Parse(output_min))
                                            {
                                                field_result = double.Parse(output_min);
                                            }
                                        }
                                    }
                                    // Otherwise just set the value to the entered value
                                    else
                                    {
                                        field_result = Double.Parse(field_formula);
                                    }

                                    if (type == PARAMDEF.DefType.s8)
                                        cell.Value = Convert.ToSByte(field_result);
                                    else if (type == PARAMDEF.DefType.u8)
                                        cell.Value = Convert.ToByte(field_result);
                                    else if (type == PARAMDEF.DefType.s16)
                                        cell.Value = Convert.ToInt16(field_result);
                                    else if (type == PARAMDEF.DefType.u16)
                                        cell.Value = Convert.ToUInt16(field_result);
                                    else if (type == PARAMDEF.DefType.s32)
                                        cell.Value = Convert.ToInt32(field_result);
                                    else if (type == PARAMDEF.DefType.u32)
                                        cell.Value = Convert.ToUInt32(field_result);
                                    else if (type == PARAMDEF.DefType.f32)
                                        cell.Value = Convert.ToSingle(field_result);

                                    output_file.WriteLine("- New Value " + cell.Value.ToString());
                                    output_file.WriteLine("");
                                }
                            }
                        }
                    }
                }

                output_file.Close();

                if (Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor && Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath != "")
                {
                    try
                    {
                        Process.Start("\"" + Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + adjustment_file_path + "\"");
                    }
                    catch
                    {
                        SystemSounds.Hand.Play();
                    }

                }

                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Field Adjustment complete.", "Field Adjuster", MessageBoxButtons.OK);
            }
        }

        #endregion

        #region Workflow Tools - Affinity Generator
        private void affinityGeneratorMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            DataGridViewRow paramRow = dgvParams.CurrentRow;
            ParamWrapper wrapper = ((ParamWrapper)paramRow.DataBoundItem);
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            AffinityGeneration.GenerateAffinityRows(paramRow, wrapper, dgvRows, gameMode);
        }

        #endregion

        #region Utility - ImportParamData
        private void ImportParamData(ParamWrapper wrapper, Boolean isSilent)
        {
            var csv_dir = GetProjectDirectory("CSV");

            Utility.DebugPrint(wrapper.Name);

            string paramName = wrapper.Name;
            string paramFile = $@"{paramName}.csv";
            string paramPath = $@"{csv_dir}\{paramFile}";

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
                Utility.ShowError($"Failed to open {paramFile}.\r\n\r\n{ex}");
                return;
            }

            // ignore the headers
            _ = reader.ReadLine();

            wrapper.Rows.Clear();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                string[] values = line.Split(Yapped_Rune_Bear.Properties.Settings.Default.ExportDelimiter.ToCharArray());

                int id = int.Parse(values[0], CultureInfo.InvariantCulture);

                // Default with no row names included
                string name = "";
                int cell_index = 1;

                // Adjust index and include name if row names are included in CSV.
                if (Yapped_Rune_Bear.Properties.Settings.Default.IncludeRowNameInCSV)
                {
                    name = values[1];
                    cell_index = 2;
                }

                PARAM.Row newRow = null;

                newRow = new PARAM.Row(id, name, wrapper.AppliedParamDef);
                wrapper.Rows.Add(newRow);

                // Sort rows by ID
                wrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));

                foreach (PARAM.Cell cell in newRow.Cells)
                {
                    //Console.WriteLine(cell.Name);
                    //Console.WriteLine(cell.Type);

                    var type = cell.Def.DisplayType;

                    if (type != PARAMDEF.DefType.dummy8)
                    {
                        var new_value = values[cell_index];
                        var exception_string = $@"Row {newRow.ID}, Field {cell.Name} has invalid value {new_value}, skipped import of this value.";

                        if (type == PARAMDEF.DefType.s8)
                        {
                            try
                            {
                                cell.Value = Convert.ToSByte(new_value, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }
                        else if (type == PARAMDEF.DefType.u8)
                        {
                            try
                            {
                                cell.Value = Convert.ToByte(new_value, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }
                        else if (type == PARAMDEF.DefType.s16)
                        {
                            try
                            {
                                cell.Value = Convert.ToInt16(new_value, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }
                        else if (type == PARAMDEF.DefType.u16)
                        {
                            try
                            {
                                cell.Value = Convert.ToUInt16(new_value, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }
                        else if (type == PARAMDEF.DefType.s32)
                        {
                            try
                            {
                                cell.Value = Convert.ToInt32(new_value, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }

                        else if (type == PARAMDEF.DefType.u32)
                        {
                            try
                            {
                                cell.Value = Convert.ToUInt32(new_value, CultureInfo.InvariantCulture);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }
                        else if (type == PARAMDEF.DefType.f32)
                        {
                            try
                            {
                                cell.Value = Convert.ToSingle(new_value);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }
                        else if (type == PARAMDEF.DefType.fixstr || type == PARAMDEF.DefType.fixstrW)
                        {
                            try
                            {
                                cell.Value = Convert.ToString(new_value);
                            }
                            catch
                            {
                                MessageBox.Show(exception_string, "Data Import");
                            }
                        }

                        cell_index = cell_index + 1;
                    }
                }
            }

            reader.Close();

            if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages && !isSilent)
                MessageBox.Show($@"{paramName} data import complete!", "Data Import");
        }
        #endregion

        #region Utility - ExportParamData
        private void ExportParamData(ParamWrapper wrapper, Boolean isSilent)
        {
            var csv_dir = GetProjectDirectory("CSV");

            string paramName = wrapper.Name;
            string paramFile = $@"{paramName}.csv";
            string paramPath = $@"{csv_dir}\{paramFile}";

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
            if (settings.IncludeHeaderInCSV && !Yapped_Rune_Bear.Properties.Settings.Default.VerboseCSVExport)
            {
                PARAM.Row first_row = wrapper.Rows.ElementAt(0);

                if (Yapped_Rune_Bear.Properties.Settings.Default.IncludeRowNameInCSV)
                    composed_line = composed_line + "Row ID" + settings.ExportDelimiter + "Row Name" + settings.ExportDelimiter;
                else
                    composed_line = composed_line + "Row ID" + settings.ExportDelimiter;

                int cell_idx = 0;

                foreach (PARAM.Cell cell in first_row.Cells)
                {
                    // Ignore the padding fields
                    if (cell.Def.DisplayType != PARAMDEF.DefType.dummy8)
                    {
                        if (first_row.Cells.Count == cell_idx)
                            composed_line = composed_line + cell.Def.InternalName;
                        else
                            composed_line = composed_line + cell.Def.InternalName + settings.ExportDelimiter;

                        cell_idx++;
                    }
                }

                char[] charsToTrim = settings.ExportDelimiter.ToCharArray();

                composed_line = composed_line.TrimEnd(charsToTrim);

                output_file.WriteLine(composed_line);
            }

            if (Yapped_Rune_Bear.Properties.Settings.Default.VerboseCSVExport)
            {
                output_file.WriteLine("UNFURLED");
            }

            // Cells
            if (Yapped_Rune_Bear.Properties.Settings.Default.VerboseCSVExport)
            {
                foreach (PARAM.Row row in wrapper.Rows)
                {
                    output_file.WriteLine(row.ID + settings.ExportDelimiter);
                    output_file.WriteLine("~#" + row.Name + settings.ExportDelimiter);

                    int cell_idx = 0;

                    foreach (PARAM.Cell cell in row.Cells)
                    {
                        // Ignore the padding fields
                        if (cell.Def.DisplayType != PARAMDEF.DefType.dummy8)
                        {
                            // At end of cells, don't add delimiter
                            if (row.Cells.Count == cell_idx)
                                output_file.WriteLine("~#" + cell.Value.ToString());
                            else
                                output_file.WriteLine("~#" + cell.Value.ToString() + settings.ExportDelimiter);
                        }

                        cell_idx++;
                    }
                }
            }
            else
            {
                foreach (PARAM.Row row in wrapper.Rows)
                {
                    composed_line = row.ID + settings.ExportDelimiter;

                    // Add row name
                    if (settings.IncludeRowNameInCSV)
                    {
                        string row_name = row.Name;

                        composed_line = composed_line + row_name + settings.ExportDelimiter;
                    }

                    int cell_idx = 0;

                    foreach (PARAM.Cell cell in row.Cells)
                    {
                        // Ignore the padding fields
                        if (cell.Def.DisplayType != PARAMDEF.DefType.dummy8)
                        {
                            // At end of cells, don't add delimiter
                            if (row.Cells.Count == cell_idx)
                                composed_line = composed_line + cell.Value.ToString();
                            else
                                composed_line = composed_line + cell.Value.ToString() + settings.ExportDelimiter;
                        }

                        cell_idx++;
                    }

                    char[] charsToTrim = settings.ExportDelimiter.ToCharArray();

                    //composed_line = composed_line.TrimEnd(charsToTrim);

                    output_file.WriteLine(composed_line);
                }
            }
            output_file.Close();

            if (Yapped_Rune_Bear.Properties.Settings.Default.UseTextEditor && Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath != "" && !isSilent)
            {
                try
                {
                    Process.Start("\"" + Yapped_Rune_Bear.Properties.Settings.Default.TextEditorPath + "\"", "\"" + Application.StartupPath + "\\" + paramPath + "\"");
                }
                catch
                {
                    SystemSounds.Hand.Play();
                }

            }

            if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages && !isSilent)
                MessageBox.Show($@"{paramName} data export complete!", "Data Export");
        }

        #endregion

        #region dgvParams
        private void dgvParams_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

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
                settings.SelectedParam = dgvParams.SelectedCells[0].RowIndex;

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
                settings.SelectedRow = dgvRows.SelectedCells[0].RowIndex;

                ParamWrapper paramFile = (ParamWrapper)dgvParams.SelectedCells[0].OwningRow.DataBoundItem;
                (int Row, int Cell) indices = dgvIndices[paramFile.Name];
                if (dgvCells.FirstDisplayedScrollingRowIndex >= 0)
                    indices.Cell = dgvCells.FirstDisplayedScrollingRowIndex;

                PARAM.Row row = (PARAM.Row)dgvRows.SelectedCells[0].OwningRow.DataBoundItem;

                // This is what populates the field section
                dgvCells.DataSource = row.Cells.Where(cell => cell.Def.DisplayType != PARAMDEF.DefType.dummy8).ToArray();

                if (indices.Cell >= dgvCells.RowCount)
                    indices.Cell = dgvCells.RowCount - 1;

                if (indices.Cell < 0)
                    indices.Cell = 0;

                dgvIndices[paramFile.Name] = indices;
                if (dgvCells.RowCount > 0)
                    dgvCells.FirstDisplayedScrollingRowIndex = indices.Cell;

                // Iterate through field rows
                for (int k = 0; k < dgvCells.Rows.Count; k++)
                {
                    var cell = (PARAM.Cell)dgvCells.Rows[k].DataBoundItem;

                    // Only display the combo box if ShowEnums is enabled
                    // Disable enum combo box in Param Difference mode
                    if (Yapped_Rune_Bear.Properties.Settings.Default.ShowEnums && !Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode)
                    {
                        // If InternalType matches a TDF type, use that TDF to populate the enum combo box
                        if (tdf_dict.ContainsKey(cell.Def.InternalType))
                        {
                            var tdf = tdf_dict[cell.Def.InternalType];
                            var enum_dict = new Dictionary<object, string>();

                            foreach (PARAMTDF.Entry entry in tdf.Entries)
                            {
                                //Utility.DebugPrint(entry.Name + " - " + (entry.Value.ToString()));

                                if (Yapped_Rune_Bear.Properties.Settings.Default.ShowEnumValueInName)
                                {
                                    enum_dict.Add(entry.Value, (entry.Value.ToString() + " - " + entry.Name));
                                }
                                else
                                {
                                    enum_dict.Add(entry.Value, entry.Name);
                                }
                            }

                            // Only show combo box if value is within enum, otherwise leave field as is
                            if (enum_dict.ContainsKey(cell.Value))
                            {
                                // Apply checkbox
                                if (bool_type_tdfs.Contains(tdf.Name) && Yapped_Rune_Bear.Properties.Settings.Default.DisplayBooleanEnumAsCheckbox)
                                {
                                    dgvCells.Rows[k].Cells[FIELD_VALUE_COL] = new DataGridViewCheckBoxCell
                                    {
                                        TrueValue = "1",
                                        FalseValue = "0",
                                        ValueType = cell.Value.GetType()
                                    };
                                }
                                // Apply combo box
                                else
                                {
                                    if (Yapped_Rune_Bear.Properties.Settings.Default.DisableEnumForCustomTypes)
                                    {
                                        if (!custom_type_tdfs.Contains(tdf.Name))
                                        {
                                            dgvCells.Rows[k].Cells[FIELD_VALUE_COL] = new DataGridViewComboBoxCell
                                            {
                                                DataSource = enum_dict.ToArray(),
                                                DisplayMember = "Value",
                                                ValueMember = "Key",
                                                ValueType = cell.Value.GetType()
                                            };
                                        }
                                    }
                                    else
                                    {
                                        dgvCells.Rows[k].Cells[FIELD_VALUE_COL] = new DataGridViewComboBoxCell
                                        {
                                            DataSource = enum_dict.ToArray(),
                                            DisplayMember = "Value",
                                            ValueMember = "Key",
                                            ValueType = cell.Value.GetType()
                                        };
                                    }
                                }
                            }
                        }
                    }
                }

                // Apply field coloring
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

                for (int j = 0; j < dgvCells.Rows.Count; j++)
                {
                    var cell = dgvCells.Rows[j];
                    var type = cell.Cells[FIELD_TYPE_COL].Value.ToString();

                    // Default
                    cell.Cells[2].Style.BackColor = Color.White;

                    // Don't apply in Param Difference mode
                    if (!Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode)
                    {
                        if (type.Contains("BOOL") || type.Contains("ON_OFF"))
                        {
                            cell.Cells[2].Style.BackColor = bool_color;
                        }
                        else if (type == "u32" || type == "s32" || type == "u16" || type == "s16" || type == "u8" || type == "s8")
                        {
                            cell.Cells[2].Style.BackColor = int_color;
                        }
                        else if (type == "f32")
                        {
                            cell.Cells[2].Style.BackColor = float_color;
                        }
                        else if (type == "fixStr" || type == "fixStrW")
                        {
                            cell.Cells[2].Style.BackColor = string_color;
                        }
                        else
                        {
                            // Covers the enum ints
                            cell.Cells[2].Style.BackColor = int_color;
                        }
                    }

                    // Param Difference mode coloring
                    if (Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode)
                    {
                        if (secondary_result != null)
                        {
                            foreach (ParamWrapper secondary_wrapper in secondary_result.ParamWrappers)
                            {
                                // Only check the current param
                                if (secondary_wrapper.Name == paramFile.Name)
                                {
                                    foreach (PARAM.Row secondary_row in secondary_wrapper.Rows)
                                    {
                                        if (row != null)
                                        {
                                            // Only check current row
                                            if (secondary_row.ID == row.ID)
                                            {
                                                // Account for the abscense of dummy8 within the dgv
                                                var dgvOffset = 0;

                                                for (int p = 0; p < secondary_row.Cells.Count; p++)
                                                {
                                                    var primary_cell = row.Cells[p];
                                                    var secondary_cell = secondary_row.Cells[p];

                                                    if (primary_cell.Def.DisplayType != PARAMDEF.DefType.dummy8)
                                                    {
                                                        if (!primary_cell.Value.Equals(secondary_cell.Value))
                                                        {
                                                            dgvCells.Rows[(p - dgvOffset)].Cells[2].Style.BackColor = Color.Yellow;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        dgvOffset++;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                ApplyCellFilter(false);
            }
        }

        private void DgvRows_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // ID Column
            if (e.ColumnIndex == ROW_ID_COL)
            {
                bool parsed = int.TryParse((string)e.FormattedValue, out int value);
                if (!parsed || value < 0)
                {
                    Utility.ShowError("Row ID must be a positive integer.\r\nEnter a valid number or press Esc to cancel.");
                    e.Cancel = true;
                }
            }
        }

        private void dgvRows_Scroll(object sender, ScrollEventArgs e)
        {
            rowContextMenu.Close();
        }
        #endregion

        #region Row Context Menu
        private void dgvRows_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Do not allow this if invalidation state is active
            if (InvalidationMode)
                return;

            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                }

                DataGridViewCell currentCell = (sender as DataGridView).CurrentCell;
                if (currentCell != null)
                {
                    ContextMenuStrip cms = currentCell.ContextMenuStrip;
                    if (cms != null)
                    {
                        Rectangle r = currentCell.DataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);
                        Point p = new Point(r.X + r.Width, r.Y + r.Height);
                        cms.Show(currentCell.DataGridView, p);
                    }
                }
            }
        }

        private string ATKPARAM_PC = "AtkParam_Pc";
        private string ATKPARAM_NPC = "AtkParam_Npc";
        private string BEHAVIORPARAM_PC = "BehaviorParam_PC";
        private string BEHAVIORPARAM_NPC = "BehaviorParam";

        private void dgvRows_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            // Do not allow this if invalidation state is active
            if (InvalidationMode)
                return;

            DataGridView dgv = (DataGridView)sender;

            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            var row_param_name = dgvParams.CurrentCell.Value.ToString();

            if (row_param_name == ATKPARAM_PC || row_param_name == ATKPARAM_NPC || row_param_name == BEHAVIORPARAM_PC || row_param_name == BEHAVIORPARAM_NPC)
            {
                e.ContextMenuStrip = rowContextMenu;
            }
        }

        private void rowContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var row = (PARAM.Row)dgvRows.Rows[dgvRows.CurrentCell.RowIndex].DataBoundItem;
            var row_param_name = dgvParams.CurrentCell.Value.ToString();

            copyToParamMenuItem.Visible = false;
            copyToParamMenuItem.Text = "";

            if (row_param_name == ATKPARAM_PC)
            {
                copyToParamMenuItem.Visible = true;
                copyToParamMenuItem.Text = $@"Copy {row.ID.ToString()} to {ATKPARAM_NPC}.";
            }
            else if (row_param_name == ATKPARAM_NPC)
            {
                copyToParamMenuItem.Visible = true;
                copyToParamMenuItem.Text = $@"Copy {row.ID.ToString()} to {ATKPARAM_PC}.";
            }
            else if (row_param_name == BEHAVIORPARAM_PC)
            {
                copyToParamMenuItem.Visible = true;
                copyToParamMenuItem.Text = $@"Copy {row.ID.ToString()} to {BEHAVIORPARAM_NPC}.";
            }
            else if (row_param_name == BEHAVIORPARAM_NPC)
            {
                copyToParamMenuItem.Visible = true;
                copyToParamMenuItem.Text = $@"Copy {row.ID.ToString()} to {BEHAVIORPARAM_PC}.";
            }
        }

        private void copyToParamMenuItem_Click(object sender, EventArgs e)
        {
            var row = (PARAM.Row)dgvRows.Rows[dgvRows.CurrentCell.RowIndex].DataBoundItem;
            var current_param_name = dgvParams.CurrentCell.Value.ToString();
            var target_param_name = "";

            ParamWrapper target_wrapper = null;

            // Select target param
            if (current_param_name == ATKPARAM_PC)
                target_param_name = ATKPARAM_NPC;
            else if (current_param_name == ATKPARAM_NPC)
                target_param_name = ATKPARAM_PC;
            else if (current_param_name == BEHAVIORPARAM_PC)
                target_param_name = BEHAVIORPARAM_NPC;
            else if (current_param_name == BEHAVIORPARAM_NPC)
                target_param_name = BEHAVIORPARAM_PC;

            // Select target wrapper
            foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
            {
                if (wrapper.Name == target_param_name)
                {
                    target_wrapper = wrapper;
                }
            }

            var newRowForm = new NewRow("New Row", row.ID, row.Name);
            if (newRowForm.ShowDialog() == DialogResult.OK)
            {
                int id = newRowForm.ResultID;
                string name = newRowForm.ResultName;

                // Add new row to target param
                if (target_wrapper.Rows.Any(wrapper_row => row.ID == id))
                {
                    Utility.ShowError($"A row with this ID already exists: {id}");
                    return;
                }
                else
                {
                    PARAM.Row row_result = new PARAM.Row(id, name, target_wrapper.AppliedParamDef);

                    for (int k = 0; k < row.Cells.Count; k++)
                    {
                        row_result.Cells[k].Value = row.Cells[k].Value;
                    }

                    target_wrapper.Rows.Add(row_result);
                    target_wrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));
                }

                int i;
                int j;
                int target_param_idx = 0;
                int target_row_idx = 0;

                // Go to target param
                for (i = 0; i < dgvParams.Rows.Count; i++)
                {
                    var param_row = dgvParams.Rows[i];
                    var param_name = param_row.Cells[0].Value.ToString();

                    if (param_name == target_param_name)
                    {
                        target_param_idx = i;

                        dgvParams.ClearSelection();
                        dgvParams.Rows[target_param_idx].Selected = true;
                    }
                }

                // Go to new row
                for (j = 0; j < dgvRows.Rows.Count; j++)
                {
                    var target_row = dgvRows.Rows[j];
                    int value = Convert.ToInt32(target_row.Cells[0].Value);

                    if (id == value)
                    {
                        target_row_idx = j;

                        dgvRows.ClearSelection();
                        dgvRows.Rows[target_row_idx].Selected = true;
                        dgvRows.CurrentCell = dgvRows.Rows[target_row_idx].Cells[0];
                    }
                }
            }
        }
        #endregion

        #region dgvCells

        private void dgvCells_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvCells.SelectedCells.Count > 0)
            {
                settings.SelectedField = dgvCells.SelectedCells[0].RowIndex;
            }
        }

        private void dgvCells_Scroll(object sender, ScrollEventArgs e)
        {
            fieldContextMenu.Close();
        }

        private void DgvCells_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowEditorNames)
            {
                dgvCells.Columns[FIELD_PARAM_NAME_COL].Visible = false;
                dgvCells.Columns[FIELD_EDITOR_NAME_COL].Visible = true;
            }
            else
            {
                dgvCells.Columns[FIELD_PARAM_NAME_COL].Visible = true;
                dgvCells.Columns[FIELD_EDITOR_NAME_COL].Visible = false;
            }

            if (Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowTypes)
            {
                dgvCells.Columns[FIELD_TYPE_COL].Visible = true;
                dgvCells.Columns[FIELD_TYPE_COL].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

                dgvCells.Columns[FIELD_VALUE_COL].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            }
            else
            {
                dgvCells.Columns[FIELD_TYPE_COL].Visible = false;
                dgvCells.Columns[FIELD_TYPE_COL].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;

                dgvCells.Columns[FIELD_VALUE_COL].AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            }
        }

        private void DgvCells_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {

        }

        private void DgvCells_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (Yapped_Rune_Bear.Properties.Settings.Default.EnableFieldValidation)
            {
                // Value Column
                if (e.ColumnIndex == FIELD_VALUE_COL)
                {
                    DataGridViewRow row = dgvCells.Rows[e.RowIndex];
                    var cell = (PARAM.Cell)row.DataBoundItem;
                    bool CheckValue = true;

                    // Ignore value validation if string type
                    if (cell.Def.DisplayType == PARAMDEF.DefType.fixstr || cell.Def.DisplayType == PARAMDEF.DefType.fixstrW)
                    {
                        CheckValue = false;
                    }

                    // Only check this if enums are enabled
                    if (Yapped_Rune_Bear.Properties.Settings.Default.ShowEnums)
                    {
                        // If InternalType matches a TDF type, skip value validation
                        if (tdf_dict.ContainsKey(cell.Def.InternalType))
                        {
                            CheckValue = false;
                        }
                    }

                    if (CheckValue)
                    {
                        float i = 0;
                        bool value_result = float.TryParse(e.FormattedValue.ToString(), out i);

                        if (value_result)
                        {
                            // Check min and max
                            float current_value = Convert.ToSingle(e.FormattedValue);
                            float value_min = Convert.ToSingle(cell.Def.Minimum);
                            float value_max = Convert.ToSingle(cell.Def.Maximum);

                            if (current_value > value_max || current_value < value_min)
                            {
                                e.Cancel = true;

                                EnterInvalidationMode();

                                if (dgvCells.EditingPanel != null)
                                {
                                    dgvCells.EditingPanel.BackColor = Color.Pink;
                                    dgvCells.EditingControl.BackColor = Color.Pink;
                                }
                                SystemSounds.Hand.Play();
                            }
                            else
                            {
                                ExitInvalidationMode();
                            }
                        }
                        else
                        {
                            e.Cancel = true;

                            EnterInvalidationMode();

                            if (dgvCells.EditingPanel != null)
                            {
                                dgvCells.EditingPanel.BackColor = Color.Pink;
                                dgvCells.EditingControl.BackColor = Color.Pink;
                            }
                            SystemSounds.Hand.Play();
                        }
                    }

                }
            }
        }

        private void EnterInvalidationMode()
        {
            InvalidationMode = true;
            dgvParams.Enabled = false;
            dgvRows.Enabled = false;
            fileToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            viewToolStripMenuItem.Enabled = false;
            ToolStripMenuItem.Enabled = false;
            WorkflowToolStripMenuItem.Enabled = false;
            settingsMenuItem.Enabled = false;

            // Filters
            filter_Params.Enabled = false;
            button_FilterParams.Enabled = false;
            button_ResetFilterParams.Enabled = false;
            filter_Rows.Enabled = false;
            button_FilterRows.Enabled = false;
            button_ResetFilterRows.Enabled = false;
            filter_Cells.Enabled = false;
            button_FilterCells.Enabled = false;
            button_ResetFilterCells.Enabled = false;
        }

        private void ExitInvalidationMode()
        {
            InvalidationMode = false;
            dgvParams.Enabled = true;
            dgvRows.Enabled = true;
            fileToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            viewToolStripMenuItem.Enabled = true;
            ToolStripMenuItem.Enabled = true;
            WorkflowToolStripMenuItem.Enabled = true;
            settingsMenuItem.Enabled = true;

            // Filters
            filter_Params.Enabled = true;
            button_FilterParams.Enabled = true;
            button_ResetFilterParams.Enabled = true;
            filter_Rows.Enabled = true;
            button_FilterRows.Enabled = true;
            button_ResetFilterRows.Enabled = true;
            filter_Cells.Enabled = true;
            button_FilterCells.Enabled = true;
            button_ResetFilterCells.Enabled = true;
        }

        private void DgvCells_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {

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
            // Field Name columns only
            if (e.ColumnIndex == FIELD_PARAM_NAME_COL || e.ColumnIndex == FIELD_EDITOR_NAME_COL)
            {
                if (e.RowIndex >= 0)
                {
                    if (Yapped_Rune_Bear.Properties.Settings.Default.ShowFieldDescriptions)
                    {
                        var cell = (PARAM.Cell)dgvCells.Rows[e.RowIndex].DataBoundItem;

                        e.ToolTipText = cell.Def.Description
                            + "\nMinimum: " + cell.Def.Minimum
                            + "\nMaximum: " + cell.Def.Maximum
                            + "\nIncrement: " + cell.Def.Increment;
                    }
                }
            }

            // Value
            if (e.ColumnIndex == FIELD_VALUE_COL)
            {
                if (e.RowIndex >= 0)
                {
                    if (dgvRows.CurrentCell != null)
                    {
                        var cell = (PARAM.Cell)dgvCells.Rows[e.RowIndex].DataBoundItem;
                        PARAM.Row current_row = (PARAM.Row)dgvRows.Rows[dgvRows.CurrentCell.RowIndex].DataBoundItem;

                        int current_row_id = current_row.ID;

                        string tooltip = "";
                        bool behaviorRow_FirstOnly = false;

                        var cell_name = cell.Name.ToString();
                        var cell_value = cell.Value;

                        if (cell.ParamRef1 != null)
                        {
                            foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
                            {
                                if (cell.ParamRef1 != null)
                                {
                                    // Param name matches reference string
                                    if (wrapper.Name.ToString() == cell.ParamRef1.ToString())
                                    {
                                        // Go through rows in target param
                                        foreach (PARAM.Row row in wrapper.Rows)
                                        {
                                            var row_id = row.ID.ToString();
                                            var row_name = (row.Name == null) ? "" : row.Name.ToString();

                                            // If current cell is a behavior reference, only display the first match
                                            if (cell_name == "behaviorVariationId")
                                            {
                                                // Only use first matched row, as variationId will match with the whole range
                                                if (!behaviorRow_FirstOnly)
                                                {
                                                    foreach (PARAM.Cell field in row.Cells)
                                                    {
                                                        // Check value for variationId
                                                        if (field.Def.InternalName == "variationId")
                                                        {
                                                            int value = Convert.ToInt32(field.Value);

                                                            // Cell value matches variationId field
                                                            if (value == Convert.ToInt32(cell_value))
                                                            {
                                                                tooltip = tooltip + $@"{row_id} {row_name}" + "\n";
                                                                behaviorRow_FirstOnly = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            // If current cell is a normal reference, display all matches
                                            else
                                            {
                                                // Cell value matches row ID
                                                if (row.ID == Convert.ToInt32(cell_value))
                                                {
                                                    tooltip = tooltip + $@"{row_id} {row_name}" + "\n";
                                                }
                                            }
                                        }
                                    }
                                }
                                if (cell.ParamRef2 != null)
                                {
                                    if (wrapper.Name.ToString() == cell.ParamRef2.ToString())
                                    {
                                        foreach (PARAM.Row row in wrapper.Rows)
                                        {
                                            var row_id = row.ID.ToString();
                                            var row_name = (row.Name == null) ? "" : row.Name.ToString();

                                            // Cell value matches row ID
                                            if (row.ID == Convert.ToInt32(cell_value))
                                            {
                                                tooltip = tooltip + $@"{row_id} {row_name}" + "\n";
                                            }
                                        }
                                    }
                                }
                                if (cell.ParamRef3 != null)
                                {
                                    if (wrapper.Name.ToString() == cell.ParamRef3.ToString())
                                    {
                                        foreach (PARAM.Row row in wrapper.Rows)
                                        {
                                            var row_id = row.ID.ToString();
                                            var row_name = (row.Name == null) ? "" : row.Name.ToString();

                                            // Cell value matches row ID
                                            if (row.ID == Convert.ToInt32(cell_value))
                                            {
                                                tooltip = tooltip + $@"{row_id} {row_name}" + "\n";
                                            }
                                        }
                                    }
                                }
                                if (cell.ParamRef4 != null)
                                {
                                    if (wrapper.Name.ToString() == cell.ParamRef4.ToString())
                                    {
                                        foreach (PARAM.Row row in wrapper.Rows)
                                        {
                                            var row_id = row.ID.ToString();
                                            var row_name = (row.Name == null) ? "" : row.Name.ToString();

                                            // Cell value matches row ID
                                            if (row.ID == Convert.ToInt32(cell_value))
                                            {
                                                tooltip = tooltip + $@"{row_id} {row_name}" + "\n";
                                            }
                                        }
                                    }
                                }
                                if (cell.ParamRef5 != null)
                                {
                                    if (wrapper.Name.ToString() == cell.ParamRef5.ToString())
                                    {
                                        foreach (PARAM.Row row in wrapper.Rows)
                                        {
                                            var row_id = row.ID.ToString();
                                            var row_name = (row.Name == null) ? "" : row.Name.ToString();

                                            // Cell value matches row ID
                                            if (row.ID == Convert.ToInt32(cell_value))
                                            {
                                                tooltip = tooltip + $@"{row_id} {row_name}" + "\n";
                                            }
                                        }
                                    }
                                }
                                if (cell.ParamRef6 != null)
                                {
                                    if (wrapper.Name.ToString() == cell.ParamRef6.ToString())
                                    {
                                        foreach (PARAM.Row row in wrapper.Rows)
                                        {
                                            var row_id = row.ID.ToString();
                                            var row_name = (row.Name == null) ? "" : row.Name.ToString();

                                            // Cell value matches row ID
                                            if (row.ID == Convert.ToInt32(cell_value))
                                            {
                                                tooltip = tooltip + $@"{row_id} {row_name}" + "\n";
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // Param Difference mode coloring
                        if (Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode)
                        {
                            var current_param_name = dgvParams.CurrentCell.Value.ToString();

                            if (secondary_result != null)
                            {
                                foreach (ParamWrapper secondary_wrapper in secondary_result.ParamWrappers)
                                {
                                    // Only check the current param
                                    if (secondary_wrapper.Name == current_param_name)
                                    {
                                        foreach (PARAM.Row secondary_row in secondary_wrapper.Rows)
                                        {
                                            if (current_row != null)
                                            {
                                                // Only check current row
                                                if (secondary_row.ID == current_row.ID)
                                                {
                                                    var primary_cell = cell;

                                                    for (int p = 0; p < secondary_row.Cells.Count; p++)
                                                    {
                                                        var secondary_cell = secondary_row.Cells[p];

                                                        if (primary_cell.Def.DisplayName == secondary_cell.Def.DisplayName)
                                                        {
                                                            if (primary_cell.Def.DisplayType != PARAMDEF.DefType.dummy8)
                                                            {
                                                                if (!primary_cell.Value.Equals(secondary_cell.Value))
                                                                {
                                                                    tooltip = tooltip + $@"Secondary Value: {secondary_cell.Value}" + "\n";
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        e.ToolTipText = tooltip;

                    }
                }
            }
        }

        #endregion

        #region Field Context Menu
        private void dgvCells_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Do not allow this if invalidation state is active
            if (InvalidationMode)
                return;

            if (e.ColumnIndex != -1 && e.RowIndex != -1 && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                DataGridViewCell c = (sender as DataGridView)[e.ColumnIndex, e.RowIndex];
                if (!c.Selected)
                {
                    c.DataGridView.ClearSelection();
                    c.DataGridView.CurrentCell = c;
                    c.Selected = true;
                }

                DataGridViewCell currentCell = (sender as DataGridView).CurrentCell;
                if (currentCell != null)
                {
                    ContextMenuStrip cms = currentCell.ContextMenuStrip;
                    if (cms != null)
                    {
                        Rectangle r = currentCell.DataGridView.GetCellDisplayRectangle(currentCell.ColumnIndex, currentCell.RowIndex, false);
                        Point p = new Point(r.X + r.Width, r.Y + r.Height);
                        cms.Show(currentCell.DataGridView, p);
                    }
                }
            }
        }

        private void dgvCells_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            // Do not allow this if invalidation state is active
            if (InvalidationMode)
                return;

            DataGridView dgv = (DataGridView)sender;

            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            var cell = (PARAM.Cell)dgvCells.Rows[e.RowIndex].DataBoundItem;

            // Only add "Go to Reference" for fields with atleast 1 reference
            if (cell.ParamRef1 != null)
            {
                int cell_value_id = Convert.ToInt32(cell.Value);

                // Only show for actual rows
                if (cell_value_id > -1)
                    e.ContextMenuStrip = fieldContextMenu;
            }
        }

        private void fieldContextMenu_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;

            int cell_value_id = Convert.ToInt32(cell.Value);

            bool behaviorRow_FirstOnly = false;

            GotoReference1MenuItem.Text = "";
            GotoReference2MenuItem.Text = "";
            GotoReference3MenuItem.Text = "";
            GotoReference4MenuItem.Text = "";
            GotoReference5MenuItem.Text = "";
            GotoReference6MenuItem.Text = "";

            GotoReference1MenuItem.Visible = false;
            GotoReference2MenuItem.Visible = false;
            GotoReference3MenuItem.Visible = false;
            GotoReference4MenuItem.Visible = false;
            GotoReference5MenuItem.Visible = false;
            GotoReference6MenuItem.Visible = false;

            foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
            {
                if (cell.ParamRef1 != null)
                {
                    if (wrapper.Name.ToString() == cell.ParamRef1.ToString())
                    {
                        foreach (PARAM.Row row in wrapper.Rows)
                        {
                            // Special Case for Behavior references - Only required for ParamRef1
                            if (cell.Name.ToString() == "behaviorVariationId" || cell.Name.ToString() == "Behavior Variation ID")
                            {
                                // Only use first matched row, as variationId will match with the whole range
                                if (!behaviorRow_FirstOnly)
                                {
                                    foreach (PARAM.Cell behavior_cell in row.Cells)
                                    {
                                        // Check value for variationId
                                        if (behavior_cell.Def.InternalName == "variationId")
                                        {
                                            int value = Convert.ToInt32(behavior_cell.Value);

                                            if (value == cell_value_id)
                                            {
                                                GotoReference1MenuItem.Visible = true;
                                                GotoReference1MenuItem.Text = $@"Go to row {row.ID.ToString()} in {cell.ParamRef1.ToString()}";
                                                behaviorRow_FirstOnly = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (row.ID == cell_value_id)
                                {
                                    GotoReference1MenuItem.Visible = true;
                                    GotoReference1MenuItem.Text = $@"Go to row {row.ID.ToString()} in {cell.ParamRef1.ToString()}";
                                }
                            }
                        }
                    }
                }
                if (cell.ParamRef2 != null)
                {
                    if (wrapper.Name.ToString() == cell.ParamRef2.ToString())
                    {
                        foreach (PARAM.Row row in wrapper.Rows)
                        {
                            if (row.ID == cell_value_id)
                            {
                                GotoReference2MenuItem.Visible = true;
                                GotoReference2MenuItem.Text = $@"Go to row {row.ID.ToString()} in {cell.ParamRef2.ToString()}";
                            }
                        }
                    }
                }
                if (cell.ParamRef3 != null)
                {
                    if (wrapper.Name.ToString() == cell.ParamRef3.ToString())
                    {
                        foreach (PARAM.Row row in wrapper.Rows)
                        {
                            if (row.ID == cell_value_id)
                            {
                                GotoReference3MenuItem.Visible = true;
                                GotoReference3MenuItem.Text = $@"Go to row {row.ID.ToString()} in {cell.ParamRef3.ToString()}";
                            }
                        }
                    }
                }
                if (cell.ParamRef4 != null)
                {
                    if (wrapper.Name.ToString() == cell.ParamRef4.ToString())
                    {
                        foreach (PARAM.Row row in wrapper.Rows)
                        {
                            if (row.ID == cell_value_id)
                            {
                                GotoReference4MenuItem.Visible = true;
                                GotoReference4MenuItem.Text = $@"Go to row {row.ID.ToString()} in {cell.ParamRef4.ToString()}";
                            }
                        }
                    }
                }
                if (cell.ParamRef5 != null)
                {
                    if (wrapper.Name.ToString() == cell.ParamRef5.ToString())
                    {
                        foreach (PARAM.Row row in wrapper.Rows)
                        {
                            if (row.ID == cell_value_id)
                            {
                                GotoReference5MenuItem.Visible = true;
                                GotoReference5MenuItem.Text = $@"Go to row {row.ID.ToString()} in {cell.ParamRef5.ToString()}";
                            }
                        }
                    }
                }
                if (cell.ParamRef6 != null)
                {
                    if (wrapper.Name.ToString() == cell.ParamRef6.ToString())
                    {
                        foreach (PARAM.Row row in wrapper.Rows)
                        {
                            if (row.ID == cell_value_id)
                            {
                                GotoReference6MenuItem.Visible = true;
                                GotoReference6MenuItem.Text = $@"Go to row {row.ID.ToString()} in {cell.ParamRef6.ToString()}";
                            }
                        }
                    }
                }
            }
        }

        private void GotoReferenceHelper(string paramref)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;
            var cell_value_id = Convert.ToInt32(cell.Value);

            bool isBehaviorReference = (cell.Name.ToString() == "behaviorVariationId" || cell.Name.ToString() == "Behavior Variation ID");

            int i;
            int j;
            //int k;

            int target_param_idx = 0;
            int target_row_idx = 0;

            for (i = 0; i < dgvParams.Rows.Count; i++)
            {
                var param_row = dgvParams.Rows[i];
                var name = param_row.Cells[0].Value.ToString();

                if (paramref == name)
                {
                    target_param_idx = i;

                    dgvParams.ClearSelection();
                    dgvParams.Rows[target_param_idx].Selected = true;
                }
            }

            // Special Case for Behavior references - Only required for ParamRef1
            if (isBehaviorReference)
            {
                int target_row = 0;
                bool isBehaviorMatched = false;

                // Get the row ID via a Behavior Variation field check
                foreach (ParamWrapper wrapper in primary_result.ParamWrappers)
                {
                    if (wrapper.Name.ToString() == cell.ParamRef1.ToString() && !isBehaviorMatched)
                    {
                        foreach (PARAM.Row wrapper_row in wrapper.Rows)
                        {
                            if (!isBehaviorMatched)
                            {
                                foreach (PARAM.Cell wrapper_cell in wrapper_row.Cells)
                                {
                                    if (!isBehaviorMatched)
                                    {
                                        // These need to match the Paramdex
                                        if (wrapper_cell.Name.ToString() == "variationId" || wrapper_cell.EditorName.ToString() == "Variation ID")
                                        {
                                            if (cell_value_id == Convert.ToInt32(wrapper_cell.Value))
                                            {
                                                target_row = wrapper_row.ID;
                                                isBehaviorMatched = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // Apply the found row ID
                for (j = 0; j < dgvRows.Rows.Count; j++)
                {
                    var row = dgvRows.Rows[j];
                    int value = Convert.ToInt32(row.Cells[0].Value);

                    if (target_row == value)
                    {
                        target_row_idx = j;

                        dgvRows.ClearSelection();
                        dgvRows.Rows[target_row_idx].Selected = true;
                        dgvRows.CurrentCell = dgvRows.Rows[target_row_idx].Cells[0];
                    }
                }
            }
            else
            {
                for (j = 0; j < dgvRows.Rows.Count; j++)
                {
                    var row = dgvRows.Rows[j];
                    int value = Convert.ToInt32(row.Cells[0].Value);

                    if (cell_value_id == value)
                    {
                        target_row_idx = j;

                        dgvRows.ClearSelection();
                        dgvRows.Rows[target_row_idx].Selected = true;
                        dgvRows.CurrentCell = dgvRows.Rows[target_row_idx].Cells[0];
                    }
                }
            }
        }

        private void GotoReference1MenuItem_Click(object sender, EventArgs e)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;
            GotoReferenceHelper(cell.ParamRef1.ToString());
        }

        private void GotoReference2MenuItem_Click(object sender, EventArgs e)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;
            GotoReferenceHelper(cell.ParamRef2.ToString());
        }

        private void GotoReference3MenuItem_Click(object sender, EventArgs e)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;
            GotoReferenceHelper(cell.ParamRef3.ToString());
        }

        private void GotoReference4MenuItem_Click(object sender, EventArgs e)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;
            GotoReferenceHelper(cell.ParamRef4.ToString());
        }

        private void GotoReference5MenuItem_Click(object sender, EventArgs e)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;
            GotoReferenceHelper(cell.ParamRef5.ToString());
        }
        private void GotoReference6MenuItem_Click(object sender, EventArgs e)
        {
            var cell = (PARAM.Cell)dgvCells.Rows[dgvCells.CurrentCell.RowIndex].DataBoundItem;
            GotoReferenceHelper(cell.ParamRef6.ToString());
        }

        #endregion

        #region Interface Settings
        private void viewInterfaceSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var newInterfaceSettings = new InterfaceSettings();

            if (newInterfaceSettings.ShowDialog() == DialogResult.OK)
            {
                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Interace Settings changed.", "Settings", MessageBoxButtons.OK);
            }
        }

        #endregion

        #region Data Settings
        private void viewDataSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var newDataSettings = new DataSettings();

            if (newDataSettings.ShowDialog() == DialogResult.OK)
            {
                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Data Settings changed.", "Settings", MessageBoxButtons.OK);
            }
        }

        #endregion

        #region Filter Settings
        private void viewFilterSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this change if invalidation state is active
            if (InvalidationMode)
                return;

            var newFilterSettings = new FilterSettings();

            if (newFilterSettings.ShowDialog() == DialogResult.OK)
            {
                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Filter Settings changed.", "Settings", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region File Data
        private void selectSecondaryFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this if invalidation state is active
            if (InvalidationMode)
                return;

            secondaryFilePath.FileName = "";

            if (secondaryFilePath.ShowDialog() == DialogResult.OK)
            {
                Yapped_Rune_Bear.Properties.Settings.Default.SecondaryFilePath = secondaryFilePath.FileName;
                LoadSecondaryParams(false);
            }
        }

        private void showParamDifferencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Do not allow this if invalidation state is active
            if (InvalidationMode)
                return;

            if (secondary_result == null)
            {
                MessageBox.Show("Secondary File not set.", "Secondary File", MessageBoxButtons.OK);
                return;
            }

            // Toggle state
            if (Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode)
            {
                Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode = false;

                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Param Difference mode coloring removed", "Param Difference Mode", MessageBoxButtons.OK);
            }
            else
            {
                Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode = true;

                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Param Difference mode coloring added", "Param Difference Mode", MessageBoxButtons.OK);
            }

        }

        private void clearSecondaryFileToolMenuItem_Click(object sender, EventArgs e)
        {
            if (secondaryFilePath.FileName == "")
            {
                MessageBox.Show("Secondary File not set.", "Secondary File", MessageBoxButtons.OK);
                return;
            }
            else
            {
                secondaryFilePath.FileName = "";
                Yapped_Rune_Bear.Properties.Settings.Default.SecondaryFilePath = secondaryFilePath.FileName;
                Yapped_Rune_Bear.Properties.Settings.Default.ParamDifferenceMode = false;

                if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                    MessageBox.Show("Removed set secondary file path.", "Secondary File", MessageBoxButtons.OK);
            }
        }
        #endregion

        #region Param Filter
        private void button_FilterParams_Click(object sender, EventArgs e)
        {
            char[] command_delimiter = Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter.ToCharArray();
            char[] section_delimiter = Yapped_Rune_Bear.Properties.Settings.Default.Filter_SectionDelimiter.ToCharArray();
            string command_delimiter_string = Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter;

            // Return if row count is 0
            if (dgvParams.Rows.Count == 0)
                return;

            // Disable normal autosize modes
            this.dgvParamsParamCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;

            // Disable user interaction
            EnterInvalidationMode();

            // Build input list
            string[] input_list = filter_Params.Text.ToLower().Split(section_delimiter);

            // Return if no actual command is within the list
            if (input_list[0].Length < 1)
            {
                Utility.ShowError("No filter command present.");
                ExitInvalidationMode();
                return;
            }

            bool hasSelectedParam = false;

            if (dgvRows.Rows.Count > 0)
                dgvRows.Rows[0].Selected = true;

            if (dgvCells.Rows.Count > 0)
                dgvCells.Rows[0].Selected = true;

            // Hide all rows - This is so the following section can apply visibility without worrying about invisibility
            for (int i = 0; i < dgvParams.Rows.Count; i++)
            {
                var param = dgvParams.Rows[i];
                var param_name = param.Cells[0].Value.ToString().ToLower();

                // Hide the row by default
                CurrencyManager currencyManager = (CurrencyManager)BindingContext[dgvParams.DataSource];
                currencyManager.SuspendBinding();
                param.Visible = false;
                param.Selected = false;
                currencyManager.ResumeBinding();

                // This determines whether to make the row visible.
                // Each input section is checked, and if all of them result in true, then the row is made visible
                List<bool> truth_list = new List<bool>();

                // Apply each input section, this allows for multiple commands to be chained
                for (int j = 0; j < input_list.Length; j++)
                {
                    truth_list.Add(false);
                    string current_input = input_list[j];

                    // View filter
                    if (current_input.Contains("view" + command_delimiter_string))
                    {
                        string[] temp_input = current_input.Split(command_delimiter);
                        current_input = temp_input[1].TrimStart(' ').ToLower();

                        List<string> view_list = BuildViewList($@"Views\\Param\\", current_input);

                        if (view_list.Count > 0)
                        {
                            // Show if within filter list
                            foreach (string view_name in view_list)
                            {
                                if (param_name.Contains(view_name))
                                {
                                    truth_list[j] = true;
                                }
                            }
                        }
                    }
                    // Exact
                    else if (current_input.Contains("exact" + command_delimiter_string))
                    {
                        string[] temp_input = current_input.Split(command_delimiter);
                        current_input = temp_input[1].TrimStart(' ').ToLower();

                        // Only change visibility if there is an actual input
                        if (current_input.Length > 0)
                        {
                            // Show if input is contained within param name
                            if (param_name.Equals(current_input))
                            {
                                truth_list[j] = true;
                            }
                        }
                    }
                    // Contains
                    else
                    {
                        // Only change visibility if there is an actual input
                        if (current_input.Length > 0)
                        {
                            // Show if input is contained within param name
                            if (param_name.Contains(current_input))
                            {
                                truth_list[j] = true;
                            }
                        }
                    }
                }

                // If all inputs result in true, make visible
                if (truth_list.All(c => c == true))
                {
                    param.Visible = true;
                    if (!hasSelectedParam)
                    {
                        hasSelectedParam = true;
                        param.Selected = true;
                    }
                }
            }

            // Restore normal autosize modes
            this.dgvParamsParamCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            ExitInvalidationMode();
        }

        private void button_ResetFilterParams_Click(object sender, EventArgs e)
        {
            // Disable normal autosize modes
            this.dgvParamsParamCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;

            bool hasSelectedFirstMatch = false;

            filter_Params.Text = "";

            for (int i = 0; i < dgvParams.Rows.Count; i++)
            {
                var dgv_row = dgvParams.Rows[i];

                dgv_row.Visible = true;
                dgv_row.Selected = false;

                if (!hasSelectedFirstMatch)
                {
                    dgv_row.Selected = true;
                    hasSelectedFirstMatch = true;
                }
            }

            // Restore normal autosize modes
            this.dgvParamsParamCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        }

        #endregion

        #region Row Filters
        private void button_FilterRows_Click(object sender, EventArgs e)
        {
            char[] command_delimiter = Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter.ToCharArray();
            char[] section_delimiter = Yapped_Rune_Bear.Properties.Settings.Default.Filter_SectionDelimiter.ToCharArray();
            string command_delimiter_string = Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter;

            // Return if row count is 0
            if (dgvRows.Rows.Count == 0)
                return;

            // Disable normal autosize modes
            this.dgvRowsIDCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvRowsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;

            // Disable user interaction
            EnterInvalidationMode();

            // Build input list
            string[] input_list = filter_Rows.Text.ToLower().Split(section_delimiter);

            // Return if no actual command is within the list
            if (input_list[0].Length < 1)
            {
                Utility.ShowError("No filter command present.");
                ExitInvalidationMode();
                return;
            }

            bool hasSelectedRow = false;

            // Hide all rows - This is so the following section can apply visibility without worrying about invisibility
            for (int i = 0; i < dgvRows.Rows.Count; i++)
            {
                var current_row = dgvRows.Rows[i];
                var current_row_name = current_row.Cells[ROW_NAME_COL].Value.ToString().ToLower();
                var current_row_id = current_row.Cells[ROW_ID_COL].Value.ToString().ToLower();
                PARAM.Row row_data = (PARAM.Row)current_row.DataBoundItem;

                // Hide the row by default
                CurrencyManager currencyManager = (CurrencyManager)BindingContext[dgvRows.DataSource];
                currencyManager.SuspendBinding();
                current_row.Visible = false;
                current_row.Selected = false;
                currencyManager.ResumeBinding();

                // This determines whether to make the row visible.
                // Each input section is checked, and if all of them result in true, then the row is made visible
                List<bool> truth_list = new List<bool>();

                // Apply each input section, this allows for multiple commands to be chained
                for (int j = 0; j < input_list.Length; j++)
                {
                    truth_list.Add(false);
                    string current_input = input_list[j];

                    // View filter
                    if (current_input.Contains("view" + command_delimiter_string))
                    {
                        string[] temp_input = current_input.Split(command_delimiter);
                        current_input = temp_input[1].TrimStart(' ').ToLower();

                        List<string> view_list = BuildViewList($@"Views\\Row\\", current_input);

                        if (view_list.Count > 0)
                        {
                            // Show if within filter list
                            foreach (string view_name in view_list)
                            {
                                if (current_row_name.Contains(view_name) || current_row_id.Contains(view_name))
                                {
                                    truth_list[j] = true;
                                }
                            }
                        }
                    }
                    // Exact
                    else if (current_input.Contains("exact" + command_delimiter_string))
                    {
                        string[] temp_input = current_input.Split(command_delimiter);
                        current_input = temp_input[1].TrimStart(' ').ToLower();

                        // Only change visibility if there is an actual input
                        if (current_input.Length > 0)
                        {
                            // Show if input is contained within param name
                            if (current_row_name.Equals(current_input) || current_row_id.Equals(current_input))
                            {
                                truth_list[j] = true;
                            }
                        }
                    }
                    // Field
                    else if (current_input.Contains("field" + command_delimiter_string))
                    {
                        string[] temp_input = current_input.Split(command_delimiter);
                        string field_input = temp_input[1].TrimStart(' ').ToLower();
                        string value_input = temp_input[2].TrimStart(' ').ToLower();

                        // Only change visibility if there is an actual input
                        if (field_input.Length > 0 && value_input.Length > 0)
                        {
                            // Check field data
                            foreach (PARAM.Cell field_cell in row_data.Cells)
                            {
                                string field_editor_name = field_cell.EditorName.ToString().ToLower();
                                string field_internal_name = field_cell.Name.ToString().ToLower();
                                string field_value = field_cell.Value.ToString();

                                // If field name matches input name
                                if (field_editor_name.Equals(field_input) || field_internal_name.Equals(field_input))
                                {
                                    // Greater than
                                    if (value_input.Contains(">"))
                                    {
                                        var temp_value = value_input.Replace(">", "");
                                        float temp_float = Convert.ToSingle(temp_value);
                                        float temp_field_value = Convert.ToSingle(field_value);

                                        if (temp_field_value > temp_float)
                                        {
                                            truth_list[j] = true;
                                        }
                                    }
                                    // Greater than equals
                                    else if (value_input.Contains(">="))
                                    {
                                        var temp_value = value_input.Replace(">=", "");
                                        float temp_float = Convert.ToSingle(temp_value);
                                        float temp_field_value = Convert.ToSingle(field_value);

                                        if (temp_field_value >= temp_float)
                                        {
                                            truth_list[j] = true;
                                        }
                                    }
                                    // Less than
                                    else if (value_input.Contains("<"))
                                    {
                                        var temp_value = value_input.Replace("<", "");
                                        float temp_float = Convert.ToSingle(temp_value);
                                        float temp_field_value = Convert.ToSingle(field_value);

                                        if (temp_field_value < temp_float)
                                        {
                                            truth_list[j] = true;
                                        }
                                    }
                                    // Less than equals
                                    else if (value_input.Contains("<="))
                                    {
                                        var temp_value = value_input.Replace("<=", "");
                                        float temp_float = Convert.ToSingle(temp_value);
                                        float temp_field_value = Convert.ToSingle(field_value);

                                        if (temp_field_value <= temp_float)
                                        {
                                            truth_list[j] = true;
                                        }
                                    }
                                    // Equality
                                    else
                                    {
                                        if (field_value.Equals(value_input))
                                        {
                                            truth_list[j] = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // Contains
                    else
                    {
                        // Only change visibility if there is an actual input
                        if (current_input.Length > 0)
                        {
                            // Show if input is contained within param name
                            if (current_row_name.Contains(current_input) || current_row_id.Contains(current_input))
                            {
                                truth_list[j] = true;
                            }
                        }
                    }
                }

                // If all inputs result in true, make visible
                if(truth_list.All(c => c == true))
                {
                    current_row.Visible = true;
                    if(!hasSelectedRow)
                    {
                        hasSelectedRow = true;
                        current_row.Selected = true;
                    }
                }
            }

            // Restore normal autosize modes
            this.dgvRowsIDCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvRowsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

            ExitInvalidationMode();
        }

        private void button_ResetFilterRows_Click(object sender, EventArgs e)
        {
            // Restore normal autosize modes
            this.dgvRowsIDCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvRowsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;

            bool hasSelectedFirstMatch = false;

            filter_Rows.Text = "";

            for (int i = 0; i < dgvRows.Rows.Count; i++)
            {
                var dgv_row = dgvRows.Rows[i];

                dgv_row.Visible = true;
                dgv_row.Selected = false;

                if (!hasSelectedFirstMatch)
                {
                    dgv_row.Selected = true;
                    hasSelectedFirstMatch = true;
                }
            }

            // Restore normal autosize modes
            this.dgvRowsIDCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvRowsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
        }
        #endregion

        #region Cell Filter
        private void button_FilterCells_Click(object sender, EventArgs e)
        {
            ApplyCellFilter(true);
        }

        private void ApplyCellFilter(bool invokeInvalidationMode)
        {
            char[] command_delimiter = Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter.ToCharArray();
            char[] section_delimiter = Yapped_Rune_Bear.Properties.Settings.Default.Filter_SectionDelimiter.ToCharArray();
            string command_delimiter_string = Yapped_Rune_Bear.Properties.Settings.Default.Filter_CommandDelimiter;

            // Return if row count is 0
            if (dgvCells.Rows.Count == 0)
                return;

            if (filter_Cells.Text == "")
                return;

            // Disable normal autosize modes
            this.dgvCellsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCellsEditorNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCellsValueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCellsTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;

            // Disable user interaction
            if (invokeInvalidationMode)
                EnterInvalidationMode();

            // Build input list
            string[] input_list = filter_Cells.Text.ToLower().Split(section_delimiter);

            // Return if no actual command is within the list
            if (input_list[0].Length > 0)
            {
                bool hasSelectedCell = false;

                // Hide all rows - This is so the following section can apply visibility without worrying about invisibility
                for (int i = 0; i < dgvCells.Rows.Count; i++)
                {
                    var cell_row = dgvCells.Rows[i];
                    var cell_row_param_name = cell_row.Cells[FIELD_PARAM_NAME_COL].Value.ToString().ToLower();
                    var cell_row_editor_name = cell_row.Cells[FIELD_EDITOR_NAME_COL].Value.ToString().ToLower();
                    var cell_row_value = cell_row.Cells[FIELD_VALUE_COL].Value.ToString().ToLower();

                    // Hide the row by default
                    CurrencyManager currencyManager = (CurrencyManager)BindingContext[dgvCells.DataSource];
                    currencyManager.SuspendBinding();
                    cell_row.Visible = false;
                    cell_row.Selected = false;
                    currencyManager.ResumeBinding();

                    // This determines whether to make the row visible.
                    // Each input section is checked, and if all of them result in true, then the row is made visible
                    List<bool> truth_list = new List<bool>();

                    // Apply each input section, this allows for multiple commands to be chained
                    for (int j = 0; j < input_list.Length; j++)
                    {
                        truth_list.Add(false);
                        string current_input = input_list[j];

                        // View filter
                        if (current_input.Contains("view" + command_delimiter_string))
                        {
                            string[] temp_input = current_input.Split(command_delimiter);
                            current_input = temp_input[1].TrimStart(' ').ToLower();

                            List<string> view_list = BuildViewList($@"Views\\Field\\", current_input);

                            if (view_list.Count > 0)
                            {
                                // Show if within filter list
                                foreach (string view_name in view_list)
                                {
                                    if (cell_row_param_name.Contains(view_name) || cell_row_editor_name.Contains(view_name) || cell_row_value.Contains(view_name))
                                    {
                                        truth_list[j] = true;
                                    }
                                }
                            }
                        }
                        // Exact
                        else if (current_input.Contains("exact" + command_delimiter_string))
                        {
                            string[] temp_input = current_input.Split(command_delimiter);
                            current_input = temp_input[1].TrimStart(' ').ToLower();

                            // Only change visibility if there is an actual input
                            if (current_input.Length > 0)
                            {
                                // Show if input is contained within param name
                                if (cell_row_param_name.Equals(current_input) || cell_row_editor_name.Equals(current_input) || cell_row_value.Equals(current_input))
                                {
                                    truth_list[j] = true;
                                }
                            }
                        }
                        // Contains
                        else
                        {
                            // Only change visibility if there is an actual input
                            if (current_input.Length > 0)
                            {
                                // Show if input is contained within param name
                                if (cell_row_param_name.Contains(current_input) || cell_row_editor_name.Contains(current_input) || cell_row_value.Contains(current_input))
                                {
                                    truth_list[j] = true;
                                }
                            }
                        }
                    }

                    // If all inputs result in true, make visible
                    if (truth_list.All(c => c == true))
                    {
                        cell_row.Visible = true;
                        if (!hasSelectedCell)
                        {
                            hasSelectedCell = true;
                            cell_row.Selected = true;
                        }
                    }
                }

                // Restore normal autosize modes
                this.dgvCellsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                this.dgvCellsEditorNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;

                if (Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowTypes)
                {
                    this.dgvCellsValueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                    this.dgvCellsTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
                }
                else
                {
                    this.dgvCellsValueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
                    this.dgvCellsTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                }
            }

            if (invokeInvalidationMode)
                ExitInvalidationMode();
        }

        private void button_ResetFilterCells_Click(object sender, EventArgs e)
        {
            // Disable normal autosize modes
            this.dgvCellsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCellsEditorNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCellsValueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dgvCellsTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;

            bool hasSelectedFirstMatch = false;

            filter_Cells.Text = "";

            for (int i = 0; i < dgvCells.Rows.Count; i++)
            {
                var dgv_row = dgvCells.Rows[i];

                dgv_row.Visible = true;
                dgv_row.Selected = false;

                if (!hasSelectedFirstMatch)
                {
                    dgv_row.Selected = true;
                    hasSelectedFirstMatch = true;
                }
            }

            // Enable normal autosize modes
            this.dgvCellsNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dgvCellsEditorNameCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;

            if (Yapped_Rune_Bear.Properties.Settings.Default.CellView_ShowTypes)
            {
                this.dgvCellsValueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
                this.dgvCellsTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            }
            else
            {
                this.dgvCellsValueCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
                this.dgvCellsTypeCol.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            }
        }
        #endregion

        #region Filter Misc
        private List<string> BuildViewList(string viewDir, string current_input)
        {
            List<string> names = new List<string>();

            bool exists = System.IO.Directory.Exists(viewDir);

            if (!exists)
            {
                Utility.ShowError("Views directory not found.");
                return names;
            }

            DirectoryInfo directory = new DirectoryInfo(viewDir);
            FileInfo[] Files = directory.GetFiles("*.txt");

            foreach (FileInfo file in Files)
            {
                var filename = file.Name.ToLower();

                if (filename.Contains(current_input))
                {
                    using (var reader = new StreamReader(file.FullName))
                    {
                        while (!reader.EndOfStream)
                        {
                            var line = reader.ReadLine();
                            names.Add(line.ToString().ToLower());
                        }
                    }
                }
            }

            return names;
        }

        private void toggleFilterVisibilityToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(Yapped_Rune_Bear.Properties.Settings.Default.EnableFilterBar)
            {
                Yapped_Rune_Bear.Properties.Settings.Default.EnableFilterBar = false;
            }
            else
            {
                Yapped_Rune_Bear.Properties.Settings.Default.EnableFilterBar = true;
            }

            if(Yapped_Rune_Bear.Properties.Settings.Default.EnableFilterBar)
            {
                menuStrip2.Visible = true;
                menuStrip3.Visible = true;
                menuStrip4.Visible = true;
            }
            else
            {
                menuStrip2.Visible = false;
                menuStrip3.Visible = false;
                menuStrip4.Visible = false;
            }
        }
        #endregion

        #region Generate Project Directories
        /// <summary>
        /// Generates the project folders for the current project name.
        /// </summary>
        private void GenerateProjectDirectories(string project)
        {
            var gameMode = (GameMode)toolStripComboBoxGame.SelectedItem;

            var projectDir = $@"Projects\\{settings.ProjectName}";

            bool exists = System.IO.Directory.Exists(projectDir);

            List<String> folders = new List<String>
            {
                "CSV",
                "Logs",
                "Names"
            };

            List<String> gametypes = new List<String>
            {
                "DS1",
                "DS1R",
                "DS2",
                "DS3",
                "SDT",
                "ER"
            };

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(projectDir);
            }

            foreach (string folder in folders)
            {
                foreach (string type in gametypes)
                {
                    var dir = projectDir + "\\" + folder + "\\" + type;

                    if (!System.IO.Directory.Exists(dir))
                        System.IO.Directory.CreateDirectory(dir);
                }
            }
        }
        #endregion

    }
}

