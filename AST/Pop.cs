﻿using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Pop : AstNode, Statement
    {
        public Pop(int lineNumber, int startCol)
        {
            LineNumber = lineNumber;
            StartCol = startCol;
        }
        public override void Accept(Visitor v) => v.Visit(this);
    }
}
