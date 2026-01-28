using AST;
using LLVMSharp.Interop;
using System.Text;

namespace CodeGeneration.Visitors
{
    /// <summary>
    /// Generates LLVM IR from BubblGum AST
    /// </summary>
    public unsafe class LLVMCodeGenerator : Visitor
    {
        private LLVMContextRef context;
        private LLVMModuleRef module;
        private LLVMBuilderRef builder;
        private TypeMapper typeMapper;
        private SymbolResolver symbolResolver;
        private GlobalSymbolTable gst;

        // Current function being generated
        private LLVMValueRef currentFunction;
        private LLVMBasicBlockRef currentBlock;

        // Return value handling
        private LLVMValueRef? returnValue;
        private bool hasReturned;

        // Named output return handling
        private Dictionary<string, int> namedOutputIndices;
        private LLVMValueRef? currentReturnStruct;

        // Static/sticky member tracking: className.memberName -> global value
        private Dictionary<string, LLVMValueRef> staticMembers;

        // Immutability tracking: variable name -> isInitialized
        private Dictionary<string, bool> immutableVars;

        // Current class context for visibility checking
        private string? currentClassName;
        private Visbility currentMethodVisibility;

        // Runtime function declarations
        private LLVMValueRef printSugarFunc;
        private LLVMValueRef printCarbFunc;
        private LLVMValueRef printKcalFunc;
        private LLVMValueRef printYumFunc;

        public LLVMCodeGenerator()
        {
            context = LLVMContextRef.Global;
            module = context.CreateModuleWithName("BubblGumModule");
            builder = context.CreateBuilder();
            typeMapper = new TypeMapper(context);
            symbolResolver = new SymbolResolver();
            returnValue = null;
            hasReturned = false;
            namedOutputIndices = new Dictionary<string, int>();
            staticMembers = new Dictionary<string, LLVMValueRef>();
            immutableVars = new Dictionary<string, bool>();
            currentClassName = null;
            currentMethodVisibility = Visbility.Bold; // Default public
        }

        /// <summary>
        /// Main entry point for code generation
        /// </summary>
        public void Execute(string filePath, Program program, GlobalSymbolTable gst)
        {
            this.gst = gst;
            symbolResolver.GST = gst;

            // Declare runtime functions
            DeclareRuntimeFunctions();

            // Generate code for the program
            Visit(program);

            // Verify the module
            if (module.TryVerify(LLVMVerifierFailureAction.LLVMPrintMessageAction, out string error))
            {
                Console.WriteLine("Module verification successful");
            }
            else
            {
                Console.WriteLine($"Module verification failed: {error}");
            }
        }

        /// <summary>
        /// Declares external runtime functions
        /// </summary>
        private void DeclareRuntimeFunctions()
        {
            // void bbgm_print_sugar(i64)
            var printSugarType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Void,
                new[] { LLVMTypeRef.Int64 }
            );
            printSugarFunc = module.AddFunction("bbgm_print_sugar", printSugarType);

            // void bbgm_print_carb(double)
            var printCarbType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Void,
                new[] { LLVMTypeRef.Double }
            );
            printCarbFunc = module.AddFunction("bbgm_print_carb", printCarbType);

            // void bbgm_print_kcal(i8*)
            var printKcalType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Void,
                new[] { LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0) }
            );
            printKcalFunc = module.AddFunction("bbgm_print_kcal", printKcalType);

            // void bbgm_print_yum(i1)
            var printYumType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Void,
                new[] { LLVMTypeRef.Int1 }
            );
            printYumFunc = module.AddFunction("bbgm_print_yum", printYumType);
        }

        /// <summary>
        /// Outputs the LLVM IR to a file
        /// </summary>
        public void EmitToFile(string outputPath)
        {
            module.PrintToFile(outputPath);
        }

        /// <summary>
        /// Gets the LLVM IR as a string
        /// </summary>
        public string GetIR()
        {
            return module.PrintToString();
        }

        // ==================== PROGRAM STRUCTURE ====================

        public void Visit(Program n)
        {
            // Generate code for all program pieces
            foreach (var piece in n.Pieces)
            {
                Visit((dynamic)piece);
            }
        }

        public void Visit(Stock n)
        {
            // Stock declarations are handled via GlobalSymbolTable
            // Actual function implementations are in the GST
            // Nothing to generate here - functions are generated from Program.Pieces
        }

        public void Visit(Function n)
        {
            // Get function signature from GST
            var funcName = n.Header.Name;

            // Clear and setup named output indices
            namedOutputIndices.Clear();
            for (int i = 0; i < n.Header.Outputs.Count; i++)
            {
                var outputName = n.Header.Outputs[i].Item2;
                if (!string.IsNullOrEmpty(outputName))
                {
                    namedOutputIndices[outputName] = i;
                }
            }

            // Map return type
            LLVMTypeRef returnType;
            bool hasVariadicReturn = n.Header.Outputs.Any(o => o.Item3); // Item3 is HasEllipses

            if (n.Header.Outputs.Count == 0)
            {
                returnType = LLVMTypeRef.Void;
            }
            else if (n.Header.Outputs.Count == 1 && !hasVariadicReturn)
            {
                returnType = typeMapper.MapType(n.Header.Outputs[0].Item1);
            }
            else
            {
                // Multiple return values - create struct
                var types = n.Header.Outputs.Select(rt => typeMapper.MapType(rt.Item1)).ToArray();
                returnType = LLVMTypeRef.CreateStruct(types, false);
            }

            // Map parameter types (handle variadic)
            bool isVariadic = n.Header.Params.Any(p => p.Item4); // Item4 is isEllipses
            var paramTypes = n.Header.Params
                .Where(p => !p.Item4) // Exclude ellipses from type signature
                .Select(p => typeMapper.MapType(p.Item2))
                .ToArray();

            // Create function type
            var functionType = LLVMTypeRef.CreateFunction(returnType, paramTypes, isVariadic);

            // Create function
            var function = module.AddFunction(funcName, functionType);
            symbolResolver.DeclareFunction(funcName, function);
            currentFunction = function;

            // Create entry block
            var entryBlock = currentFunction.AppendBasicBlock("entry");
            builder.PositionAtEnd(entryBlock);
            currentBlock = entryBlock;

            // Enter function scope
            symbolResolver.EnterScope();

            // Allocate return struct for named outputs
            if (n.Header.Outputs.Count > 1 || hasVariadicReturn)
            {
                currentReturnStruct = builder.BuildAlloca(returnType, "return_struct");
            }
            else
            {
                currentReturnStruct = null;
            }

            // Allocate and store parameters (excluding variadic)
            int paramIndex = 0;
            for (int i = 0; i < n.Header.Params.Count; i++)
            {
                var param = n.Header.Params[i];

                // Skip variadic parameters in allocation (they're handled by va_list)
                if (param.Item4) continue;

                var llvmParam = function.GetParam((uint)paramIndex);
                llvmParam.Name = param.Item3; // name is Item3

                // Create alloca for parameter (so it can be modified)
                var paramType = typeMapper.MapType(param.Item2); // type is Item2
                var alloca = builder.BuildAlloca(paramType, param.Item3);
                builder.BuildStore(llvmParam, alloca);

                symbolResolver.DeclareLocal(param.Item3, alloca);
                paramIndex++;
            }

            // Generate function body
            hasReturned = false;
            returnValue = null;

            if (n.Statements != null && n.Statements.Count > 0)
            {
                foreach (var stmt in n.Statements)
                {
                    if (hasReturned)
                        break;
                    Visit((dynamic)stmt);
                }
            }

            // If no return was generated, add default return
            if (!hasReturned)
            {
                if (returnType.Kind == LLVMTypeKind.LLVMVoidTypeKind)
                {
                    builder.BuildRetVoid();
                }
                else
                {
                    // Return default value
                    var defaultValue = typeMapper.GetDefaultValue(returnType);
                    builder.BuildRet(defaultValue);
                }
            }

            // Exit function scope
            symbolResolver.ExitScope();
        }

        public void Visit(Class n)
        {
            // Create class structure with vtable
            var className = n.Name;
            currentClassName = className;

            // Collect member fields and methods
            var fields = new List<LLVMTypeRef>();
            var methods = new List<Function>();

            foreach (var (isSticky, getScope, setScope, member) in n.ClassMemberInfo)
            {
                if (member is PrimitiveDeclaration1 decl1)
                {
                    var fieldType = typeMapper.MapType(new PrimitiveType(decl1.TypeInfo));

                    if (isSticky)
                    {
                        // Generate static/sticky members as global variables
                        foreach (var varName in decl1.Variables)
                        {
                            var globalName = $"{className}_{varName}_static";
                            var globalVar = module.AddGlobal(fieldType, globalName);
                            globalVar.Linkage = LLVMLinkage.LLVMInternalLinkage;
                            globalVar.Initializer = LLVMValueRef.CreateConstNull(fieldType);

                            // Track static member
                            staticMembers[$"{className}.{varName}"] = globalVar;
                        }
                    }
                    else
                    {
                        // Instance fields
                        foreach (var varName in decl1.Variables)
                        {
                            fields.Add(fieldType);
                        }
                    }
                }
                else if (member is PrimitiveDeclaration2 decl2)
                {
                    foreach (var (typeInfo, varName) in decl2.TypeVarPair)
                    {
                        var fieldType = typeMapper.MapType(new PrimitiveType(typeInfo));

                        if (isSticky)
                        {
                            // Generate static member as global
                            var globalName = $"{className}_{varName}_static";
                            var globalVar = module.AddGlobal(fieldType, globalName);
                            globalVar.Linkage = LLVMLinkage.LLVMInternalLinkage;
                            globalVar.Initializer = LLVMValueRef.CreateConstNull(fieldType);

                            staticMembers[$"{className}.{varName}"] = globalVar;
                        }
                        else
                        {
                            fields.Add(fieldType);
                        }
                    }
                }
                else if (member is Function func)
                {
                    methods.Add(func);
                }
            }

            // Create struct type for class (vtable pointer + fields)
            var vtablePtrType = LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
            var allFields = new List<LLVMTypeRef> { vtablePtrType };
            allFields.AddRange(fields);

            var classStructType = LLVMTypeRef.CreateStruct(allFields.ToArray(), false);

            // Register the class type (would be used for instantiation)
            // For now, just generate the methods
            foreach (var method in methods)
            {
                Visit(method);
            }

            currentClassName = null;
        }

        public void Visit(Struct n)
        {
            // Create struct type
            var structName = n.Name;
            var fields = new List<LLVMTypeRef>();

            foreach (var piece in n.Statements)
            {
                if (piece is PrimitiveDeclaration1 decl1)
                {
                    var fieldType = typeMapper.MapType(new PrimitiveType(decl1.TypeInfo));
                    foreach (var varName in decl1.Variables)
                    {
                        fields.Add(fieldType);
                    }
                }
                else if (piece is PrimitiveDeclaration2 decl2)
                {
                    foreach (var (typeInfo, varName) in decl2.TypeVarPair)
                    {
                        var fieldType = typeMapper.MapType(new PrimitiveType(typeInfo));
                        fields.Add(fieldType);
                    }
                }
            }

            // Create named struct type
            if (fields.Count > 0)
            {
                var structType = LLVMTypeRef.CreateStruct(fields.ToArray(), false);
                // Would register this type for later use in instantiation
            }
        }

        public void Visit(Interface n)
        {
            // Interface code generation - creates vtable layout
            var interfaceName = n.Name;
            var methodTypes = new List<LLVMTypeRef>();

            // Collect method signatures from interface members
            foreach (var (isSticky, getScope, setScope, member) in n.InterfaceMemberInfo)
            {
                if (member is FunctionHeader funcHeader)
                {
                    // Build function type from header
                    var returnType = LLVMTypeRef.Void;
                    var paramTypes = new List<LLVMTypeRef>();

                    // Add this pointer as first parameter
                    paramTypes.Add(LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0));

                    var funcType = LLVMTypeRef.CreateFunction(returnType, paramTypes.ToArray());
                    methodTypes.Add(LLVMTypeRef.CreatePointer(funcType, 0));
                }
            }

            // Create vtable structure type
            if (methodTypes.Count > 0)
            {
                var vtableType = LLVMTypeRef.CreateStruct(methodTypes.ToArray(), false);
                // Register vtable type for this interface (would be stored in type system)
            }
        }

        public void Visit(FunctionHeader n)
        {
            // Function headers are handled within Visit(Function)
            // No standalone code generation needed
        }

        // ==================== STATEMENTS ====================

        public void Visit(StatementList n)
        {
            foreach (var stmt in n.Statements)
            {
                if (hasReturned)
                    break; // Don't generate dead code

                Visit((dynamic)stmt);
            }
        }

        public void Visit(Assignment n)
        {
            // Evaluate the right-hand side first
            Visit((dynamic)n.Result);
            var rhsValue = returnValue!.Value;

            // Process each assignee
            foreach (var assignee in n.Assignees)
            {
                if (assignee is IdentifierExp lhsId)
                {
                    // Look up variable to check if it's immutable
                    var varInfo = symbolResolver.ResolveVariableInfo(lhsId.Value);

                    // Check immutability
                    if (varInfo != null && varInfo.IsImmutable)
                    {
                        // Check if this is the first initialization
                        if (immutableVars.ContainsKey(lhsId.Value) && immutableVars[lhsId.Value])
                        {
                            // Already initialized - cannot reassign
                            throw new Exception($"Cannot reassign immutable variable '{lhsId.Value}' at line {n.LineNumber}");
                        }
                        else
                        {
                            // First initialization - mark as initialized
                            immutableVars[lhsId.Value] = true;
                        }
                    }

                    var location = symbolResolver.ResolveVariable(lhsId.Value);
                    if (location.HasValue)
                    {
                        builder.BuildStore(rhsValue, location.Value);
                    }
                }
                else if (assignee is AssignDeclLHS declLhs)
                {
                    // Declaration with assignment: (type) varName
                    // Handle immutable declarations
                    if (declLhs.IsImmutable)
                    {
                        immutableVars[declLhs.VarName] = true; // Mark as initialized immediately
                    }

                    var varType = typeMapper.MapType(declLhs.Type);
                    var alloca = builder.BuildAlloca(varType, declLhs.VarName);
                    builder.BuildStore(rhsValue, alloca);
                    symbolResolver.DeclareLocal(declLhs.VarName, alloca);
                }
            }
        }

        public void Visit(PrimitiveDeclaration1 n)
        {
            // sugar x, y, z - just declares variables
            // Note: IsImmutable is not tracked in PrimitiveDeclaration1
            // Immutability is only tracked in AssignDeclLHS
            var primType = new PrimitiveType(n.TypeInfo);
            var varType = typeMapper.MapType(primType);
            foreach (var varName in n.Variables)
            {
                var alloca = builder.BuildAlloca(varType, varName);
                // Initialize with default value
                var defaultValue = typeMapper.GetDefaultValue(varType);
                builder.BuildStore(defaultValue, alloca);
                symbolResolver.DeclareLocal(varName, alloca);
            }
        }

        public void Visit(PrimitiveDeclaration2 n)
        {
            // (sugar x, carb y) - list of type-var pairs
            foreach (var (typeInfo, varName) in n.TypeVarPair)
            {
                var primType = new PrimitiveType(typeInfo);
                var varType = typeMapper.MapType(primType);
                var alloca = builder.BuildAlloca(varType, varName);
                // Initialize with default value
                var defaultValue = typeMapper.GetDefaultValue(varType);
                builder.BuildStore(defaultValue, alloca);
                symbolResolver.DeclareLocal(varName, alloca);

                // Track if immutable (for individual type-var pairs with $ prefix)
                // Note: PrimitiveDeclaration2 doesn't have IsImmutable flag in AST
                // Immutability would be part of typeInfo if needed
            }
        }

        public void Visit(Pop n)
        {
            // Pop has no properties - it's just a return void statement
            builder.BuildRetVoid();
            hasReturned = true;
        }

        public void Visit(Print n)
        {
            Visit((dynamic)n.Thing);
            var value = returnValue!.Value;
            var valueType = value.TypeOf;

            // Get function type for the print function
            var printType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Void, new[] { valueType });

            // Call appropriate print function based on type
            if (valueType == LLVMTypeRef.Int64)
            {
                builder.BuildCall2(printType, printSugarFunc, new[] { value }, "");
            }
            else if (valueType == LLVMTypeRef.Double)
            {
                builder.BuildCall2(printType, printCarbFunc, new[] { value }, "");
            }
            else if (valueType == LLVMTypeRef.Int1)
            {
                builder.BuildCall2(printType, printYumFunc, new[] { value }, "");
            }
            else if (valueType.Kind == LLVMTypeKind.LLVMPointerTypeKind)
            {
                builder.BuildCall2(printType, printKcalFunc, new[] { value }, "");
            }
        }

        public void Visit(Debug n)
        {
            // Debug output - includes line number and goes to stderr
            Visit((dynamic)n.Thing);
            var value = returnValue!.Value;
            var valueType = value.TypeOf;

            // Create debug info string with line number
            var debugInfo = $"[DEBUG Line {n.LineNumber}] ";
            var debugInfoPtr = builder.BuildGlobalStringPtr(debugInfo, "debuginfo");

            // Print debug header to stderr first
            var printfType = LLVMTypeRef.CreateFunction(
                LLVMTypeRef.Int32,
                new[] { LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0) },
                true
            );
            var fprintfFunc = module.GetNamedFunction("fprintf");
            if (fprintfFunc.Handle == IntPtr.Zero)
            {
                fprintfFunc = module.AddFunction("fprintf", printfType);
            }

            // Get stderr
            var stderrGlobal = module.GetNamedGlobal("stderr");
            if (stderrGlobal.Handle == IntPtr.Zero)
            {
                stderrGlobal = module.AddGlobal(
                    LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0),
                    "stderr"
                );
                stderrGlobal.Linkage = LLVMLinkage.LLVMExternalLinkage;
            }
            var stderr = builder.BuildLoad2(
                LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0),
                stderrGlobal,
                "stderr"
            );

            // Print debug info
            builder.BuildCall2(
                printfType,
                fprintfFunc,
                new[] { stderr, debugInfoPtr },
                ""
            );

            // Then print the value using appropriate debug function
            var debugType = LLVMTypeRef.CreateFunction(LLVMTypeRef.Void, new[] { valueType });

            if (valueType == LLVMTypeRef.Int64)
            {
                builder.BuildCall2(debugType, printSugarFunc, new[] { value }, "");
            }
            else if (valueType == LLVMTypeRef.Double)
            {
                builder.BuildCall2(debugType, printCarbFunc, new[] { value }, "");
            }
            else if (valueType == LLVMTypeRef.Int1)
            {
                builder.BuildCall2(debugType, printYumFunc, new[] { value }, "");
            }
            else if (valueType.Kind == LLVMTypeKind.LLVMPointerTypeKind)
            {
                builder.BuildCall2(debugType, printKcalFunc, new[] { value }, "");
            }
        }

        public void Visit(SingleIf n)
        {
            // if (condition): body

            var function = currentFunction;

            // Create blocks
            var thenBlock = function.AppendBasicBlock("if.then");
            var endBlock = function.AppendBasicBlock("if.end");

            // Evaluate condition
            Visit((dynamic)n.Cond);
            var condition = returnValue!.Value;

            // Branch
            builder.BuildCondBr(condition, thenBlock, endBlock);

            // Generate then block
            builder.PositionAtEnd(thenBlock);
            currentBlock = thenBlock;
            foreach (var stmt in n.Statements)
            {
                Visit((dynamic)stmt);
                if (hasReturned) break;
            }
            if (!hasReturned)
            {
                builder.BuildBr(endBlock);
            }

            // Position at end block
            builder.PositionAtEnd(endBlock);
            currentBlock = endBlock;
            hasReturned = false;
        }

        public void Visit(MultiIf n)
        {
            // if (cond1): body1 elif (cond2): body2 else: body3

            var function = currentFunction;
            var endBlock = function.AppendBasicBlock("if.end");

            // First if block
            Visit((dynamic)n.Cond);
            var condition = returnValue!.Value;
            var thenBlock = function.AppendBasicBlock("if.then.0");
            var nextBlock = (n.Elifs.Count > 0 || n.Else != null)
                ? function.AppendBasicBlock("if.next.0")
                : endBlock;
            builder.BuildCondBr(condition, thenBlock, nextBlock);

            builder.PositionAtEnd(thenBlock);
            foreach (var stmt in n.Statements)
            {
                Visit((dynamic)stmt);
                if (hasReturned) break;
            }
            if (!hasReturned)
            {
                builder.BuildBr(endBlock);
            }
            hasReturned = false;

            // Process elif branches
            for (int i = 0; i < n.Elifs.Count; i++)
            {
                builder.PositionAtEnd(nextBlock);
                Visit((dynamic)n.Elifs[i].Item1);
                condition = returnValue!.Value;
                thenBlock = function.AppendBasicBlock($"if.then.{i + 1}");
                nextBlock = (i < n.Elifs.Count - 1 || n.Else != null)
                    ? function.AppendBasicBlock($"if.next.{i + 1}")
                    : endBlock;
                builder.BuildCondBr(condition, thenBlock, nextBlock);

                builder.PositionAtEnd(thenBlock);
                foreach (var stmt in n.Elifs[i].Item2)
                {
                    Visit((dynamic)stmt);
                    if (hasReturned) break;
                }
                if (!hasReturned)
                {
                    builder.BuildBr(endBlock);
                }
                hasReturned = false;
            }

            // Generate else block if present
            if (n.Else != null && n.Else.Count > 0)
            {
                builder.PositionAtEnd(nextBlock);
                foreach (var stmt in n.Else)
                {
                    Visit((dynamic)stmt);
                    if (hasReturned) break;
                }
                if (!hasReturned)
                {
                    builder.BuildBr(endBlock);
                }
                hasReturned = false;
            }
            else if (nextBlock.Handle != endBlock.Handle)
            {
                builder.PositionAtEnd(nextBlock);
                builder.BuildBr(endBlock);
            }

            // Position at end block
            builder.PositionAtEnd(endBlock);
            currentBlock = endBlock;
            hasReturned = false;
        }

        public void Visit(While n)
        {
            var function = currentFunction;

            // Create blocks
            var condBlock = function.AppendBasicBlock("while.cond");
            var bodyBlock = function.AppendBasicBlock("while.body");
            var endBlock = function.AppendBasicBlock("while.end");

            // Jump to condition
            builder.BuildBr(condBlock);

            // Generate condition block
            builder.PositionAtEnd(condBlock);
            currentBlock = condBlock;
            Visit((dynamic)n.Cond);
            var condition = returnValue!.Value;
            builder.BuildCondBr(condition, bodyBlock, endBlock);

            // Generate body block
            builder.PositionAtEnd(bodyBlock);
            currentBlock = bodyBlock;
            foreach (var stmt in n.Statements)
            {
                Visit((dynamic)stmt);
                if (hasReturned) break;
            }
            if (!hasReturned)
            {
                builder.BuildBr(condBlock);
            }

            // Position at end block
            builder.PositionAtEnd(endBlock);
            currentBlock = endBlock;
            hasReturned = false;
        }

        public void Visit(RepeatLoop n)
        {
            // repeatUp(start, end): body  or repeatDown(start, end): body

            var function = currentFunction;

            // Evaluate start and end
            Visit((dynamic)n.Start);
            var startValue = returnValue!.Value;
            Visit((dynamic)n.End);
            var endValue = returnValue!.Value;

            // Create loop variable
            var loopVar = builder.BuildAlloca(LLVMTypeRef.Int64, "loop.var");
            builder.BuildStore(startValue, loopVar);

            // Create blocks
            var condBlock = function.AppendBasicBlock("repeat.cond");
            var bodyBlock = function.AppendBasicBlock("repeat.body");
            var endBlock = function.AppendBasicBlock("repeat.end");

            // Jump to condition
            builder.BuildBr(condBlock);

            // Generate condition block
            builder.PositionAtEnd(condBlock);
            currentBlock = condBlock;
            var currentValue = builder.BuildLoad2(LLVMTypeRef.Int64, loopVar, "");

            LLVMValueRef condition;
            if (!n.IsUp) // repeatDown
            {
                condition = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGE, currentValue, endValue, "");
            }
            else // repeatUp
            {
                condition = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLE, currentValue, endValue, "");
            }

            builder.BuildCondBr(condition, bodyBlock, endBlock);

            // Generate body block
            builder.PositionAtEnd(bodyBlock);
            currentBlock = bodyBlock;

            // Make loop variable accessible in body
            symbolResolver.EnterScope();
            symbolResolver.DeclareLocal(n.VarName, loopVar);

            foreach (var stmt in n.Statements)
            {
                Visit((dynamic)stmt);
                if (hasReturned) break;
            }

            symbolResolver.ExitScope();

            // Increment/decrement loop variable
            if (!hasReturned)
            {
                var loadedValue = builder.BuildLoad2(LLVMTypeRef.Int64, loopVar, "");
                var stepValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, 1, false);
                var nextValue = !n.IsUp
                    ? builder.BuildSub(loadedValue, stepValue, "")
                    : builder.BuildAdd(loadedValue, stepValue, "");
                builder.BuildStore(nextValue, loopVar);
                builder.BuildBr(condBlock);
            }

            // Position at end block
            builder.PositionAtEnd(endBlock);
            currentBlock = endBlock;
            hasReturned = false;
        }

        public void Visit(PopLoop n)
        {
            // Foreach loop: pop x from collection
            Visit((dynamic)n.Exp);
            var collectionPtr = returnValue!.Value;

            // For array iteration, assume collection is array pointer
            // Get array size (would need runtime support for dynamic arrays)
            var arraySize = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, 10, false); // Default size

            // Create loop variable for index
            var indexVar = builder.BuildAlloca(LLVMTypeRef.Int64, "loopindex");
            builder.BuildStore(LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, 0, false), indexVar);

            var condBlock = currentFunction.AppendBasicBlock("foreach.cond");
            var bodyBlock = currentFunction.AppendBasicBlock("foreach.body");
            var endBlock = currentFunction.AppendBasicBlock("foreach.end");

            builder.BuildBr(condBlock);

            // Condition: index < size
            builder.PositionAtEnd(condBlock);
            var index = builder.BuildLoad2(LLVMTypeRef.Int64, indexVar, "index");
            var cond = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT, index, arraySize, "cond");
            builder.BuildCondBr(cond, bodyBlock, endBlock);

            // Body: get element and execute statements
            builder.PositionAtEnd(bodyBlock);
            symbolResolver.EnterScope();

            // Load element at current index
            var elemPtr = builder.BuildGEP2(LLVMTypeRef.Int64, collectionPtr, new[] { index }, "elemptr");
            var elemValue = builder.BuildLoad2(LLVMTypeRef.Int64, elemPtr, "elem");

            // Store in loop variable
            var loopVar = builder.BuildAlloca(LLVMTypeRef.Int64, n.VarName);
            builder.BuildStore(elemValue, loopVar);
            symbolResolver.DeclareLocal(n.VarName, loopVar);

            // Execute loop body
            foreach (var stmt in n.Statements)
            {
                Visit((dynamic)stmt);
                if (hasReturned) break;
            }

            symbolResolver.ExitScope();

            // Increment index
            if (!hasReturned)
            {
                var nextIndex = builder.BuildAdd(index, LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, 1, false), "nextidx");
                builder.BuildStore(nextIndex, indexVar);
                builder.BuildBr(condBlock);
            }

            builder.PositionAtEnd(endBlock);
            currentBlock = endBlock;
            hasReturned = false;
        }

        public void Visit(PopVar n)
        {
            // Variable unpacking from tuples/arrays or assigning to named outputs
            Visit((dynamic)n.Var);
            var varValue = returnValue!.Value;

            if (n.UseOutput)
            {
                // Check if Output is an identifier referring to a named output
                if (n.Output is IdentifierExp identExpr)
                {
                    var outputName = identExpr.Value;

                    // Check if this is a named output
                    if (namedOutputIndices.ContainsKey(outputName))
                    {
                        // Assign to named output in return struct
                        var outputIndex = namedOutputIndices[outputName];

                        if (currentReturnStruct.HasValue)
                        {
                            var fieldPtr = builder.BuildStructGEP2(
                                currentReturnStruct.Value.TypeOf.ElementType,
                                currentReturnStruct.Value,
                                (uint)outputIndex,
                                "output_field"
                            );
                            builder.BuildStore(varValue, fieldPtr);
                        }
                        returnValue = varValue;
                        return;
                    }
                }

                // Not a named output - treat as index or expression
                Visit((dynamic)n.Output);
                var outputValue = returnValue!.Value;

                // If output is an index, use it to access array element
                if (varValue.TypeOf.Kind == LLVMTypeKind.LLVMPointerTypeKind)
                {
                    var elemPtr = builder.BuildGEP2(
                        LLVMTypeRef.Int64,
                        varValue,
                        new[] { outputValue },
                        "popelem"
                    );
                    returnValue = builder.BuildLoad2(LLVMTypeRef.Int64, elemPtr, "popval");
                }
                else if (outputValue.IsConstant)
                {
                    // Numeric index into struct - for now use first field
                    // TODO: Extract actual constant value when LLVM API is available
                    var fieldPtr = builder.BuildStructGEP2(
                        varValue.TypeOf.ElementType,
                        varValue,
                        0,
                        "field"
                    );
                    returnValue = builder.BuildLoad2(fieldPtr.TypeOf.ElementType, fieldPtr, "fieldval");
                }
                else
                {
                    returnValue = varValue;
                }
            }
            else
            {
                // No output specified, just access the variable
                returnValue = varValue;
            }
        }

        public void Visit(IncDec n)
        {
            // E1 += E2 or E1 -= E2
            if (n.E1 is IdentifierExp identifier)
            {
                var location = symbolResolver.ResolveVariable(identifier.Value);
                if (location.HasValue)
                {
                    var currentValue = builder.BuildLoad2(LLVMTypeRef.Int64, location.Value, "");
                    Visit((dynamic)n.E2);
                    var amount = returnValue!.Value;
                    var newValue = n.ShouldIncrement
                        ? builder.BuildAdd(currentValue, amount, "")
                        : builder.BuildSub(currentValue, amount, "");
                    builder.BuildStore(newValue, location.Value);
                }
            }
        }

        // ==================== EXPRESSIONS ====================

        /// <summary>
        /// Checks if a member with given visibility can be accessed from current context
        /// </summary>
        private bool CanAccessMember(Visbility memberVisibility, string? memberClassName)
        {
            // Bold (public) - always accessible
            if (memberVisibility == Visbility.Bold) return true;

            // Bland (private) - only accessible from same class
            if (memberVisibility == Visbility.Bland)
            {
                return currentClassName != null && currentClassName == memberClassName;
            }

            // Subtle (protected) - accessible from same class or subclasses
            if (memberVisibility == Visbility.Subtle)
            {
                // Would need inheritance hierarchy to check properly
                // For now, same class check
                return currentClassName != null && currentClassName == memberClassName;
            }

            return false; // Default deny
        }

        public void Visit(Integer n)
        {
            returnValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, (ulong)n.Value, true);
        }

        public void Visit(AST.Double n)
        {
            returnValue = LLVMValueRef.CreateConstReal(LLVMTypeRef.Double, n.Value);
        }

        public void Visit(Bool n)
        {
            returnValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, (ulong)(n.Value ? 1 : 0), false);
        }

        public void Visit(CharLiteral n)
        {
            returnValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int8, (ulong)n.Value, false);
        }

        public void Visit(StringLiteral n)
        {
            // Create global string constant
            var str = builder.BuildGlobalStringPtr(n.Value, "str");
            returnValue = str;
        }

        public void Visit(IdentifierExp n)
        {
            // Handle 'gum' keyword (this pointer) in class methods
            if (n.Value == "gum" && currentFunction.Handle != IntPtr.Zero)
            {
                // Get the first parameter (this pointer)
                var thisParam = currentFunction.GetParam(0);
                returnValue = thisParam;
                return;
            }

            var location = symbolResolver.ResolveVariable(n.Value);
            if (location.HasValue)
            {
                // Load the value from memory
                var type = location.Value.TypeOf.ElementType;
                returnValue = builder.BuildLoad2(type, location.Value, n.Value);
            }
        }

        public void Visit(Plus n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildAdd(left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFAdd(left, right, "");
            }
        }

        public void Visit(Minus n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildSub(left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFSub(left, right, "");
            }
        }

        public void Visit(Multiply n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildMul(left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFMul(left, right, "");
            }
        }

        public void Visit(Divide n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildSDiv(left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFDiv(left, right, "");
            }
        }

        public void Visit(Modulo n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildSRem(left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFRem(left, right, "");
            }
        }

        public void Visit(Power n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            // Declare pow function if not already declared
            var powFunc = module.GetNamedFunction("llvm.pow.f64");
            if (powFunc.Handle == IntPtr.Zero)
            {
                var powType = LLVMTypeRef.CreateFunction(
                    LLVMTypeRef.Double,
                    new[] { LLVMTypeRef.Double, LLVMTypeRef.Double }
                );
                powFunc = module.AddFunction("llvm.pow.f64", powType);
            }

            // Convert to double if needed
            LLVMValueRef leftDouble = left;
            LLVMValueRef rightDouble = right;

            if (left.TypeOf.Kind == LLVMTypeKind.LLVMIntegerTypeKind)
            {
                leftDouble = builder.BuildSIToFP(left, LLVMTypeRef.Double, "todouble");
            }
            if (right.TypeOf.Kind == LLVMTypeKind.LLVMIntegerTypeKind)
            {
                rightDouble = builder.BuildSIToFP(right, LLVMTypeRef.Double, "todouble");
            }

            var result = builder.BuildCall2(powFunc.TypeOf, powFunc, new[] { leftDouble, rightDouble }, "pow");
            returnValue = result;
        }

        public void Visit(LessThan n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLT, left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLT, left, right, "");
            }
        }

        public void Visit(LessThanEquals n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntSLE, left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOLE, left, right, "");
            }
        }

        public void Visit(GreaterThan n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGT, left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGT, left, right, "");
            }
        }

        public void Visit(GreaterThanEquals n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf))
            {
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntSGE, left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOGE, left, right, "");
            }
        }

        public void Visit(Equals n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf) || left.TypeOf.Kind == LLVMTypeKind.LLVMIntegerTypeKind)
            {
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFCmp(LLVMRealPredicate.LLVMRealOEQ, left, right, "");
            }
        }

        public void Visit(NotEquals n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            if (typeMapper.IsIntegerType(left.TypeOf) || left.TypeOf.Kind == LLVMTypeKind.LLVMIntegerTypeKind)
            {
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntNE, left, right, "");
            }
            else if (typeMapper.IsFloatType(left.TypeOf))
            {
                returnValue = builder.BuildFCmp(LLVMRealPredicate.LLVMRealONE, left, right, "");
            }
        }

        public void Visit(And n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            returnValue = builder.BuildAnd(left, right, "");
        }

        public void Visit(Or n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            returnValue = builder.BuildOr(left, right, "");
        }

        public void Visit(Not n)
        {
            Visit((dynamic)n.E1);
            var value = returnValue!.Value;

            returnValue = builder.BuildNot(value, "");
        }

        public void Visit(Xor n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            returnValue = builder.BuildXor(left, right, "");
        }

        public void Visit(Xnor n)
        {
            // XNOR = NOT(XOR)
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            var xorResult = builder.BuildXor(left, right, "");
            returnValue = builder.BuildNot(xorResult, "");
        }

        public void Visit(LeftShift n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            returnValue = builder.BuildShl(left, right, "");
        }

        public void Visit(RightShift n)
        {
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            returnValue = builder.BuildAShr(left, right, "");
        }

        public void Visit(MethodCall n)
        {
            // Function call - Lhs should be IdentifierExp for simple calls
            if (n.Lhs is IdentifierExp funcName)
            {
                var function = symbolResolver.ResolveFunction(funcName.Value);
                if (function.HasValue)
                {
                    // Evaluate arguments
                    var args = new LLVMValueRef[n.Args.Count];
                    for (int i = 0; i < n.Args.Count; i++)
                    {
                        Visit((dynamic)n.Args[i]);
                        args[i] = returnValue!.Value;
                    }

                    // Call function - TypeOf gives us the function type
                    var funcType = function.Value.TypeOf;
                    returnValue = builder.BuildCall2(funcType, function.Value, args, "");
                }
            }
        }

        // ==================== NOT YET IMPLEMENTED ====================

        public void Visit(Cast n)
        {
            Visit((dynamic)n.E1);
            var value = returnValue!.Value;
            var targetType = typeMapper.MapType(n.CastType);

            // Integer to Integer (different sizes)
            if (value.TypeOf.Kind == LLVMTypeKind.LLVMIntegerTypeKind &&
                targetType.Kind == LLVMTypeKind.LLVMIntegerTypeKind)
            {
                if (value.TypeOf.IntWidth < targetType.IntWidth)
                    returnValue = builder.BuildSExt(value, targetType, "sext");
                else if (value.TypeOf.IntWidth > targetType.IntWidth)
                    returnValue = builder.BuildTrunc(value, targetType, "trunc");
                else
                    returnValue = value;
            }
            // Integer to Float
            else if (value.TypeOf.Kind == LLVMTypeKind.LLVMIntegerTypeKind &&
                     targetType.Kind == LLVMTypeKind.LLVMDoubleTypeKind)
            {
                returnValue = builder.BuildSIToFP(value, targetType, "i2f");
            }
            // Float to Integer
            else if (value.TypeOf.Kind == LLVMTypeKind.LLVMDoubleTypeKind &&
                     targetType.Kind == LLVMTypeKind.LLVMIntegerTypeKind)
            {
                returnValue = builder.BuildFPToSI(value, targetType, "f2i");
            }
            // Pointer casts
            else if (value.TypeOf.Kind == LLVMTypeKind.LLVMPointerTypeKind)
            {
                returnValue = builder.BuildBitCast(value, targetType, "ptrcast");
            }
            else
            {
                returnValue = value; // No conversion needed
            }
        }
        public void Visit(Is n)
        {
            // Pointer/reference equality check
            Visit((dynamic)n.E1);
            var left = returnValue!.Value;
            Visit((dynamic)n.E2);
            var right = returnValue!.Value;

            // Compare pointers
            if (left.TypeOf.Kind == LLVMTypeKind.LLVMPointerTypeKind)
            {
                // Convert pointers to integers and compare
                var leftInt = builder.BuildPtrToInt(left, LLVMTypeRef.Int64, "ptr2int");
                var rightInt = builder.BuildPtrToInt(right, LLVMTypeRef.Int64, "ptr2int");
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, leftInt, rightInt, "is");
            }
            else
            {
                // For non-pointers, use regular equality
                returnValue = builder.BuildICmp(LLVMIntPredicate.LLVMIntEQ, left, right, "is");
            }
        }
        public void Visit(PackAccess n)
        {
            // Array access: arr[index]
            Visit((dynamic)n.E1);
            var arrayPtr = returnValue!.Value;
            Visit((dynamic)n.E2);
            var index = returnValue!.Value;

            // Get element pointer and load
            var elemPtr = builder.BuildGEP2(LLVMTypeRef.Int64, arrayPtr, new[] { index }, "elemptr");
            returnValue = builder.BuildLoad2(LLVMTypeRef.Int64, elemPtr, "elem");
        }
        public void Visit(MemberAccess n)
        {
            // Object/struct member access: obj.field or obj->field
            // Check for static member access first (ClassName.staticField)
            if (n.E1 is IdentifierExp className && n.E2 is IdentifierExp memberName)
            {
                var staticKey = $"{className.Value}.{memberName.Value}";
                if (staticMembers.ContainsKey(staticKey))
                {
                    // Access static member
                    var staticMember = staticMembers[staticKey];
                    var memberType = staticMember.TypeOf.ElementType;
                    returnValue = builder.BuildLoad2(memberType, staticMember, memberName.Value);
                    return;
                }
            }

            // Instance member access
            Visit((dynamic)n.E1);
            var objPtr = returnValue!.Value;

            // E2 should be an identifier representing the field name
            if (n.E2 is IdentifierExp fieldName)
            {
                // Visibility checking would happen here
                // For now, we need type information to determine:
                // 1. The actual field index
                // 2. The field's visibility
                // 3. Whether access is allowed from current context

                // TODO: Look up field info from GST/class definition
                // var fieldInfo = gst.GetFieldInfo(objType, fieldName.Value);
                // if (!CanAccessMember(fieldInfo.Visibility, currentClassName))
                // {
                //     throw new Exception($"Cannot access {fieldInfo.Visibility} member '{fieldName.Value}'");
                // }

                // For now, assume field index 0 (would need type info for proper indexing)
                var indices = new[] {
                    LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false),
                    LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false)
                };

                var elemType = LLVMTypeRef.Int64; // Default, would need type system
                var fieldPtr = builder.BuildGEP2(elemType, objPtr, indices, "fieldptr");
                returnValue = builder.BuildLoad2(elemType, fieldPtr, fieldName.Value);
            }
            else
            {
                returnValue = objPtr; // Fallback
            }
        }
        public void Visit(NamespaceAccess n)
        {
            // Namespace-qualified access: Namespace::Member
            // Name contains the namespace, E1 contains the member
            Visit((dynamic)n.E1);
            // returnValue is set by visiting E1
        }
        public void Visit(GlobalAccess n)
        {
            // Global scope access: ::identifier
            // Access global symbol bypassing local scopes
            if (n.E1 is IdentifierExp identifier)
            {
                var globalVar = module.GetNamedGlobal(identifier.Value);
                if (globalVar.Handle != IntPtr.Zero)
                {
                    var type = globalVar.TypeOf.ElementType;
                    returnValue = builder.BuildLoad2(type, globalVar, identifier.Value);
                }
                else
                {
                    // Try as function
                    var globalFunc = module.GetNamedFunction(identifier.Value);
                    if (globalFunc.Handle != IntPtr.Zero)
                    {
                        returnValue = globalFunc;
                    }
                }
            }
        }
        public void Visit(NewPack n)
        {
            // Create array with initial elements
            // Allocate space for array
            var arraySize = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, (ulong)n.Elements.Count, false);
            var arrayType = LLVMTypeRef.CreateArray(LLVMTypeRef.Int64, (uint)n.Elements.Count);
            var arrayAlloca = builder.BuildAlloca(arrayType, "array");

            // Store each element
            for (int i = 0; i < n.Elements.Count; i++)
            {
                Visit((dynamic)n.Elements[i]);
                var elemValue = returnValue!.Value;
                var indices = new[] {
                    LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false),
                    LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, (ulong)i, false)
                };
                var elemPtr = builder.BuildGEP2(arrayType, arrayAlloca, indices, "elemptr");
                builder.BuildStore(elemValue, elemPtr);
            }

            // Return pointer to array
            var indices0 = new[] { LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false) };
            returnValue = builder.BuildGEP2(arrayType, arrayAlloca, indices0, "arrptr");
        }
        public void Visit(NewEmptyPack n)
        {
            // Allocate empty array - requires size parameter from type info
            // For now, allocate a default-sized array
            var arraySize = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, 10, false);
            var arrayType = LLVMTypeRef.CreateArray(LLVMTypeRef.Int64, 10);
            var arrayAlloca = builder.BuildAlloca(arrayType, "emptyarray");

            var indices = new[] { LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false) };
            returnValue = builder.BuildGEP2(arrayType, arrayAlloca, indices, "arrptr");
        }
        public void Visit(NewTuple n)
        {
            // Create tuple structure
            var fieldTypes = new LLVMTypeRef[n.Exps.Count];
            var fieldValues = new LLVMValueRef[n.Exps.Count];

            // Evaluate each expression and get its type
            for (int i = 0; i < n.Exps.Count; i++)
            {
                Visit((dynamic)n.Exps[i]);
                fieldValues[i] = returnValue!.Value;
                fieldTypes[i] = fieldValues[i].TypeOf;
            }

            // Create struct type for tuple
            var tupleType = LLVMTypeRef.CreateStruct(fieldTypes, false);
            var tupleAlloca = builder.BuildAlloca(tupleType, "tuple");

            // Store each field
            for (int i = 0; i < n.Exps.Count; i++)
            {
                var indices = new[] {
                    LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false),
                    LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, (ulong)i, false)
                };
                var fieldPtr = builder.BuildGEP2(tupleType, tupleAlloca, indices, $"field{i}ptr");
                builder.BuildStore(fieldValues[i], fieldPtr);
            }

            // Return the tuple pointer
            returnValue = tupleAlloca;
        }
        public void Visit(Mintpack n)
        {
            // Mintpack operations - create package/array from elements
            // This is similar to NewPack but with different syntax
            // For now, create an empty array that can be populated
            var defaultSize = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, 10, false);
            var arrayType = LLVMTypeRef.CreateArray(LLVMTypeRef.Int64, 10);
            var arrayAlloca = builder.BuildAlloca(arrayType, "mintpack");

            var indices = new[] { LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false) };
            returnValue = builder.BuildGEP2(arrayType, arrayAlloca, indices, "packptr");
        }

        public void Visit(Flavorless n)
        {
            // Flavorless (nflv) - NOT null, but a default-initialized object with flavorless flag
            // According to README: "This cow actually points to a default Cow reference"
            // 
            // Create a struct with:
            // - Field 0: flavorless flag (i1 boolean, set to true)
            // - Field 1: default-initialized data (zeroed out)

            var fieldTypes = new[] {
                LLVMTypeRef.Int1,  // flavorless flag
                LLVMTypeRef.Int64  // default data (can be any type, using i64 as placeholder)
            };

            var flavorlessType = LLVMTypeRef.CreateStruct(fieldTypes, false);
            var flavorlessAlloca = builder.BuildAlloca(flavorlessType, "flavorless");

            // Set flavorless flag to true (1)
            var flagIndices = new[] {
                LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false),
                LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false)
            };
            var flagPtr = builder.BuildGEP2(flavorlessType, flavorlessAlloca, flagIndices, "flagptr");
            var trueValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int1, 1, false);
            builder.BuildStore(trueValue, flagPtr);

            // Initialize data field to zero (default value)
            var dataIndices = new[] {
                LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false),
                LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 1, false)
            };
            var dataPtr = builder.BuildGEP2(flavorlessType, flavorlessAlloca, dataIndices, "dataptr");
            var zeroValue = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int64, 0, false);
            builder.BuildStore(zeroValue, dataPtr);

            // Return pointer to the flavorless object
            returnValue = flavorlessAlloca;
        }

        public void Visit(ChewPath n)
        {
            // Path/import operations
            // Not directly code-generated (handled in preprocessing)
        }

        public void Visit(ChewNames n)
        {
            // Import names
            // Not directly code-generated (handled in preprocessing)
        }

        public void Visit(Identifier n)
        {
            // Identifier nodes are typically handled by IdentifierExp
            // This is for compatibility with certain AST patterns
        }

        public void Visit(AssignDeclLHS n)
        {
            // Assignment declaration left-hand side
            // Handled within Assignment visitor
        }

        public void Visit(SubClassOf n)
        {
            // Subclass relationship - used in type checking
            // Not directly code-generated
        }

        public void Visit(PopStream n)
        {
            // Stream operations - pop value from stream
            Visit((dynamic)n.Var);
            var streamPtr = returnValue!.Value;

            if (n.HasOutputIdx)
            {
                // Pop with index specification
                Visit((dynamic)n.OutputIdx);
                var index = returnValue!.Value;

                // Access stream element at index
                if (streamPtr.TypeOf.Kind == LLVMTypeKind.LLVMPointerTypeKind)
                {
                    var elemPtr = builder.BuildGEP2(
                        LLVMTypeRef.Int64,
                        streamPtr,
                        new[] { index },
                        "streamelem"
                    );
                    var value = builder.BuildLoad2(LLVMTypeRef.Int64, elemPtr, "streamval");

                    // Remove element from stream (would need runtime support)
                    // For now, just return the value
                    returnValue = value;
                }
            }
            else
            {
                // Pop first element from stream
                if (streamPtr.TypeOf.Kind == LLVMTypeKind.LLVMPointerTypeKind)
                {
                    var zero = LLVMValueRef.CreateConstInt(LLVMTypeRef.Int32, 0, false);
                    var elemPtr = builder.BuildGEP2(
                        LLVMTypeRef.Int64,
                        streamPtr,
                        new[] { zero },
                        "streamfirst"
                    );
                    returnValue = builder.BuildLoad2(LLVMTypeRef.Int64, elemPtr, "streamval");
                }
            }
        }
    }
}
