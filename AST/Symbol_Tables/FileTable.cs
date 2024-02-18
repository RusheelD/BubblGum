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
        public StockTable? Namespace;

        public Dictionary<string, WrapperTable> Interfaces;
        public Dictionary<string, GumTable> Classes; 
        public Dictionary<string, CandyTable> Structs; 
        public Dictionary<string, Dictionary<string, RecipeTable>> Functions; // key = recipe name, second_key = recipe key
        public ScopeTable OuterScope;

        public List<StockTable> ImportedNamespaces;
        public List<FileTable> ImportedFiles;

        public FileTable(string filename)
        {
            Name = filename;
            
            Interfaces = new();
            Classes = new();
            Structs = new();
            Functions = new();
            OuterScope = new();

            ImportedNamespaces = new();
            ImportedFiles = new();
        }
    }
}
