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

        public Namespace(string name) 
        {
            Name = name;
            FilePaths = new();
            ChildNamespaces = new();
        }

        // returns full namespace path given list of nested namespace names
        // returns empty string if path couldn't be generated
      /* public static string GetPath(List<string> indvNamespaceNames) {

           if (indvNamespaceNames == null || indvNamespaceNames.Count == 0)
                return "";

            var sb = new StringBuilder();
            sb.Append(indvNamespaceNames[0]);

            for (int i = 1; i < indvNamespaceNames.Count; i++)
                sb.Append("." + indvNamespaceNames[i]);

            return sb.ToString();
        }*/
    }
}
