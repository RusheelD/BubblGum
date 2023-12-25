using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class VarAssign : Statement
    {
        public List<Exp> Variables;
        public Exp Value;

        public VarAssign(List<Exp> variables, Exp value, int lineNumber, int startCol)
        {
            Variables = variables;
            Value = value;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
