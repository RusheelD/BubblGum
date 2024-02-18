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
    /// <summary> Initializes every needed symbol table in a specified file to the global symbol table.
    /// Catch errors such as duplicate class/interface/struct/recipes or duplicate vars in the same scope, etc. </summary>
    public class CreateGSTPass1 : Visitor
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

        // Adds every symbol table needed in a specified file to the global symbol table (GST)
        // Requires the file path, the program of that file, and a GST reference
        public void Execute(string filePath, Program n, GlobalSymbolTable gst)
        {
            this.gst = gst;
            this.filePath = filePath;
            file = new FileTable(filePath);
            gst.Files[filePath] = file;

            namesspace = gst.Namespaces[""];
            Visit(n);
        }

        // fill in any GST references
        public void Visit(Program n)
        {
            foreach (var piece in n.Pieces) {

                 // for a function, looks at its header and create a recipe table for it. add it to the GST
                if (piece is Function) {
                    var node = (Function)piece;
                    (string name, List<RecipeFlavorInfo> parameters, List<RecipeFlavorInfo> outputs) = getFunctionInfo(node.Header);
                    currFunction = new RecipeTable(name, parameters, outputs, piece.LineNumber, piece.StartCol);
                    piece.Accept(this);

                    // create unique function key and add it to the function header node itself (in the AST)
                    node.Header.Key = RecipeKeys.Generate(name, node.Header.Params);

                    // if keys aren't unique, then functions are literal duplicates (same name, same parameters, etc.)
                    if (namesspace.Functions.ContainsKey(node.Header.Key))
                        Errors.DuplicateRecipeInNamespace(name, namesspace.Name, file.Name, currFunction.LineNum);

                    // update function inside file and namespace table in GST
                    else {
                        if (!file.Functions.ContainsKey(name))
                            file.Functions[name] = new();
                        file.Functions[name][node.Header.Key] = currFunction;

                        if (!namesspace.Functions.ContainsKey(name))
                            namesspace.Functions[name] = new();
                        namesspace.Functions[name][node.Header.Key] = currFunction;
                    }
                } 
                // for anything else, 
                else
                    piece.Accept(this);
            }
        }

        // set the namespace of a file in the GST
        public void Visit(Stock n)
        {
            if (n.Names.Count == 0) { 
                namesspace = null;
                return;
            }

            // get name of namespace
            var sb = new StringBuilder();
            sb.Append(n.Names[0]);
            for (int i = 1; i < n.Names.Count; i++)
            {
                sb.Append("->");
                sb.Append(n.Names[i]);
            }

            string name = sb.ToString();
            
            // add namesspace table by name to gst (if doesn't already exist)
            if (gst!.Namespaces.ContainsKey(name)) {
                namesspace = gst.Namespaces[name];
            }
            else {
                namesspace = new StockTable(name);
                gst.Namespaces[name] = namesspace;
            }
            
            // update file's corresponding namespace table
            file!.Namespace = namesspace;
        }
        
        public void Visit(ChewNames n) {}

        public void Visit(ChewPath n) {}

        // update current class table
        public void Visit(Class n)
         {
            // update current class table (note it has various scopes inside it, starting from outermost scope)
            currClass = new GumTable(n.Visbility, n.IsSticky, n.Name, n.LineNumber, n.StartCol);
            currClass.OutermostScope.ParentScope = currScope;
            currScope = currClass.OutermostScope;

            foreach (var info in n.ClassMemberInfo) {
                if (info.Item4 is Function)
                {
                    Function node = (Function)info.Item4;
                    (string name, List<RecipeFlavorInfo> parameters, List<RecipeFlavorInfo> outputs) = getFunctionInfo(node.Header);

                    // create unique function key and add it to the function header node itself (in the AST)
                    node.Header.Key = RecipeKeys.Generate(name, node.Header.Params);

                    // if keys aren't unique, then functions are literal duplicates (same name, same parameters, etc.)
                    Dictionary<string, RecipeTable> recipesWithSameName = currClass.Functions[name];
                    if (recipesWithSameName[name] != null && recipesWithSameName.ContainsKey(node.Header.Key)) {
                        Errors.DuplicateRecipeInClass(node.Header.Key, currClass.Name, filePath, node.LineNumber);
                        continue;
                    }

                    // add function to class table
                    currFunction = new GumRecipeTable(info.Item2, info.Item1, name, parameters, outputs, 
                        node.LineNumber, node.StartCol);
                    Visit((Function)info.Item4);

                    if (!currClass.Functions.ContainsKey(name))
                        currClass.Functions[name] = new();

                    currClass.Functions[name][node.Header.Key] = (GumRecipeTable)currFunction;
                }
                else if (info.Item4 is PrimitiveDeclaration1) {
                    PrimitiveDeclaration1 node = (PrimitiveDeclaration1)info.Item4;
                    addToScope(node, info.Item2, info.Item3, info.Item1);
                }
                else if (info.Item4 is PrimitiveDeclaration2) {
                    PrimitiveDeclaration2 node = (PrimitiveDeclaration2)info.Item4;
                    addToScope(node, info.Item2, info.Item3, info.Item1);
                }
                else if (info.Item4 is Assignment) {
                    Assignment node = (Assignment)info.Item4;
                    addToScope(node, info.Item2, info.Item3, info.Item1);
                }
            }
            
            currScope = currScope!.ParentScope;

            if (namesspace!.Classes.ContainsKey(n.Name) 
                || namesspace!.Interfaces.ContainsKey(n.Name)
                || namesspace!.Structs.ContainsKey(n.Name)) {
                Errors.DuplicateClassInNamespace(n.Name, namesspace.Name, filePath, n.LineNumber);
            }
            else {
                namesspace.Classes.Add(n.Name, currClass);
                file!.Classes.Add(n.Name, currClass);
            }
        }

        public void Visit(Interface n) {
            // set interface table and update its info by scopes
            currInterface = new WrapperTable(n.Visbility, n.IsSticky, n.Name, n.LineNumber, n.StartCol);
            ScopeTable ogScope = currScope;
            currScope = new ScopeTable();

            foreach (var info in n.InterfaceMemberInfo) {

                // update function table inside interface table
                if (info.Item4 is FunctionHeader) {
                    FunctionHeader node = (FunctionHeader)info.Item4;
                    (string name, List<RecipeFlavorInfo> parameters, List<RecipeFlavorInfo> outputs) = getFunctionInfo(node);
                    
                    currFunction = new GumRecipeTable(info.Item2, info.Item1, name, parameters, outputs, 
                        node.LineNumber, node.StartCol);

                    // create unique function key and add it to the function header node itself (in the AST)
                    node.Key = RecipeKeys.Generate(name, node.Params);

                    if (!currInterface.Functions.ContainsKey(name))
                        currInterface.Functions[name] = new();

                    currInterface.Functions[name][node.Key] = (GumRecipeTable)currFunction;
                }
                else if (info.Item4 is PrimitiveDeclaration1) {
                    PrimitiveDeclaration1 node = (PrimitiveDeclaration1)info.Item4;
                    addToScope(node, info.Item2, info.Item3, info.Item1);
                }
                else if (info.Item4 is PrimitiveDeclaration2) {
                    PrimitiveDeclaration2 node = (PrimitiveDeclaration2)info.Item4;
                    addToScope(node, info.Item2, info.Item3, info.Item1);
                }
                else if (info.Item4 is Assignment) {
                    Assignment node = (Assignment)info.Item4;
                    addToScope(node, info.Item2, info.Item3, info.Item1);
                }
            }
            
            // copy over all flavors/vars found to the interface's map
            foreach (var pair in currScope.Vars)
                currInterface.Flavors[pair.Key] = (GumFlavorInfo)pair.Value;
            
            currScope = ogScope;

            // add this interface table to it's corresponding namespace and file table
            if (namesspace!.Classes.ContainsKey(n.Name) 
                || namesspace!.Interfaces.ContainsKey(n.Name)
                || namesspace!.Structs.ContainsKey(n.Name)) {
                Errors.DuplicateInterfaceInNamespace(n.Name, namesspace.Name, filePath, n.LineNumber);
            }
            else {
                namesspace!.Interfaces.Add(n.Name, currInterface);
                file!.Interfaces.Add(n.Name, currInterface);
            }
        }

        
        public void Visit(Struct n) {

            // create struct table and update its info by scope
            currStruct = new CandyTable(n.Name, n.Visibility, n.LineNumber, n.StartCol);
            ScopeTable ogScope = currScope!;
            currScope = new ScopeTable();

            foreach (var info in n.Statements) {
                if (info is PrimitiveDeclaration1) {
                    PrimitiveDeclaration1 node = (PrimitiveDeclaration1)info;
                    Visit(node);
                }
                else if (info is PrimitiveDeclaration2) {
                    PrimitiveDeclaration2 node = (PrimitiveDeclaration2)info;
                    Visit(node);
                }
                else if (info is Assignment) {
                    Assignment node = (Assignment)info;
                    Visit(node);
                }
            }
            
            // copy over all flavors/vars found to the interface's map
            foreach (var pair in currScope.Vars)
                currStruct.Variables[pair.Key] = pair.Value;
            
            currScope = ogScope;
            
            // added struct table to its corresponding file table and namespace table
            if (namesspace!.Classes.ContainsKey(n.Name) 
                || namesspace.Interfaces.ContainsKey(n.Name)
                || namesspace.Structs.ContainsKey(n.Name)) {
                Errors.DuplicateStructInNamespace(n.Name, namesspace.Name, filePath, n.LineNumber);
            }
            else {
                namesspace.Structs.Add(n.Name, currStruct);
                file!.Structs.Add(n.Name, currStruct);
            }
        }

        // returns variable info for a function given a function header node
        // returns the function name, a list of info about its parameter vars (flavors), and a list
        // of info about its output vars (flavors)
        private (string, List<RecipeFlavorInfo>, List<RecipeFlavorInfo>) getFunctionInfo(FunctionHeader n) 
        {
            var parameters = new List<RecipeFlavorInfo>();
            var outputs = new List<RecipeFlavorInfo>();
            foreach (var param in n.Params) {
                var flavorinfo = new RecipeFlavorInfo(param.Item3, param.Item2, param.Item1, param.Item4, n.LineNumber, n.StartCol);
                parameters.Add(flavorinfo);
            }

            foreach (var output in n.Outputs) {
                var flavorinfo = new RecipeFlavorInfo(output.Item2, output.Item1, false, output.Item3,  n.LineNumber, n.StartCol);
                outputs.Add(flavorinfo);
            }

            return (n.Name, parameters, outputs);
        }

        // update function table
        public void Visit(Function n)
        {
            currFunction!.OutermostScope.ParentScope = currScope;
            currScope = currFunction!.OutermostScope;
            
            foreach (AstNode statement in n.Statements) {
                statement.Accept(this);
            }
        }

        // creates symbol for variable info, and adds a reference to that in current scope table. Requires a type of var declaration
        private void addToScope(PrimitiveDeclaration1 node, Visbility get, Visbility set, bool isSticky) {
            foreach (string name in node.Variables) {
                var newVar = new GumFlavorInfo(get, set, isSticky, name, 
                    new PrimitiveType(node.TypeInfo), false, node.LineNumber, node.StartCol);

                if (currScope!.Vars.ContainsKey(name))
                    Errors.DuplicateVarInScope(name, filePath, node.LineNumber);
                else
                    currScope!.Vars.Add(name, newVar);
            }
        }

        // creates symbol for variable info, and adds a reference to that in current scope table. Requires a type of var declaration
        private void addToScope(PrimitiveDeclaration2 node, Visbility get, Visbility set, bool isSticky) {
            foreach (var pair in node.TypeVarPair) {
                PrimitiveType type = new PrimitiveType(pair.Item1);
                string name = pair.Item2;
                var newVar = new GumFlavorInfo(get, set, isSticky, name, 
                    type, false, node.LineNumber, node.StartCol);
                
                if (currScope!.Vars.ContainsKey(name))
                    Errors.DuplicateVarInScope(name, filePath, node.LineNumber);
                else
                    currScope!.Vars.Add(name, newVar);
            }
        }

        // creates symbol for variable info, and adds a reference to that in current scope table. Requires a var assignment
        private void addToScope(Assignment node, Visbility get, Visbility set, bool isSticky) {
            // we only care about new vars being assigned
            foreach (var assignee in node.Assignees) {
                if (assignee is AssignDeclLHS) {
                    var varInfo = (AssignDeclLHS) assignee;
                    var newVar = new GumFlavorInfo(get, set, isSticky, varInfo.VarName, 
                    varInfo.Type, varInfo.IsImmutable, node.LineNumber, node.StartCol);
                    
                    if (currScope!.Vars.ContainsKey(varInfo.VarName))
                        Errors.DuplicateVarInScope(varInfo.VarName, filePath, varInfo.LineNumber);
                    else
                        currScope!.Vars.Add(varInfo.VarName, newVar);
                }
            }
        }

        public void Visit(FunctionHeader n) {}

        public void Visit(AssignDeclLHS n) {}

        //
        public void Visit(Assignment n)
        { 
            // we only care about new vars being assigned
            foreach (var assignee in n.Assignees) {
                if (assignee is AssignDeclLHS) {
                    var varInfo = (AssignDeclLHS) assignee;
                    var newVar = new FlavorInfo(varInfo.VarName, varInfo.Type, 
                        varInfo.IsImmutable, n.LineNumber, n.StartCol);
                    
                    if (currScope!.Vars.ContainsKey(varInfo.VarName))
                        Errors.DuplicateVarInScope(varInfo.VarName, filePath, varInfo.LineNumber);
                    else
                        currScope!.Vars.Add(varInfo.VarName, newVar);
                }
            }
        }

        public void Visit(PrimitiveDeclaration1 n)
        {
            foreach (string name in n.Variables) {
                var newVar = new FlavorInfo(name, new PrimitiveType(n.TypeInfo), false, n.LineNumber, n.StartCol);
                
                if (currScope!.Vars.ContainsKey(name))
                    Errors.DuplicateVarInScope(name, filePath, n.LineNumber);
                else
                    currScope!.Vars.Add(name, newVar);
            }
        }

        public void Visit(PrimitiveDeclaration2 n)
        {
            foreach (var pair in n.TypeVarPair) { 
                PrimitiveType type = new PrimitiveType(pair.Item1);
                string name = pair.Item2;
                var newVar = new FlavorInfo(name, type, false, n.LineNumber, n.StartCol);

                if (currScope!.Vars.ContainsKey(name))
                    Errors.DuplicateVarInScope(name, filePath, n.LineNumber);
                else
                    currScope!.Vars.Add(name, newVar);
            }
        }

        public void Visit(Pop n) {}

        public void Visit(PopLoop n)
        {
            processArea(n.Statements);
        }

        public void Visit(PopStream n) {}

        public void Visit(PopVar n) {}

        public void Visit(Print n) {}

        public void Visit(Debug n) {}

        public void Visit(SingleIf n) => processArea(n.Statements);

        public void Visit(MultiIf n)
        {
            processArea(n.Statements);

            foreach (var elif in n.Elifs)
                processArea(elif.Item2);
            
            processArea(n.Else);
        }

        public void Visit(IncDec n) {}

        public void Visit(RepeatLoop n)
        {
            processArea(n.Statements);
        }

        public void Visit(While n)
        {  
            processArea(n.Statements);
        }


        public void Visit(GlobalAccess n) {}

        public void Visit(MemberAccess n) {}

        public void Visit(MethodCall n) {}

        public void Visit(Bool n) {}

        public void Visit(CharLiteral n) {}

        public void Visit(Flavorless n) {}

        public void Visit(Identifier n) {}

        public void Visit(Double n) {}

        public void Visit(Integer n) {}

        public void Visit(StringLiteral n) {}

        public void Visit(Mintpack n) {}

        public void Visit(Cast n) {}

        public void Visit(NotEquals n) {}

        public void Visit(Equals n) {}

        public void Visit(GreaterThan n) {}

        public void Visit(GreaterThanEquals n) {}

        public void Visit(LessThan n) {}

        public void Visit(LessThanEquals n) {}

        public void Visit(Is n) {}

        public void Visit(SubClassOf n) {}

        public void Visit(Not n) {}

        public void Visit(And n) {}

        public void Visit(Or n) {}

        public void Visit(Plus n) {}

        public void Visit(Minus n) {}

        public void Visit(Multiply n) {}

        public void Visit(Divide n) {}

        public void Visit(Modulo n) {}

        public void Visit(Power n) {}

        public void Visit(LeftShift n) {}

        public void Visit(RightShift n) {}

        public void Visit(Xor n) {}

        public void Visit(Xnor n) {}

        public void Visit(NewPack n) {}

        public void Visit(NewTuple n) {}

        public void Visit(IdentifierExp n) {}

        public void Visit(PackAccess n) {}

        public void Visit(NamespaceAccess n) {}

        public void Visit(NewEmptyPack n) {}

        private void processArea(List<AstNode> statements) {
            var area = new ScopeTable();
            currScope!.NestedScopes.Add(area);
            area.ParentScope = currScope;

            currScope = area;
            foreach (var statement in statements)
                statement.Accept(this);
                
            if (!currScope.Equals(area))
                Console.Error.WriteLine("bruh u messed up scope tracking");

            currScope = area.ParentScope;
        }
    }
}
