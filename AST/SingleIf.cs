using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class SingleIf : Statement
    {
        public Exp Cond;
        public List<Statement> Statements;

        public SingleIf(Exp cond,  List<Statement> statements)
        {
            Cond = cond;
            Statements = statements;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
