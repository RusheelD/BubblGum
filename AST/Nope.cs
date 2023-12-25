using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Nope : Exp
    {
        public Nope(int lineNumber, int startCol)
        {
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
