using SoulsFormats;
using System;
using System.Collections.Generic;

namespace Yapped
{
    internal class ParamWrapper : IComparable<ParamWrapper>
    {
        public bool Error { get; }

        public string Name { get; }

        public string Description { get; }

        public PARAM Param;

        public PARAM.Layout Layout;

        public List<PARAM.Row> Rows => Param.Rows;

        public ParamWrapper(string name, PARAM param, PARAM.Layout layout, string description)
        {
            if (layout == null || layout.Size != param.DetectedSize)
            {
                layout = new PARAM.Layout();
                layout.Add(new PARAM.Layout.Entry(PARAM.CellType.dummy8, "Unknown", (int)param.DetectedSize, null));
                Error = true;
            }

            Name = name;
            Param = param;
            Layout = layout;
            Param.SetLayout(layout);
            Description = description;
        }

        public int CompareTo(ParamWrapper other) => Name.CompareTo(other.Name);
    }
}
