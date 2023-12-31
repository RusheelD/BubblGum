﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Class : AstNode, ProgramPiece
    {
        public bool IsSticky;
        public Visbility Visbility;
        public string Name;
        public List<string> InterfacesAndParentClasses;

        // IsSticky, GetScope, SetScope, ClassMember
        public List<(bool, Visbility, Visbility, AstNode)> ClassMemberInfo;

        public Class(bool isSticky, Visbility visbility, string name, 
            List<string> interfacesAndParentClasses,
            List<(bool, Visbility, Visbility, AstNode)> classMemberInfo, int lineNumber, int startCol)
        {
            IsSticky = isSticky;
            Visbility = visbility;
            Name = name;
            InterfacesAndParentClasses = interfacesAndParentClasses;
            ClassMemberInfo = classMemberInfo;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }
}
