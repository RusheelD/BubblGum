using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Pop : Statement
    {
        public override void Accept(Visitor v) => v.Visit(this);
    }
}
