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
        private string currFilePath, directoryPrefix;
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
        public (bool, List<string>) Execute(string currFilePath, string directoryPrefix)
        {
            this.currFilePath = currFilePath;
            this.directoryPrefix = directoryPrefix;
            
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
                else if (node is Stock) {
                    visit((Stock)node);
                }
            }
        }

        private void visit(Stock n) {
            Namespace currNamespace = baseNamespace;
            foreach (string name in n.Names)
            {
                if (!currNamespace.ChildNamespaces.ContainsKey(name))
                    Console.Error.WriteLine("this error should never appear");

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

        private void visit(ChewNames n) 
        {
            Namespace currNamespace = baseNamespace;
            foreach (string name in n.Names)
            {
                if (!currNamespace.ChildNamespaces.ContainsKey(name)) {
                    Console.Error.WriteLine($"Invalid Namespace {name} in {directoryPrefix}\\{currFilePath}");
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
            string? shortDirectory = Path.GetDirectoryName(currFilePath);
            if (shortDirectory == null)
            {
                success = false;
                return;
            }
            else if (shortDirectory.Equals(string.Empty))
                shortDirectory = "";

            var fullPath = Path.Combine(Path.Combine(directoryPrefix, shortDirectory), n.Path);
            var key = Path.GetRelativePath(directoryPrefix, fullPath);

            var sb = new StringBuilder();

            if (filePathToProgram.ContainsKey(key) && !filesUsed.Contains(key))
                newFilesToImport.Add(key);
            else {
                Console.Error.WriteLine($"Invalid import {n.Path} in {directoryPrefix}\\{currFilePath}");
                success = false;
            }
        }
    }
}
