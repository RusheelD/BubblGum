using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class PopStream : AstNode, Statement
    {
        public Exp Var;
        public bool HasOutputIdx;
        public Exp OutputIdx;

        public PopStream(Exp var, bool hasOutputIdx, Exp outputIdx, 
            int lineNumber, int startCol)
        {
            Var = var;
            HasOutputIdx = hasOutputIdx;
            OutputIdx = outputIdx;

            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
