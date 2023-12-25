using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class SingularArrayType : AnyType {
        public String VarName;
        public AnyType Type;

        public SingularArrayType(String varName, AnyType type) {
            VarName = varName;
            Type = type;
        }
   }
}
