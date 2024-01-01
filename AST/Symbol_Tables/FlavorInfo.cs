using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class FlavorInfo
    {
        public string Name;
        public bool isEmpty;
        //public Origin
        public AnyType Type;
        public int LineNum, Column;
    }

}
