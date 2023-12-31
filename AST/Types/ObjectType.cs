﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AST
{
   public class ObjectType : AnyType {
        public String Name;

        public ObjectType(String name) {
            Name = name;
        }

        public override void Accept(TypeVisitor v) => v.Visit(this);
    }
}
