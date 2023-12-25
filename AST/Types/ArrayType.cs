using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class ArrayType : AnyType {
        public String VarName;
        public List<AnyType> Types;

        public ArrayType(String varName, List<AnyType> types) {
            VarName = varName;
            Types = types;
        }
   }
}
