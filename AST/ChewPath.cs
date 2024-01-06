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
        public bool SpecifiedFrom;
        public string ImportPath;

        public ChewPath(string path, int lineNumber, int startCol, bool specifiedFrom = false, string importPath = "")
        {
            Path = path;
            LineNumber = lineNumber;
            StartCol = startCol;
            SpecifiedFrom = specifiedFrom;
            ImportPath = importPath;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
