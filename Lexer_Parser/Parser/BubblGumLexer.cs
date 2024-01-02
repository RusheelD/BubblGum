//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ./Lexer_Parser/Parser/BubblGum.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class BubblGumLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		THIS=1, SWEETS=2, STOCK=3, RECIPE=4, CANDY=5, GUM=6, CHEW=7, FLAVOR=8, 
		FLAVORS=9, SUGAR=10, CARB=11, CAL=12, KCAL=13, YUM=14, BOLD=15, SUBTLE=16, 
		BLAND=17, POP=18, PURE=19, STICKY=20, WRAPPER=21, MINTPACK=22, PACK=23, 
		SUGARPACK=24, CARBPACK=25, CALPACK=26, KCALPACK=27, YUMPACK=28, YUP=29, 
		NOPE=30, AND=31, OR=32, XOR=33, XNOR=34, FLAVORLESS=35, IF=36, ELSE=37, 
		ELIF=38, WHILE=39, REPEAT_UP=40, REPEAT_DOWN=41, POPSTREAM=42, NOT=43, 
		IN=44, IS=45, ASSIGN=46, LEFT_PAREN=47, RIGHT_PAREN=48, LEFT_SQUARE_BRACKET=49, 
		RIGHT_SQUARE_BRACKET=50, LEFT_CURLY_BRACKET=51, RIGHT_CURLY_BRACKET=52, 
		LEFT_ANGLE_BRACKET=53, RIGHT_ANGLE_BRACKET=54, COMMA=55, SEMICOLON=56, 
		COLON=57, ELLIPSES=58, DOT=59, PRINT=60, DEBUG=61, DOUBLE_QUOTE=62, SINGLE_QUOTE=63, 
		BACK_TICK=64, IMMUTABLE=65, GT_EQ=66, LT_EQ=67, LEFT_SHIFT=68, RIGHT_SHIFT=69, 
		NOT_EQ_1=70, NOT_EQ_2=71, PLUS_COLON=72, MINUS_COLON=73, THIN_ARROW=74, 
		THICK_ARROW=75, SUBCLASS_OF=76, EQUALS=77, AND_OP=78, OR_OP=79, NOT_OP=80, 
		XOR_OP=81, PLUS=82, MINUS=83, POWER=84, MULTIPLY=85, DIVIDE=86, MODULO=87, 
		IDENTIFIER=88, LETTER=89, INTEGER_LITERAL=90, STRING_LITERAL=91, CHAR_LITERAL=92, 
		ESCAPE_SEQUENCE=93, WHITE=94, EOL=95, SINGLE_LINE_COMMENT=96, MULTI_LINE_COMMENT=97;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"THIS", "SWEETS", "STOCK", "RECIPE", "CANDY", "GUM", "CHEW", "FLAVOR", 
		"FLAVORS", "SUGAR", "CARB", "CAL", "KCAL", "YUM", "BOLD", "SUBTLE", "BLAND", 
		"POP", "PURE", "STICKY", "WRAPPER", "MINTPACK", "PACK", "SUGARPACK", "CARBPACK", 
		"CALPACK", "KCALPACK", "YUMPACK", "YUP", "NOPE", "AND", "OR", "XOR", "XNOR", 
		"FLAVORLESS", "IF", "ELSE", "ELIF", "WHILE", "REPEAT_UP", "REPEAT_DOWN", 
		"POPSTREAM", "NOT", "IN", "IS", "ASSIGN", "LEFT_PAREN", "RIGHT_PAREN", 
		"LEFT_SQUARE_BRACKET", "RIGHT_SQUARE_BRACKET", "LEFT_CURLY_BRACKET", "RIGHT_CURLY_BRACKET", 
		"LEFT_ANGLE_BRACKET", "RIGHT_ANGLE_BRACKET", "COMMA", "SEMICOLON", "COLON", 
		"ELLIPSES", "DOT", "PRINT", "DEBUG", "DOUBLE_QUOTE", "SINGLE_QUOTE", "BACK_TICK", 
		"IMMUTABLE", "GT_EQ", "LT_EQ", "LEFT_SHIFT", "RIGHT_SHIFT", "NOT_EQ_1", 
		"NOT_EQ_2", "PLUS_COLON", "MINUS_COLON", "THIN_ARROW", "THICK_ARROW", 
		"SUBCLASS_OF", "EQUALS", "AND_OP", "OR_OP", "NOT_OP", "XOR_OP", "PLUS", 
		"MINUS", "POWER", "MULTIPLY", "DIVIDE", "MODULO", "IDENTIFIER", "LETTER", 
		"INTEGER_LITERAL", "STRING_LITERAL", "CHAR_LITERAL", "ESCAPE_SEQUENCE", 
		"WHITE", "EOL", "SINGLE_LINE_COMMENT", "MULTI_LINE_COMMENT"
	};


	public BubblGumLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public BubblGumLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "'gum'", "'sweets'", "'stock'", "'recipe'", "'candy'", "'Gum'", 
		"'Chew'", "'flavor'", "'flavors'", "'sugar'", "'carb'", "'cal'", "'kcal'", 
		"'yum'", "'bold'", "'subtle'", "'bland'", "'pop'", "'pure'", "'sticky'", 
		"'Wrapper'", "'mintpack'", "'pack'", "'sugarpack'", "'carbpack'", "'calpack'", 
		"'kcalpack'", "'yumpack'", "'yup'", "'nope'", "'and'", "'or'", "'xor'", 
		"'xnor'", null, "'if'", "'else'", "'elif'", "'while'", "'repeatUp'", "'repeatDown'", 
		"'popstream'", "'not'", "'in'", "'is'", "'::'", "'('", "')'", "'['", "']'", 
		"'{'", "'}'", "'<'", "'>'", "','", "';'", "':'", "'...'", "'.'", "'!'", 
		"'?'", "'\"'", "'''", "'`'", "'$'", "'>='", "'<='", "'<:'", "':>'", "'<>'", 
		"'~='", "'+:'", "'-:'", "'->'", "'=>'", "':<'", "'='", "'&'", "'|'", "'~'", 
		"'^'", "'+'", "'-'", "'**'", "'*'", "'/'", "'%'", null, null, null, null, 
		null, null, null, "'\\r\\n'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "THIS", "SWEETS", "STOCK", "RECIPE", "CANDY", "GUM", "CHEW", "FLAVOR", 
		"FLAVORS", "SUGAR", "CARB", "CAL", "KCAL", "YUM", "BOLD", "SUBTLE", "BLAND", 
		"POP", "PURE", "STICKY", "WRAPPER", "MINTPACK", "PACK", "SUGARPACK", "CARBPACK", 
		"CALPACK", "KCALPACK", "YUMPACK", "YUP", "NOPE", "AND", "OR", "XOR", "XNOR", 
		"FLAVORLESS", "IF", "ELSE", "ELIF", "WHILE", "REPEAT_UP", "REPEAT_DOWN", 
		"POPSTREAM", "NOT", "IN", "IS", "ASSIGN", "LEFT_PAREN", "RIGHT_PAREN", 
		"LEFT_SQUARE_BRACKET", "RIGHT_SQUARE_BRACKET", "LEFT_CURLY_BRACKET", "RIGHT_CURLY_BRACKET", 
		"LEFT_ANGLE_BRACKET", "RIGHT_ANGLE_BRACKET", "COMMA", "SEMICOLON", "COLON", 
		"ELLIPSES", "DOT", "PRINT", "DEBUG", "DOUBLE_QUOTE", "SINGLE_QUOTE", "BACK_TICK", 
		"IMMUTABLE", "GT_EQ", "LT_EQ", "LEFT_SHIFT", "RIGHT_SHIFT", "NOT_EQ_1", 
		"NOT_EQ_2", "PLUS_COLON", "MINUS_COLON", "THIN_ARROW", "THICK_ARROW", 
		"SUBCLASS_OF", "EQUALS", "AND_OP", "OR_OP", "NOT_OP", "XOR_OP", "PLUS", 
		"MINUS", "POWER", "MULTIPLY", "DIVIDE", "MODULO", "IDENTIFIER", "LETTER", 
		"INTEGER_LITERAL", "STRING_LITERAL", "CHAR_LITERAL", "ESCAPE_SEQUENCE", 
		"WHITE", "EOL", "SINGLE_LINE_COMMENT", "MULTI_LINE_COMMENT"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "BubblGum.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static BubblGumLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,97,640,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,
		6,2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,
		7,14,2,15,7,15,2,16,7,16,2,17,7,17,2,18,7,18,2,19,7,19,2,20,7,20,2,21,
		7,21,2,22,7,22,2,23,7,23,2,24,7,24,2,25,7,25,2,26,7,26,2,27,7,27,2,28,
		7,28,2,29,7,29,2,30,7,30,2,31,7,31,2,32,7,32,2,33,7,33,2,34,7,34,2,35,
		7,35,2,36,7,36,2,37,7,37,2,38,7,38,2,39,7,39,2,40,7,40,2,41,7,41,2,42,
		7,42,2,43,7,43,2,44,7,44,2,45,7,45,2,46,7,46,2,47,7,47,2,48,7,48,2,49,
		7,49,2,50,7,50,2,51,7,51,2,52,7,52,2,53,7,53,2,54,7,54,2,55,7,55,2,56,
		7,56,2,57,7,57,2,58,7,58,2,59,7,59,2,60,7,60,2,61,7,61,2,62,7,62,2,63,
		7,63,2,64,7,64,2,65,7,65,2,66,7,66,2,67,7,67,2,68,7,68,2,69,7,69,2,70,
		7,70,2,71,7,71,2,72,7,72,2,73,7,73,2,74,7,74,2,75,7,75,2,76,7,76,2,77,
		7,77,2,78,7,78,2,79,7,79,2,80,7,80,2,81,7,81,2,82,7,82,2,83,7,83,2,84,
		7,84,2,85,7,85,2,86,7,86,2,87,7,87,2,88,7,88,2,89,7,89,2,90,7,90,2,91,
		7,91,2,92,7,92,2,93,7,93,2,94,7,94,2,95,7,95,2,96,7,96,1,0,1,0,1,0,1,0,
		1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,2,1,2,1,2,1,2,1,2,1,2,1,3,1,3,1,3,1,3,1,
		3,1,3,1,3,1,4,1,4,1,4,1,4,1,4,1,4,1,5,1,5,1,5,1,5,1,6,1,6,1,6,1,6,1,6,
		1,7,1,7,1,7,1,7,1,7,1,7,1,7,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,9,1,9,1,
		9,1,9,1,9,1,9,1,10,1,10,1,10,1,10,1,10,1,11,1,11,1,11,1,11,1,12,1,12,1,
		12,1,12,1,12,1,13,1,13,1,13,1,13,1,14,1,14,1,14,1,14,1,14,1,15,1,15,1,
		15,1,15,1,15,1,15,1,15,1,16,1,16,1,16,1,16,1,16,1,16,1,17,1,17,1,17,1,
		17,1,18,1,18,1,18,1,18,1,18,1,19,1,19,1,19,1,19,1,19,1,19,1,19,1,20,1,
		20,1,20,1,20,1,20,1,20,1,20,1,20,1,21,1,21,1,21,1,21,1,21,1,21,1,21,1,
		21,1,21,1,22,1,22,1,22,1,22,1,22,1,23,1,23,1,23,1,23,1,23,1,23,1,23,1,
		23,1,23,1,23,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,24,1,25,1,25,1,
		25,1,25,1,25,1,25,1,25,1,25,1,26,1,26,1,26,1,26,1,26,1,26,1,26,1,26,1,
		26,1,27,1,27,1,27,1,27,1,27,1,27,1,27,1,27,1,28,1,28,1,28,1,28,1,29,1,
		29,1,29,1,29,1,29,1,30,1,30,1,30,1,30,1,31,1,31,1,31,1,32,1,32,1,32,1,
		32,1,33,1,33,1,33,1,33,1,33,1,34,1,34,1,34,1,34,1,34,1,34,1,34,1,34,1,
		34,1,34,1,34,1,34,1,34,1,34,3,34,413,8,34,1,35,1,35,1,35,1,36,1,36,1,36,
		1,36,1,36,1,37,1,37,1,37,1,37,1,37,1,38,1,38,1,38,1,38,1,38,1,38,1,39,
		1,39,1,39,1,39,1,39,1,39,1,39,1,39,1,39,1,40,1,40,1,40,1,40,1,40,1,40,
		1,40,1,40,1,40,1,40,1,40,1,41,1,41,1,41,1,41,1,41,1,41,1,41,1,41,1,41,
		1,41,1,42,1,42,1,42,1,42,1,43,1,43,1,43,1,44,1,44,1,44,1,45,1,45,1,45,
		1,46,1,46,1,47,1,47,1,48,1,48,1,49,1,49,1,50,1,50,1,51,1,51,1,52,1,52,
		1,53,1,53,1,54,1,54,1,55,1,55,1,56,1,56,1,57,1,57,1,57,1,57,1,58,1,58,
		1,59,1,59,1,60,1,60,1,61,1,61,1,62,1,62,1,63,1,63,1,64,1,64,1,65,1,65,
		1,65,1,66,1,66,1,66,1,67,1,67,1,67,1,68,1,68,1,68,1,69,1,69,1,69,1,70,
		1,70,1,70,1,71,1,71,1,71,1,72,1,72,1,72,1,73,1,73,1,73,1,74,1,74,1,74,
		1,75,1,75,1,75,1,76,1,76,1,77,1,77,1,78,1,78,1,79,1,79,1,80,1,80,1,81,
		1,81,1,82,1,82,1,83,1,83,1,83,1,84,1,84,1,85,1,85,1,86,1,86,1,87,1,87,
		5,87,575,8,87,10,87,12,87,578,9,87,1,88,1,88,1,89,1,89,5,89,584,8,89,10,
		89,12,89,587,9,89,1,90,1,90,1,90,5,90,592,8,90,10,90,12,90,595,9,90,1,
		90,1,90,1,91,1,91,1,91,3,91,602,8,91,1,91,1,91,1,92,1,92,1,92,1,93,1,93,
		1,93,1,93,1,94,1,94,1,94,1,94,1,94,1,95,1,95,5,95,620,8,95,10,95,12,95,
		623,9,95,1,95,1,95,1,96,1,96,1,96,1,96,5,96,631,8,96,10,96,12,96,634,9,
		96,1,96,1,96,1,96,1,96,1,96,1,632,0,97,1,1,3,2,5,3,7,4,9,5,11,6,13,7,15,
		8,17,9,19,10,21,11,23,12,25,13,27,14,29,15,31,16,33,17,35,18,37,19,39,
		20,41,21,43,22,45,23,47,24,49,25,51,26,53,27,55,28,57,29,59,30,61,31,63,
		32,65,33,67,34,69,35,71,36,73,37,75,38,77,39,79,40,81,41,83,42,85,43,87,
		44,89,45,91,46,93,47,95,48,97,49,99,50,101,51,103,52,105,53,107,54,109,
		55,111,56,113,57,115,58,117,59,119,60,121,61,123,62,125,63,127,64,129,
		65,131,66,133,67,135,68,137,69,139,70,141,71,143,72,145,73,147,74,149,
		75,151,76,153,77,155,78,157,79,159,80,161,81,163,82,165,83,167,84,169,
		85,171,86,173,87,175,88,177,89,179,90,181,91,183,92,185,93,187,94,189,
		95,191,96,193,97,1,0,9,3,0,65,90,95,95,97,122,4,0,48,57,65,90,95,95,97,
		122,2,0,65,90,97,122,1,0,48,57,2,0,34,34,92,92,2,0,39,39,92,92,8,0,34,
		34,39,39,92,92,98,98,102,102,110,110,114,114,116,116,3,0,9,10,13,13,32,
		32,2,0,10,10,13,13,647,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,0,0,0,
		0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,0,19,1,
		0,0,0,0,21,1,0,0,0,0,23,1,0,0,0,0,25,1,0,0,0,0,27,1,0,0,0,0,29,1,0,0,0,
		0,31,1,0,0,0,0,33,1,0,0,0,0,35,1,0,0,0,0,37,1,0,0,0,0,39,1,0,0,0,0,41,
		1,0,0,0,0,43,1,0,0,0,0,45,1,0,0,0,0,47,1,0,0,0,0,49,1,0,0,0,0,51,1,0,0,
		0,0,53,1,0,0,0,0,55,1,0,0,0,0,57,1,0,0,0,0,59,1,0,0,0,0,61,1,0,0,0,0,63,
		1,0,0,0,0,65,1,0,0,0,0,67,1,0,0,0,0,69,1,0,0,0,0,71,1,0,0,0,0,73,1,0,0,
		0,0,75,1,0,0,0,0,77,1,0,0,0,0,79,1,0,0,0,0,81,1,0,0,0,0,83,1,0,0,0,0,85,
		1,0,0,0,0,87,1,0,0,0,0,89,1,0,0,0,0,91,1,0,0,0,0,93,1,0,0,0,0,95,1,0,0,
		0,0,97,1,0,0,0,0,99,1,0,0,0,0,101,1,0,0,0,0,103,1,0,0,0,0,105,1,0,0,0,
		0,107,1,0,0,0,0,109,1,0,0,0,0,111,1,0,0,0,0,113,1,0,0,0,0,115,1,0,0,0,
		0,117,1,0,0,0,0,119,1,0,0,0,0,121,1,0,0,0,0,123,1,0,0,0,0,125,1,0,0,0,
		0,127,1,0,0,0,0,129,1,0,0,0,0,131,1,0,0,0,0,133,1,0,0,0,0,135,1,0,0,0,
		0,137,1,0,0,0,0,139,1,0,0,0,0,141,1,0,0,0,0,143,1,0,0,0,0,145,1,0,0,0,
		0,147,1,0,0,0,0,149,1,0,0,0,0,151,1,0,0,0,0,153,1,0,0,0,0,155,1,0,0,0,
		0,157,1,0,0,0,0,159,1,0,0,0,0,161,1,0,0,0,0,163,1,0,0,0,0,165,1,0,0,0,
		0,167,1,0,0,0,0,169,1,0,0,0,0,171,1,0,0,0,0,173,1,0,0,0,0,175,1,0,0,0,
		0,177,1,0,0,0,0,179,1,0,0,0,0,181,1,0,0,0,0,183,1,0,0,0,0,185,1,0,0,0,
		0,187,1,0,0,0,0,189,1,0,0,0,0,191,1,0,0,0,0,193,1,0,0,0,1,195,1,0,0,0,
		3,199,1,0,0,0,5,206,1,0,0,0,7,212,1,0,0,0,9,219,1,0,0,0,11,225,1,0,0,0,
		13,229,1,0,0,0,15,234,1,0,0,0,17,241,1,0,0,0,19,249,1,0,0,0,21,255,1,0,
		0,0,23,260,1,0,0,0,25,264,1,0,0,0,27,269,1,0,0,0,29,273,1,0,0,0,31,278,
		1,0,0,0,33,285,1,0,0,0,35,291,1,0,0,0,37,295,1,0,0,0,39,300,1,0,0,0,41,
		307,1,0,0,0,43,315,1,0,0,0,45,324,1,0,0,0,47,329,1,0,0,0,49,339,1,0,0,
		0,51,348,1,0,0,0,53,356,1,0,0,0,55,365,1,0,0,0,57,373,1,0,0,0,59,377,1,
		0,0,0,61,382,1,0,0,0,63,386,1,0,0,0,65,389,1,0,0,0,67,393,1,0,0,0,69,412,
		1,0,0,0,71,414,1,0,0,0,73,417,1,0,0,0,75,422,1,0,0,0,77,427,1,0,0,0,79,
		433,1,0,0,0,81,442,1,0,0,0,83,453,1,0,0,0,85,463,1,0,0,0,87,467,1,0,0,
		0,89,470,1,0,0,0,91,473,1,0,0,0,93,476,1,0,0,0,95,478,1,0,0,0,97,480,1,
		0,0,0,99,482,1,0,0,0,101,484,1,0,0,0,103,486,1,0,0,0,105,488,1,0,0,0,107,
		490,1,0,0,0,109,492,1,0,0,0,111,494,1,0,0,0,113,496,1,0,0,0,115,498,1,
		0,0,0,117,502,1,0,0,0,119,504,1,0,0,0,121,506,1,0,0,0,123,508,1,0,0,0,
		125,510,1,0,0,0,127,512,1,0,0,0,129,514,1,0,0,0,131,516,1,0,0,0,133,519,
		1,0,0,0,135,522,1,0,0,0,137,525,1,0,0,0,139,528,1,0,0,0,141,531,1,0,0,
		0,143,534,1,0,0,0,145,537,1,0,0,0,147,540,1,0,0,0,149,543,1,0,0,0,151,
		546,1,0,0,0,153,549,1,0,0,0,155,551,1,0,0,0,157,553,1,0,0,0,159,555,1,
		0,0,0,161,557,1,0,0,0,163,559,1,0,0,0,165,561,1,0,0,0,167,563,1,0,0,0,
		169,566,1,0,0,0,171,568,1,0,0,0,173,570,1,0,0,0,175,572,1,0,0,0,177,579,
		1,0,0,0,179,581,1,0,0,0,181,588,1,0,0,0,183,598,1,0,0,0,185,605,1,0,0,
		0,187,608,1,0,0,0,189,612,1,0,0,0,191,617,1,0,0,0,193,626,1,0,0,0,195,
		196,5,103,0,0,196,197,5,117,0,0,197,198,5,109,0,0,198,2,1,0,0,0,199,200,
		5,115,0,0,200,201,5,119,0,0,201,202,5,101,0,0,202,203,5,101,0,0,203,204,
		5,116,0,0,204,205,5,115,0,0,205,4,1,0,0,0,206,207,5,115,0,0,207,208,5,
		116,0,0,208,209,5,111,0,0,209,210,5,99,0,0,210,211,5,107,0,0,211,6,1,0,
		0,0,212,213,5,114,0,0,213,214,5,101,0,0,214,215,5,99,0,0,215,216,5,105,
		0,0,216,217,5,112,0,0,217,218,5,101,0,0,218,8,1,0,0,0,219,220,5,99,0,0,
		220,221,5,97,0,0,221,222,5,110,0,0,222,223,5,100,0,0,223,224,5,121,0,0,
		224,10,1,0,0,0,225,226,5,71,0,0,226,227,5,117,0,0,227,228,5,109,0,0,228,
		12,1,0,0,0,229,230,5,67,0,0,230,231,5,104,0,0,231,232,5,101,0,0,232,233,
		5,119,0,0,233,14,1,0,0,0,234,235,5,102,0,0,235,236,5,108,0,0,236,237,5,
		97,0,0,237,238,5,118,0,0,238,239,5,111,0,0,239,240,5,114,0,0,240,16,1,
		0,0,0,241,242,5,102,0,0,242,243,5,108,0,0,243,244,5,97,0,0,244,245,5,118,
		0,0,245,246,5,111,0,0,246,247,5,114,0,0,247,248,5,115,0,0,248,18,1,0,0,
		0,249,250,5,115,0,0,250,251,5,117,0,0,251,252,5,103,0,0,252,253,5,97,0,
		0,253,254,5,114,0,0,254,20,1,0,0,0,255,256,5,99,0,0,256,257,5,97,0,0,257,
		258,5,114,0,0,258,259,5,98,0,0,259,22,1,0,0,0,260,261,5,99,0,0,261,262,
		5,97,0,0,262,263,5,108,0,0,263,24,1,0,0,0,264,265,5,107,0,0,265,266,5,
		99,0,0,266,267,5,97,0,0,267,268,5,108,0,0,268,26,1,0,0,0,269,270,5,121,
		0,0,270,271,5,117,0,0,271,272,5,109,0,0,272,28,1,0,0,0,273,274,5,98,0,
		0,274,275,5,111,0,0,275,276,5,108,0,0,276,277,5,100,0,0,277,30,1,0,0,0,
		278,279,5,115,0,0,279,280,5,117,0,0,280,281,5,98,0,0,281,282,5,116,0,0,
		282,283,5,108,0,0,283,284,5,101,0,0,284,32,1,0,0,0,285,286,5,98,0,0,286,
		287,5,108,0,0,287,288,5,97,0,0,288,289,5,110,0,0,289,290,5,100,0,0,290,
		34,1,0,0,0,291,292,5,112,0,0,292,293,5,111,0,0,293,294,5,112,0,0,294,36,
		1,0,0,0,295,296,5,112,0,0,296,297,5,117,0,0,297,298,5,114,0,0,298,299,
		5,101,0,0,299,38,1,0,0,0,300,301,5,115,0,0,301,302,5,116,0,0,302,303,5,
		105,0,0,303,304,5,99,0,0,304,305,5,107,0,0,305,306,5,121,0,0,306,40,1,
		0,0,0,307,308,5,87,0,0,308,309,5,114,0,0,309,310,5,97,0,0,310,311,5,112,
		0,0,311,312,5,112,0,0,312,313,5,101,0,0,313,314,5,114,0,0,314,42,1,0,0,
		0,315,316,5,109,0,0,316,317,5,105,0,0,317,318,5,110,0,0,318,319,5,116,
		0,0,319,320,5,112,0,0,320,321,5,97,0,0,321,322,5,99,0,0,322,323,5,107,
		0,0,323,44,1,0,0,0,324,325,5,112,0,0,325,326,5,97,0,0,326,327,5,99,0,0,
		327,328,5,107,0,0,328,46,1,0,0,0,329,330,5,115,0,0,330,331,5,117,0,0,331,
		332,5,103,0,0,332,333,5,97,0,0,333,334,5,114,0,0,334,335,5,112,0,0,335,
		336,5,97,0,0,336,337,5,99,0,0,337,338,5,107,0,0,338,48,1,0,0,0,339,340,
		5,99,0,0,340,341,5,97,0,0,341,342,5,114,0,0,342,343,5,98,0,0,343,344,5,
		112,0,0,344,345,5,97,0,0,345,346,5,99,0,0,346,347,5,107,0,0,347,50,1,0,
		0,0,348,349,5,99,0,0,349,350,5,97,0,0,350,351,5,108,0,0,351,352,5,112,
		0,0,352,353,5,97,0,0,353,354,5,99,0,0,354,355,5,107,0,0,355,52,1,0,0,0,
		356,357,5,107,0,0,357,358,5,99,0,0,358,359,5,97,0,0,359,360,5,108,0,0,
		360,361,5,112,0,0,361,362,5,97,0,0,362,363,5,99,0,0,363,364,5,107,0,0,
		364,54,1,0,0,0,365,366,5,121,0,0,366,367,5,117,0,0,367,368,5,109,0,0,368,
		369,5,112,0,0,369,370,5,97,0,0,370,371,5,99,0,0,371,372,5,107,0,0,372,
		56,1,0,0,0,373,374,5,121,0,0,374,375,5,117,0,0,375,376,5,112,0,0,376,58,
		1,0,0,0,377,378,5,110,0,0,378,379,5,111,0,0,379,380,5,112,0,0,380,381,
		5,101,0,0,381,60,1,0,0,0,382,383,5,97,0,0,383,384,5,110,0,0,384,385,5,
		100,0,0,385,62,1,0,0,0,386,387,5,111,0,0,387,388,5,114,0,0,388,64,1,0,
		0,0,389,390,5,120,0,0,390,391,5,111,0,0,391,392,5,114,0,0,392,66,1,0,0,
		0,393,394,5,120,0,0,394,395,5,110,0,0,395,396,5,111,0,0,396,397,5,114,
		0,0,397,68,1,0,0,0,398,399,5,102,0,0,399,400,5,108,0,0,400,401,5,97,0,
		0,401,402,5,118,0,0,402,403,5,111,0,0,403,404,5,114,0,0,404,405,5,108,
		0,0,405,406,5,101,0,0,406,407,5,115,0,0,407,413,5,115,0,0,408,409,5,110,
		0,0,409,410,5,102,0,0,410,411,5,108,0,0,411,413,5,118,0,0,412,398,1,0,
		0,0,412,408,1,0,0,0,413,70,1,0,0,0,414,415,5,105,0,0,415,416,5,102,0,0,
		416,72,1,0,0,0,417,418,5,101,0,0,418,419,5,108,0,0,419,420,5,115,0,0,420,
		421,5,101,0,0,421,74,1,0,0,0,422,423,5,101,0,0,423,424,5,108,0,0,424,425,
		5,105,0,0,425,426,5,102,0,0,426,76,1,0,0,0,427,428,5,119,0,0,428,429,5,
		104,0,0,429,430,5,105,0,0,430,431,5,108,0,0,431,432,5,101,0,0,432,78,1,
		0,0,0,433,434,5,114,0,0,434,435,5,101,0,0,435,436,5,112,0,0,436,437,5,
		101,0,0,437,438,5,97,0,0,438,439,5,116,0,0,439,440,5,85,0,0,440,441,5,
		112,0,0,441,80,1,0,0,0,442,443,5,114,0,0,443,444,5,101,0,0,444,445,5,112,
		0,0,445,446,5,101,0,0,446,447,5,97,0,0,447,448,5,116,0,0,448,449,5,68,
		0,0,449,450,5,111,0,0,450,451,5,119,0,0,451,452,5,110,0,0,452,82,1,0,0,
		0,453,454,5,112,0,0,454,455,5,111,0,0,455,456,5,112,0,0,456,457,5,115,
		0,0,457,458,5,116,0,0,458,459,5,114,0,0,459,460,5,101,0,0,460,461,5,97,
		0,0,461,462,5,109,0,0,462,84,1,0,0,0,463,464,5,110,0,0,464,465,5,111,0,
		0,465,466,5,116,0,0,466,86,1,0,0,0,467,468,5,105,0,0,468,469,5,110,0,0,
		469,88,1,0,0,0,470,471,5,105,0,0,471,472,5,115,0,0,472,90,1,0,0,0,473,
		474,5,58,0,0,474,475,5,58,0,0,475,92,1,0,0,0,476,477,5,40,0,0,477,94,1,
		0,0,0,478,479,5,41,0,0,479,96,1,0,0,0,480,481,5,91,0,0,481,98,1,0,0,0,
		482,483,5,93,0,0,483,100,1,0,0,0,484,485,5,123,0,0,485,102,1,0,0,0,486,
		487,5,125,0,0,487,104,1,0,0,0,488,489,5,60,0,0,489,106,1,0,0,0,490,491,
		5,62,0,0,491,108,1,0,0,0,492,493,5,44,0,0,493,110,1,0,0,0,494,495,5,59,
		0,0,495,112,1,0,0,0,496,497,5,58,0,0,497,114,1,0,0,0,498,499,5,46,0,0,
		499,500,5,46,0,0,500,501,5,46,0,0,501,116,1,0,0,0,502,503,5,46,0,0,503,
		118,1,0,0,0,504,505,5,33,0,0,505,120,1,0,0,0,506,507,5,63,0,0,507,122,
		1,0,0,0,508,509,5,34,0,0,509,124,1,0,0,0,510,511,5,39,0,0,511,126,1,0,
		0,0,512,513,5,96,0,0,513,128,1,0,0,0,514,515,5,36,0,0,515,130,1,0,0,0,
		516,517,5,62,0,0,517,518,5,61,0,0,518,132,1,0,0,0,519,520,5,60,0,0,520,
		521,5,61,0,0,521,134,1,0,0,0,522,523,5,60,0,0,523,524,5,58,0,0,524,136,
		1,0,0,0,525,526,5,58,0,0,526,527,5,62,0,0,527,138,1,0,0,0,528,529,5,60,
		0,0,529,530,5,62,0,0,530,140,1,0,0,0,531,532,5,126,0,0,532,533,5,61,0,
		0,533,142,1,0,0,0,534,535,5,43,0,0,535,536,5,58,0,0,536,144,1,0,0,0,537,
		538,5,45,0,0,538,539,5,58,0,0,539,146,1,0,0,0,540,541,5,45,0,0,541,542,
		5,62,0,0,542,148,1,0,0,0,543,544,5,61,0,0,544,545,5,62,0,0,545,150,1,0,
		0,0,546,547,5,58,0,0,547,548,5,60,0,0,548,152,1,0,0,0,549,550,5,61,0,0,
		550,154,1,0,0,0,551,552,5,38,0,0,552,156,1,0,0,0,553,554,5,124,0,0,554,
		158,1,0,0,0,555,556,5,126,0,0,556,160,1,0,0,0,557,558,5,94,0,0,558,162,
		1,0,0,0,559,560,5,43,0,0,560,164,1,0,0,0,561,562,5,45,0,0,562,166,1,0,
		0,0,563,564,5,42,0,0,564,565,5,42,0,0,565,168,1,0,0,0,566,567,5,42,0,0,
		567,170,1,0,0,0,568,569,5,47,0,0,569,172,1,0,0,0,570,571,5,37,0,0,571,
		174,1,0,0,0,572,576,7,0,0,0,573,575,7,1,0,0,574,573,1,0,0,0,575,578,1,
		0,0,0,576,574,1,0,0,0,576,577,1,0,0,0,577,176,1,0,0,0,578,576,1,0,0,0,
		579,580,7,2,0,0,580,178,1,0,0,0,581,585,7,3,0,0,582,584,7,3,0,0,583,582,
		1,0,0,0,584,587,1,0,0,0,585,583,1,0,0,0,585,586,1,0,0,0,586,180,1,0,0,
		0,587,585,1,0,0,0,588,593,3,123,61,0,589,592,3,185,92,0,590,592,8,4,0,
		0,591,589,1,0,0,0,591,590,1,0,0,0,592,595,1,0,0,0,593,591,1,0,0,0,593,
		594,1,0,0,0,594,596,1,0,0,0,595,593,1,0,0,0,596,597,3,123,61,0,597,182,
		1,0,0,0,598,601,3,125,62,0,599,602,3,185,92,0,600,602,8,5,0,0,601,599,
		1,0,0,0,601,600,1,0,0,0,602,603,1,0,0,0,603,604,3,125,62,0,604,184,1,0,
		0,0,605,606,5,92,0,0,606,607,7,6,0,0,607,186,1,0,0,0,608,609,7,7,0,0,609,
		610,1,0,0,0,610,611,6,93,0,0,611,188,1,0,0,0,612,613,5,13,0,0,613,614,
		5,10,0,0,614,615,1,0,0,0,615,616,6,94,0,0,616,190,1,0,0,0,617,621,5,35,
		0,0,618,620,8,8,0,0,619,618,1,0,0,0,620,623,1,0,0,0,621,619,1,0,0,0,621,
		622,1,0,0,0,622,624,1,0,0,0,623,621,1,0,0,0,624,625,6,95,0,0,625,192,1,
		0,0,0,626,627,5,35,0,0,627,628,5,62,0,0,628,632,1,0,0,0,629,631,9,0,0,
		0,630,629,1,0,0,0,631,634,1,0,0,0,632,633,1,0,0,0,632,630,1,0,0,0,633,
		635,1,0,0,0,634,632,1,0,0,0,635,636,5,60,0,0,636,637,5,35,0,0,637,638,
		1,0,0,0,638,639,6,96,0,0,639,194,1,0,0,0,9,0,412,576,585,591,593,601,621,
		632,1,0,1,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
