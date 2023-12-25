grammar BubblGum;

program: (class | interface | function | struct | statement)* EOF;

class: STICKY? GUM IDENTIFIER (COLON IDENTIFIER (COMMA IDENTIFIER)*)? LEFT_CURLY_BRACKET class_member* RIGHT_CURLY_BRACKET;

interface: STICKY? WRAPPER IDENTIFIER (COLON IDENTIFIER (COMMA IDENTIFIER)*)? LEFT_CURLY_BRACKET interface_member* RIGHT_CURLY_BRACKET;

interface_member: STICKY? visibility? (function_header 
            | (primitive_declaration (PRINT | DEBUG)?) 
            | (assignment (PRINT | DEBUG)?));

class_member: STICKY? visibility? (function
           | (primitive_declaration (PRINT | DEBUG)?)
           | (assignment (PRINT | DEBUG)?));

visibility: BOLD | SUBTLE | BLAND;

struct: CANDY COLON IDENTIFIER ((COLON single_statement) | scope_body);
function: function_header ((COLON single_statement) | scope_body);
function_header: RECIPE COLON IDENTIFIER parameters (outputs | type)?; // outputStream | singleOutput
parameters: LEFT_PAREN (IMMUTABLE? type IDENTIFIER ELIPSES? (COMMA IMMUTABLE? type IDENTIFIER ELIPSES?)*)? RIGHT_PAREN;
outputs: LEFT_ANGLE_BRACKET ((type (IDENTIFIER)? ELIPSES? (COMMA type IDENTIFIER? ELIPSES?)*)) RIGHT_ANGLE_BRACKET;

scope_body: LEFT_CURLY_BRACKET statement_list RIGHT_CURLY_BRACKET; // { statements }
statement_list: (statement)*; // statements

statement: single_statement | scope_body;
single_statement: base_statement | print_statement | debug_statement | if_statement | loop;
print_statement: (base_statement | expression) PRINT PRINT?;
debug_statement: (base_statement | expression) DEBUG;

// anything that can be printed out or debugged
base_statement: primitive_declaration | assignment | variable_inc_dec | return_statement;
return_statement: (POP) | (POP expression (THICK_ARROW expression)?) |
              (POP expression THICK_ARROW POPSTREAM (LEFT_PAREN expression RIGHT_PAREN)?);

primitive_declaration: primitive IDENTIFIER (COMMA primitive IDENTIFIER)*;
assignment: ((IMMUTABLE? (type | FLAVOR)? IDENTIFIER) | expression)
    (COMMA ((IMMUTABLE? (type | FLAVOR)? IDENTIFIER) | expression))*
    ASSIGN expression;
// supports anything on LHS (ex. $Cow c, sugar a, [sugar] b, flavor d, loneWolf, Life->HappinessCount, a[0] :: b )

variable_inc_dec: expression (PLUS_COLON | MINUS_COLON) expression;

if_statement: IF expression ((COLON single_statement) | scope_body) elif_statement* else_statement?;
elif_statement: (ELIF expression ((COLON single_statement) | scope_body));
else_statement: (ELSE ((COLON single_statement) | scope_body));

loop: while_loop | repeat_loop | pop_loop;
while_loop: WHILE expression ((COLON single_statement) | scope_body);
repeat_loop: IDENTIFIER COLON (REPEAT_DOWN | REPEAT_UP) LEFT_PAREN (INTEGER_LITERAL | expression) COMMA
             (INTEGER_LITERAL | expression) RIGHT_PAREN ((COLON single_statement) | scope_body);
pop_loop: POP FLAVORS IDENTIFIER IN expression THICK_ARROW (single_statement | scope_body);

// operator precedence loosely based off https://introcs.cs.princeton.edu/java/11precedence/
expression: LEFT_PAREN expression RIGHT_PAREN |
              SWEETS THIN_ARROW expression | // global access
              expression LEFT_SQUARE_BRACKET expression RIGHT_SQUARE_BRACKET | // array access
              expression THIN_ARROW SIZE | // array size access
              expression THIN_ARROW EMPTY | // object empty access
              expression THIN_ARROW expression | // member access
              expression LEFT_PAREN (expression? | (expression (COMMA expression)*))  RIGHT_PAREN | // method call or new object
              array LEFT_PAREN expression RIGHT_PAREN | // new array
              LEFT_ANGLE_BRACKET expression (COMMA expression)* RIGHT_ANGLE_BRACKET | // new tuple object
              (PLUS_PLUS | MINUS_MINUS) expression | // start of operator precedence
              expression (PLUS_PLUS | MINUS_MINUS) |
              (NOT | NOT_OP) expression |
              expression (POWER | MODULO) expression |
              expression (MULTIPLY | DIVIDE) expression|
              expression (PLUS | MINUS) expression |
              expression (LEFT_SHIFT | RIGHT_SHIFT) expression |
              expression (GT_EQ | LT_EQ | LEFT_ANGLE_BRACKET | RIGHT_ANGLE_BRACKET) expression |
              expression (EQUALS | NOT_EQ_1 | NOT_EQ_2 | IS | SUBCLASS_OF) expression |
              expression (AND | AND_OP) expression |
              expression (XOR | XOR_OP) expression |
              expression (OR | OR_OP) expression | // end of operator precedence
              boolean |
              identifier |
              double |
              int |
              STRING_LITERAL |
              CHAR_LITERAL |
              FLAVORLESS;

double : (PLUS | MINUS)? INTEGER_LITERAL DOT INTEGER_LITERAL?;
int : (PLUS | MINUS)? INTEGER_LITERAL;
boolean: YUP | NOPE;

//string_formatted : STRING_FORMAT_LESS | (STRING_FORMAT_OPEN (expression? STRING_FORMAT_INNER)* expression? STRING_FORMAT_CLOSE);
//BACK_TICK (ESCAPE_SEQUENCE | '\\{' | '\\}' | STRING_FORMAT_TOKEN | '{' expression '}')* BACK_TICK;

//// identifier: any identifier you can find in code
identifier: (IDENTIFIER | THIS);

type: primitive | array | tuple | IDENTIFIER;
array: primitive_pack | any_array | identifier PACK;
primitive: SUGAR | CARB | CAL | KCAL | YUM | (PURE SUGAR);
tuple: LEFT_ANGLE_BRACKET (type | FLAVOR) IDENTIFIER? (COMMA (type | FLAVOR) IDENTIFIER?)* RIGHT_ANGLE_BRACKET;
primitive_pack: SUGARPACK | CARBPACK | CALPACK | KCALPACK | YUMPACK | (PURE SUGARPACK);
any_array: LEFT_SQUARE_BRACKET (type | FLAVOR) IDENTIFIER? (COMMA (type | FLAVOR) IDENTIFIER?)* RIGHT_SQUARE_BRACKET;

/* ------------------------ TOKENS ------------------------*/
// keywords
THIS: 'gum';            // this
SWEETS: 'sweets';       // global namespace
RECIPE: 'recipe';       // method
CANDY: 'candy';         // struct
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
EMPTY: 'empty';         // Object's empty status
INPUT: 'input';         // input from stdin     
PURE: 'pure';           // unsinged
STICKY: 'sticky';       // static
WRAPPER: 'Wrapper';     // interface

PACK: 'pack';
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
FLAVORLESS: 'flavorless' | 'nflv';
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
DOUBLE_QUOTE: '"';
SINGLE_QUOTE: '\'';
BACK_TICK: '`';
IMMUTABLE: '$';

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
SUBCLASS_OF: ':<';
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
STRING_LITERAL : DOUBLE_QUOTE (ESCAPE_SEQUENCE | ~["\\])* DOUBLE_QUOTE;
CHAR_LITERAL: SINGLE_QUOTE (ESCAPE_SEQUENCE | ~['\\]) SINGLE_QUOTE;
ESCAPE_SEQUENCE: '\\' [btnfr"'\\];

//STRING_FORMATLESS: BACK_TICK (~[`\\{}])* BACK_TICK;
//STRING_FORMAT_OPEN: BACK_TICK (~[`\\{}])* LEFT_CURLY_BRACKET;
//STRING_FORMAT_INNER: RIGHT_CURLY_BRACKET (~[`\\{}])* LEFT_CURLY_BRACKET;
//STRING_FORMAT_CLOSE: RIGHT_CURLY_BRACKET (~[`\\{}])* BACK_TICK;

WHITE: [\r\n\t ] -> channel(HIDDEN);
EOL: '\r\n' -> channel(HIDDEN);
SINGLE_LINE_COMMENT: '#' ~[\r\n]* -> channel(HIDDEN);
MULTI_LINE_COMMENT: '#>' .*? '<#' -> channel(HIDDEN);
