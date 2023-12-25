using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class RepeatLoop : Statement
    {
        public string VarName;
        public bool IsUp;
        public Exp Start, End;

        public RepeatLoop(string varName, bool isUp, Exp start, Exp end, int lineNumber, int startCol)
        {
            VarName = varName;
            IsUp = isUp;
            Start = start;
            End = end;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
