using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrCSharp.AST
{
    public class Class : AstNode
    {
        public bool IsSticky;
        public string Name;
        public List<string> InterfacesAndParentClasses;
    }
}
