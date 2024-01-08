using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class NamespaceObjectType : AnyType {
        public String Name;
        public String HomeNamespace;

        public NamespaceObjectType(String name, String homeNamespace) {
            Name = name;
            HomeNamespace = homeNamespace;
        }

        public override void Accept(TypeVisitor v) => v.Visit(this);
    }
}
