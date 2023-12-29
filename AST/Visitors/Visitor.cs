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
    public interface Visitor
    {
        public void Visit(Plus n);
        public void Visit(Minus n);
        public void Visit(Multiply n);
        public void Visit(Divide n);
        public void Visit(Integer n);
        public void Visit(Double n);
    }
}
