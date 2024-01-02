using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class ChewPath : AstNode, ProgramPiece
    {
        public string Path;

        public ChewPath(string path, int lineNumber, int startCol)
        {
            Path = path;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
