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
        public bool IsPack;

        public NamespaceObjectType(String name, String homeNamespace, bool isPack) {
            Name = name;
            HomeNamespace = homeNamespace;
            IsPack = isPack;
        }

        public override void Accept(TypeVisitor v) => v.Visit(this);
    }
}
