﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class Pack : Exp
    {
        public AnyType Type;
        public Exp Exp;

        public Pack(AnyType type, Exp exp, int lineNumber, int startCol)
        {
            Type = type;
            Exp = exp;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
