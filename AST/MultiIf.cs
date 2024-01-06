using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class MultiIf : AstNode, Statement
    {
        public Exp Cond;
        public List<AstNode> Statements;
        public List<(Exp, List<AstNode>)> Elifs;
        public List<AstNode> Else;

        public MultiIf(Exp cond, List<AstNode> statements,
            List<(Exp, List<AstNode>)> elifs, List<AstNode> Else,
            int lineNumber, int startCol)
        {
            Cond = cond;
            Statements = statements;
            Elifs = elifs;
            this.Else = Else;

            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
