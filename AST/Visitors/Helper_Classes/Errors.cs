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

       public static void duplicateClass() {
          Console.Error.WriteLine("Duplicate class error");
          numErrors++;
       }
    }
}
