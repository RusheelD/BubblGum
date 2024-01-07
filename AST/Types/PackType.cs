using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class PackType : AnyType
    {
        public TypePack Type;

        public PackType(TypePack type)
        {
            Type = type;
        }

        public override void Accept(TypeVisitor v) => v.Visit(this);
    }
}
