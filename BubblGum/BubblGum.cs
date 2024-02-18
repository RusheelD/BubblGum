
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
using System.Text.Json;

public class BubblGum
{

    // If debugging, print compiler messages to output file. Else print to Console.Out
    private const bool DEBUG_MODE = false;

    // Directory of the entry point file for the compiler
    private static string? directoryPrefix;

    private const string OUTPUT_FILE = "./Main/Output.txt";
    private const string ERROR_MSG = "Please specify a valid mode (-P) to parse file and a file to scan";


    /// <summary>
    ///  Handles bubble gum terminal run commands. Requires an args list with what part of the compiler to run (see options below)
    ///  and the main entry file's path.
    ///  
    ///  Run options (strings):
    ///    - A (run entire compiler)
    ///    - H (Get all files used)
    ///    - P (Parse files used to ASTs)
    ///    - T (Execute semantic analysis on ASTs)
    /// </summary>
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
            (bool success, HashSet<string> filePaths) = GetAllFilesUsed(args[1], outStream);
            if (!success)
                return;
            
            success = ParseFilesToASTs(filePaths.ToList(), outStream);
            if (!success)
                return;

            success = ExecuteSemanticAnalysis(args[1], outStream);
        } 
        else if (args[0].Equals("-H")) {
            GetAllFilesUsed(args[1], outStream);
        } 
        else if (args[0].Equals("-P")) {
            ParseFilesToASTs(new List<string>() { args[1] }, outStream);
        }
        else if (args[0].Equals("-T")) {
            ExecuteSemanticAnalysis(args[1], outStream);
        }

        else
            Console.Error.WriteLine("Specified compiler mode not yet implemented");
    }

    /// <summary>
    ///  Requires the main entry file's path, and an outstream to print compiler messages to. 
    ///  Finds all the files USED by the overall program based on main file, it's stock, 
    ///  and all imported stocks + files. Also finds files USED based on config files.
    /// </summary>
    public static (bool, HashSet<string>?) GetAllFilesUsed(string filePath, TextWriter outStream) 
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Invalid entry point for the compiler");
            return (false, null);
        }
        
        directoryPrefix = Path.GetDirectoryName(filePath);
        if (directoryPrefix == null) {
            Console.Error.WriteLine("Directory of entry file could not be established");
            return (false, null);
        }
        else if (directoryPrefix.Equals(string.Empty))
            directoryPrefix = ".";

        // update outstream
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        // add all file paths recursively in main file's directory + subdirectories
        // add all file paths in imported external directories from config files
        filePath = Path.GetRelativePath(directoryPrefix, filePath);
        List<string>filePathsFound = getFilePathsRecursively();

        // create a namespace table and populate it with all namespaces and files in each namespace or sub-namespace
        var baseNamespace = new Namespace("");
        var filePathToProgram = new Dictionary<string, Program>();
        (bool success, string errorPath) = populateNamespaceTable(baseNamespace, filePathToProgram, filePathsFound);

        if (!success) {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Parsing failed for file {errorPath} due to syntax errors.");
            return (false, null);
        }

        // generate condensed list of all files USED (optimization before code-generation, not all coded files need to be generated)
        // start with the main entry point file, and scan its import statements
        var allFilesUsed = new HashSet<string>() {};
        var filesToScan = new Queue<string>();
        filesToScan.Enqueue(filePath);

        // do import scanning. 
        var scanImports = new ScanImports(baseNamespace, filePathToProgram, allFilesUsed);
        int errorCount = 0;

        // scan all files used in the provided file path, and then repeat for those files 
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

        return (errorCount == 0, allFilesUsed);
    }

    // Takes in an outstream to print compiler messages to, and a list of file paths to parse into ASTs.
    // Returns whether parsing was successful (ie. 0 errors)
    public static bool ParseFilesToASTs(List<string> filePaths, TextWriter outStream)
    {
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        foreach (var path in filePaths)
        {
            string finalPath =  (directoryPrefix == null) ? path : Path.Combine(directoryPrefix, path);
            string inputTxt = File.ReadAllText(finalPath);

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
                Console.WriteLine($"Parsing failed in {finalPath} due to syntax errors.");
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

    // add all file paths recursively in main file's directory + subdirectories
    // add all file paths in imported external directories from config files
    private static List<string> getFilePathsRecursively() 
    {
        string requiredFileEnding = "*" + Constants.FILE_EXTENSION;
        string requiredConfigEnding = "*" + Constants.CONFIG_EXTENSION;
        var filePathsFound = new List<string>();

        var directoriesToSearch = new Queue<string>();
        directoriesToSearch.Enqueue(directoryPrefix);

        while (directoriesToSearch.Count > 0)
        {
            string currDirectory = directoriesToSearch.Dequeue();

            string[] files = Directory.GetFiles(currDirectory, requiredFileEnding);
            foreach (var file in files)
                filePathsFound.Add(Path.GetRelativePath(directoryPrefix, file));

            string[] configs = Directory.GetFiles(currDirectory, requiredConfigEnding);
            parseConfigs(configs, directoriesToSearch, currDirectory);

            string[] directories = Directory.GetDirectories(currDirectory);
            foreach (var dir in directories)
                directoriesToSearch.Enqueue(dir);
        }

        return filePathsFound;
    }

    // add all file paths recursively in main file's directory + subdirectories
    private static (bool, string) populateNamespaceTable(Namespace baseNamespace, 
        Dictionary<string, Program> filePathToProgram, List<string> filePathsFound)
    {
        
        foreach (string path in filePathsFound)
        {
            var inputTxt = File.ReadAllText(Path.Combine(directoryPrefix, path));

            #pragma warning disable
            AntlrInputStream input = new AntlrInputStream(inputTxt);
            BubblGumHeaderLexer lexer = new BubblGumHeaderLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            BubblGumHeaderParser parser = new BubblGumHeaderParser(tokens);
            BubblGumHeaderParser.ProgramContext programContext = parser.program();
            #pragma warning restore

            if (parser.NumberOfSyntaxErrors > 0)
                return (false, path);

            var createASTHeader = new CreateHeaderAST();
            Program program = createASTHeader.Visit(programContext);
            
            filePathToProgram[path] = program;

            var gatherNamespaces = new GatherNamespaces();
            gatherNamespaces.Execute(program, baseNamespace, path);
        }
        
        return (true, "");
    }

    // read all the specified config files, and add their listed directories to a collection of directories to search
    // also requires the current directory (that config files are in)
    private static void parseConfigs(string[] configs, Queue<string> directoriesToSearch, string currDirectory)
    {
        foreach (var config in configs)
        {
            string readJSONString = File.ReadAllText(config);
            try
            {
                Config? deserializedConfig = JsonSerializer.Deserialize<Config>(readJSONString);

                if (deserializedConfig != null && deserializedConfig.paths != null)
                {
                    foreach (var dir in deserializedConfig.paths)
                    {
                        var fullPath = Path.Combine(currDirectory, dir);
                        if (Directory.Exists(fullPath))
                            directoriesToSearch.Enqueue(fullPath);
                        else
                            Console.Error.WriteLine($"External directory {{{dir}}} could not be imported in {{{config}}}");
                    }
                }
                else
                    Console.Error.WriteLine($"Invalid config file at {config}");
            }
            catch
            {
                var shortenedConfig = Path.GetRelativePath(currDirectory, config);
                Console.Error.WriteLine($"Error parsing {{{shortenedConfig}}} in {{{currDirectory}}}");
            }
        }
    }

}