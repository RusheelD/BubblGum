
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
            (bool success, List<string> filePaths) = ExecuteHeaderParser(args[1], outStream);
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

    
    public static (bool, List<string>) ExecuteHeaderParser(string filePath, TextWriter outStream) {

        if (!File.Exists(filePath))
            return (false, null);

        // update outstream
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        // add all file paths recursively in main file's directory + subdirectories
        var allFilePaths = new List<string>();
        string? mainDirectory = Path.GetDirectoryName("./" + filePath);
        if (mainDirectory != null && !mainDirectory.Equals(string.Empty))
        {
            var directoriesToSearch = new Queue<string>();
            directoriesToSearch.Enqueue(mainDirectory);

            while (directoriesToSearch.Count > 0)
            {
                string currDirectory = directoriesToSearch.Dequeue();
                string[] files = Directory.GetFiles(currDirectory);
                allFilePaths.AddRange(files);

                string[] directories = Directory.GetDirectories(currDirectory);
                foreach (var dir in directories)
                    directoriesToSearch.Enqueue(dir);
            }
        }
        else
            allFilePaths.Add(filePath);

        // gather namespace info for all files
        var baseNamespace = new Namespace("");
        var filePathToProgram = new Dictionary<string, Program>();
        foreach (string path in allFilePaths)
        {
            var inputTxt = File.ReadAllText(path);

            #pragma warning disable
            AntlrInputStream input = new AntlrInputStream(inputTxt);
            BubblGumHeaderLexer lexer = new BubblGumHeaderLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            BubblGumHeaderParser parser = new BubblGumHeaderParser(tokens);
            BubblGumHeaderParser.ProgramContext programContext = parser.program();
            #pragma warning restore

            if (parser.NumberOfSyntaxErrors > 0)
            {
                Console.SetOut(originalOutStream);
                Console.WriteLine($"Parsing failed for file {path} due to syntax errors.");
                return (false, null);
            }

            var createASTHeader = new CreateHeaderAST();
            Program program = createASTHeader.Visit(programContext);
            filePathToProgram[path] = program;

            var gatherNamespaces = new GatherNamespaces();
            gatherNamespaces.Execute(program, baseNamespace, path);
        }

        // generate list of all files used
        var filePathsToScan = new Queue<string>();
        filePathsToScan.Enqueue(filePath);
        var filePathsUsed = new List<string>();

        var scanHeader = new ScanHeader();

        while (filePathsToScan.Count > 0)
        {
            var path = filePathsToScan.Dequeue();
            var currProgram = filePathToProgram[path];
            var newPathsUsed = scanHeader.Execute(currProgram, baseNamespace, filePathToProgram);
            filePathsUsed.AddRange(newPathsUsed);
        }

        Console.Out.Close();
        Console.SetOut(originalOutStream);
        return (true, filePathsUsed);
    }

    // Takes in an outstream to print compiler messages to, and a file to parse
    // Returns whether parsing was successful (ie. 0 errors)
    public static bool ExecuteParser(List<string> filePaths, TextWriter outStream)
    {
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        foreach (var path in filePaths)
        {
            var inputTxt = File.ReadAllText(path);

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
        }

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