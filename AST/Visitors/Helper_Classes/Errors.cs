using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BubblGumParser;

namespace AST
{
    public class Errors
    {
       public static int numErrors = 0;

       public static void DuplicateClassInNamespace(string namespaceName, string className, string fileName, int line) {
         if (namespaceName != "")
            Console.Error.WriteLine($"Duplicate class {className} in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Duplicate class {className} in global namespace at file {fileName} on line {line}");
         
         numErrors++;
       }
    }
}
