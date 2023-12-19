using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrCSharp.AST
{
    public class NonObjectType : AnyType
    {
        public TypeBI Type;

        public NonObjectType(TypeBI type)
        {
            Type = type;
        }
    }
}
