﻿using Ast.enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Class : AstNode
    {
        public bool IsSticky;
        public Visbility Visbility;
        public string Name;
        public List<string> InterfacesAndParentClasses;

        public Class(bool isSticky, Visbility visbility, string name, 
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
