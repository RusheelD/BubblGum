using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Stock : AstNode, ProgramPiece
    {
        public List<string> Names;

        public Stock(List<string> names, int lineNumber, int startCol)
        {
            Names = names;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(VisitorHeader v) => v.Visit(this);
    }
}
