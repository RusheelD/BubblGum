using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Antlr4.Runtime.Atn.SemanticContext;

namespace   AST
{
    public class VarDecl : Statement
    {
        public TypeBI TypeInfo;
        public List<Identifier> Variables;

        public VarDecl(TypeBI typeInfo, List<Identifier> variables, int lineNumber, int startCol)
        {
            TypeInfo = typeInfo;
            Variables = variables;
            
            LineNumber = lineNumber;
            StartCol = startCol;
        }
    }
}