grammar BubblGumHeader;

program: chew_import* define_stock? (ANYCHAR|IDENTIFIER|STRING_LITERAL|THIN_ARROW|STOCK|CHEW|ESCAPE_SEQUENCE|FROM)* EOF;

define_stock: STOCK (IDENTIFIER) (THIN_ARROW IDENTIFIER)*;
chew_import: CHEW ((IDENTIFIER (THIN_ARROW IDENTIFIER)*) | STRING_LITERAL);

/* ------------------------ TOKENS ------------------------*/
// keywords

STOCK: 'stock';         // define new namespace
CHEW: 'Chew';           // using/import
FROM: 'from';

IDENTIFIER: [a-zA-Z_] [a-zA-Z_0-9]*;
THIN_ARROW: '->';
STRING_LITERAL : '"' (ESCAPE_SEQUENCE | ~["\\])* '"';
ESCAPE_SEQUENCE: '\\' [btnfr"'\\];

WHITE: [\r\n\t ] -> channel(HIDDEN);
EOL: '\r\n' -> channel(HIDDEN);
SINGLE_LINE_COMMENT: '#' ~[\r\n]* -> channel(HIDDEN);
MULTI_LINE_COMMENT: '#>' .*? '<#' -> channel(HIDDEN);

ANYCHAR: .;