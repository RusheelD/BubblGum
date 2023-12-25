using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Tuple : Exp
    {
        public List<Exp> Exps;

        public Tuple(List<Exp> exps, int lineNumber, int startCol)
        {
            Exps = exps;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
