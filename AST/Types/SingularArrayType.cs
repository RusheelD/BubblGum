using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class SingularArrayType : AnyType {
        public AnyType Type;

        public SingularArrayType(AnyType type) {
            Type = type;
        }
   }
}
