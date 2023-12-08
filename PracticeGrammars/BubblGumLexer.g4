lexer grammar BubblGumLexer;

UNARY_OPERATOR: NOT_OP | PLUS | MINUS;
BINARY_OPERATOR: LESS_THAN | GREATER_THAN | EQUALS | GT_EQ | LT_EQ | NOT_EQ_1 | NOT_EQ_2 | AND_OP | OR_OP | CONTAINS | IN;
ARITHMETIC_OPERATOR: PLUS | MINUS;
GEOMETRIC_OPERATOR: MULTIPLY | DIVIDE;

OUT: 'out';
CONTAINS: 'contains';
IN: 'in';
IF: 'If' | 'if';
ELIF: 'Elif' | 'elif';
ELSE: 'Else' | 'else';
TRUE: 'true' | 'True';
FALSE: 'false' | 'False';
SLICE: 'Slice';
TAGS: 'Tags';
AND: 'and';
OR: 'or';
NOT: 'not';

IDENTIFIER: [a-zA-Z_] [a-zA-Z_0-9]*;
NUMBER: [1-9] [0-9*]+;
STRING: '\'' ~('\'')* '\'' | '"' ~('"')* '"';

OPEN_PAREN : '(';
CLOSE_PAREN : ')';
OPEN_BRACK : '[';
CLOSE_BRACK : ']';
OPEN_BRACE : '{';
CLOSE_BRACE : '}';
OPEN_ANGLE : '<';
CLOSE_ANGLE : '>';
ASSIGN: '::';
LESS_THAN : '<';
GREATER_THAN : '>';
EQUALS : '=';
GT_EQ : '>=';
LT_EQ : '<=';
NOT_EQ_1 : '<>';
NOT_EQ_2 : '!=';
AND_OP: '&';
OR_OP: '|';
NOT_OP: '!';
PLUS: '+';
MINUS: '-';
MULTIPLY: '*';
DIVIDE: '/';

COMMA: ',';
DOT: '.';
COLON: ':';

//NEW_LINE: '\n';
//NEW_LINE: RN;
//fragment RN: '\r'?'\n';

NEW_LINE
 : ( {this.atStartOfInput()}?   SPACES
   | ( '\r'? '\n' | '\r' | '\f' ) SPACES?
   )
   {this.onNewLine();}
 ;

IGNORED: (COMMENT | SPACES) -> skip;

COMMENT: (LINE_COMMENT | MULTILINE_COMMENT);
fragment LINE_COMMENT: ('#' ~[\r\n\f]*);
fragment MULTILINE_COMMENT: ('#>' (.+?)* '<#');
fragment SPACES: [ \t]+;