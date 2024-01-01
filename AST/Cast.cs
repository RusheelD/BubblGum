using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Cast : Exp
    {
        public AnyType CastType;
        public Exp E1;

        public Cast(AnyType castType, Exp e1, int lineNumber, int startCol)
        {
            CastType = castType;
            E1 = e1;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
