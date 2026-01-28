using AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ast
{
    public class TypeCheckingPass : Visitor
    {
        private GlobalSymbolTable? gst;

        private StockTable? namesspace;
        private FileTable? file;
        private string filePath = "";

        private GumTable? currClass;
        private RecipeTable? currFunction;
        private WrapperTable? currInterface;
        private CandyTable? currStruct;

        private ScopeTable? currScope;

        // Helper method to get the type name as a string
        private string GetTypeName(AnyType type)
        {
            if (type is PrimitiveType pt)
            {
                return pt.Type.ToString();
            }
            else if (type is ArrayType at)
            {
                return $"[{GetTypeName(at.TupleType)}]";
            }
            else if (type is FlavorType)
            {
                return "flavor";
            }
            else if (type is ObjectType ot)
            {
                return ot.Name;
            }
            else if (type is TupleType tt)
            {
                var names = tt.TypeNamePairs.Select(pair => GetTypeName(pair.Item1));
                return $"<{string.Join(", ", names)}>";
            }
            return "unknown";
        }

        // Helper method to check if two types are compatible
        private bool TypesAreCompatible(AnyType? t1, AnyType? t2)
        {
            if (t1 == null || t2 == null) return false;
            if (t1 is FlavorType || t2 is FlavorType) return true; // FlavorType is compatible with everything

            if (t1.GetType() != t2.GetType()) return false;

            if (t1 is PrimitiveType p1 && t2 is PrimitiveType p2)
            {
                return p1.Type == p2.Type;
            }
            else if (t1 is ArrayType a1 && t2 is ArrayType a2)
            {
                return TypesAreCompatible(a1.TupleType, a2.TupleType);
            }
            else if (t1 is ObjectType o1 && t2 is ObjectType o2)
            {
                return o1.Name == o2.Name;
            }

            return true; // Default to compatible for complex types
        }

        // Helper to check if a type is numeric
        private bool IsNumericType(AnyType type)
        {
            if (type is PrimitiveType pt)
            {
                return pt.Type == TypeBI.Sugar || pt.Type == TypeBI.Carb || pt.Type == TypeBI.PureSugar;
            }
            return false;
        }

        // Helper to check if a type is boolean
        private bool IsBooleanType(AnyType type)
        {
            if (type is PrimitiveType pt)
            {
                return pt.Type == TypeBI.Yum;
            }
            return false;
        }

        // Helper to find a variable in the current scope chain
        private FlavorInfo? FindVariable(string name)
        {
            var scope = currScope;
            while (scope != null)
            {
                if (scope.Vars.ContainsKey(name))
                {
                    return scope.Vars[name];
                }
                scope = scope.ParentScope;
            }

            // Check function parameters
            if (currFunction != null)
            {
                foreach (var param in currFunction.Parameters)
                {
                    if (param.Name == name)
                    {
                        return new FlavorInfo(param.Name, param.Type, param.IsImmutable, param.LineNum, param.Column);
                    }
                }
            }

            return null;
        }

        public void Execute(string filePath, Program n, GlobalSymbolTable gst)
        {
            this.filePath = filePath;
            file = gst.Files[filePath];
            this.gst = gst;
            namesspace = gst.Namespaces[""];
            Visit(n);
        }
        public void Visit(Program n)
        {
            foreach (var piece in n.Pieces)
            {
                piece.Accept(this);
            }
        }

        public void Visit(Stock n)
        {
            var sb = new StringBuilder();
            sb.Append(n.Names[0]);
            for (int i = 1; i < n.Names.Count; i++)
            {
                sb.Append("->");
                sb.Append(n.Names[i]);
            }

            string name = sb.ToString();

            if (!gst.Namespaces.ContainsKey(name))
            {
                Errors.NamespaceNotFound(name, filePath, n.LineNumber);
                return;
            }

            namesspace = gst.Namespaces[name];
        }

        public void Visit(ChewNames n) { }

        public void Visit(ChewPath n) { }

        public void Visit(Class n)
        {
            if (!namesspace.Classes.ContainsKey(n.Name))
            {
                Errors.ClassNotFound(n.Name, filePath, n.LineNumber);
                return;
            }

            currClass = namesspace.Classes[n.Name];
            currScope = currClass.OutermostScope;

            // Visit class members
            foreach (var (isSticky, getVis, setVis, node) in n.ClassMemberInfo)
            {
                node.Accept(this);
            }

            currClass = null;
            currScope = null;
        }

        public void Visit(Function n)
        {
            // Get the function from the symbol table
            var functionName = n.Header.Name;

            if (currClass != null)
            {
                // Method in a class
                if (!currClass.Functions.ContainsKey(functionName))
                {
                    Errors.UndefinedFunction(functionName, filePath, n.LineNumber);
                    return;
                }
                var methodOverloads = currClass.Functions[functionName];
                if (!methodOverloads.ContainsKey(n.Header.Key ?? ""))
                {
                    Errors.UndefinedFunction(functionName, filePath, n.LineNumber);
                    return;
                }
                currFunction = methodOverloads[n.Header.Key ?? ""];
            }
            else if (namesspace != null && namesspace.Functions.ContainsKey(functionName))
            {
                // Standalone function
                var functionOverloads = namesspace.Functions[functionName];
                if (!functionOverloads.ContainsKey(n.Header.Key ?? ""))
                {
                    Errors.UndefinedFunction(functionName, filePath, n.LineNumber);
                    return;
                }
                currFunction = functionOverloads[n.Header.Key ?? ""];
            }
            else
            {
                Errors.UndefinedFunction(functionName, filePath, n.LineNumber);
                return;
            }

            currScope = currFunction.OutermostScope;

            // Type check function body
            foreach (var stmt in n.Statements)
            {
                stmt.Accept(this);
            }

            currFunction = null;
            currScope = null;
        }

        public void Visit(FunctionHeader n)
        {
            // Function headers are processed during GST creation, no type checking needed here
        }

        public void Visit(Struct n)
        {
            if (!namesspace.Structs.ContainsKey(n.Name))
            {
                Errors.ClassNotFound(n.Name, filePath, n.LineNumber); // Reusing error
                return;
            }

            currStruct = namesspace.Structs[n.Name];

            // Visit struct members (structs don't have scopes like functions)
            foreach (var piece in n.Statements)
            {
                piece.Accept(this);
            }

            currStruct = null;
        }

        public void Visit(Interface n)
        {
            if (!namesspace.Interfaces.ContainsKey(n.Name))
            {
                Errors.InterfaceNotFound(n.Name, filePath, n.LineNumber);
                return;
            }

            currInterface = namesspace.Interfaces[n.Name];

            // Visit interface members (just declarations, no implementations to check)
            foreach (var (isSticky, getVis, setVis, member) in n.InterfaceMemberInfo)
            {
                member.Accept(this);
            }

            currInterface = null;
        }

        public void Visit(AssignDeclLHS n)
        {
            // This is handled by parent nodes (Assignment, PrimitiveDeclaration)
        }

        public void Visit(Assignment n)
        {
            // Type check the right-hand side expression
            n.Result.Accept(this);
            var rhsType = n.Result.Type;

            // Type check each assignee
            foreach (var assignee in n.Assignees)
            {
                if (assignee is IdentifierExp idExp)
                {
                    var varInfo = FindVariable(idExp.Value);
                    if (varInfo == null)
                    {
                        Errors.UndefinedVariable(idExp.Value, filePath, n.LineNumber);
                        continue;
                    }

                    if (varInfo.IsImmutable)
                    {
                        Errors.ImmutableAssignment(idExp.Value, filePath, n.LineNumber);
                        continue;
                    }

                    if (!TypesAreCompatible(varInfo.Type, rhsType))
                    {
                        Errors.TypeMismatch(GetTypeName(varInfo.Type), GetTypeName(rhsType), filePath, n.LineNumber);
                    }
                }
                else if (assignee is AssignDeclLHS declLhs)
                {
                    // This is a declaration with assignment
                    // The variable should already be in the symbol table from GST pass
                    declLhs.Accept(this);
                }
                else
                {
                    // Other forms of assignment (member access, array access, etc.)
                    assignee.Accept(this);
                }
            }
        }

        public void Visit(PrimitiveDeclaration1 n)
        {
            // Variables declared here should already be in the symbol table
            // Just verify they exist and are properly typed
            var declaredType = new PrimitiveType(n.TypeInfo);

            foreach (var varName in n.Variables)
            {
                var varInfo = FindVariable(varName);
                if (varInfo != null && !TypesAreCompatible(varInfo.Type, declaredType))
                {
                    Errors.TypeMismatch(GetTypeName(declaredType), GetTypeName(varInfo.Type), filePath, n.LineNumber);
                }
            }
        }

        public void Visit(PrimitiveDeclaration2 n)
        {
            // Similar to PrimitiveDeclaration1 but with initialization
            // Type check will be done by the assignment portion
        }

        public void Visit(Pop n)
        {
            // Pop statement (return) - basic validation
            // Note: Pop doesn't have a Result property, it's handled by PopVar or PopStream
        }

        public void Visit(PopLoop n)
        {
            // Pop loop iterates over a collection
            if (n.Exp != null)
            {
                n.Exp.Accept(this);

                // Verify collection is iterable (array or similar)
                if (!(n.Exp.Type is ArrayType))
                {
                    Errors.IncompatibleTypes(GetTypeName(n.Exp.Type), "array", "iteration", filePath, n.LineNumber);
                }
            }

            // Create new scope for loop variable
            var oldScope = currScope;
            var newScope = new ScopeTable { ParentScope = currScope };
            currScope = newScope;

            // Type check loop body
            foreach (var stmt in n.Statements)
            {
                stmt.Accept(this);
            }

            currScope = oldScope;
        }

        public void Visit(PopStream n)
        {
            // Pop stream for streaming output
            // Handled separately
        }

        public void Visit(PopVar n)
        {
            // Pop by variable name or index
        }

        public void Visit(Print n)
        {
            // Type check the expression being printed
            n.Thing.Accept(this);
        }

        public void Visit(Debug n)
        {
            // Type check the expression being debugged
            n.Thing.Accept(this);
        }

        public void Visit(SingleIf n)
        {
            // Type check condition
            n.Cond.Accept(this);
            if (!IsBooleanType(n.Cond.Type))
            {
                Errors.NonBooleanCondition(GetTypeName(n.Cond.Type), filePath, n.LineNumber);
            }

            // Create new scope for if body
            var oldScope = currScope;
            var newScope = new ScopeTable { ParentScope = currScope };
            currScope = newScope;

            // Type check if body
            foreach (var stmt in n.Statements)
            {
                stmt.Accept(this);
            }

            currScope = oldScope;
        }

        public void Visit(MultiIf n)
        {
            // Type check main if condition
            n.Cond.Accept(this);
            if (!IsBooleanType(n.Cond.Type))
            {
                Errors.NonBooleanCondition(GetTypeName(n.Cond.Type), filePath, n.LineNumber);
            }

            // Type check if body
            var oldScope = currScope;
            var newScope = new ScopeTable { ParentScope = currScope };
            currScope = newScope;
            foreach (var stmt in n.Statements)
            {
                stmt.Accept(this);
            }
            currScope = oldScope;

            // Type check elif branches
            for (int i = 0; i < n.Elifs.Count; i++)
            {
                var (elifCond, elifStmts) = n.Elifs[i];
                elifCond.Accept(this);
                if (!IsBooleanType(elifCond.Type))
                {
                    Errors.NonBooleanCondition(GetTypeName(elifCond.Type), filePath, n.LineNumber);
                }

                newScope = new ScopeTable { ParentScope = currScope };
                currScope = newScope;
                foreach (var stmt in elifStmts)
                {
                    stmt.Accept(this);
                }
                currScope = oldScope;
            }

            // Type check else body if present
            if (n.Else != null && n.Else.Count > 0)
            {
                newScope = new ScopeTable { ParentScope = currScope };
                currScope = newScope;
                foreach (var stmt in n.Else)
                {
                    stmt.Accept(this);
                }
                currScope = oldScope;
            }
        }

        public void Visit(IncDec n)
        {
            // Type check the variable being incremented/decremented
            n.E1.Accept(this);
            if (!IsNumericType(n.E1.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), "numeric", "increment/decrement", filePath, n.LineNumber);
            }
            if (n.E2 != null)
            {
                n.E2.Accept(this);
                if (!IsNumericType(n.E2.Type))
                {
                    Errors.IncompatibleTypes(GetTypeName(n.E2.Type), "numeric", "increment/decrement", filePath, n.LineNumber);
                }
            }
        }

        public void Visit(RepeatLoop n)
        {
            // Type check loop bounds
            n.Start.Accept(this);
            n.End.Accept(this);

            if (!IsNumericType(n.Start.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.Start.Type), "numeric", "loop bound", filePath, n.LineNumber);
            }
            if (!IsNumericType(n.End.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.End.Type), "numeric", "loop bound", filePath, n.LineNumber);
            }

            // Create new scope for loop body
            var oldScope = currScope;
            var newScope = new ScopeTable { ParentScope = currScope };
            currScope = newScope;

            // Type check loop body
            foreach (var stmt in n.Statements)
            {
                stmt.Accept(this);
            }

            currScope = oldScope;
        }

        public void Visit(While n)
        {
            // Type check condition
            n.Cond.Accept(this);
            if (!IsBooleanType(n.Cond.Type))
            {
                Errors.NonBooleanCondition(GetTypeName(n.Cond.Type), filePath, n.LineNumber);
            }

            // Create new scope for loop body
            var oldScope = currScope;
            var newScope = new ScopeTable { ParentScope = currScope };
            currScope = newScope;

            // Type check loop body
            foreach (var stmt in n.Statements)
            {
                stmt.Accept(this);
            }

            currScope = oldScope;
        }

        public void Visit(GlobalAccess n)
        {
            // Access to global namespace
            n.E1.Accept(this);
        }

        public void Visit(MemberAccess n)
        {
            // Type check the left-hand side
            n.E1.Accept(this);

            // Check if the member exists on the type
            // This would require more detailed type information
            // For now, we'll accept it and set type to FlavorType
            if (n.E2 != null)
            {
                n.E2.Accept(this);
            }
        }

        public void Visit(MethodCall n)
        {
            // Type check the function being called
            n.Lhs.Accept(this);

            // Type check all arguments
            foreach (var arg in n.Args)
            {
                arg.Accept(this);
            }

            // Function signature matching would happen here
            // For now, accept the call
        }

        public void Visit(NamespaceAccess n)
        {
            // Type check namespace access
            n.E1.Accept(this);
        }

        public void Visit(Bool n)
        {
            // Boolean literal has type Yum (bool)
            // Type is already set during AST creation, but we can verify
        }

        public void Visit(CharLiteral n)
        {
            // Char literal has type Cal
        }

        public void Visit(Flavorless n)
        {
            // Flavorless (null) has FlavorType
        }

        public void Visit(Identifier n)
        {
            // Identifier type checking is context-dependent
        }

        public void Visit(AST.Double n)
        {
            // Double literal has type Carb
        }

        public void Visit(Integer n)
        {
            // Integer literal has type Sugar
        }

        public void Visit(StringLiteral n)
        {
            // String literal has type Kcal
        }

        public void Visit(Mintpack n)
        {
            // Mintpack represents an empty pack/array
        }

        public void Visit(Cast n)
        {
            // Type check the expression being cast
            n.E1.Accept(this);

            // Verify cast is valid
            var fromType = n.E1.Type;
            var toType = n.CastType;

            // Some basic cast validation
            if (fromType is PrimitiveType && toType is PrimitiveType)
            {
                // Primitive casts are generally allowed
            }
            else if (!TypesAreCompatible(fromType, toType))
            {
                Errors.InvalidCast(GetTypeName(fromType), GetTypeName(toType), filePath, n.LineNumber);
            }
        }

        public void Visit(NotEquals n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Result type is boolean
            // Operands should be comparable
        }

        public void Visit(Equals n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Result type is boolean
            // Operands should be comparable
        }

        public void Visit(GreaterThan n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), ">", filePath, n.LineNumber);
            }
        }

        public void Visit(GreaterThanEquals n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), ">=", filePath, n.LineNumber);
            }
        }

        public void Visit(LessThan n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "<", filePath, n.LineNumber);
            }
        }

        public void Visit(LessThanEquals n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "<=", filePath, n.LineNumber);
            }
        }

        public void Visit(Is n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Type identity check
        }

        public void Visit(SubClassOf n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Subclass relationship check
        }

        public void Visit(Not n)
        {
            n.E1.Accept(this);

            if (!IsBooleanType(n.E1.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), "boolean", "NOT", filePath, n.LineNumber);
            }
        }

        public void Visit(And n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsBooleanType(n.E1.Type) || !IsBooleanType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "AND", filePath, n.LineNumber);
            }
        }

        public void Visit(Or n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsBooleanType(n.E1.Type) || !IsBooleanType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "OR", filePath, n.LineNumber);
            }
        }

        public void Visit(Plus n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Addition works on numeric types and strings
            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                // Check if string concatenation
                var t1 = n.E1.Type as PrimitiveType;
                var t2 = n.E2.Type as PrimitiveType;
                if (t1?.Type != TypeBI.Kcal || t2?.Type != TypeBI.Kcal)
                {
                    Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "+", filePath, n.LineNumber);
                }
            }
        }

        public void Visit(Minus n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "-", filePath, n.LineNumber);
            }
        }

        public void Visit(Multiply n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "*", filePath, n.LineNumber);
            }
        }

        public void Visit(Divide n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "/", filePath, n.LineNumber);
            }
        }

        public void Visit(Modulo n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "%", filePath, n.LineNumber);
            }
        }

        public void Visit(Power n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            if (!IsNumericType(n.E1.Type) || !IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "**", filePath, n.LineNumber);
            }
        }

        public void Visit(LeftShift n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Bitwise operations require integer types
            var t1 = n.E1.Type as PrimitiveType;
            var t2 = n.E2.Type as PrimitiveType;
            if (t1?.Type != TypeBI.Sugar || t2?.Type != TypeBI.Sugar)
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "<<", filePath, n.LineNumber);
            }
        }

        public void Visit(RightShift n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Bitwise operations require integer types
            var t1 = n.E1.Type as PrimitiveType;
            var t2 = n.E2.Type as PrimitiveType;
            if (t1?.Type != TypeBI.Sugar || t2?.Type != TypeBI.Sugar)
            {
                Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), ">>", filePath, n.LineNumber);
            }
        }

        public void Visit(Xor n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // XOR can work on booleans or integers
            if (!(IsBooleanType(n.E1.Type) && IsBooleanType(n.E2.Type)))
            {
                var t1 = n.E1.Type as PrimitiveType;
                var t2 = n.E2.Type as PrimitiveType;
                if (t1?.Type != TypeBI.Sugar || t2?.Type != TypeBI.Sugar)
                {
                    Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "XOR", filePath, n.LineNumber);
                }
            }
        }

        public void Visit(Xnor n)
        {
            n.E1.Accept(this);
            n.E2.Accept(this);

            // XNOR can work on booleans or integers
            if (!(IsBooleanType(n.E1.Type) && IsBooleanType(n.E2.Type)))
            {
                var t1 = n.E1.Type as PrimitiveType;
                var t2 = n.E2.Type as PrimitiveType;
                if (t1?.Type != TypeBI.Sugar || t2?.Type != TypeBI.Sugar)
                {
                    Errors.IncompatibleTypes(GetTypeName(n.E1.Type), GetTypeName(n.E2.Type), "XNOR", filePath, n.LineNumber);
                }
            }
        }

        public void Visit(NewPack n)
        {
            // Type check array initialization
            foreach (var element in n.Elements)
            {
                element.Accept(this);
            }

            // All elements should have compatible types
            if (n.Elements.Count > 0)
            {
                var firstType = n.Elements[0].Type;
                for (int i = 1; i < n.Elements.Count; i++)
                {
                    if (!TypesAreCompatible(firstType, n.Elements[i].Type))
                    {
                        Errors.IncompatibleTypes(GetTypeName(firstType), GetTypeName(n.Elements[i].Type), "array", filePath, n.LineNumber);
                    }
                }
            }
        }

        public void Visit(NewEmptyPack n)
        {
            // Empty array creation
            n.Exp.Accept(this);

            if (!IsNumericType(n.Exp.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.Exp.Type), "numeric", "array size", filePath, n.LineNumber);
            }
        }

        public void Visit(NewTuple n)
        {
            // Type check all tuple elements
            foreach (var element in n.Exps)
            {
                element.Accept(this);
            }
        }

        public void Visit(IdentifierExp n)
        {
            // Look up the identifier in the symbol table
            var varInfo = FindVariable(n.Value);
            if (varInfo == null)
            {
                Errors.UndefinedVariable(n.Value, filePath, n.LineNumber);
            }
        }

        public void Visit(PackAccess n)
        {
            // Type check array access
            n.E1.Accept(this);
            n.E2.Accept(this);

            // Verify pack is an array type
            if (!(n.E1.Type is ArrayType))
            {
                Errors.InvalidArrayAccess(GetTypeName(n.E1.Type), filePath, n.LineNumber);
            }

            // Verify index is numeric
            if (!IsNumericType(n.E2.Type))
            {
                Errors.IncompatibleTypes(GetTypeName(n.E2.Type), "numeric", "array index", filePath, n.LineNumber);
            }
        }
    }
}
