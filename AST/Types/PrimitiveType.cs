using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class PrimitiveType : AnyType
    {
        public String VarName;
        public TypeBI Type;

        public PrimitiveType(String varName, TypeBI type)
        {
            VarName = varName;
            Type = type;
        }
    }
}
