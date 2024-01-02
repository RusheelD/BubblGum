using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public abstract class AstNode
    {
        public int LineNumber;
        public int StartCol;

        public abstract void Accept(VisitorHeader v);
    }
}
