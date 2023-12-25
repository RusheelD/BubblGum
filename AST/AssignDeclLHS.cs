using Ast.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class AssignDeclLHS : AstNode, AssignLHS
    {
        public bool IsImmutable;
        public AnyType Type;
        public string VarName;

        public AssignDeclLHS(bool isImmutable, AnyType type, string varName,
            int lineNumber, int startCol)
        {
            IsImmutable = isImmutable;
            Type = type;
            VarName = varName;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}