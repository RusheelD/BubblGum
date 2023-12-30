using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class PrimitiveDeclaration1 : Statement, ClassMember, InterfaceMember, StructPiece
    {
        public TypeBI TypeInfo;
        public List<string> Variables;

        public PrimitiveDeclaration1(TypeBI typeInfo, List<string> variables, 
            int lineNumber, int startCol)
        {
            TypeInfo = typeInfo;
            Variables = variables;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
