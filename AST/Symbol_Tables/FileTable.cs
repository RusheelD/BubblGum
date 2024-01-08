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
    /// <summary> Stores info about a file's symbols </summary>
    public class FileTable
    {   
        public string Name;
        public string Namespace = ""; // empty means no namespace

        public Dictionary<string, WrapperTable> Interfaces; // interfaces
        public Dictionary<string, GumTable> Classes; // classes
        public Dictionary<string, CandyTable> Structs; // structs
        public Dictionary<string, RecipeTable> Functions; // global functions
        public ScopeTable OuterScope; // outermost scope

        public List<StockTable> ImportedNamespaces;

        public FileTable(string filename)
        {
            Name = filename;
            
            Interfaces = new();
            Classes = new();
            Structs = new();
            Functions = new();
            OuterScope = new();

            ImportedNamespaces = new();
        }
    }
}
