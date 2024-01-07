using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class PopLoop : AstNode, Statement
    {
        public string VarName;
        public Exp Exp;
        public List<AstNode> Statements;

        public PopLoop(string varName, Exp exp, List<AstNode> statements, 
            int lineNumber, int startCol)
        {
            VarName = varName;
            Exp = exp;
            Statements = statements;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
