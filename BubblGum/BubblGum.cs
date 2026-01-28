
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
using CodeGeneration.Visitors;

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
    ///    - C (Compile to LLVM IR and generate executable)
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

        if (args[0].Equals("-A"))
        {
            (bool success, HashSet<string> filePaths) = GetAllFilesUsed(args[1], outStream);
            if (!success)
                return;

            success = ParseFilesToASTs(filePaths.ToList(), outStream);
            if (!success)
                return;

            if (!success)
                return;

            success = CompileToLLVM(args[1], outStream);
        }
        else if (args[0].Equals("-H"))
        {
            GetAllFilesUsed(args[1], outStream);
        }
        else if (args[0].Equals("-P"))
        {
            ParseFilesToASTs(new List<string>() { args[1] }, outStream);
        }
        else if (args[0].Equals("-T"))
        {
            ExecuteSemanticAnalysis(args[1], outStream);
        }
        else if (args[0].Equals("-C"))
        {
            CompileToLLVM(args[1], outStream);
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
        if (directoryPrefix == null)
        {
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
        List<string> filePathsFound = getFilePathsRecursively();

        // create a namespace table and populate it with all namespaces and files in each namespace or sub-namespace
        var baseNamespace = new Namespace("");
        var filePathToProgram = new Dictionary<string, Program>();
        (bool success, string errorPath) = populateNamespaceTable(baseNamespace, filePathToProgram, filePathsFound);

        if (!success)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Parsing failed for file {errorPath} due to syntax errors.");
            return (false, null);
        }

        // generate condensed list of all files USED (optimization before code-generation, not all coded files need to be generated)
        // start with the main entry point file, and scan its import statements
        var allFilesUsed = new HashSet<string>() { };
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
            string finalPath = (directoryPrefix == null) ? path : Path.Combine(directoryPrefix, path);
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

    /// <summary>
    ///  Executes semantic analysis on all files used by the program.
    ///  Requires the main entry file's path and an outstream to print compiler messages to.
    ///  Performs Global Symbol Table creation (2 passes) and type checking.
    /// </summary>
    public static bool ExecuteSemanticAnalysis(string filePath, TextWriter outStream)
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Invalid entry point for the compiler");
            return false;
        }

        directoryPrefix = Path.GetDirectoryName(filePath);
        if (directoryPrefix == null)
        {
            Console.Error.WriteLine("Directory of entry file could not be established");
            return false;
        }
        else if (directoryPrefix.Equals(string.Empty))
            directoryPrefix = ".";

        // Get all files used by the program
        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        filePath = Path.GetRelativePath(directoryPrefix, filePath);
        List<string> filePathsFound = getFilePathsRecursively();

        // Parse all files to ASTs
        var baseNamespace = new Namespace("");
        var filePathToProgram = new Dictionary<string, Program>();
        (bool success, string errorPath) = populateNamespaceTable(baseNamespace, filePathToProgram, filePathsFound);

        if (!success)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Parsing failed for file {errorPath} due to syntax errors.");
            return false;
        }

        // Get all files actually used (optimization)
        var allFilesUsed = new HashSet<string>() { };
        var filesToScan = new Queue<string>();
        filesToScan.Enqueue(filePath);

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

        if (errorCount > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Import validation failed with {errorCount} error(s).");
            return false;
        }

        // Create Global Symbol Table (GST) - Pass 1
        var gst = new GlobalSymbolTable();
        foreach (var path in allFilesUsed)
        {
            if (!filePathToProgram.ContainsKey(path))
                continue;

            var program = filePathToProgram[path];
            var gstPass1 = new CreateGSTPass1();
            gstPass1.Execute(path, program, gst);
        }

        if (Errors.numErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"GST Pass 1 failed with {Errors.numErrors} error(s).");
            return false;
        }

        // Create Global Symbol Table (GST) - Pass 2
        foreach (var path in allFilesUsed)
        {
            if (!filePathToProgram.ContainsKey(path))
                continue;

            var program = filePathToProgram[path];
            var gstPass2 = new CreateGSTPass2();
            gstPass2.Execute(path, program, gst, directoryPrefix);
        }

        if (Errors.numErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"GST Pass 2 failed with {Errors.numErrors} error(s).");
            return false;
        }

        // Type Checking Pass
        foreach (var path in allFilesUsed)
        {
            if (!filePathToProgram.ContainsKey(path))
                continue;

            var program = filePathToProgram[path];
            var typeChecker = new Ast.TypeCheckingPass();
            typeChecker.Execute(path, program, gst);
        }

        if (Errors.numErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Type checking failed with {Errors.numErrors} error(s).");
            return false;
        }

        Console.Out.Close();
        Console.SetOut(originalOutStream);
        Console.WriteLine("Semantic analysis completed successfully.");
        return true;
    }

    /// <summary>
    ///  Compiles the BubblGum program to LLVM IR and generates an executable.
    ///  Requires the main entry file's path and an outstream to print compiler messages to.
    ///  Assumes semantic analysis has already been performed successfully.
    /// </summary>
    public static bool CompileToLLVM(string filePath, TextWriter outStream)
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"Invalid entry point for the compiler");
            return false;
        }

        directoryPrefix = Path.GetDirectoryName(filePath);
        if (directoryPrefix == null)
        {
            Console.Error.WriteLine("Directory of entry file could not be established");
            return false;
        }
        else if (directoryPrefix.Equals(string.Empty))
            directoryPrefix = ".";

        var originalOutStream = Console.Out;
        Console.SetOut(outStream);

        // First run semantic analysis to get ASTs and GST
        filePath = Path.GetRelativePath(directoryPrefix, filePath);
        List<string> filePathsFound = getFilePathsRecursively();

        var baseNamespace = new Namespace("");
        var filePathToProgram = new Dictionary<string, Program>();
        (bool success, string errorPath) = populateNamespaceTable(baseNamespace, filePathToProgram, filePathsFound);

        if (!success)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Parsing failed for file {errorPath} due to syntax errors.");
            return false;
        }

        // Get all files used
        var allFilesUsed = new HashSet<string>() { };
        var filesToScan = new Queue<string>();
        filesToScan.Enqueue(filePath);

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

        if (errorCount > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Import validation failed with {errorCount} error(s).");
            return false;
        }

        // Create GST
        var gst = new GlobalSymbolTable();
        foreach (var path in allFilesUsed)
        {
            if (!filePathToProgram.ContainsKey(path))
                continue;

            var program = filePathToProgram[path];
            var gstPass1 = new CreateGSTPass1();
            gstPass1.Execute(path, program, gst);
        }

        if (Errors.numErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"GST Pass 1 failed with {Errors.numErrors} error(s).");
            return false;
        }

        foreach (var path in allFilesUsed)
        {
            if (!filePathToProgram.ContainsKey(path))
                continue;

            var program = filePathToProgram[path];
            var gstPass2 = new CreateGSTPass2();
            gstPass2.Execute(path, program, gst, directoryPrefix);
        }

        if (Errors.numErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"GST Pass 2 failed with {Errors.numErrors} error(s).");
            return false;
        }

        // Type check
        foreach (var path in allFilesUsed)
        {
            if (!filePathToProgram.ContainsKey(path))
                continue;

            var program = filePathToProgram[path];
            var typeChecker = new Ast.TypeCheckingPass();
            typeChecker.Execute(path, program, gst);
        }

        if (Errors.numErrors > 0)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"Type checking failed with {Errors.numErrors} error(s).");
            return false;
        }

        Console.WriteLine("Semantic analysis completed. Starting code generation...");

        try
        {
            // Generate LLVM IR
            var codeGenerator = new LLVMCodeGenerator();

            // Combine all ASTs into a single program for code generation
            var combinedProgram = new Program(new List<AstNode>(), 0, 0);
            foreach (var path in allFilesUsed)
            {
                if (!filePathToProgram.ContainsKey(path))
                    continue;

                var program = filePathToProgram[path];
                combinedProgram.Pieces.AddRange(program.Pieces);
            }

            // Generate code for combined program
            // Use the first file path as the identifier
            var mainPath = allFilesUsed.FirstOrDefault() ?? filePath;
            codeGenerator.Execute(mainPath, combinedProgram, gst);

            // Emit to file
            var outputPath = Path.ChangeExtension(filePath, ".ll");
            codeGenerator.EmitToFile(outputPath);

            Console.WriteLine($"LLVM IR generated successfully: {outputPath}");

            // Try to compile to executable
            var exePath = CompileIRToExecutable(outputPath, outStream);

            Console.Out.Close();
            Console.SetOut(originalOutStream);

            if (exePath != null)
            {
                Console.WriteLine($"\n✅ Compilation successful!");
                Console.WriteLine($"   Executable: {exePath}");
                Console.WriteLine($"   LLVM IR: {outputPath}");
            }
            else
            {
                Console.WriteLine($"\n⚠️  LLVM IR generated: {outputPath}");
                Console.WriteLine("   Could not generate executable (LLVM tools not found)");
                Console.WriteLine("   Install LLVM to compile to native executables.");
            }

            return true;
        }
        catch (DllNotFoundException ex)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine("\n========================================");
            Console.Error.WriteLine("ERROR: LLVM native libraries not found!");
            Console.Error.WriteLine("========================================");
            Console.Error.WriteLine($"\nDetails: {ex.Message}");
            Console.Error.WriteLine("\nTo use the LLVM backend, you must install LLVM:");
            Console.Error.WriteLine("  Windows: Download from https://github.com/llvm/llvm-project/releases");
            Console.Error.WriteLine("           Or use: choco install llvm");
            Console.Error.WriteLine("  macOS:   brew install llvm@20");
            Console.Error.WriteLine("  Linux:   sudo apt-get install llvm-20");
            Console.Error.WriteLine("\nAfter installation, add LLVM to your PATH.");
            Console.Error.WriteLine("See LLVM_SETUP.md for detailed instructions.");
            return false;
        }
        catch (Exception ex)
        {
            Console.SetOut(originalOutStream);
            Console.Error.WriteLine($"\nCode generation failed: {ex.Message}");
            Console.Error.WriteLine($"Stack trace: {ex.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Compiles LLVM IR file to native executable using llc and clang
    /// </summary>
    private static string? CompileIRToExecutable(string llPath, TextWriter outStream)
    {
        try
        {
            var baseName = Path.GetFileNameWithoutExtension(llPath);
            var directory = Path.GetDirectoryName(llPath) ?? ".";
            var objPath = Path.Combine(directory, baseName + ".o");
            var exePath = Path.Combine(directory, baseName + ".exe");

            // Step 1: Compile .ll to .o using llc
            var llcProcess = new ProcessStartInfo
            {
                FileName = "llc",
                Arguments = $"\"{llPath}\" -filetype=obj -o \"{objPath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(llcProcess))
            {
                if (process == null) return null;
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    Console.Error.WriteLine($"llc error: {error}");
                    return null;
                }
            }

            // Step 2: Link .o to .exe using clang
            var clangProcess = new ProcessStartInfo
            {
                FileName = "clang",
                Arguments = $"\"{objPath}\" -o \"{exePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(clangProcess))
            {
                if (process == null) return null;
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    var error = process.StandardError.ReadToEnd();
                    Console.Error.WriteLine($"clang error: {error}");
                    return null;
                }
            }

            // Clean up intermediate .o file
            if (File.Exists(objPath))
                File.Delete(objPath);

            return exePath;
        }
        catch (Exception ex)
        {
            // LLVM tools not found - that's okay, just return null
            return null;
        }
    }

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