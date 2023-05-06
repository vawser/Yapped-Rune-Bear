using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SoulsFormats;
using SoulsFormats.XmlExtensions;
using System.Xml;
using System.Xml.Linq;

namespace Yapped.Util
{
   class Utility
   {
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

        public static void DebugPrint(string message)
        {
            bool DebugMode = true;

            if(DebugMode)
            {
                Console.WriteLine(message);
            }
        }

        // Credit to Philiquaz (DSMS)
        public static string Dummy8Write(Byte[] dummy8)
        {
            string val = null;
            foreach (Byte b in dummy8)
            {
                if (val == null)
                    val = "[" + b;
                else
                    val += "|" + b;
            }
            if (val == null)
                val = "[]";
            else
                val += "]";
            return val;
        }

        // Credit to Philiquaz (DSMS)
        public static Byte[] Dummy8Read(string dummy8, int expectedLength)
        {
            Byte[] nval = new Byte[expectedLength];
            if (!(dummy8.StartsWith("[") && dummy8.EndsWith("]")))
                return null;
            string[] spl = dummy8.Substring(1, dummy8.Length - 2).Split('|');
            if (nval.Length != spl.Length)
            {
                return null;
            }
            for (int i = 0; i < nval.Length; i++)
            {
                if (!byte.TryParse(spl[i], out nval[i]))
                    return null;
            }
            return nval;
        }
    }
}
