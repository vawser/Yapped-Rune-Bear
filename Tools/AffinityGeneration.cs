using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Yapped.Util;
using SoulsFormats;
using System.IO;

namespace Yapped.Tools
{
    internal static class AffinityGeneration
    {
        public static void GenerateAffinityRows(DataGridViewRow paramRow, ParamWrapper wrapper, DataGridView dgvRows, GameMode gameMode)
        {
            // Exit early if in wrong param
            if (paramRow.Index != 45)
            {
                Utility.ShowError("You can't generate Affinity rows outside of the EquipWeaponParam.");
                return;
            }

            // Exit early if no row is selected
            if (dgvRows.SelectedCells.Count == 0)
            {
                Utility.ShowError("You can't generate Affinity rows without a row selected!");
                return;
            }

            // Exit early if configuration folder isn't present
            string configDir = $@"Tools\AffinityGeneration\\";

            bool exists = System.IO.Directory.Exists(configDir);

            if (!exists)
            {
                Utility.ShowError("Affinity Generation configuration directory not found.");
                return;
            }

            // Row Info
            int index = dgvRows.SelectedCells[0].RowIndex;
            PARAM.Row currentRow = wrapper.Rows[index];

            Console.WriteLine(currentRow.Name);

            // Get configuration info
            DirectoryInfo directory = new DirectoryInfo(configDir);
            FileInfo[] Files = directory.GetFiles("*.txt");

            // Each file represents a new row
            foreach (FileInfo file in Files)
            {
                string rawName = file.Name;
                char splitter = "-".ToCharArray()[0];
                string offset = rawName.Split(splitter)[0].Trim();
                string name = rawName.Split(splitter)[1].Trim().Replace(".txt", "");

                List<string[]> instructions = new List<string[]>();

                // Build instruction list
                using (var reader = new StreamReader(file.FullName))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        instructions.Add(values);
                    }
                }

                // Build new id and name
                int new_id = currentRow.ID + Convert.ToInt32(offset);
                string new_name = currentRow.Name + " [" + name + "]";

                // Create new row and copy base row data into it
                PARAM.Row newRow = new PARAM.Row(new_id, new_name, wrapper.AppliedParamDef);

                for (int i = 0; i < currentRow.Cells.Count; i++)
                {
                    newRow.Cells[i].Value = currentRow.Cells[i].Value;
                }

                // Execute instructions
                foreach (string[] instruction in instructions)
                {
                    AffinityGeneration.ModifyAffinityField(currentRow, newRow, instruction);
                }

                wrapper.Rows.Add(newRow);
                wrapper.Rows.Sort((r1, r2) => r1.ID.CompareTo(r2.ID));
            }

            if (!Yapped_Rune_Bear.Properties.Settings.Default.ShowConfirmationMessages)
                MessageBox.Show($@"Affinity rows generated!", "Affinity Generator");
        }

        public static void ModifyAffinityField(PARAM.Row baseRow, PARAM.Row row, string[] instruction)
        {
            string instruction_field = instruction[0];
            string instruction_type = instruction[1];
            string instruction_value = instruction[2];

            string base_STR_correction = "";
            string base_DEX_correction = "";
            string base_INT_correction = "";
            string base_FTH_correction = "";
            string base_HIGHEST_correction = "";

            string base_PHYSICAL_damage = "";
            string base_MAGIC_damage = "";
            string base_FIRE_damage = "";
            string base_LIGHTNING_damage = "";
            string base_HOLY_damage = "";

            foreach (PARAM.Cell cell in baseRow.Cells)
            {
                PARAMDEF.DefType type = cell.Def.DisplayType;

                string name = cell.Def.InternalName.ToString();
                string value = cell.Value.ToString();

                if (type != PARAMDEF.DefType.dummy8)
                {
                    switch (name)
                    {
                        case "correctStrength":
                            base_STR_correction = value;
                            break;

                        case "correctAgility":
                            base_DEX_correction = value;
                            break;

                        case "correctMagic":
                            base_INT_correction = value;
                            break;

                        case "correctFaith":
                            base_FTH_correction = value;
                            break;

                        case "attackBasePhysics":
                            base_PHYSICAL_damage = value;
                            break;

                        case "attackBaseMagic":
                            base_MAGIC_damage = value;
                            break;

                        case "attackBaseFire":
                            base_FIRE_damage = value;
                            break;

                        case "attackBaseThunder":
                            base_LIGHTNING_damage = value;
                            break;

                        case "attackBaseDark":
                            base_HOLY_damage = value;
                            break;

                        case "disableGemAttr":
                            cell.Value = 0;
                            break;
                    }
                }
            }

            // Get highest correction
            List<string> corrections = new List<string>();
            corrections.Add(base_STR_correction);
            corrections.Add(base_DEX_correction);
            corrections.Add(base_INT_correction);
            corrections.Add(base_FTH_correction);

            float highest_value = 0;

            foreach (string correction in corrections)
            {
                float value = Convert.ToSingle(correction);

                if (value >= highest_value)
                {
                    highest_value = value;
                }
            }

            base_HIGHEST_correction = Convert.ToString(highest_value);

            // Apply instructions
            int index = 0;
            foreach (PARAM.Cell cell in row.Cells)
            {
                PARAMDEF.DefType type = cell.Def.DisplayType;

                string name = cell.Def.InternalName;
                string value = cell.Value.ToString();
                string base_value = baseRow.Cells[index].Value.ToString();

                string cell_formula = "";
                StringToFormula stf = new StringToFormula();

                if (type != PARAMDEF.DefType.dummy8)
                {
                    if (instruction_field == name)
                    {
                        switch (instruction_type)
                        {
                            // Set the cell value to the instruction value
                            case "SET":
                                if (type == PARAMDEF.DefType.s8)
                                    cell.Value = Convert.ToSByte(instruction_value);
                                else if (type == PARAMDEF.DefType.u8)
                                    cell.Value = Convert.ToByte(instruction_value);
                                else if (type == PARAMDEF.DefType.s16)
                                    cell.Value = Convert.ToInt16(instruction_value);
                                else if (type == PARAMDEF.DefType.u16)
                                    cell.Value = Convert.ToUInt16(instruction_value);
                                else if (type == PARAMDEF.DefType.s32)
                                    cell.Value = Convert.ToInt32(instruction_value);
                                else if (type == PARAMDEF.DefType.u32)
                                    cell.Value = Convert.ToUInt32(instruction_value);
                                else if (type == PARAMDEF.DefType.f32)
                                    cell.Value = Convert.ToSingle(instruction_value);
                                else if (type == PARAMDEF.DefType.fixstr)
                                    cell.Value = Convert.ToString(instruction_value);
                                else if (type == PARAMDEF.DefType.fixstrW)
                                    cell.Value = Convert.ToString(instruction_value);
                                break;

                            // Calculate the cell value 
                            case "CALC":
                                double result;
                                cell_formula = instruction_value.Replace("x", base_value);
                                result = stf.Eval(cell_formula);

                                result = (float)Math.Floor(result);

                                if (type == PARAMDEF.DefType.s8)
                                    cell.Value = Convert.ToSByte(result);
                                else if (type == PARAMDEF.DefType.u8)
                                    cell.Value = Convert.ToByte(result);
                                else if (type == PARAMDEF.DefType.s16)
                                    cell.Value = Convert.ToInt16(result);
                                else if (type == PARAMDEF.DefType.u16)
                                    cell.Value = Convert.ToUInt16(result);
                                else if (type == PARAMDEF.DefType.s32)
                                    cell.Value = Convert.ToInt32(result);
                                else if (type == PARAMDEF.DefType.u32)
                                    cell.Value = Convert.ToUInt32(result);
                                else if (type == PARAMDEF.DefType.f32)
                                    cell.Value = Convert.ToSingle(result);

                                break;

                            case "STAT_CALC":
                                double stat_result;

                                if (instruction_value.Contains("STR"))
                                {
                                    cell_formula = instruction_value.Replace("STR", base_STR_correction);
                                }
                                else if (instruction_value.Contains("DEX"))
                                {
                                    cell_formula = instruction_value.Replace("DEX", base_DEX_correction);
                                }
                                else if (instruction_value.Contains("INT"))
                                {
                                    cell_formula = instruction_value.Replace("INT", base_INT_correction);
                                }
                                else if (instruction_value.Contains("FTH"))
                                {
                                    cell_formula = instruction_value.Replace("FTH", base_FTH_correction);
                                }
                                else if (instruction_value.Contains("HIGHEST"))
                                {
                                    cell_formula = instruction_value.Replace("HIGHEST", base_HIGHEST_correction);
                                }

                                stat_result = stf.Eval(cell_formula);
                                stat_result = (float)Math.Floor(stat_result);

                                if (type == PARAMDEF.DefType.s8)
                                    cell.Value = Convert.ToSByte(stat_result);
                                else if (type == PARAMDEF.DefType.u8)
                                    cell.Value = Convert.ToByte(stat_result);
                                else if (type == PARAMDEF.DefType.s16)
                                    cell.Value = Convert.ToInt16(stat_result);
                                else if (type == PARAMDEF.DefType.u16)
                                    cell.Value = Convert.ToUInt16(stat_result);
                                else if (type == PARAMDEF.DefType.s32)
                                    cell.Value = Convert.ToInt32(stat_result);
                                else if (type == PARAMDEF.DefType.u32)
                                    cell.Value = Convert.ToUInt32(stat_result);
                                else if (type == PARAMDEF.DefType.f32)
                                    cell.Value = Convert.ToSingle(stat_result);

                                break;

                            case "DMG_CALC":
                                double dmg_result;

                                if (instruction_value.Contains("PHYSICAL"))
                                {
                                    cell_formula = instruction_value.Replace("PHYSICAL", base_PHYSICAL_damage);
                                }
                                else if (instruction_value.Contains("MAGIC"))
                                {
                                    cell_formula = instruction_value.Replace("MAGIC", base_MAGIC_damage);
                                }
                                else if (instruction_value.Contains("FIRE"))
                                {
                                    cell_formula = instruction_value.Replace("FIRE", base_FIRE_damage);
                                }
                                else if (instruction_value.Contains("LIGHTNING"))
                                {
                                    cell_formula = instruction_value.Replace("LIGHTNING", base_LIGHTNING_damage);
                                }
                                else if (instruction_value.Contains("HOLY"))
                                {
                                    cell_formula = instruction_value.Replace("HOLY", base_HOLY_damage);
                                }

                                dmg_result = stf.Eval(cell_formula);
                                dmg_result = (float)Math.Floor(dmg_result);

                                if (type == PARAMDEF.DefType.s8)
                                    cell.Value = Convert.ToSByte(dmg_result);
                                else if (type == PARAMDEF.DefType.u8)
                                    cell.Value = Convert.ToByte(dmg_result);
                                else if (type == PARAMDEF.DefType.s16)
                                    cell.Value = Convert.ToInt16(dmg_result);
                                else if (type == PARAMDEF.DefType.u16)
                                    cell.Value = Convert.ToUInt16(dmg_result);
                                else if (type == PARAMDEF.DefType.s32)
                                    cell.Value = Convert.ToInt32(dmg_result);
                                else if (type == PARAMDEF.DefType.u32)
                                    cell.Value = Convert.ToUInt32(dmg_result);
                                else if (type == PARAMDEF.DefType.f32)
                                    cell.Value = Convert.ToSingle(dmg_result);

                                break;

                            default:
                                break;
                        }
                    }
                }

                index++;
            }

            return;
        }
    }
}
