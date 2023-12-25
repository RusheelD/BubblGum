using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class PopLoop : Statement
    {
        public string VarName;
        public Exp Exp;
        public List<Statement> Statements;

        public PopLoop(string varName, Exp exp, List<Statement> statements, 
            int lineNumber, int startCol)
        {
            VarName = varName;
            Exp = exp;
            Statements = statements;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
