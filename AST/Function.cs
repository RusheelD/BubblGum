using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Function : AstNode, ClassMember, ProgramPiece
    {
        public FunctionHeader Header;
        public List<AstNode> Statements;

        public Function(FunctionHeader header, List<AstNode> statements, int lineNumber, int startCol)
        {
            Header = header;
            Statements = statements;
            LineNumber = lineNumber;
            StartCol = startCol;
        }

        public override void Accept(Visitor v) => v.Visit(this);
    }

    /*
    function_header: RECIPE COLON IDENTIFIER parameters(outputs | type)?; 

    parameters: LEFT_PAREN(IMMUTABLE? type IDENTIFIER ELIPSES?
        (COMMA IMMUTABLE? type IDENTIFIER ELIPSES?)*)? 
        RIGHT_PAREN;
    
    outputs: LEFT_ANGLE_BRACKET ((type (IDENTIFIER)? ELIPSES? 
        (COMMA type IDENTIFIER? ELIPSES?)*)) RIGHT_ANGLE_BRACKET;
    */
}
