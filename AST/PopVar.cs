using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class PopVar : Statement
    {
        public Exp Var;
        public bool UseOutput;
        public Exp Output;

        public PopVar(Exp var, bool useOutput, Exp output, int lineNumber, int startCol)
        {
            Var = var;
            UseOutput = useOutput;
            Output = output;

            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
