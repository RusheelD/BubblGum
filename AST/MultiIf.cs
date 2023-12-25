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
        public List<Exp> ElifConditions;
        public List<List<Statement>> ElifStatements;
        public List<Statement> Else;
    }
}
