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
        // files to their info
        public Dictionary<string, FileTable> Files; // files

        // namespaces to all their info
        public Dictionary<string, StockTable> Namespaces; // namespaces

        public GlobalSymbolTable()
        {
            Files = new();
            Namespaces = new();
        }
    }
}
