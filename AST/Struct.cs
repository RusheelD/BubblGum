using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Struct : AstNode
    {
        public string Name;
        public List<string> InterfacesAndParentClasses;

        // IsSticky, GetScope, SetScope, ClassMember
        public List<(bool, Visbility, Visbility, ClassMember)> ClassMemberInfo;
    }
}
