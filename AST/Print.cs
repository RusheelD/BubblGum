using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Print : Statement
    {
        public Printable Thing;
        public bool UseNewLine;

        public Print(Printable thing, bool useNewLine,
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
