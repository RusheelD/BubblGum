using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class PrimitiveDeclaration1 : AstNode, ClassMember, InterfaceMember
    {
        public TypeBI TypeInfo;
        public List<Identifier> Variables;

        public PrimitiveDeclaration1(TypeBI typeInfo, List<Identifier> variables, 
            int lineNumber, int startCol)
        {
            TypeInfo = typeInfo;
            Variables = variables;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
