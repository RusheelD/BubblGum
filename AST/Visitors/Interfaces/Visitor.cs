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
    public interface Visitor
    {
        // program + program pieces
        public void Visit(Program n);
        public void Visit(Class n);
        public void Visit(Function n);
        public void Visit(FunctionHeader n);
        public void Visit(Struct n);
        public void Visit(Interface n);

        // assignment and declaration statments
        public void Visit(AssignDeclLHS n);
        public void Visit(Assignment n);
        public void Visit(PrimitiveDeclaration1 n);
        public void Visit(PrimitiveDeclaration2 n);

        // pop statements
        public void Visit(Pop n);
        public void Visit(PopLoop n);
        public void Visit(PopStream n);
        public void Visit(PopVar n);

        // other statements
        public void Visit(Print n);
        public void Visit(Debug n);
        public void Visit(SingleIf n);
        public void Visit(MultiIf n);
        public void Visit(IncDec n);
        public void Visit(RepeatLoop n);
        public void Visit(While n);

        // access expressions
        public void Visit(GlobalAccess n);
        public void Visit(MemberAccess n);
        public void Visit(MethodCall n);

        // value expressions
        public void Visit(Bool n);
        public void Visit(CharLiteral n);
        public void Visit(Flavorless n);
        public void Visit(Identifier n);
        public void Visit(Double n);
        public void Visit(Integer n);
        public void Visit(StringLiteral n);

        // comparison expressions
        public void Visit(NotEquals n);
        public void Visit(Equals n);
        public void Visit(GreaterThan n);
        public void Visit(GreaterThanEquals n);
        public void Visit(LessThan n);
        public void Visit(LessThanEquals n);

        // boolean expressions
        public void Visit(Not n);
        public void Visit(And n);
        public void Visit(Or n);

        // arithmetic expressions
        public void Visit(Plus n);
        public void Visit(Minus n);
        public void Visit(Multiply n);
        public void Visit(Divide n);
        public void Visit(Modulo n);
        public void Visit(Power n);

        // bit expressions
        public void Visit(LeftShift n);
        public void Visit(RightShift n);
        public void Visit(Xor n);

        // creation expressions and pack expressions
        public void Visit(NewPack n);
        public void Visit(NewTuple n);
        public void Visit(IdentifierExp n);
        public void Visit(ObjectEmpty n);
        public void Visit(PackAccess n);
        public void Visit(PackSize n);
    }
}
