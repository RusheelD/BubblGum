﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrCSharp.AST
{
    public class MethodCall : Exp
    {
        public Exp Lhs;
        public List<Exp> Args;
    }
}
