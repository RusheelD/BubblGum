using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class GumFlavorInfo : FlavorInfo
    {
        public Visbility Get, Set;
        public bool IsSticky;


        public GumFlavorInfo(Visbility get, Visbility set, bool isSticky,
            string name, AnyType type, bool isEmpty, bool isImmutable, int line, int col) 
            : base(name, type, isEmpty, isImmutable, line, col)
        {
           Get = get;
           Set = set;
           IsSticky = isSticky;
        }

        // public GumFlavorInfo(Visbility get, Visbility set, bool isSticky,
        //     string name, bool isEmpty, bool isImmutable, int line, int col) 
        //     : base(name, isEmpty, isImmutable, line, col)
        // {
        //    Get = get;
        //    Set = set;
        //    IsSticky = isSticky;
        // }
    }

}
