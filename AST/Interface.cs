using Ast.enums;
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

        public Interface(bool isSticky, Visbility visbility, string name,
            List<string> interfacesAndParentClasses, int lineNumber, int startCol)
        {
            IsSticky = isSticky;
            Visbility = visbility;
            Name = name;
            InterfacesAndParentClasses = interfacesAndParentClasses;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
