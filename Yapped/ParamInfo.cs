using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Yapped
{
    internal class ParamInfo
    {
        public static Dictionary<string, ParamInfo> ReadParamInfo(string path)
        {
            var result = new Dictionary<string, ParamInfo>();
            if (File.Exists(path))
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(path);
                foreach (XmlNode node in xml.SelectNodes("params/param"))
                {
                    string name = node.Attributes["name"].InnerText;
                    bool blocked = bool.Parse(node.Attributes["blocked"]?.InnerText ?? "false");
                    bool hidden = bool.Parse(node.Attributes["hidden"]?.InnerText ?? "false");
                    string description = node.InnerText;
                    result[name] = new ParamInfo(blocked, hidden, description);
                }
            }
            return result;
        }

        public bool Blocked { get; set; }

        public bool Hidden { get; set; }

        public string Description { get; set; }

        private ParamInfo(bool blocked, bool hidden, string description)
        {
            Blocked = blocked;
            Hidden = hidden;
            Description = description;
        }
    }
}
