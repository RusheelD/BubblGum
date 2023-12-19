using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrCSharp.AST
{
    public class ObjectAssign : Statement
    {
        public List<(bool, ObjectType, Identifier)> Variables; // (IsImmutable Type VarName)
        public Exp Value;
        public bool IsFlavorless;

        public ObjectAssign(List<(bool, ObjectType, Identifier)> variables, 
            Exp value, bool isFlavorless, int lineNumber, int startCol)
        {
            Variables = variables;
            Value = value;
            IsFlavorless = isFlavorless;

            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}
