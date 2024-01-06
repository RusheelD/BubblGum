using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class PrimitiveDeclaration2 : AstNode, Statement, ClassMember, InterfaceMember, StructPiece
    {
        public List<(TypeBI, string)> TypeVarPair;

        public PrimitiveDeclaration2(List<(TypeBI, string)> typeVarPair, 
            int lineNumber, int startCol)
        {
            TypeVarPair = typeVarPair;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
