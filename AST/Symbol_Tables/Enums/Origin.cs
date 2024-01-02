using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public enum Origin
    {
        Default,
        Overrided, // same name and same params
        Overloaded, // same name and change params
        Inherited,
        Shadow
    }

}
