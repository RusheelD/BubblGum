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
    public class CreateGSTPass1 : Visitor, TypeVisitor
    {
        private GlobalSymbolTable gst;

        private StockTable namespace_;
        private FileTable file;
        private string filePath;

        private GumTable currClass;
        private RecipeTable currFunction;
        private FlavorInfo currVariable;
        private WrapperTable currInterface;
        private CandyTable currStruct;

        public void Execute(string filePath, Program n, GlobalSymbolTable gst)
        {
            this.gst = gst;
            this.filePath = filePath;
            file = new FileTable(filePath);
            gst.Files[filePath] = file;

            namespace_ = gst.Namespaces[""];
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

            if (gst.Namespaces.ContainsKey(name))
                namespace_ = gst.Namespaces[name];
            else {
                namespace_ = new StockTable(name);
                gst.Namespaces[name] = namespace_;
            }
        }
        
        public void Visit(ChewNames n) {}

        public void Visit(ChewPath n) {}

        public void Visit(Class n)
         {
            currClass = new GumTable(n.Visbility, n.IsSticky, n.Name, n.LineNumber, n.StartCol);

            /*
        // IsSticky, GetScope, SetScope, ClassMember
        public List<(bool, Visbility, Visbility, AstNode)> ClassMemberInfo;
            */

        // public Dictionary<string, GumRecipeTable> Recipes;

        // public Dictionary<string, GumFlavorInfo> Flavors;

            foreach (var info in n.ClassMemberInfo) {
                // IsSticky, GetScope, SetScope, ClassMember
                if (info.Item4 is Function) {
                    
                    Function node = (Function)info.Item4;
                    (string name, List<RecipeFlavorInfo> parameters, List<RecipeFlavorInfo> outputs) = 
                    parseHeader(node.Header);

                    // fix this
                    currFunction = new GumRecipeTable(info.Item2, info.Item1, name, parameters, new Dictionary<string, FlavorInfo>(), 
                        outputs, node.LineNumber, node.StartCol);

                    Visit((Function)info.Item4);
                }
                else if (info.Item4 is PrimitiveDeclaration1) {
                    PrimitiveDeclaration1 node = (PrimitiveDeclaration1)info.Item4;

                    // add every var to the class
                    foreach (string name in node.Variables) {
                        var newVar = new GumFlavorInfo(info.Item2, info.Item3, info.Item1, name, 
                            new PrimitiveType(node.TypeInfo), false, node.LineNumber, node.StartCol);
                        currClass.Flavors.Add(name, newVar);
                    }
                }
                else if (info.Item4 is PrimitiveDeclaration2) {
                    PrimitiveDeclaration2 node = (PrimitiveDeclaration2)info.Item4;

                    // add every var to the class
                    foreach (var pair in node.TypeVarPair) {
                        PrimitiveType type = new PrimitiveType(pair.Item1);
                        string name = pair.Item2;

                        var newVar = new GumFlavorInfo(info.Item2, info.Item3, info.Item1, name, 
                           type, false, node.LineNumber, node.StartCol);
                        currClass.Flavors.Add(name, newVar);
                    }
                }
                else if (info.Item4 is Assignment) {
                     Assignment node = (Assignment)info.Item4;

                    // we only care about new vars being assigned
                    foreach (var assignee in node.Assignees) {
                        if (assignee is AssignDeclLHS) {
                            var varInfo = (AssignDeclLHS) assignee;
                            var newVar = new GumFlavorInfo(info.Item2, info.Item3, info.Item1, varInfo.VarName, 
                            varInfo.Type, varInfo.IsImmutable, node.LineNumber, node.StartCol);

                            currClass.Flavors.Add(varInfo.VarName, newVar);
                        }
                    }
                }
            }
             

            if (namespace_.Classes.ContainsKey(n.Name) || namespace_.Interfaces.ContainsKey(n.Name)
                || namespace_.Structs.ContainsKey(n.Name)) {
                Errors.DuplicateClassInNamespace(namespace_.Name, n.Name, filePath, n.LineNumber);
            }
            else {
                namespace_.Classes.Add(n.Name, currClass);
                file.Classes.Add(n.Name, currClass);
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
            foreach (AstNode statement in n.Statements) {
                statement.Accept(this);
            }
        }

        public void Visit(FunctionHeader n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Struct n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Interface n)
        {
            throw new NotImplementedException();
        }

        public void Visit(AssignDeclLHS n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Assignment n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PrimitiveDeclaration1 n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PrimitiveDeclaration2 n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Pop n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PopLoop n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PopStream n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PopVar n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Print n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Debug n)
        {
            throw new NotImplementedException();
        }

        public void Visit(SingleIf n)
        {
            throw new NotImplementedException();
        }

        public void Visit(MultiIf n)
        {
            throw new NotImplementedException();
        }

        public void Visit(IncDec n)
        {
            throw new NotImplementedException();
        }

        public void Visit(RepeatLoop n)
        {
            throw new NotImplementedException();
        }

        public void Visit(While n)
        {
            throw new NotImplementedException();
        }


        public void Visit(GlobalAccess n)
        {
            throw new NotImplementedException();
        }

        public void Visit(MemberAccess n)
        {
            throw new NotImplementedException();
        }

        public void Visit(MethodCall n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Bool n)
        {
            throw new NotImplementedException();
        }

        public void Visit(CharLiteral n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Flavorless n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Identifier n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Double n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Integer n)
        {
            throw new NotImplementedException();
        }

        public void Visit(StringLiteral n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Mintpack n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Cast n)
        {
            throw new NotImplementedException();
        }

        public void Visit(NotEquals n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Equals n)
        {
            throw new NotImplementedException();
        }

        public void Visit(GreaterThan n)
        {
            throw new NotImplementedException();
        }

        public void Visit(GreaterThanEquals n)
        {
            throw new NotImplementedException();
        }

        public void Visit(LessThan n)
        {
            throw new NotImplementedException();
        }

        public void Visit(LessThanEquals n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Is n)
        {
            throw new NotImplementedException();
        }

        public void Visit(SubClassOf n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Not n)
        {
            throw new NotImplementedException();
        }

        public void Visit(And n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Or n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Plus n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Minus n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Multiply n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Divide n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Modulo n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Power n)
        {
            throw new NotImplementedException();
        }

        public void Visit(LeftShift n)
        {
            throw new NotImplementedException();
        }

        public void Visit(RightShift n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Xor n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Xnor n)
        {
            throw new NotImplementedException();
        }

        public void Visit(NewPack n)
        {
            throw new NotImplementedException();
        }

        public void Visit(NewTuple n)
        {
            throw new NotImplementedException();
        }

        public void Visit(IdentifierExp n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PackAccess n)
        {
            throw new NotImplementedException();
        }


        public void Visit(ArrayType n)
        {
            throw new NotImplementedException();
        }

        public void Visit(FlavorType n)
        {
            throw new NotImplementedException();
        }

        public void Visit(ObjectType n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PackType n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PrimitiveType n)
        {
            throw new NotImplementedException();
        }

        public void Visit(SingularArrayType n)
        {
            throw new NotImplementedException();
        }

        public void Visit(TupleType n)
        {
            throw new NotImplementedException();
        }
    }
}
