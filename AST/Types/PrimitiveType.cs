using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class PrimitiveType : AnyType
    {
        public TypeBI Type;

        public PrimitiveType(TypeBI type)
        {
            Type = type;
        }
    }
}
