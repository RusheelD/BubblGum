grammar BubblGum;


//program: (expression | function | class)* EOF;
program: (ASSIGN | GUM | RECIPE | FLAVOR)* EOF;


/* ------------------------ TOKENS ------------------------*/

// keywords
ASSIGN: '::';
THIS: 'gum';
RECIPE: 'recipe';
GUM: 'Gum';
FLAVOR: 'flavor';
SUGAR: 'sugar';
CARB: 'carb';
CAL: 'cal';
KCAL: 'kcal';
YUM: 'yum';
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
NOT: 'not';

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
DOT: '.';

// OPERATORS
EQUALS: '=';
LESS_THAN : '<';
GREATER_THAN : '>';
GT_EQ : '>=';
LT_EQ : '<=';
NOT_EQ_1 : '<>';
NOT_EQ_2 : '~=';
AND_OP: '&';
OR_OP: '|';
NOT_OP: '~';
PLUS: '+';
MINUS: '-';
MULTIPLY: '*';
DIVIDE: '/';
MODULO: '%';
THIN_ARROW: '->';
THICK_ARROW: '=>';

IDENTIFIER: [a-zA-Z_] [a-zA-Z_0-9]*;
LETTER: [a-zA-Z];
INTEGER_LITERAL: ([0] | ([1-9] [0-9*]+));

WHITE: [\r\n\t ] -> channel(HIDDEN);
EOL: '\r\n' -> channel(HIDDEN);
SINGLE_LINE_COMMENT: '#' ~[\r\n]* -> channel(HIDDEN);
MULTI_LINE_COMMENT: '#>' .*? '<#' -> channel(HIDDEN);

