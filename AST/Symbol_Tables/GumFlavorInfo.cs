﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    /// <summary> Stores info for a local variable of a class </summary>
    public class GumFlavorInfo : FlavorInfo
    {
        public Visbility Get, Set;
        public bool IsSticky;


        public GumFlavorInfo(Visbility get, Visbility set, bool isSticky,
            string name, AnyType type, bool isImmutable, int line, int col) 
            : base(name, type, isImmutable, line, col)
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
