using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SoulsFormats.MSB3.Region;

namespace Yapped_Rune_Bear.Util
{
    internal class Logger
    {
        public string log;

        public Logger()
        {
            log = "";
        }

        public void WriteLog()
        {
            string logDir = GetLogDir();

            bool exists = System.IO.Directory.Exists(logDir);

            if (!exists)
            {
                System.IO.Directory.CreateDirectory(logDir);
            }

            File.AppendAllText($"{logDir}//yapped_log.txt", log);

            log = "";
        }

        public void ClearLog()
        {
            string logDir = GetLogDir();

            File.Delete($"{logDir}//yapped_log.txt");
        }

        public void AddToLog(string text)
        {
            log = log + text + "\n";
        }

        public string GetLogDir()
        {
            return System.AppDomain.CurrentDomain.BaseDirectory;
        }
    }
}
