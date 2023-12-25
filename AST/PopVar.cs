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
        public bool ShouldOutput;
        public Exp Output;

        public PopVar(Exp var, bool shouldOutput, Exp output, int lineNumber, int startCol)
        {
            Var = var;
            ShouldOutput = shouldOutput;
            Output = output;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
