using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public abstract class AnyType {

        public abstract void Accept(TypeVisitor v);
   }

   //public const string EMPTY = "";
}
