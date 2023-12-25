using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class IncDec : Exp
    {
        public Exp E1;
        public bool IncOrDec;
        public bool PreOrPost;

        public IncDec(Exp e1, bool incOrDec, bool preOrPost, int lineNumber, int startCol)
        {
            E1 = e1;
            IncOrDec = incOrDec;
            PreOrPost = preOrPost;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
