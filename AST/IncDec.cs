using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class IncDec : AstNode, Statement
    {
        public Exp E1, E2;
        public bool ShouldIncrement; // decrement if false

        public IncDec(Exp e1, Exp e2, bool shouldIncrement, int lineNumber, int startCol)
        {
            E1 = e1;
            E2 = e2;
            ShouldIncrement = shouldIncrement;

            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
