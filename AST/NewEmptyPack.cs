using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class NewEmptyPack : Exp
    {
        public AnyType PackType;
        public Exp Exp;

        public NewEmptyPack(AnyType type, Exp exp, int lineNumber, int startCol)
        {
            PackType = type;
            Exp = exp;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
