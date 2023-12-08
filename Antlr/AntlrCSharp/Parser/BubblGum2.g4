grammar BubblGum2;



// program: (expression | function | class)* EOF;
// program: ((ASSIGN | GUM | RECIPE | FLAVOR)* EOF) | program2;
program: (function | class | statement)* EOF;


class: GUM IDENTIFIER LEFT_CURLY_BRACKET class_member* RIGHT_CURLY_BRACKET;
class_member: visibility? (function | variable_declaration | variable_declaration_assignment);
visibility: BOLD | SUBTLE;
function: function_header scope_body;
function_header: constructor_header (outputs | single_output)?;
constructor_header: ((RECIPE COLON) | RECIPE_COLON) IDENTIFIER parameters;
parameters: LEFT_PAREN ((type IDENTIFIER)? | (type IDENTIFIER (COMMA type IDENTIFIER)*)) RIGHT_PAREN;
outputs: LEFT_ANGLE_BRACKET ((type IDENTIFIER?)? | (type IDENTIFIER? (COMMA type IDENTIFIER?)*)) RIGHT_ANGLE_BRACKET;
single_output: type;

if_statement: IF LEFT_PAREN expression RIGHT_PAREN scope_body (ELIF LEFT_PAREN expression RIGHT_PAREN scope_body)* (ELSE scope_body)?;
loop: while_loop | repeat_loop | pop_loop;
while_loop: WHILE LEFT_PAREN expression RIGHT_PAREN scope_body;
repeat_loop: IDENTIFIER COLON (REPEAT_DOWN | REPEAT_UP) LEFT_PAREN (INTEGER_LITERAL | expression) COMMA (INTEGER_LITERAL | expression) RIGHT_PAREN scope_body;
pop_loop: POP FLAVORS IDENTIFIER IN identifier THICK_ARROW scope_body;
scope_body: LEFT_CURLY_BRACKET (statement)* RIGHT_CURLY_BRACKET;


statement: base_statement | print_statement | debug_statement | loop | if_statement;
print_statement:base_statement PRINT;
debug_statement: base_statement DEBUG;

// anything that can be printed out or debugged
base_statement: variable_declaration | variable_declaration_assignment | variable_assignment; 

// return_statement: (POP expression (THICK_ARROW (INTEGER_LITERAL | IDENTIFIER))?) | (POP FLAVORS IDENTIFIER IN identifier THICK_ARROW POPSTREAM);
variable_declaration_assignment: type IDENTIFIER ASSIGN expression;
variable_declaration: type IDENTIFIER;

        
variable_assignment: expression ASSIGN term;

term: expression;

expression: expression operators expression |
            expression LEFT_SQUARE_BRACKET expression RIGHT_SQUARE_BRACKET |
            boolean | 
            (primitive_pack | (primitive PACK)) LEFT_PAREN expression RIGHT_PAREN |
            NOT expression |
            LEFT_PAREN expression RIGHT_PAREN |
            expression LEFT_PAREN (expression? | (expression (COMMA expression)*))  RIGHT_PAREN |
            expression THIN_ARROW SIZE |
            identifier | 
            INTEGER_LITERAL | 
            number;

            
number: ((ARITHMETIC_OPERATOR)? INTEGER_LITERAL) | ((ARITHMETIC_OPERATOR)? INTEGER_LITERAL DOT INTEGER_LITERAL);
boolean: YUP | NOPE;

// identifier: any identifier you can find in code
identifier: (IDENTIFIER | THIS) (THIN_ARROW IDENTIFIER)*; 

type: primitive | primitive PACK | primitive_pack | IDENTIFIER;
primitive: FLAVOR | SUGAR | CARB | CAL | KCAL | YUM;
primitive_pack: FLAVORPACK | SUGARPACK | CARBPACK | CALPACK | KCALPACK | YUMPACK;
operators: PLUS | MINUS | POWER | MULTIPLY | DIVIDE | LEFT_SHIFT | RIGHT_SHIFT | MODULO | BINARY_OPERATOR 
            | LEFT_ANGLE_BRACKET | RIGHT_ANGLE_BRACKET;


/* ------------------------ TOKENS ------------------------*/
// keywords
ASSIGN: '::';
THIS: 'gum';
RECIPE: 'recipe';
RECIPE_COLON: 'recipe:';
GUM: 'Gum';
FLAVOR: 'flavor';
FLAVORS: 'flavors';
SUGAR: 'sugar'; 
CARB: 'carb';
CAL: 'cal';
KCAL: 'kcal';
YUM: 'yum';
BOLD: 'bold';
SUBTLE: 'subtle';
POP: 'pop';
SIZE: 'size';

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
PRINT: '!';
DEBUG: '?';
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

// DELIMEITERS
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
DOT: '.';

UNARY_OPERATOR: NOT_OP | NOT;
OPERATOR: PLUS | MINUS | POWER | MULTIPLY | DIVIDE | LEFT_SHIFT | RIGHT_SHIFT | MODULO | BINARY_OPERATOR
            | LEFT_ANGLE_BRACKET | RIGHT_ANGLE_BRACKET;
BINARY_OPERATOR: EQUALS | GT_EQ | LT_EQ | NOT_EQ_1 | NOT_EQ_2 | AND_OP | OR_OP | AND | OR;

// OPERATORS
EQUALS: '=';
GT_EQ : '>=';
LT_EQ : '<=';
LEFT_SHIFT: '<<';
RIGHT_SHIFT: '>>';
NOT_EQ_1 : '<>';
NOT_EQ_2 : '~=';
AND_OP: '&';
OR_OP: '|';
NOT_OP: '~';
PLUS: '+';
MINUS: '-';
POWER: '**';
MULTIPLY: '*';
DIVIDE: '/';
MODULO: '%';
THIN_ARROW: '->';
THICK_ARROW: '=>';

IDENTIFIER: [a-zA-Z_] [a-zA-Z_0-9]*;
LETTER: [a-zA-Z];
INTEGER_LITERAL: ([0] | ([1-9] [0-9]*));

WHITE: [\r\n\t ] -> channel(HIDDEN);
EOL: '\r\n' -> channel(HIDDEN);
SINGLE_LINE_COMMENT: '#' ~[\r\n]* -> channel(HIDDEN);
MULTI_LINE_COMMENT: '#>' .*? '<#' -> channel(HIDDEN);

