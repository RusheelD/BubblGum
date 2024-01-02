
using System.Text;
using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Xml.Serialization;
using AST;
using System.Diagnostics;
using System.Xml.Linq;

public class BubblGum
{

    // print compiler messages to output file if debugging, Console.Out if not debugging
    private const bool DEBUG_MODE = false;

    private const string OUTPUT_FILE = "./Main/Output.txt";
    private const string ERROR_MSG = "Please specify a valid mode (-P) to parse file and a file to scan";

    public static void Execute(string[] args)
    {
        if (args.Length != 2)
        {
            Console.Error.WriteLine(ERROR_MSG);
            return;
        }

        if (!File.Exists(args[1]))
        {
            Console.Error.WriteLine($"File at path {args[1]} could not be found");
            return;
        }

        TextWriter outStream = (!DEBUG_MODE) ? Console.Out : new StreamWriter(OUTPUT_FILE, false);

        if (args[0].Equals("-A")) {
            bool success = ExecuteHeaderParser(args[1], outStream);
            if (!success)
                return;
            
            success = ExecuteParser(args[1], outStream);
            if (!success)
                return;

            success = ExecuteSemanticAnalysis(args[1], outStream);
        } 
        else if (args[0].Equals("-H")) {
            ExecuteHeaderParser(args[1], outStream);
        } 
        else if (args[0].Equals("-P")) {
            ExecuteParser(args[1], outStream);
        }
        else if (args[0].Equals("-T")) {
            ExecuteSemanticAnalysis(args[1], outStream);
        }

        else
            Console.Error.WriteLine("Specified compiler mode not yet implemented");
    }

    
    public static bool ExecuteHeaderParser(string filePath, TextWriter outStream) {
            /*
               Plan:
                  
                create queue of files and add main entry point file to queue
                create list of processed files = empty

                while (queue is not empty) {
                  file f = dequeue
                  add files it is using to queue
                  mark f as processed
                }

                Console.error.writeline

                //
                stock a->asdf->asdf    -> adsf     Gum bob {}


                string namespace -> List<string> filePaths
                recipe add files:
                   look for word stock
                   if present, take the word/phrase after stock
            
            */ 
        var inputTxt = File.ReadAllText(filePath);
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        #pragma warning disable
        AntlrInputStream input = new AntlrInputStream(inputTxt);
        BubblGumLexer lexer = new BubblGumLexer(input);
        CommonTokenStream tokens = new CommonTokenStream(lexer);
        BubblGumHeaderParser parser = new BubblGumHeaderParser(tokens);
        BubblGumHeaderParser.ProgramContext programContext = parser.program();
        #pragma warning restore

        if (parser.NumberOfSyntaxErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.WriteLine("Parsing failed in header due to syntax errors.");
            return false;
        }

        var createAST = new CreateHeaderAST();
        Program program = createAST.Visit(programContext);

        var printAST = new PrintAST();
        printAST.Visit(program);

        Console.Out.Close();
        Console.SetOut(originalOutStream);
        return true;
    }

    // Takes in an outstream to print compiler messages to, and a file to parse
    // Returns whether parsing was successful (ie. 0 errors)
    public static bool ExecuteParser(string filePath, TextWriter outStream)
    {
        var inputTxt = File.ReadAllText(filePath);
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        #pragma warning disable
        AntlrInputStream input = new AntlrInputStream(inputTxt);
        BubblGumLexer lexer = new BubblGumLexer(input);
        CommonTokenStream tokens = new CommonTokenStream(lexer);
        BubblGumParser parser = new BubblGumParser(tokens);
        BubblGumParser.ProgramContext programContext = parser.program();
        #pragma warning restore

        if (parser.NumberOfSyntaxErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.WriteLine("Parsing failed due to syntax errors.");
            return false;
        }

        var createAST = new CreateAST();
        Program program = createAST.Visit(programContext);

        var printAST = new PrintAST();
        printAST.Visit(program);

        Console.Out.Close();

        Console.SetOut(originalOutStream);
        return true;
    }
    public static bool ExecuteSemanticAnalysis(string filePath, TextWriter outStream) {return true;}

    private static void printProgramPieces(Program program)
    {
        var nodes = new Stack<Statement>();
        for (int i = program.Pieces.Count - 1; i >= 0; i--)
            nodes.Push((Statement)program.Pieces[i]);

        while (nodes.Count > 0)
        {
            var node = nodes.Pop();
            Console.WriteLine($"{node.GetType()}");
        }
    }

    private static void dfs(BubblGumParser.ProgramContext rootNode)
    {
        var branches = new Stack<IParseTree>();
        for (int i = rootNode.children.Count - 1; i >= 0; i--)
            branches.Push(rootNode.children[i]);

        int lineNum = 1;
        while (branches.Count > 0)
        {
            var tree = branches.Pop();
            for (int i = tree.ChildCount - 1; i >= 0; i--)
                branches.Push(tree.GetChild(i));

            if (tree.ChildCount == 0)
            {
                var token = (IToken)tree.Payload;

                // EOF
                if (token.Type == -1)
                    break;

                if (token.Line != lineNum)
                {
                    lineNum = token.Line;
                    Console.Out.Write("\n" + token.Text + " ");
                    Console.Out.Write(token.Line + " " + token.Column + " " +
                        BubblGumLexer.ruleNames[token.Type-1] + "\n");
                }
                else
                {
                    Console.Write(token.Text + " ");
                    Console.Out.Write(token.Line + " " + token.Column + " " + 
                        BubblGumLexer.ruleNames[token.Type-1] + "\n");
                }
            }
        }
    }

    private static void bfs(BubblGumParser.ProgramContext rootNode)
    {
        var treesToExplore = new Queue<IParseTree>();
        for (int i = 0; i < rootNode.children.Count; i++)
            treesToExplore.Enqueue(rootNode.children[i]);

        while (treesToExplore.Count > 0)
        {
            var tree = treesToExplore.Dequeue();
            for (int i = 0; i < tree.ChildCount; i++)
                treesToExplore.Enqueue(tree.GetChild(i));

            if (tree.ChildCount == 0)
                Console.Out.Write(tree.GetText() + " ");
        }
    }
}