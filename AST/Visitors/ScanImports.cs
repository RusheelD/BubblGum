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
        private List<string> newlyConfirmedFilePaths;
        private string currFilePath, mainDirectory;
        private bool success;


        // requires a namespace tree, a map of file paths to their header programs, and
        // a set of any file paths that have been already confirmed as imported/required
        public ScanImports(Namespace baseNamespace, Dictionary<string, Program> filePathToProgram) {

            this.baseNamespace = baseNamespace;
            this.filePathToProgram = filePathToProgram;
        }

        // requires a current file path that exists and the main directory of the entry point file
        // returns whether import scanning was a success (0 errors),
        // and the paths of all files used in imports (ie. file imports or namespace imports)
        public (bool, List<string>) Execute(string currFilePath, string mainDirectory)
        {
            this.currFilePath = currFilePath;
            this.mainDirectory = mainDirectory;
            success = true;
            newlyConfirmedFilePaths = new();

            Visit(filePathToProgram[currFilePath]);
            return (success, newlyConfirmedFilePaths);
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
                    return;
                }

                currNamespace = currNamespace.ChildNamespaces[name];
            }
            
            if (currNamespace.IsImported)
                return;

            var namespacesToAdd = new Queue<Namespace>();
            namespacesToAdd.Enqueue(currNamespace);
            while (namespacesToAdd.Count > 0) {
                var tempNamespace = namespacesToAdd.Dequeue();
                tempNamespace.IsImported = true;

                foreach (string path in tempNamespace.FilePaths) {
                    newlyConfirmedFilePaths.Add(path);
                }

                foreach (var childNamespace in tempNamespace.ChildNamespaces.Values) {
                    if (!childNamespace.IsImported)
                        namespacesToAdd.Enqueue(childNamespace);
                }
            }
        }

        public void Visit(ChewPath n)
        {
            string fileUsedFullPath = Path.GetFullPath(Path.Combine(mainDirectory, n.Path));

            var sb = new StringBuilder();
            sb.Append(Path.GetRelativePath(mainDirectory, fileUsedFullPath));
            var file = sb.ToString();
            if (filePathToProgram.ContainsKey(file))
                newlyConfirmedFilePaths.Add(file);
            else {
                Console.Error.WriteLine($"File {mainDirectory}\\{file} not found in file {mainDirectory}\\{currFilePath}");
                success = false;
            }
        }
    }
}
