using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BubblGumParser;

namespace AST
{
    public interface TypeVisitor
    {
        public void Visit(ArrayType n);
        public void Visit(FlavorType n);
        public void Visit(ObjectType n);
        public void Visit(NamespaceObjectType n);
        public void Visit(PackType n);
        public void Visit(PrimitiveType n);
        public void Visit(SingularArrayType n);
        public void Visit(FlavorpackType n);
        public void Visit(TupleType n);
    }
}
