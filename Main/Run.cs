
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
        BubblGum.Execute(args);
    }
}