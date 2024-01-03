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
    public class ScanHeader
    {
        private Namespace baseNamespace;
        private Dictionary<string, Program> filePathToProgram;
        private List<string> filesUsed;

        // return paths of all files used (based off imports)
        public List<string> Execute(Program n, Namespace baseNamespace, Dictionary<string, Program> filePathToProgram)
        {
            this.baseNamespace = baseNamespace;
            this.filePathToProgram = filePathToProgram;

            filesUsed = new();
            Visit(n);
            return filesUsed;
        }

        public void Visit(Program n)
        {
            foreach (var node in n.Pieces)
            {
                if (node is ChewNames)
                    Visit((ChewNames)node);
                else if (node is ChewPath)
                    Visit((ChewPath)node);
            }
        }

        public void Visit(ChewNames n) 
        {
            Namespace currNamespace = baseNamespace;
            foreach (string name in n.Names)
            {
                if (!currNamespace.ChildNamespaces.ContainsKey(name))
                    throw new Exception($"Namespace {name} not found");

                currNamespace = currNamespace.ChildNamespaces[name];
            }

            var namespacesToAdd = new Queue<Namespace>();
            namespacesToAdd.Enqueue(currNamespace);
            while (namespacesToAdd.Count > 0) {
                currNamespace = namespacesToAdd.Dequeue();
                filesUsed.AddRange(currNamespace.FilePaths);

                foreach (var childNamespace in currNamespace.ChildNamespaces.Values)
                    namespacesToAdd.Enqueue(childNamespace);
            }
        }

        public void Visit(ChewPath n)
        {
            string fileUsed = $".\\{Path.GetRelativePath(".\\", n.Path)}";
            filesUsed.Add(fileUsed);
        }
    }
}
