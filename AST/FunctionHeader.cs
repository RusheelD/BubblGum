using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class FunctionHeader : AstNode, InterfaceMember
    {
        public string Name;
        public List<(bool, AnyType, string, bool)> Params; // isImmutable, type, name, isEllipses
        public List<(AnyType, string, bool)> Outputs; // Type, VarName, HasElipses
        public const string EMPTY = "";

        public FunctionHeader(string name, List<(bool, AnyType, string, bool)> Params,
            List<(AnyType, string, bool)> outputs, int lineNumber, int startCol)
        {
            Name = name;
            this.Params = Params;
            Outputs = outputs;
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
