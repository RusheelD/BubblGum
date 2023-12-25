﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
    public class ObjectEmpty : Exp
    {
        public Exp E1;

        public ObjectEmpty(Exp e1, int lineNumber, int startCol)
        {
            E1 = e1;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}