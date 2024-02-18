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
    /// <summary> Add file paths to namespaces in Namespace Table </summary>
    public class GatherNamespaces
    {
        private Namespace baseNamespace;
        private string filePath;

        /// <summary> Add current file path to this namespace in Namespace Table </summary>
        public void Execute(Program n, Namespace baseNamespace, string filePath)
        {
            this.baseNamespace = baseNamespace;
            this.filePath = filePath;
            Visit(n);
        }

        public void Visit(Program n)
        {
            foreach (var node in n.Pieces) {
                if (node is Stock)
                    Visit((Stock)node);
            }
        }

        // add current file path to this namespace
        public void Visit(Stock n)
        {
            // get a reference to the corresponding Namespace node based on namespace label
            Namespace currNamespace = baseNamespace;
            foreach (string name in n.Names)
            {
                if (!currNamespace.ChildNamespaces.ContainsKey(name))
                    currNamespace.ChildNamespaces[name] = new Namespace(name);

                currNamespace = currNamespace.ChildNamespaces[name];
            }

            currNamespace.FilePaths.Add(filePath);
        }

        public void Visit(ChewNames n) { }

        public void Visit(ChewPath n) { }
    }
}
