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

        public void Execute(string filePath, Program n, GlobalSymbolTable gst)
        {
            this.gst = gst;
            this.filePath = filePath;
            file = new FileTable(filePath);
            gst.Files[filePath] = file;

            namesspace = gst.Namespaces[""];
            Visit(n);
        }

        public void Visit(Program n)
        {
            foreach (var piece in n.Pieces)
                piece.Accept(this);
        }

        public void Visit(Stock n)
        {
            if (n.Names.Count == 0)
                return;

            var sb = new StringBuilder();
            sb.Append(n.Names[0]);
            for (int i = 1; i < n.Names.Count; i++)
            {
                sb.Append("->");
                sb.Append(n.Names[i]);
            }

            string name = sb.ToString();

            if (gst!.Namespaces.ContainsKey(name)) {
                namesspace = gst.Namespaces[name];
            }
            else {
                namesspace = new StockTable(name);
                gst.Namespaces[name] = namesspace;
            }
            
            file!.Namespace = namesspace;
        }
        
        public void Visit(ChewNames n) {}

        public void Visit(ChewPath n) {}

        public void Visit(Class n)
         {
            currClass = new GumTable(n.Visbility, n.IsSticky, n.Name, n.LineNumber, n.StartCol);
            currClass.OutermostScope.ParentScope = currScope;
            currScope = currClass.OutermostScope;

            foreach (var info in n.ClassMemberInfo) {
                if (info.Item4 is Function) {
                    
                    Function node = (Function)info.Item4;
                    (string name, List<RecipeFlavorInfo> parameters, List<RecipeFlavorInfo> outputs) = parseHeader(node.Header);
                    
                    currFunction = new GumRecipeTable(info.Item2, info.Item1, name, parameters, outputs, 
                        node.LineNumber, node.StartCol);
                    Visit((Function)info.Item4);
                    currClass.Recipes[currFunction.Name] = (GumRecipeTable)currFunction;
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

            if (namesspace!.Classes.ContainsKey(n.Name) || namesspace.Interfaces.ContainsKey(n.Name)
                || namesspace.Structs.ContainsKey(n.Name)) {
                Errors.DuplicateClassInNamespace(namesspace.Name, n.Name, filePath, n.LineNumber);
            }
            else {
                namesspace.Classes.Add(n.Name, currClass);
                file!.Classes.Add(n.Name, currClass);
            }
        }

        public void Visit(Interface n) {
            currInterface = new WrapperTable(n.Visbility, n.IsSticky, n.Name, n.LineNumber, n.StartCol);

            ScopeTable ogScope = currScope;
            currScope = new ScopeTable();

            foreach (var info in n.InterfaceMemberInfo) {
                if (info.Item4 is FunctionHeader) {
                    FunctionHeader node = (FunctionHeader)info.Item4;
                    (string name, List<RecipeFlavorInfo> parameters, List<RecipeFlavorInfo> outputs) = parseHeader(node);
                    
                    currFunction = new GumRecipeTable(info.Item2, info.Item1, name, parameters, outputs, 
                        node.LineNumber, node.StartCol);
                    currInterface.Recipes[currFunction.Name] = (GumRecipeTable)currFunction;
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

            //  fix this ---
            if (namesspace!.Classes.ContainsKey(n.Name) || namesspace.Interfaces.ContainsKey(n.Name)
                || namesspace.Structs.ContainsKey(n.Name)) {
                Errors.DuplicateClassInNamespace(namesspace.Name, n.Name, filePath, n.LineNumber);
            }
            else {
                namesspace.Interfaces.Add(n.Name, currInterface);
                file!.Interfaces.Add(n.Name, currInterface);
            }
        }

        
        public void Visit(Struct n) {
            currStruct = new CandyTable(n.Name, n.LineNumber, n.StartCol);
            
            ScopeTable ogScope = currScope;
            currScope = new ScopeTable();

            foreach (var info in n.Statements) {
                if (info is PrimitiveDeclaration1) {
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

            //  fix this ---
            if (namesspace!.Classes.ContainsKey(n.Name) || namesspace.Interfaces.ContainsKey(n.Name)
                || namesspace.Structs.ContainsKey(n.Name)) {
                Errors.DuplicateClassInNamespace(namesspace.Name, n.Name, filePath, n.LineNumber);
            }
            else {
                namesspace.Interfaces.Add(n.Name, currInterface);
                file!.Interfaces.Add(n.Name, currInterface);
            }
        }


        private (string, List<RecipeFlavorInfo>, List<RecipeFlavorInfo>) parseHeader(FunctionHeader n) 
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

        public void Visit(Function n)
        {
            currFunction!.OutermostScope.ParentScope = currScope;
            currScope = currFunction!.OutermostScope;
            
            foreach (AstNode statement in n.Statements) {
                statement.Accept(this);
            }
        }

        private void addToScope(PrimitiveDeclaration1 node, Visbility get, Visbility set, bool isSticky) {
            foreach (string name in node.Variables) {
                var newVar = new GumFlavorInfo(get, set, isSticky, name, 
                    new PrimitiveType(node.TypeInfo), false, node.LineNumber, node.StartCol);
                currScope!.Vars.Add(name, newVar);
            }
        }
         private void addToScope(PrimitiveDeclaration2 node, Visbility get, Visbility set, bool isSticky) {
            foreach (var pair in node.TypeVarPair) {
                PrimitiveType type = new PrimitiveType(pair.Item1);
                string name = pair.Item2;
                var newVar = new GumFlavorInfo(get, set, isSticky, name, 
                    type, false, node.LineNumber, node.StartCol);
                
                currScope!.Vars.Add(name, newVar);
            }
        }
        private void addToScope(Assignment node, Visbility get, Visbility set, bool isSticky) {
            // we only care about new vars being assigned
            foreach (var assignee in node.Assignees) {
                if (assignee is AssignDeclLHS) {
                    var varInfo = (AssignDeclLHS) assignee;
                    var newVar = new GumFlavorInfo(get, set, isSticky, varInfo.VarName, 
                    varInfo.Type, varInfo.IsImmutable, node.LineNumber, node.StartCol);

                    currScope!.Vars.Add(varInfo.VarName, newVar);
                }
            }
        }

        public void Visit(FunctionHeader n) {}

        public void Visit(AssignDeclLHS n) {}

        public void Visit(Assignment n)
        { 
            // we only care about new vars being assigned
            foreach (var assignee in n.Assignees) {
                if (assignee is AssignDeclLHS) {
                    var varInfo = (AssignDeclLHS) assignee;
                    var newVar = new FlavorInfo(varInfo.VarName, varInfo.Type, 
                        varInfo.IsImmutable, n.LineNumber, n.StartCol);
                    currScope!.Vars.Add(varInfo.VarName, newVar);
                }
            }
        }

        public void Visit(PrimitiveDeclaration1 n)
        {
            foreach (string name in n.Variables) {
                var newVar = new FlavorInfo(name, new PrimitiveType(n.TypeInfo), false, n.LineNumber, n.StartCol);
                currScope!.Vars.Add(name, newVar);
            }
        }

        public void Visit(PrimitiveDeclaration2 n)
        {
            foreach (var pair in n.TypeVarPair) { 
                PrimitiveType type = new PrimitiveType(pair.Item1);
                string name = pair.Item2;
                var newVar = new FlavorInfo(name, type, false, n.LineNumber, n.StartCol);
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
