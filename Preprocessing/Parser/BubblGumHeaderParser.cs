//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ./Preprocessing/Parser/BubblGumHeader.g4 by ANTLR 4.13.1

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
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class BubblGumHeaderParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		STOCK=1, CHEW=2, FROM=3, IDENTIFIER=4, THIN_ARROW=5, STRING_LITERAL=6, 
		ESCAPE_SEQUENCE=7, WHITE=8, EOL=9, SINGLE_LINE_COMMENT=10, MULTI_LINE_COMMENT=11, 
		ANYCHAR=12;
	public const int
		RULE_program = 0, RULE_define_stock = 1, RULE_chew_import = 2;
	public static readonly string[] ruleNames = {
		"program", "define_stock", "chew_import"
	};

	private static readonly string[] _LiteralNames = {
		null, "'stock'", "'Chew'", "'from'", null, "'->'", null, null, null, "'\\r\\n'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "STOCK", "CHEW", "FROM", "IDENTIFIER", "THIN_ARROW", "STRING_LITERAL", 
		"ESCAPE_SEQUENCE", "WHITE", "EOL", "SINGLE_LINE_COMMENT", "MULTI_LINE_COMMENT", 
		"ANYCHAR"
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

	public override string GrammarFileName { get { return "BubblGumHeader.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static BubblGumHeaderParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public BubblGumHeaderParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public BubblGumHeaderParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class ProgramContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode Eof() { return GetToken(BubblGumHeaderParser.Eof, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public Chew_importContext[] chew_import() {
			return GetRuleContexts<Chew_importContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public Chew_importContext chew_import(int i) {
			return GetRuleContext<Chew_importContext>(i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public Define_stockContext define_stock() {
			return GetRuleContext<Define_stockContext>(0);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] ANYCHAR() { return GetTokens(BubblGumHeaderParser.ANYCHAR); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode ANYCHAR(int i) {
			return GetToken(BubblGumHeaderParser.ANYCHAR, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] IDENTIFIER() { return GetTokens(BubblGumHeaderParser.IDENTIFIER); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER(int i) {
			return GetToken(BubblGumHeaderParser.IDENTIFIER, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] STRING_LITERAL() { return GetTokens(BubblGumHeaderParser.STRING_LITERAL); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING_LITERAL(int i) {
			return GetToken(BubblGumHeaderParser.STRING_LITERAL, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] THIN_ARROW() { return GetTokens(BubblGumHeaderParser.THIN_ARROW); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode THIN_ARROW(int i) {
			return GetToken(BubblGumHeaderParser.THIN_ARROW, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] STOCK() { return GetTokens(BubblGumHeaderParser.STOCK); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STOCK(int i) {
			return GetToken(BubblGumHeaderParser.STOCK, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] CHEW() { return GetTokens(BubblGumHeaderParser.CHEW); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode CHEW(int i) {
			return GetToken(BubblGumHeaderParser.CHEW, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] ESCAPE_SEQUENCE() { return GetTokens(BubblGumHeaderParser.ESCAPE_SEQUENCE); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode ESCAPE_SEQUENCE(int i) {
			return GetToken(BubblGumHeaderParser.ESCAPE_SEQUENCE, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] FROM() { return GetTokens(BubblGumHeaderParser.FROM); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode FROM(int i) {
			return GetToken(BubblGumHeaderParser.FROM, i);
		}
		public ProgramContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_program; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IBubblGumHeaderListener typedListener = listener as IBubblGumHeaderListener;
			if (typedListener != null) typedListener.EnterProgram(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IBubblGumHeaderListener typedListener = listener as IBubblGumHeaderListener;
			if (typedListener != null) typedListener.ExitProgram(this);
		}
	}

	[RuleVersion(0)]
	public ProgramContext program() {
		ProgramContext _localctx = new ProgramContext(Context, State);
		EnterRule(_localctx, 0, RULE_program);
		int _la;
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 9;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,0,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					State = 6;
					chew_import();
					}
					} 
				}
				State = 11;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,0,Context);
			}
			State = 13;
			ErrorHandler.Sync(this);
			switch ( Interpreter.AdaptivePredict(TokenStream,1,Context) ) {
			case 1:
				{
				State = 12;
				define_stock();
				}
				break;
			}
			State = 18;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			while ((((_la) & ~0x3f) == 0 && ((1L << _la) & 4350L) != 0)) {
				{
				{
				State = 15;
				_la = TokenStream.LA(1);
				if ( !((((_la) & ~0x3f) == 0 && ((1L << _la) & 4350L) != 0)) ) {
				ErrorHandler.RecoverInline(this);
				}
				else {
					ErrorHandler.ReportMatch(this);
				    Consume();
				}
				}
				}
				State = 20;
				ErrorHandler.Sync(this);
				_la = TokenStream.LA(1);
			}
			State = 21;
			Match(Eof);
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Define_stockContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STOCK() { return GetToken(BubblGumHeaderParser.STOCK, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] IDENTIFIER() { return GetTokens(BubblGumHeaderParser.IDENTIFIER); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER(int i) {
			return GetToken(BubblGumHeaderParser.IDENTIFIER, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] THIN_ARROW() { return GetTokens(BubblGumHeaderParser.THIN_ARROW); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode THIN_ARROW(int i) {
			return GetToken(BubblGumHeaderParser.THIN_ARROW, i);
		}
		public Define_stockContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_define_stock; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IBubblGumHeaderListener typedListener = listener as IBubblGumHeaderListener;
			if (typedListener != null) typedListener.EnterDefine_stock(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IBubblGumHeaderListener typedListener = listener as IBubblGumHeaderListener;
			if (typedListener != null) typedListener.ExitDefine_stock(this);
		}
	}

	[RuleVersion(0)]
	public Define_stockContext define_stock() {
		Define_stockContext _localctx = new Define_stockContext(Context, State);
		EnterRule(_localctx, 2, RULE_define_stock);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 23;
			Match(STOCK);
			{
			State = 24;
			Match(IDENTIFIER);
			}
			State = 29;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,3,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					{
					{
					State = 25;
					Match(THIN_ARROW);
					State = 26;
					Match(IDENTIFIER);
					}
					} 
				}
				State = 31;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,3,Context);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class Chew_importContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode CHEW() { return GetToken(BubblGumHeaderParser.CHEW, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode STRING_LITERAL() { return GetToken(BubblGumHeaderParser.STRING_LITERAL, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] IDENTIFIER() { return GetTokens(BubblGumHeaderParser.IDENTIFIER); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IDENTIFIER(int i) {
			return GetToken(BubblGumHeaderParser.IDENTIFIER, i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode[] THIN_ARROW() { return GetTokens(BubblGumHeaderParser.THIN_ARROW); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode THIN_ARROW(int i) {
			return GetToken(BubblGumHeaderParser.THIN_ARROW, i);
		}
		public Chew_importContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_chew_import; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override void EnterRule(IParseTreeListener listener) {
			IBubblGumHeaderListener typedListener = listener as IBubblGumHeaderListener;
			if (typedListener != null) typedListener.EnterChew_import(this);
		}
		[System.Diagnostics.DebuggerNonUserCode]
		public override void ExitRule(IParseTreeListener listener) {
			IBubblGumHeaderListener typedListener = listener as IBubblGumHeaderListener;
			if (typedListener != null) typedListener.ExitChew_import(this);
		}
	}

	[RuleVersion(0)]
	public Chew_importContext chew_import() {
		Chew_importContext _localctx = new Chew_importContext(Context, State);
		EnterRule(_localctx, 4, RULE_chew_import);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 32;
			Match(CHEW);
			State = 42;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case IDENTIFIER:
				{
				{
				State = 33;
				Match(IDENTIFIER);
				State = 38;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,4,Context);
				while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
					if ( _alt==1 ) {
						{
						{
						State = 34;
						Match(THIN_ARROW);
						State = 35;
						Match(IDENTIFIER);
						}
						} 
					}
					State = 40;
					ErrorHandler.Sync(this);
					_alt = Interpreter.AdaptivePredict(TokenStream,4,Context);
				}
				}
				}
				break;
			case STRING_LITERAL:
				{
				State = 41;
				Match(STRING_LITERAL);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	private static int[] _serializedATN = {
		4,1,12,45,2,0,7,0,2,1,7,1,2,2,7,2,1,0,5,0,8,8,0,10,0,12,0,11,9,0,1,0,3,
		0,14,8,0,1,0,5,0,17,8,0,10,0,12,0,20,9,0,1,0,1,0,1,1,1,1,1,1,1,1,5,1,28,
		8,1,10,1,12,1,31,9,1,1,2,1,2,1,2,1,2,5,2,37,8,2,10,2,12,2,40,9,2,1,2,3,
		2,43,8,2,1,2,0,0,3,0,2,4,0,1,2,0,1,7,12,12,47,0,9,1,0,0,0,2,23,1,0,0,0,
		4,32,1,0,0,0,6,8,3,4,2,0,7,6,1,0,0,0,8,11,1,0,0,0,9,7,1,0,0,0,9,10,1,0,
		0,0,10,13,1,0,0,0,11,9,1,0,0,0,12,14,3,2,1,0,13,12,1,0,0,0,13,14,1,0,0,
		0,14,18,1,0,0,0,15,17,7,0,0,0,16,15,1,0,0,0,17,20,1,0,0,0,18,16,1,0,0,
		0,18,19,1,0,0,0,19,21,1,0,0,0,20,18,1,0,0,0,21,22,5,0,0,1,22,1,1,0,0,0,
		23,24,5,1,0,0,24,29,5,4,0,0,25,26,5,5,0,0,26,28,5,4,0,0,27,25,1,0,0,0,
		28,31,1,0,0,0,29,27,1,0,0,0,29,30,1,0,0,0,30,3,1,0,0,0,31,29,1,0,0,0,32,
		42,5,2,0,0,33,38,5,4,0,0,34,35,5,5,0,0,35,37,5,4,0,0,36,34,1,0,0,0,37,
		40,1,0,0,0,38,36,1,0,0,0,38,39,1,0,0,0,39,43,1,0,0,0,40,38,1,0,0,0,41,
		43,5,6,0,0,42,33,1,0,0,0,42,41,1,0,0,0,43,5,1,0,0,0,6,9,13,18,29,38,42
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
