
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
    public static void Main(string[] args)
    {
        var newArgs = new string[args.Length-1];
        for (int i = 1; i < args.Length; i++)
            newArgs[i-1] = args[i];

        if (args[0] == "run")
            BubblGum.Execute(newArgs);

        else if (args[0] == "test")
        {
            (bool success, HashSet<string> filePath) = BubblGum.ExecuteHeaderParser(args[1], Console.Out);
            if (filePath != null)
            {
                foreach (string path in filePath)
                    Console.WriteLine(path);
            }
            Console.WriteLine(success);
        }
    }
}