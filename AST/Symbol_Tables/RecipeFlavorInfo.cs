using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class RecipeFlavorInfo : FlavorInfo
    {
        public bool HasEllipses;

        public RecipeFlavorInfo(string name, AnyType type, bool isImmutable,   bool hasEllipses, int line, int col)
        : base(name, type, isImmutable, line, col)
        {
            Name = name;
            Type = type;
            IsImmutable = isImmutable;
            HasEllipses = hasEllipses;
            LineNum = line;
            Column = col;
        }

        public RecipeFlavorInfo(string name, bool isImmutable, bool hasEllipses, int line, int col)
          : base(name, isImmutable, line, col)
        {
            Name = name;
            IsImmutable = isImmutable;
            HasEllipses = hasEllipses;
            LineNum = line;
            Column = col;
        }
    }

}
