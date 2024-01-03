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
    public class GatherNamespaces
    {
        private Namespace baseNamespace;
        private string filePath;

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

        public void Visit(Stock n)
        {
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
