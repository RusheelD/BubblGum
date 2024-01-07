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
        public bool IsEmpty, IsImmutable;
        
        public Origin origin;

        public AnyType Type = new FlavorType();
         
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

        public FlavorInfo(string name, AnyType type, bool isEmpty, bool isImmutable, int line, int col)
        {
            Name = name;
            Type = type;
            IsEmpty = isEmpty;
            IsImmutable = isImmutable;
            LineNum = line;
            Column = col;
        }

        public FlavorInfo(string name, bool isEmpty, bool isImmutable, int line, int col)
        {
            Name = name;
            IsEmpty = isEmpty;
            IsImmutable = isImmutable;
            LineNum = line;
            Column = col;
        }
    }

}
