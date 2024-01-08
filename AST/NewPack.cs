using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class NewPack : Exp
    {
        public List<Exp> Elements;

        public NewPack(List<Exp> elements, int lineNumber, int startCol)
        {
            Elements = elements;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
