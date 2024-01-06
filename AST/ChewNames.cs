using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class ChewNames : AstNode, ProgramPiece
    {
        public List<string> Names;
        public bool SpecifiedFrom;
        public string ImportPath;

        public ChewNames(List<string> names, int lineNumber, int startCol, bool specifiedFrom = false, string importPath = "")
        {
            Names = names;
            LineNumber = lineNumber;
            StartCol = startCol;
            SpecifiedFrom = specifiedFrom;
            ImportPath = importPath;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
