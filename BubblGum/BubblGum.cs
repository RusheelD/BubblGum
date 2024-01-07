
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
using Microsoft.VisualBasic;

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
            (bool success, HashSet<string> filePaths) = ExecuteHeaderParser(args[1], outStream);
            if (!success)
                return;
            
            success = ExecuteParser(filePaths.ToList(), outStream);
            if (!success)
                return;

            success = ExecuteSemanticAnalysis(args[1], outStream);
        } 
        else if (args[0].Equals("-H")) {
            ExecuteHeaderParser(args[1], outStream);
        } 
        else if (args[0].Equals("-P")) {
            ExecuteParser(new List<string>() { args[1] }, outStream);
        }
        else if (args[0].Equals("-T")) {
            ExecuteSemanticAnalysis(args[1], outStream);
        }

        else
            Console.Error.WriteLine("Specified compiler mode not yet implemented");
    }

    public static (bool, HashSet<string>?) ExecuteHeaderParser(string filePath, TextWriter outStream) {

        // directory_prefix (get directory name of filepath typed in compiler when running)
        // import path relative to the entry point directory
        // store path with dot dots if it goes outside

        // file's path relative to the entry point -> GetRelativePath(directory_prefix,path)
        // import statement (very specific path import relative to file)  ex. ./Other/Joe.bbgm -> n.Path

        // import physics
        // import physicsExt

        // import a and b

        // import physics from "location" // in case namespace is imported outside
        // import file // import file (inside or outside)

        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Invalid entry point for the compiler");
            return (false, null);
        }
        
        string? directoryPrefix = Path.GetDirectoryName(filePath);
        if (directoryPrefix == null) {
            Console.Error.WriteLine("Directory of entry file could not be established");
            return (false, null);
        }
        else if (directoryPrefix.Equals(string.Empty))
            directoryPrefix = ".";

        filePath = Path.GetRelativePath(directoryPrefix, filePath);

        // update outstream
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        // add all file paths recursively in main file's directory + subdirectories
        string requiredFileEnding = "*" + Constants.FILE_EXTENSION;
        var filePathsFound = new List<string>();

        var directoriesToSearch = new Queue<string>();
        directoriesToSearch.Enqueue(directoryPrefix);

        while (directoriesToSearch.Count > 0)
        {
            string currDirectory = directoriesToSearch.Dequeue();
            string[] files = Directory.GetFiles(currDirectory, requiredFileEnding);
            filePathsFound.AddRange(files);

            string[] directories = Directory.GetDirectories(currDirectory);
            foreach (var dir in directories)
                directoriesToSearch.Enqueue(dir);
        }

        // store namespace info for all files in a tree
        var baseNamespace = new Namespace("");

        // map a file path to a program containing it's namespace (if applicable) and it's imports
        var filePathToProgram = new Dictionary<string, Program>();
        foreach (string path in filePathsFound)
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
                Console.Error.WriteLine($"Parsing failed for file {path} due to syntax errors.");
                return (false, null);
            }

            var createASTHeader = new CreateHeaderAST();
            Program program = createASTHeader.Visit(programContext);
            
            var shortenedPath = Path.GetRelativePath(directoryPrefix, path);
            filePathToProgram[shortenedPath] = program;

            var gatherNamespaces = new GatherNamespaces();
            gatherNamespaces.Execute(program, baseNamespace, shortenedPath);
        }

        // generate list of all files used
        // start with the main entry point file, and scan its import statements with BFS
        var allFilesUsed = new HashSet<string>() {};
        var filesToScan = new Queue<string>();
        filesToScan.Enqueue(filePath);

        // setup import scanning
        var scanImports = new ScanImports(baseNamespace, filePathToProgram, allFilesUsed);

        int errorCount = 0;
        while (filesToScan.Count > 0)
        {
            var path = filesToScan.Dequeue();
            allFilesUsed.Add(path);

            (bool noErrors, var newFilesToImport) = scanImports.Execute(path, directoryPrefix);

            if (!noErrors)
                errorCount++;

            foreach (var newFile in newFilesToImport)
                filesToScan.Enqueue(newFile);
        }

        Console.Out.Close();
        Console.SetOut(originalOutStream);

        bool success = errorCount == 0;
        return (success, allFilesUsed);
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