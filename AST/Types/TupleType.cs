using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class TupleType : AnyType {
        public List<(AnyType, string)> TypeNamePairs;
        public const string NoName = "";

        public TupleType(List<(AnyType, string)> typeNamePairs) {
            TypeNamePairs = typeNamePairs;
        }

        public override void Accept(TypeVisitor v) => v.Visit(this);
    }
}
