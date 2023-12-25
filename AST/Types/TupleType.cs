using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class TupleType : AnyType {
        public List<(AnyType, string)> TypeNamePairs;
        public string VarName;

        public TupleType(string varName, List<(AnyType, string)> typeNamePairs) {
            VarName = varName;
            TypeNamePairs = typeNamePairs;
        }
   }
}
