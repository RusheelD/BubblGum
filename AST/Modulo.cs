using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Modulo : Exp
    {
        public Exp E1, E2;

        public Modulo(Exp e1, Exp e2, int lineNumber, int startCol)
        {
            E1 = e1;
            E2 = e2;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
