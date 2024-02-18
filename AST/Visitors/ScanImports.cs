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
    /// <summary> Does import scanning, and returns a list of all new files that should be imported 
    /// because of the provided file. </summary>
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


        // entry point
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

        // import the namespace this file is literally a part of
        private void visit(Stock n) {

            // get a reference to the corresponding Namespace node based on namespace label
            Namespace currNamespace = baseNamespace;
            foreach (string name in n.Names)
            {
                if (!currNamespace.ChildNamespaces.ContainsKey(name))
                    Console.Error.WriteLine("this error should never appear");

                currNamespace = currNamespace.ChildNamespaces[name];
            }
            
           importNamespace(currNamespace);
        }

        // import an external namespace
        private void visit(ChewNames n) 
        {
            // get a reference to the corresponding Namespace node based on namespace label
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
            
            // import said namespace
            importNamespace(currNamespace);
        }
        
        // based on import path name, add a specific imported file to list of new Files to Import
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

            if (filePathToProgram.ContainsKey(key) && !filesUsed.Contains(key))
                newFilesToImport.Add(key);
            else {
                Console.Error.WriteLine($"Invalid import {n.Path} in {directoryPrefix}\\{currFilePath}");
                success = false;
            }
        }

        // Mark the provided namespace and all it's child namespaces (recursively) as imported
        // Adds all files (in all newly imported namespaces) to the list of new Files To Import
        private void importNamespace(Namespace currNamespace) {
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
    }
}
