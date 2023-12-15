using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntlrCSharp.AST
{
    public class VarDeclAssign : Statement
    {
        public List<(bool, TypeBI, Identifier)> Variables; // (IsImmutable Type VarName)
        public Exp Value;
        public bool IsFlavorless;

        public VarDeclAssign(List<(bool, TypeBI, Identifier)> variables,
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
