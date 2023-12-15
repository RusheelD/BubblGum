
using System.Collections.Generic;
using System;
using System.IO;

class Program
{
    public static void Test()
    {
        BubblGum bG = new BubblGum();
        bG.Execute(CompilerMode.Parser, "./Tests/HelloWorld.txt");
    }
}
