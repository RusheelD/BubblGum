using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class FlavorpackType : AnyType {
        public override void Accept(TypeVisitor v) => v.Visit(this);
    }
}
