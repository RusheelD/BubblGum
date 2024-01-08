using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class NamespaceAccess : Exp
    {
        public string Name;
        public Exp E1;

        public NamespaceAccess(string name, Exp e1, int lineNumber, int startCol)
        {
            Name = name;
            E1 = e1;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
