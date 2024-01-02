grammar BubblGumHeader;

program: chew_import* define_stock? (ANYCHAR|IDENTIFIER|STRING_LITERAL|THIN_ARROW|STOCK|CHEW|ESCAPE_SEQUENCE)* EOF;

define_stock: STOCK (IDENTIFIER) (THIN_ARROW IDENTIFIER)*;
chew_import: CHEW ((IDENTIFIER (THIN_ARROW IDENTIFIER)*) | STRING_LITERAL);

/* ------------------------ TOKENS ------------------------*/
// keywords

STOCK: 'stock';         // define new namespace


CHEW: 'Chew';           // using/import

IDENTIFIER: [a-zA-Z_] [a-zA-Z_0-9]*;
THIN_ARROW: '->';
STRING_LITERAL : '"' (ESCAPE_SEQUENCE | ~["\\])* '"';
ESCAPE_SEQUENCE: '\\' [btnfr"'\\];

//STRING_FORMATLESS: BACK_TICK (~[`\\{}])* BACK_TICK;
//STRING_FORMAT_OPEN: BACK_TICK (~[`\\{}])* LEFT_CURLY_BRACKET;
//STRING_FORMAT_INNER: RIGHT_CURLY_BRACKET (~[`\\{}])* LEFT_CURLY_BRACKET;
//STRING_FORMAT_CLOSE: RIGHT_CURLY_BRACKET (~[`\\{}])* BACK_TICK;

WHITE: [\r\n\t ] -> channel(HIDDEN);
EOL: '\r\n' -> channel(HIDDEN);
SINGLE_LINE_COMMENT: '#' ~[\r\n]* -> channel(HIDDEN);
MULTI_LINE_COMMENT: '#>' .*? '<#' -> channel(HIDDEN);

ANYCHAR: .;