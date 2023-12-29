using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public interface AssignLHS
    {
        public void Accept(Visitor v);
    }
}

// Assign LHS:
// identifier
// (immutable anytype) identifier
// expression