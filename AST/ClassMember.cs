using Ast.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public interface ClassMember
    {
        public bool IsSticky { get; }
        public Visbility Visbility { get; }
    }
}
