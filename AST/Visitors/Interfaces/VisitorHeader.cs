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
    public interface VisitorHeader
    {
        public void Visit(Program n);
        public void Visit(Stock n);
        public void Visit(ChewNames n);
        public void Visit(ChewPath n);
    }
}