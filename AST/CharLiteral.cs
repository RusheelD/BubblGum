using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class CharLiteral : Exp
    {
        public char Value;

        public CharLiteral(char value, int lineNumber, int startCol)
        {
            Value = value;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
