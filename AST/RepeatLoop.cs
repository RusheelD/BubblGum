using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class RepeatLoop : AstNode, Statement
    {
        public string VarName;
        public bool IsUp;
        public Exp Start, End;
        public List<AstNode> Statements;

        public RepeatLoop(string varName, bool isUp, Exp start, Exp end,
             List<AstNode> statements, int lineNumber, int startCol)
        {
            VarName = varName;
            IsUp = isUp;
            Start = start;
            End = end;
            Statements = statements;

            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
