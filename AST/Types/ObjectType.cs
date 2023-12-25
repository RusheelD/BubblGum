using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class ObjectType : AnyType {
        public String Name;
        public bool IsPack;

        public ObjectType(String name, bool isPack) {
            Name = name;
            IsPack = isPack;
        }
   }
}
