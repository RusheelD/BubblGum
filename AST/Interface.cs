using AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Interface : AstNode
    {
        public bool IsSticky;
        public Visbility Visbility;
        public string Name;
        public List<string> InterfacesAndParentClasses;
    }
}
