using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class PrimitiveDeclaration2 : Statement, ClassMember, InterfaceMember, StructPiece
    {
        public List<(TypeBI, Identifier)> TypeVarPair;

        public PrimitiveDeclaration2(List<(TypeBI, Identifier)> typeVarPair, 
            int lineNumber, int startCol)
        {
            TypeVarPair = typeVarPair;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
