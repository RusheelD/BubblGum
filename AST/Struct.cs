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
        public List<AstNode> Statements; // confirm it is struct pieces at runtime

        public Struct(string name, List<AstNode> statements, int lineNumber, int startCol)
        {
            Name = name;
            Statements = statements;

            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
