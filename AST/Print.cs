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

        public Print(Printable thing,
           int lineNumber, int startCol)
        {
            Thing = thing;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
