using Ast.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Assignment : Statement, ClassMember, InterfaceMember
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
    }
}

// Interface AssignmentLHS
// identifier
// (bool anytype) identifier
// expression
