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
        private GumTable currClass;
        private RecipeTable currFunction;
        private FlavorInfo currVariable;
        private WrapperTable currInterface;
        private CandyTable currStruct;
        private StockTable currNamespace;

        public GlobalSymbolTable Generate(Program n)
        {
           gst = new GlobalSymbolTable();
           Visit(n);
           return gst;
        }

        public void Visit(Program n)
        {
            foreach (var piece in n.Pieces)
                piece.Accept(this);
        }

        public void Visit(Stock n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Class n)
         {
        //     public bool IsSticky;
        // public Visbility Visbility;
        // public string Name;
        // public List<string> InterfacesAndParentClasses;

        // // IsSticky, GetScope, SetScope, ClassMember
        // public List<(bool, Visbility, Visbility, AstNode)> ClassMemberInfo;
           
            var newClass = new GumTable(n.Visbility, n.IsSticky, n.Name, n.LineNumber, n.StartCol);
            
        }

        public void Visit(Function n)
        {
            throw new NotImplementedException();
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

        public void Visit(ChewNames n)
        {
            throw new NotImplementedException();
        }

        public void Visit(ChewPath n)
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
