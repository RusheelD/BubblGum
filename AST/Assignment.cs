using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Assignment : Statement, ClassMember, InterfaceMember, StructPiece
    {
        public List<AssignLHS> Assignees;
        public Exp Result;

        public Assignment(List<AssignLHS> assignees, Exp result,
            int lineNumber, int startCol)
        {
            Assignees = assignees;
            Result = result;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}

// Assign LHS:
// identifier
// (immutable anytype) identifier
// expression
