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
    public class CreateASTHelper
    {
        private Dictionary<TypeContext, AnyType> typeConversions;

        public CreateASTHelper()
        {
            typeConversions = new Dictionary<TypeContext, AnyType>();
           // typeConversions.Add()
        }

       /* public AnyType ConvertType(TypeContext t1)
        {

        }*/
    }
}
