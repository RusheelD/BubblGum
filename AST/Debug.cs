using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Debug : AstNode, Statement
    {
        public AstNode Thing; // only add Printable at runtime
        public bool UseNewLine;

        public Debug(AstNode thing, bool useNewLine,
           int lineNumber, int startCol)
        {
            Thing = thing;
            UseNewLine = useNewLine;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
