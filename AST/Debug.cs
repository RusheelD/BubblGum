using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Debug : Statement
    {
        public Printable Thing;

        public Debug(Printable thing,
           int lineNumber, int startCol)
        {
            Thing = thing;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
