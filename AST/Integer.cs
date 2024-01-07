using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Integer : Exp
    {
        public int Value;

        public Integer(int value, int lineNumber, int startCol)
        {
            Value = value;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
