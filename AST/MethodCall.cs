﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrCSharp.AST
{
    public abstract class AstNode
    {
        public int LineNumber;
        public int StartCol;
    }
}