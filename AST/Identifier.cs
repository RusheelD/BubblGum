using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Identifier : AstNode, AssignLHS
    {
        public string Value;

        public Identifier(string value, int lineNumber, int startCol)
        {
            Value = value;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
