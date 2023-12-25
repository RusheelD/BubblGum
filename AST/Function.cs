using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class Function : AstNode, ClassMember
    {
        public FunctionHeader Header;
        public List<Statement> Statements;

        public Function(FunctionHeader header, List<Statement> statements, int lineNumber, int startCol)
        {
            Header = header;
            Statements = statements;
            LineNumber = lineNumber;
            StartCol = startCol;
        }
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
