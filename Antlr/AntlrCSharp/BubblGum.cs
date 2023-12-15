
using System.Text;
using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;

public class BubblGum
{
    private const bool DEBUG_MODE = false;
    private const string OUTPUT_FILE = "Output.txt";

    public void Execute(CompilerMode mode, String filePath)
    { 
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File at path {Path.GetFullPath(filePath)} could not be found");
            return;
        }

        // print compiler messages to output file if debugging, Console.Out if not debugging
        TextWriter outStream = Console.Out;
        if (DEBUG_MODE)
           outStream = new StreamWriter(OUTPUT_FILE, false);

        if (mode == CompilerMode.Parser)
            executeParser(outStream, filePath);
        else
            Console.Error.WriteLine("Specified compiler mode not yet implemented");
    }

    // Takes in an outstream to print compiler messages to, and a file to parse
    // Returns whether parsing was successful (ie. 0 errors)
    private bool executeParser(TextWriter outStream, string filePath)
    {
        string inputTxt = File.ReadAllText(filePath);
        TextWriter originalOutStream = Console.Out;
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

        //BubblGumParser.ProgramContext currNode = parser.program();
        BubblGumParser.ProgramContext rootNode = parser.program(); 
        var treesToExplore = new Queue<IParseTree>();
        for (int i = 0; i < rootNode.children.Count; i++) {
            Console.Out.WriteLine(rootNode.children[i].GetText());
            treesToExplore.Enqueue(rootNode.children[i]);
            //Console.Out.WriteLine(rootNode.children[i].GetChild(0).GetChild(0).ChildCount);
        }


        while (treesToExplore.Count > 0) {
            var tree = treesToExplore.Dequeue();
            for (int i = 0; i < tree.ChildCount; i++) {
                treesToExplore.Enqueue(tree.GetChild(i));
            }
            Console.Out.WriteLine(tree.GetText());
        }
       
        Console.SetOut(originalOutStream);
        return true;
    }
}

public enum CompilerMode
{
    Parser,
    AST,
    CodeGen
}
