﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class ArrayType : AnyType {
        public TupleType TupleType;

        public ArrayType(TupleType tupleType) {
            TupleType = tupleType;
        }

        public override void Accept(TypeVisitor v) => v.Visit(this);
    }
}
