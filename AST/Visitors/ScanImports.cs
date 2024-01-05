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
    public class ScanImports
    {
        private Namespace baseNamespace;
        private Dictionary<string, Program> filePathToProgram;
        private List<string> filesUsed;
        private string currFilePath, mainDirectory;
        private bool success;

        // requires a current file path that exists, the main directory of the entry point file,
        // a namespace tree, and a map of files to programs
        // returns whether import scanning was a success (0 errors),
        // and the paths of all files used in imports (directly or through namespace imports)
        public (bool, List<string>) Execute(string currFilePath, string mainDirectory, 
            Namespace baseNamespace, Dictionary<string, Program> filePathToProgram)
        {
            this.currFilePath = currFilePath;
            this.mainDirectory = mainDirectory;
            this.baseNamespace = baseNamespace;
            this.filePathToProgram = filePathToProgram;
            success = true;

            filesUsed = new();
            Visit(filePathToProgram[currFilePath]);
            return (success, filesUsed);
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
                if (!currNamespace.ChildNamespaces.ContainsKey(name)) {
                    Console.Error.WriteLine($"Namespace {name} not found in file {currFilePath}");
                    success = false;
                }

                currNamespace = currNamespace.ChildNamespaces[name];
            }

            var namespacesToAdd = new Queue<Namespace>();
            namespacesToAdd.Enqueue(currNamespace);
            while (namespacesToAdd.Count > 0) {
                currNamespace = namespacesToAdd.Dequeue();

                foreach (string path in currNamespace.FilePaths)
                    filesUsed.Add(path);

                foreach (var childNamespace in currNamespace.ChildNamespaces.Values)
                    namespacesToAdd.Enqueue(childNamespace);
            }
        }

        public void Visit(ChewPath n)
        {
            string fileUsedFullPath = Path.GetFullPath(Path.Combine(mainDirectory, n.Path));

            var sb = new StringBuilder();
            sb.Append(Path.GetRelativePath(mainDirectory, fileUsedFullPath));
            var file = sb.ToString();
            if (filePathToProgram.ContainsKey(file))
               filesUsed.Add(file);
            else {
                Console.Error.WriteLine($"File {mainDirectory}\\{file} not found in file {mainDirectory}\\{currFilePath}");
                success = false;
            }
        }
    }
}
