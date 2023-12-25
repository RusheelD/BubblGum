using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Struct : AstNode, ProgramPiece
    {
        public string Name;
        public List<Statement> Statements;

        public Struct(string name, List<Statement> statements, int lineNumber, int startCol)
        {
            Name = name;
            Statements = statements;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
