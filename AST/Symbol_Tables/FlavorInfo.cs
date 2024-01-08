using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    /// <summary>
    /// Stores info about a typical variable (declared outside of classes, inside functions, etc.)
    /// </summary>
    public class FlavorInfo : Info
    {
        public string Name;
        public bool IsEmpty, IsImmutable;
        
        public Origin origin;

        public AnyType Type = new FlavorType();
         
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

        public FlavorInfo(string name, AnyType type, bool isImmutable, int line, int col)
        {
            Name = name;
            Type = type;
            IsImmutable = isImmutable;
            LineNum = line;
            Column = col;
        }

        public FlavorInfo(string name, bool isImmutable, int line, int col)
        {
            Name = name;
            IsImmutable = isImmutable;
            LineNum = line;
            Column = col;
        }
    }

}
