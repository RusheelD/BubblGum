using System.Text;
using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using System.Xml.Serialization;

public class Run
{
    private const string VERSION = "1.0.0";

    public static void Main(string[] args)
    {
        // No arguments - show help
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        // Check for help/version flags
        if (args[0] == "--help" || args[0] == "-h")
        {
            ShowHelp();
            return;
        }

        if (args[0] == "--version" || args[0] == "-v")
        {
            ShowVersion();
            return;
        }

        // Legacy mode for backwards compatibility
        if (args[0] == "run" || args[0] == "test")
        {
            var newArgs = new string[args.Length - 1];
            for (int i = 1; i < args.Length; i++)
                newArgs[i - 1] = args[i];

            if (args[0] == "run")
                BubblGum.Execute(newArgs);
            else if (args[0] == "test")
                Console.WriteLine("Test mode not available in production build");

            return;
        }

        // Modern CLI: bbgm compile file.bbgm
        if (args[0] == "compile" || args[0] == "build")
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Error: No input file specified");
                Console.Error.WriteLine("Usage: bbgm compile <file.bbgm>");
                Environment.Exit(1);
            }

            CompileFile(args[1]);
            return;
        }

        // Direct compilation: bbgm file.bbgm
        if (args[0].EndsWith(".bbgm"))
        {
            CompileFile(args[0]);
            return;
        }

        // Unknown command
        Console.Error.WriteLine($"Unknown command: {args[0]}");
        Console.Error.WriteLine("Run 'bbgm --help' for usage information");
        Environment.Exit(1);
    }

    private static void CompileFile(string filePath)
    {
        Console.WriteLine($"🍬 BubblGum Compiler v{VERSION}");
        Console.WriteLine($"📄 Compiling: {filePath}\\n");

        var args = new[] { "-C", filePath };
        BubblGum.Execute(args);
    }

    private static void ShowVersion()
    {
        Console.WriteLine($"BubblGum Compiler v{VERSION}");
        Console.WriteLine("A statically-typed compiled language with LLVM backend");
    }

    private static void ShowHelp()
    {
        Console.WriteLine(@"
🍬 BubblGum Compiler v" + VERSION + @"

USAGE:
    bbgm <file.bbgm>              Compile a BubblGum source file
    bbgm compile <file.bbgm>      Compile a BubblGum source file
    bbgm build <file.bbgm>        Same as compile

OPTIONS:
    -h, --help                    Show this help message
    -v, --version                 Show version information

ADVANCED (Developer Mode):
    bbgm run -C <file>            Compile to LLVM IR and executable
    bbgm run -P <file>            Parse file to AST only
    bbgm run -T <file>            Run semantic analysis only
    bbgm run -H <file>            Show all files used by program

EXAMPLES:
    bbgm hello.bbgm               Compile hello.bbgm to executable
    bbgm compile program.bbgm     Compile program.bbgm
    bbgm --help                   Show this help

OUTPUT:
    - file.ll                     LLVM intermediate representation
    - file.exe                    Native executable (if LLVM tools available)

REQUIREMENTS:
    - .NET 8.0 Runtime (if not using standalone executable)
    - LLVM Tools (optional, for executable generation)
        Windows: choco install llvm
        macOS:   brew install llvm
        Linux:   sudo apt-get install llvm

For more information, see: https://github.com/RusheelD/BubblGum
");
    }
}