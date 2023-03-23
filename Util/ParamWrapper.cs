using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SoulsFormats;

namespace Yapped.Util
{
    internal class ParamWrapper : IComparable<ParamWrapper>
    {
        public bool Error { get; }

        public string Name { get; }

        public string Description { get; }

        public PARAM Param;

        public PARAMDEF AppliedParamDef;

        public List<PARAM.Row> Rows => Param.Rows;

        public ParamWrapper(string name, PARAM param, PARAMDEF paramdef)
        {
            Name = name;
            Param = param;
            AppliedParamDef = paramdef;
        }

        public int CompareTo(ParamWrapper other) => Name.CompareTo(other.Name);
    }
}
