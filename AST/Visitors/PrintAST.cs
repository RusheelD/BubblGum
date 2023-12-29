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
    public class PrintAST : Visitor, TypeVisitor
    {
        // program + program pieces

        public void Visit(Program n)
        {
            throw new NotImplementedException();
        }

        public void Visit(Class n)
        {
            throw new NotImplementedException();
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


        // assignment-related statments

        public void Visit(Assignment n)
        {
            for (int i = 0; i < n.Assignees.Count; i++)
                n.Assignees[i].Accept(this);

            n.Result.Accept(this);
        }

        public void Visit(AssignDeclLHS n)
        {
            Console.Write(n.IsImmutable ? "$" : "");
            n.Type.Accept(this);
            Console.Write(" " + n.VarName);
        }

        public void Visit(PrimitiveDeclaration1 n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PrimitiveDeclaration2 n)
        {
            throw new NotImplementedException();
        }


        // pop statements

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


        // other statements

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



        // access expressions

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


        // value expressions

        public void Visit(Bool n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(CharLiteral n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(Flavorless n)
        {
            Console.WriteLine("flavorless");
        }

        public void Visit(Identifier n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(Double n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(Integer n)
        {
            Console.WriteLine(n.Value);
        }

        public void Visit(StringLiteral n)
        {

        }


        // comparison expressions

        public void Visit(NotEquals n)
        {
            n.E1.Accept(this);
            Console.Write(" ~= ");
            n.E2.Accept(this);
        }

        public void Visit(Equals n)
        {
            n.E1.Accept(this);
            Console.Write(" = ");
            n.E2.Accept(this);
        }

        public void Visit(GreaterThan n)
        {
            n.E1.Accept(this);
            Console.Write(" > ");
            n.E2.Accept(this);
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


        // boolean expressions

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


        // arithmetic expressions

        public void Visit(Plus n)
        {
            n.E1.Accept(this);
            Console.Write(" + ");
            n.E2.Accept(this);
        }

        public void Visit(Minus n)
        {
            n.E1.Accept(this);
            Console.Write(" - ");
            n.E2.Accept(this);
        }

        public void Visit(Multiply n)
        {
            n.E1.Accept(this);
            Console.Write(" * ");
            n.E2.Accept(this);
        }

        public void Visit(Divide n)
        {
            n.E1.Accept(this);
            Console.Write(" / ");
            n.E2.Accept(this);
        }

        public void Visit(Modulo n)
        {
            n.E1.Accept(this);
            Console.Write(" % ");
            n.E2.Accept(this);
        }

        public void Visit(Power n)
        {
            n.E1.Accept(this);
            Console.Write(" ** ");
            n.E2.Accept(this);
        }


        // bit expressions

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


        // creation expressions and pack expressions

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

        public void Visit(ObjectEmpty n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PackAccess n)
        {
            throw new NotImplementedException();
        }

        public void Visit(PackSize n)
        {
            throw new NotImplementedException();
        }


        // types
        public void Visit(SingularArrayType n)
        {
            Console.Write("[");
            n.Type.Accept(this);
            Console.Write("]");
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

        public void Visit(TupleType n)
        {
            
        }
    }
}
