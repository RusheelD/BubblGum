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
    public class PrintAST : Visitor
    {
        public void Visit(Plus n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);
        }

        public void Visit(Minus n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);
        }

        public void Visit(Multiply n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);
        }

        public void Visit(Divide n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);
        }

        public void Visit(Integer n)
        {
        }

        public void Visit(Double n)
        {
        }
    }
}
