using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Antlr4.Runtime.Atn.SemanticContext;

namespace AST
{
    // not in final AST tree, but useful immediate node for generating it
    public class StatementList : Statement, Printable, ProgramPiece
    {
        public List<Statement> Statements;

        public StatementList(List<Statement> statements, int lineNumber, int startCol)
        {
            Statements = statements;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
