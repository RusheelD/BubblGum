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
    public class Namespace
    {
        // individual path name, not full path name
        public string Name;

        public List<string> FilePaths;

        public Dictionary<string, Namespace> ChildNamespaces;

        public bool IsImported;

        public Namespace(string name) 
        {
            Name = name;
            FilePaths = new();
            ChildNamespaces = new();
            IsImported = false;
        }
    }
}
