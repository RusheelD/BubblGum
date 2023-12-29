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
    public class PrintAST2 : Visitor
    {
        public void Visit(Plus n)
        {
            Visit((dynamic)n.E1);
            Visit((dynamic)n.E2);
        }

        public void Visit(Minus n)
        {
            Visit((dynamic)n.E1);
            Visit((dynamic)n.E2);
        }

        public void Visit(Multiply n)
        {
            Visit((dynamic)n.E1);
            Visit((dynamic)n.E2);
        }

        public void Visit(Divide n)
        {
            Visit((dynamic)n.E1);
            Visit((dynamic)n.E2);
        }

        public void Visit(Integer n)
        {
            
        }

        public void Visit(Double n)
        {
           
        }
    }
}
