grammar BubblGum2;


 program: (statement)* EOF;
//program: (function | class | statement)* EOF;

//class: GUM IDENTIFIER LEFT_CURLY_BRACKET class_member* RIGHT_CURLY_BRACKET;
//class_member: visibility? (function | variable_declaration | variable_declaration_assignment);
//visibility: BOLD | SUBTLE | BLAND;
//function: function_header scope_body;
//function_header: constructor_header (outputs | single_output)?;
constructor_header: (RECIPE COLON) IDENTIFIER parameters;
parameters: LEFT_PAREN ((ref_type IDENTIFIER)? | (ref_type IDENTIFIER (COMMA ref_type IDENTIFIER)*)) RIGHT_PAREN;
outputs: LEFT_ANGLE_BRACKET ((ref_type IDENTIFIER?)? | (ref_type IDENTIFIER? (COMMA ref_type IDENTIFIER?)*)) RIGHT_ANGLE_BRACKET;
ref_type: type | IDENTIFIER;

/*

if a:
    (3 * 4)!

if a = 2:
   b :: 3
a :: 3 singl

if b = 2: a :: 3

if b = 2 { a :: 2}


if (b = 2) {
   b :: 3
   a :: 3
}elif a:
    b::3
    a::3

    
if (b = 2) {
   b :: 3
   a :: 3
}elif a {b :: 3}
else:
    a::3
    b::3

    
if (b = 2) {
   b :: 3
   a :: 3
}elif a {b :: 3}
b:: 2
else:
    a::3
    b::3

while a:
    b :: c
    c::d

while t {
    c :: d
    e::f
}
a : repeatUp (c, d):
    a :: b
    c :: d

a : repeatDown (c, d){
    a :: b
    c :: d
}

sugar c :: 0
pop flavors a in z => c +: a

/*
 {}
 {statement}

{
sugar a :: 2
sugar b :: 2
}
{}

public class repeat
 bool isUp
 Exp e1

 public class IntegerLiteral : Exp

{{statement1; statement2; statement3;} {statement4; statement5; statement1;} }
*/

scope_body: LEFT_CURLY_BRACKET statement_list RIGHT_CURLY_BRACKET; // { statements }
statement_list: (statement)*; // statements

statement: single_statement | scope_body;
single_statement: base_statement | print_statement | debug_statement | if_statement | loop;
print_statement: (base_statement | expression) PRINT;
debug_statement: (base_statement | expression) DEBUG;

// anything that can be printed out or debugged
base_statement: variable_declaration | variable_declaration_assignment | variable_assignment | variable_inc_dec | object_declaration_assignment | return_statement;
return_statement: (POP expression (THICK_ARROW expression)?) |
              (POP expression THICK_ARROW POPSTREAM (LEFT_PAREN expression RIGHT_PAREN)?);
object_declaration_assignment: IDENTIFIER IDENTIFIER ASSIGN (FLAVORLESS | expression);
variable_declaration_assignment: type IDENTIFIER ASSIGN expression;
variable_declaration: primitive IDENTIFIER;
variable_assignment: expression ASSIGN expression;
variable_inc_dec: expression (PLUS_COLON | MINUS_COLON) expression;

if_statement: IF expression ((COLON single_statement) | scope_body) elif_statement* else_statement?;
elif_statement: (ELIF expression ((COLON single_statement) | scope_body));
else_statement: (ELSE ((COLON single_statement) | scope_body));

loop: while_loop | repeat_loop | pop_loop;
while_loop: WHILE expression ((COLON single_statement) | scope_body);
repeat_loop: IDENTIFIER COLON (REPEAT_DOWN | REPEAT_UP) LEFT_PAREN (INTEGER_LITERAL | expression) COMMA (INTEGER_LITERAL | expression) RIGHT_PAREN ((COLON single_statement) | scope_body);
pop_loop: POP FLAVORS IDENTIFIER IN expression THICK_ARROW (single_statement | scope_body);

/*
IDENTIfIER is IDENTIfIER
a is b
a is Animal Animal

a = b // compare valeus (values of primitives or values at pointers)
a is b // check if a and b are the same reference (same Object reference, same string Reference)
a :< b // check if a is a subclass of b, or a is the same type as b
  
*/

// operator precedence loosely based off https://introcs.cs.princeton.edu/java/11precedence/
expression: LEFT_PAREN expression RIGHT_PAREN |
              expression LEFT_SQUARE_BRACKET expression RIGHT_SQUARE_BRACKET | // array access
              expression THIN_ARROW SIZE | // array size access
              expression get_member | // member access
              (primitive_pack | (primitive PACK)) LEFT_PAREN expression RIGHT_PAREN | // new array
              expression LEFT_PAREN (expression? | (expression (COMMA expression)*))  RIGHT_PAREN | // method call
              expression LEFT_PAREN RIGHT_PAREN | // new object
              (PLUS_PLUS | MINUS_MINUS) expression | // start of operator precedence
              expression (PLUS_PLUS | MINUS_MINUS) |
              (NOT | NOT_OP) expression |
              expression (POWER | MODULO) expression |
              expression (MULTIPLY | DIVIDE) expression|
              expression (PLUS | MINUS) expression |
              expression (LEFT_SHIFT | RIGHT_SHIFT) expression |
              expression (GT_EQ | LT_EQ | LEFT_ANGLE_BRACKET | RIGHT_ANGLE_BRACKET) expression |
              expression (EQUALS | NOT_EQ_1 | NOT_EQ_2 | IS | POINTER_EQUAL) expression |
              expression (AND | AND_OP) expression |
              expression (XOR | XOR_OP) expression |
              expression (OR | OR_OP) expression | // end of operator precedence
              boolean |
              identifier |
              double |
              int;

double : (PLUS | MINUS)? INTEGER_LITERAL DOT INTEGER_LITERAL?;
int : (PLUS | MINUS)? INTEGER_LITERAL;
boolean: YUP | NOPE;

//// identifier: any identifier you can find in code
identifier: (IDENTIFIER | THIS);
get_member: (THIN_ARROW IDENTIFIER)+;

type: primitive | primitive PACK | primitive_pack;
primitive: FLAVOR | SUGAR | CARB | CAL | KCAL | YUM | (PURE SUGAR);
primitive_pack: FLAVORPACK | SUGARPACK | CARBPACK | CALPACK | KCALPACK | YUMPACK | (PURE SUGARPACK);

/* ------------------------ TOKENS ------------------------*/
// keywords
THIS: 'gum';
RECIPE: 'recipe';       // method
GUM: 'Gum';             // class
FLAVOR: 'flavor';       // var
FLAVORS: 'flavors';     // keyword
SUGAR: 'sugar';         // int
CARB: 'carb';           // double
CAL: 'cal';             // char
KCAL: 'kcal';           // string
YUM: 'yum';             // bool
BOLD: 'bold';           // public
SUBTLE: 'subtle';       // protected
BLAND: 'bland';         // private
POP: 'pop';             // return (but better) (also a foreach loop)
SIZE: 'size';           // array size      
PURE: 'pure';           // unsinged

PACK: 'pack';
FLAVORPACK: 'flavorpack';
SUGARPACK: 'sugarpack'; 
CARBPACK: 'carbpack';
CALPACK: 'calpack';
KCALPACK: 'kcalpack';
YUMPACK: 'yumpack';

YUP: 'yup';
NOPE: 'nope';
AND: 'and';
OR: 'or';
XOR: 'xor';
XNOR: 'xnor';
FLAVORLESS: 'flavorless';
IF: 'if';
ELSE: 'else';
ELIF: 'elif';
WHILE: 'while';
REPEAT_UP: 'repeatUp';
REPEAT_DOWN: 'repeatDown';
POPSTREAM: 'popstream';
NOT: 'not';
IN: 'in';
IS: 'is';

// DELIMITERS
ASSIGN: '::';
LEFT_PAREN: '(';
RIGHT_PAREN: ')';
LEFT_SQUARE_BRACKET: '[';
RIGHT_SQUARE_BRACKET: ']';
LEFT_CURLY_BRACKET: '{';
RIGHT_CURLY_BRACKET: '}';
LEFT_ANGLE_BRACKET: '<';
RIGHT_ANGLE_BRACKET: '>';
COMMA: ',';
SEMICOLON: ';';
COLON: ':';
ELIPSES: '...';
DOT: '.';
PRINT: '!';
DEBUG: '?';

// OPERATORS
GT_EQ : '>=';
LT_EQ : '<=';
LEFT_SHIFT: '<<';
RIGHT_SHIFT: '>>';
NOT_EQ_1 : '<>';
NOT_EQ_2 : '~=';
PLUS_PLUS: '++';
MINUS_MINUS: '--';
PLUS_COLON: '+:';
MINUS_COLON: '-:';
THIN_ARROW: '->';
THICK_ARROW: '=>';
POINTER_EQUAL: ':<';
EQUALS: '=';
AND_OP: '&';
OR_OP: '|';
NOT_OP: '~';
XOR_OP: '^';
PLUS: '+';
MINUS: '-';
POWER: '**';
MULTIPLY: '*';
DIVIDE: '/';
MODULO: '%';

IDENTIFIER: [a-zA-Z_] [a-zA-Z_0-9]*;
LETTER: [a-zA-Z];
INTEGER_LITERAL: (([0-9] [0-9]*));

WHITE: [\r\n\t ] -> channel(HIDDEN);
EOL: '\r\n' -> channel(HIDDEN);
SINGLE_LINE_COMMENT: '#' ~[\r\n]* -> channel(HIDDEN);
MULTI_LINE_COMMENT: '#>' .*? '<#' -> channel(HIDDEN);

