﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public abstract class Exp : AstNode, AssignLHS, Printable
    {
        public AnyType Type { get; }
    }
}
