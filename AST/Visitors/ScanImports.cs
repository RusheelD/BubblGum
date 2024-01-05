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
        private List<string>? newFilesToImport;
        private HashSet<string> filesUsed;
        private string? currFilePath, mainDirectory;
        private bool success;


        /// <summary>
        /// Requires a namespace tree, a map of file paths to their header programs, and
        /// a set of any file paths that have been already confirmed as imported/used
        /// </summary>
        public ScanImports(Namespace baseNamespace, Dictionary<string, Program> filePathToProgram, HashSet<string> filesUsed) {

            this.baseNamespace = baseNamespace;
            this.filePathToProgram = filePathToProgram;
            this.filesUsed = filesUsed;
        }

        /// <summary>
        /// Requires a path of some existing file, and the main directory of the compiler's entry point.
        /// Returns whether import scanning was a success (0 errors),
        /// and a list of all new files that should be imported because of the provided file.
        /// </summary>
        public (bool, List<string>) Execute(string currFilePath, string mainDirectory)
        {
            this.currFilePath = currFilePath;
            this.mainDirectory = mainDirectory;
            
            success = true;
            newFilesToImport = new();

            visit(filePathToProgram[currFilePath]);
            return (success, newFilesToImport);
        }

        private void visit(Program n)
        {
            foreach (var node in n.Pieces)
            {
                if (node is ChewNames)
                    visit((ChewNames)node);
                else if (node is ChewPath)
                    visit((ChewPath)node);
            }
        }

        private void visit(ChewNames n) 
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
                    if (!filesUsed.Contains(path))
                        newFilesToImport.Add(path);
                }

                foreach (var childNamespace in tempNamespace.ChildNamespaces.Values) {
                    if (!childNamespace.IsImported)
                        namespacesToAdd.Enqueue(childNamespace);
                }
            }
        }

        private void visit(ChewPath n)
        {
            string fileUsedFullPath = Path.GetFullPath(Path.Combine(mainDirectory, n.Path));

            var sb = new StringBuilder();
            sb.Append(Path.GetRelativePath(mainDirectory, fileUsedFullPath));
            var file = sb.ToString();
            if (filePathToProgram.ContainsKey(file) && !filesUsed.Contains(file))
                newFilesToImport.Add(file);
            else {
                Console.Error.WriteLine($"File {mainDirectory}\\{file} not found in file {mainDirectory}\\{currFilePath}");
                success = false;
            }
        }
    }
}
