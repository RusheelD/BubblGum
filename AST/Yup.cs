using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Yup : Exp
    {
        public Yup(int lineNumber, int startCol)
        {
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
