using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Program : AstNode
    {
        public List<AstNode> Pieces; // confirm it is a program piece

        public Program(List<AstNode> pieces,
           int lineNumber, int startCol)
        {
            Pieces = pieces;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
