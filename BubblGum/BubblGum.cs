
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

public class BubblGum
{

    // print compiler messages to output file if debugging, Console.Out if not debugging
    private const bool DEBUG_MODE = false;

    private const string OUTPUT_FILE = "Output.txt";
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

        if (args[0].Equals("-P"))
            ExecuteParser(args[1], outStream);
        else
            Console.Error.WriteLine("Specified compiler mode not yet implemented");
    }

    // Takes in an outstream to print compiler messages to, and a file to parse
    // Returns whether parsing was successful (ie. 0 errors)
    public static bool ExecuteParser(string filePath, TextWriter outStream)
    {
        var inputTxt = File.ReadAllText(filePath);
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        AntlrInputStream input = new AntlrInputStream(inputTxt);
        BubblGumLexer lexer = new BubblGumLexer(input);
        CommonTokenStream tokens = new CommonTokenStream(lexer);
        BubblGumParser parser = new BubblGumParser(tokens);

        if (parser.NumberOfSyntaxErrors > 0)
        {
            Console.Out.WriteLine("Parsing failed due to syntax errors.");
            Console.SetOut(originalOutStream);
            return false;
        }

        BubblGumParser.ProgramContext rootNode = parser.program();
        // DFS(rootNode);

        var createAST = new CreateAST();
        Program program = createAST.Visit(rootNode);
        PrintProgramPieces(program);

        // test setup
        var ints = new Integer[100];
        var doubles = new AST.Double[100];
        for (int i = 0; i < 100; i++)
        {
            ints[i] = new Integer(i, i, i);
            doubles[i] = new AST.Double(i*0.5f, i, i);
        }

        Plus startExp = new Plus(createExp(20, ints, doubles), createExp(20, ints, doubles), 12, 13);
        var printAST = new PrintAST();

        Stopwatch timer = new Stopwatch();
        timer.Start();
        Console.WriteLine($"{timer.ElapsedMilliseconds}");
        printAST.Visit(startExp);
        Console.WriteLine($"{timer.ElapsedMilliseconds}");
        timer.Stop();

        Console.SetOut(originalOutStream);
        return true;
    }

    private static Exp createExp(int i, Integer[] ints, AST.Double[] doubles)
    {
        Random r = new Random();
        if (i == 0)
            return r.Next(2) == 0 ? ints[r.Next(100)] : doubles[r.Next(100)];

        int j = r.Next(4);

        if (j == 0)
        {
            return new Plus(createExp(i - 1, ints, doubles), createExp(i - 1, ints, doubles), 
                r.Next(100), r.Next(100));
        }
        else if (j == 1)
        {
            return new Minus(createExp(i - 1, ints, doubles), createExp(i - 1, ints, doubles),
                r.Next(100), r.Next(100));
        }
        else if (j == 2)
        {
            return new Multiply(createExp(i - 1, ints, doubles), createExp(i - 1, ints, doubles),
                r.Next(100), r.Next(100));
        }
        else
        {
            return new Divide(createExp(i - 1, ints, doubles), createExp(i - 1, ints, doubles),
                r.Next(100), r.Next(100));
        }
    }

    private static void PrintProgramPieces(Program program)
    {
        var nodes = new Stack<AstNode>();
        for (int i = program.Pieces.Count - 1; i >= 0; i--)
            nodes.Push((AstNode)program.Pieces[i]);

        while (nodes.Count > 0)
        {
            var node = nodes.Pop();
            Console.WriteLine($"{node.GetType()}");
        }
    }

    private static void DFS(BubblGumParser.ProgramContext rootNode)
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

    private static void BFS(BubblGumParser.ProgramContext rootNode)
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