using AST;
using LLVMSharp.Interop;

namespace CodeGeneration
{
    /// <summary>
    /// Tracks variable and function locations during code generation
    /// </summary>
    public class SymbolResolver
    {
        // Stack of local variable scopes (for nested blocks)
        private Stack<Dictionary<string, LLVMValueRef>> localScopes;

        // Global variables
        private Dictionary<string, LLVMValueRef> globals;

        // Function definitions
        private Dictionary<string, LLVMValueRef> functions;

        // Current function being generated
        public LLVMValueRef? CurrentFunction { get; set; }

        // Global Symbol Table (from semantic analysis)
        public GlobalSymbolTable? GST { get; set; }

        public SymbolResolver()
        {
            localScopes = new Stack<Dictionary<string, LLVMValueRef>>();
            globals = new Dictionary<string, LLVMValueRef>();
            functions = new Dictionary<string, LLVMValueRef>();
            CurrentFunction = null;
        }

        /// <summary>
        /// Enters a new local scope (e.g., function body, block)
        /// </summary>
        public void EnterScope()
        {
            localScopes.Push(new Dictionary<string, LLVMValueRef>());
        }

        /// <summary>
        /// Exits the current local scope
        /// </summary>
        public void ExitScope()
        {
            if (localScopes.Count > 0)
            {
                localScopes.Pop();
            }
        }

        /// <summary>
        /// Declares a local variable in the current scope
        /// </summary>
        public void DeclareLocal(string name, LLVMValueRef alloca)
        {
            if (localScopes.Count > 0)
            {
                localScopes.Peek()[name] = alloca;
            }
        }

        /// <summary>
        /// Declares a global variable
        /// </summary>
        public void DeclareGlobal(string name, LLVMValueRef global)
        {
            globals[name] = global;
        }

        /// <summary>
        /// Declares a function
        /// </summary>
        public void DeclareFunction(string name, LLVMValueRef function)
        {
            functions[name] = function;
        }

        /// <summary>
        /// Resolves a variable by name (searches local scopes, then globals)
        /// </summary>
        public LLVMValueRef? ResolveVariable(string name)
        {
            // Search local scopes from innermost to outermost
            foreach (var scope in localScopes)
            {
                if (scope.TryGetValue(name, out var value))
                {
                    return value;
                }
            }

            // Search globals
            if (globals.TryGetValue(name, out var globalValue))
            {
                return globalValue;
            }

            return null;
        }

        /// <summary>
        /// Resolves a function by name
        /// </summary>
        public LLVMValueRef? ResolveFunction(string name)
        {
            if (functions.TryGetValue(name, out var function))
            {
                return function;
            }
            return null;
        }

        /// <summary>
        /// Checks if a variable exists in any scope
        /// </summary>
        public bool VariableExists(string name)
        {
            return ResolveVariable(name) != null;
        }

        /// <summary>
        /// Checks if a function exists
        /// </summary>
        public bool FunctionExists(string name)
        {
            return functions.ContainsKey(name);
        }

        /// <summary>
        /// Gets the current scope depth
        /// </summary>
        public int ScopeDepth => localScopes.Count;

        /// <summary>
        /// Resolves variable info from GST to check properties like immutability
        /// </summary>
        public FlavorInfo? ResolveVariableInfo(string name)
        {
            // Search in GST if available
            // For now, return null (simplified - full implementation would search GST scope hierarchy)
            return null;
        }
    }
}
