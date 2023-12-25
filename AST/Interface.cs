using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Interface : AstNode, ProgramPiece
    {
        public bool IsSticky;
        public Visbility Visbility;
        public string Name;
        public List<string> ParentInterfaces;

        // IsSticky, GetScope, SetScope, InterfaceMember
        public List<(bool, Visbility, Visbility, InterfaceMember)> InterfaceMemberInfo;

        public Interface(bool isSticky, Visbility visbility, string name,
            List<string> parentInterfaces, 
            List<(bool, Visbility, Visbility, InterfaceMember)> interfaceMemberInfo,
            int lineNumber, int startCol)
        {
            IsSticky = isSticky;
            Visbility = visbility;
            Name = name;
            ParentInterfaces = parentInterfaces;
            InterfaceMemberInfo = interfaceMemberInfo;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
