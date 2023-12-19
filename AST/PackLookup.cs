using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AntlrCSharp.AST
{
    public class PackSize : Exp
    {
        public Exp E1;

        public PackSize(Exp e1, int lineNumber, int startCol)
        {
            E1 = e1;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
