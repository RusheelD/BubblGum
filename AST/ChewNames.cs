using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class ChewNames : Statement
    {
        public List<string> Names;

        public ChewNames(List<string> names, int lineNumber, int startCol)
        {
            Names = names;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
