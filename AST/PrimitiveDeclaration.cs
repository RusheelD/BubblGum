using Ast.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class PrimitiveDeclaration : AstNode, ClassMember
    {
        public bool IsSticky { get; set; }
        public Visbility Visbility { get; set; }

        public TypeBI TypeInfo;
        public List<Identifier> Variables;

        public PrimitiveDeclaration(bool isSticky, Visbility visibility, 
            TypeBI typeInfo, List<Identifier> variables, int lineNumber, int startCol)
        {
            IsSticky = isSticky;
            Visbility = visibility;

            TypeInfo = typeInfo;
            Variables = variables;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
