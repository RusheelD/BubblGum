using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class MultiIf : Statement
    {
        public Exp Cond;
        public List<Statement> Statements;
        public List<(Exp, List<Statement>)> Elifs;
        public List<Statement> Else;

        public MultiIf(Exp cond, List<Statement> statements,
            List<(Exp, List<Statement>)> elifs, List<Statement> Else,
            int lineNumber, int startCol)
        {
            Cond = cond;
            Statements = statements;
            Elifs = elifs;
            this.Else = Else;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
