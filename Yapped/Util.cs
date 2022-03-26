using SoulsFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CellType = SoulsFormats.PARAM.CellType;

namespace Yapped
{
    internal static class Util
    {
        public static Dictionary<string, PARAM.Layout> LoadLayouts(string directory)
        {
            var layouts = new Dictionary<string, PARAM.Layout>();
            if (Directory.Exists(directory))
            {
                foreach (string path in Directory.GetFiles(directory, "*.xml"))
                {
                    string paramID = Path.GetFileNameWithoutExtension(path);
                    try
                    {
                        PARAM.Layout layout = PARAM.Layout.ReadXMLFile(path);
                        layouts[paramID] = layout;
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Failed to load layout {paramID}.txt\r\n\r\n{ex}");
                    }
                }
            }
            return layouts;
        }

        public static LoadParamsResult LoadParams(string paramPath, Dictionary<string, ParamInfo> paramInfo,
            Dictionary<string, PARAM.Layout> layouts, GameMode gameMode, bool hideUnusedParams)
        {
            if (!File.Exists(paramPath))
            {
                ShowError($"Parambnd not found:\r\n{paramPath}\r\nPlease browse to the Data0.bdt or parambnd you would like to edit.");
                return null;
            }

            var result = new LoadParamsResult();
            try
            {
                if (BND4.Is(paramPath))
                {
                    result.ParamBND = BND4.Read(paramPath);
                    result.Encrypted = false;
                }
                else if (BND3.Is(paramPath))
                {
                    result.ParamBND = BND3.Read(paramPath);
                    result.Encrypted = false;
                }
                else if (gameMode.Game == GameMode.GameType.DarkSouls2)
                {
                    result.ParamBND = DecryptDS2Regulation(paramPath);
                    result.Encrypted = true;
                }
                else if (gameMode.Game == GameMode.GameType.DarkSouls3)
                {
                    result.ParamBND = SFUtil.DecryptDS3Regulation(paramPath);
                    result.Encrypted = true;
                }
                else if (gameMode.Game == GameMode.GameType.EldenRing)
                {
                    result.ParamBND = SFUtil.DecryptERRegulation(paramPath);
                    result.Encrypted = true;
                }
                else
                {
                    throw new FormatException("Unrecognized file format.");
                }
            }
            catch (DllNotFoundException ex) when (ex.Message.Contains("oo2core_6_win64.dll"))
            {
                ShowError("In order to load Sekiro params, you must copy oo2core_6_win64.dll from Sekiro into Yapped's lib folder.");
                return null;
            }
            catch (Exception ex)
            {
                ShowError($"Failed to load parambnd:\r\n{paramPath}\r\n\r\n{ex}");
                return null;
            }

            Console.WriteLine( result.ParamBND.ToString() );

            result.ParamWrappers = new List<ParamWrapper>();
            foreach (BinderFile file in result.ParamBND.Files.Where(f => f.Name.EndsWith(".param")))
            {
                string name = Path.GetFileNameWithoutExtension(file.Name);
                if (paramInfo.ContainsKey(name))
                {
                    if (paramInfo[name].Blocked || paramInfo[name].Hidden && hideUnusedParams)
                        continue;
                }

                try
                {
                    PARAM param = PARAM.Read(file.Bytes);
                    
                    PARAM.Layout layout = null;
                    if (layouts.ContainsKey(param.ID))
                        layout = layouts[param.ID];
                    
                    string description = null;
                    if (paramInfo.ContainsKey(name))
                        description = paramInfo[name].Description;

                    var wrapper = new ParamWrapper(name, param, layout, description);
                    result.ParamWrappers.Add(wrapper);
                }
                catch (Exception ex)
                {
                    ShowError($"Failed to load param file: {name}.param\r\n\r\n{ex}");
                }
            }

            result.ParamWrappers.Sort();
            return result;
        }

        private static byte[] ds2RegulationKey = {
            0x40, 0x17, 0x81, 0x30, 0xDF, 0x0A, 0x94, 0x54, 0x33, 0x09, 0xE1, 0x71, 0xEC, 0xBF, 0x25, 0x4C };

        public static BND4 DecryptDS2Regulation(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            byte[] iv = new byte[16];
            iv[0] = 0x80;
            Array.Copy(bytes, 0, iv, 1, 11);
            iv[15] = 1;
            byte[] input = new byte[bytes.Length - 32];
            Array.Copy(bytes, 32, input, 0, bytes.Length - 32);
            using (var ms = new MemoryStream(input))
            {
                byte[] decrypted = CryptographyUtility.DecryptAesCtr(ms, ds2RegulationKey, iv);
                File.WriteAllBytes("ffff.bnd", decrypted);
                return BND4.Read(decrypted);
            }
        }

        public static void EncryptDS2Regulation(string path, BND4 bnd)
        {
            //var rand = new Random();
            //byte[] iv = new byte[16];
            //byte[] ivPart = new byte[11];
            //rand.NextBytes(ivPart);
            //iv[0] = 0x80;
            //Array.Copy(ivPart, 0, iv, 1, 11);
            //iv[15] = 1;
            //byte[] decrypted = bnd.Write();
            //byte[] encrypted = CryptographyUtility.EncryptAesCtr(decrypted, ds2RegulationKey, iv);
            //byte[] output = new byte[encrypted.Length + 11];

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            bnd.Write(path); // xddddd
        }

        public static void ShowError(string message)
        {
            MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    internal class LoadParamsResult
    {
        public bool Encrypted { get; set; }

        public IBinder ParamBND { get; set; }

        public List<ParamWrapper> ParamWrappers { get; set; }
    }
}
