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
        public Visbility Visibility;
        public List<AstNode> Statements;

        public Struct(string name, Visbility visibility, List<AstNode> statements, int lineNumber, int startCol)
        {
            Name = name;
            Statements = statements;
            Visibility = visibility;

            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
