using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class IdentifierExp : Exp
    {
        public string Value;

        public IdentifierExp(string value, int lineNumber, int startCol)
        {
            Value = value;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
