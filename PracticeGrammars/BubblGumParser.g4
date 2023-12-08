parser grammar BubblGumParser;

ruleSet: (slice | definition | COMMENT )* EOF;
slice: sliceHeader (NEW_LINE+ tags)? (NEW_LINE+ definition)* (NEW_LINE+ (statement))+;

statement: ifStatement | assignment NEW_LINE+;
ifStatement: IF condition COLON block (ELIF condition COLON block)* (ELSE COLON block)?;
condition: (innerCondition) | (OPEN_PAREN innerCondition CLOSE_PAREN);

innerCondition: (unaryCondition) (BINARY_OPERATOR (unaryCondition))*;
unaryCondition: UNARY_OPERATOR? (IDENTIFIER | value);

//block: (NEW_LINE+ (assignment | ifStatement))+;

block: NEW_LINE INDENT statement+ DEDENT;

assignment: (OUT payloadValue ASSIGN value) | definition;
definition: IDENTIFIER ASSIGN (value);

value: (mathOperation | reference) | (OPEN_PAREN condition CLOSE_PAREN);

// TODO: We should make this simpler
mathOperation: mathOperationTerm (ARITHMETIC_OPERATOR mathOperationTerm)*;
mathOperationTerm: mathOperationFactor (GEOMETRIC_OPERATOR mathOperationFactor)*;
mathOperationFactor: mathOperationUnit;
mathOperationUnit: ((ARITHMETIC_OPERATOR)? NUMBER | reference | ((ARITHMETIC_OPERATOR)? OPEN_PAREN mathOperation CLOSE_PAREN));

reference: payloadValue | internalValue | list;
payloadValue: IDENTIFIER (DOT IDENTIFIER)*;
internalValue: TRUE | FALSE | NUMBER | STRING;

sliceHeader: SLICE IDENTIFIER;
tags: TAGS ASSIGN list;
list: OPEN_BRACK (reference (COMMA reference)*)? CLOSE_BRACK;