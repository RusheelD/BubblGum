using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class GlobalSymbolTable
    {
        // map of files, key is name
        public Dictionary<string, FileTable> Files;

        // map of namespaces, key is name
        public Dictionary<string, StockTable> Namespaces;

        public GlobalSymbolTable()
        {
            Files = new();
            Namespaces = new();

            // represents true global (entities without a namespace)
            Namespaces[""] = new StockTable("");
        }

        // namespaces -> files
           // list of files:

        // files (not in namespaces) 
          // classes
          // functions
          // vars
          // interfaces
          // structs
          
           // we don't want duplicate class names, interfaces, structs
           // we don't want duplicate function names
           // we don't want duplicate var names
    }
}
