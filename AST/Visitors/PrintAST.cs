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
        private int nestedPrints = 0;
        private bool printTupleWithBrackets;
        // program + program pieces

        public void Visit(Program n)
        {
            foreach (var piece in n.Pieces)
                piece.Accept(this);
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
            n.Assignees[0].Accept(this);

            for (int i = 1; i < n.Assignees.Count; i++)
            {
                Console.Write(", ");
                n.Assignees[i].Accept(this);
            }

            Console.Write(" :: ");
            n.Result.Accept(this);

            endStatement();
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
            Console.Write("(");
            nestedPrints++;
            n.Thing.Accept(this);
            nestedPrints--;
            Console.Write(") !");
            if (!n.UseNewLine)
                Console.Write("!");    
            endStatement();
        }

        public void Visit(Debug n)
        {
            Console.Write("(");
            nestedPrints++;
            n.Thing.Accept(this);
            nestedPrints--;
            Console.Write(") ?");
            if (!n.UseNewLine)
                Console.Write("?");
            endStatement();
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
            n.E1.Accept(this);
            Console.Write(n.ShouldIncrement ? " +: " : " -: ");
            n.E2.Accept(this);
        }

        public void Visit(RepeatLoop n)
        {
            Console.Write(n.VarName + ": ");
            Console.Write(n.IsUp ? " repeatUp( " : " repeatDown( ");
            n.Start.Accept(this);
            Console.Write(", ");
            n.End.Accept(this);
            Console.WriteLine(") {");

            foreach (Statement s in n.Statements)
                s.Accept(this);

            Console.Write("}");
            endStatement();
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
            Console.Write(n.Value ? "yup" : "nope");
        }

        public void Visit(CharLiteral n)
        {
            Console.Write(n.Value);
        }

        public void Visit(Flavorless n)
        {
            Console.Write("flavorless");
        }

        public void Visit(Identifier n)
        {
            Console.Write(n.Value);
        }

        public void Visit(Double n)
        {
            Console.Write($"{n.Value:0.0#}");
        }

        public void Visit(Integer n)
        {
            Console.Write(n.Value);
        }

        public void Visit(StringLiteral n)
        {
            Console.Write(n.Value);
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
            n.E1.Accept(this);
            Console.Write(" >= ");
            n.E2.Accept(this);
        }

        public void Visit(LessThan n)
        {
            n.E1.Accept(this);
            Console.Write(" < ");
            n.E2.Accept(this);
        }

        public void Visit(LessThanEquals n)
        {
            n.E1.Accept(this);
            Console.Write(" <= ");
            n.E2.Accept(this);
        }

        public void Visit(Is n)
        {
            n.E1.Accept(this);
            Console.Write(" is ");
            n.E2.Accept(this);
        }

        public void Visit(SubClassOf n)
        {
            n.E1.Accept(this);
            Console.Write(" :< ");
            n.E2.Accept(this);
        }


        // boolean expressions

        public void Visit(Not n)
        {
            Console.Write("~");
            n.E1.Accept(this);
        }

        public void Visit(And n)
        {
            n.E1.Accept(this);
            Console.Write(" & ");
            n.E2.Accept(this);
        }

        public void Visit(Or n)
        {
            n.E1.Accept(this);
            Console.Write(" | ");
            n.E2.Accept(this);
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
            n.E1.Accept(this);
            Console.Write(" <: ");
            n.E2.Accept(this);
        }

        public void Visit(RightShift n)
        {
            n.E1.Accept(this);
            Console.Write(" :> ");
            n.E2.Accept(this);
        }

        public void Visit(Xor n)
        {
            n.E1.Accept(this);
            Console.Write(" ^ ");
            n.E2.Accept(this);
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
            Console.Write(n.Value);
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
            Console.Write("[");
            n.TupleType.Accept(this);
            Console.Write("]");
        }

        public void Visit(FlavorType n)
        {
            Console.Write("flavor");
        }

        public void Visit(ObjectType n)
        {
            Console.Write(n.Name + " ");

            if (n.IsPack)
                Console.Write(" pack");
        }

        public void Visit(PackType n)
        {
            Console.Write(n.Type.ToString().ToLower());
        }

        public void Visit(PrimitiveType n)
        {
            Console.Write(n.Type.ToString().ToLower());
        }

        public void Visit(TupleType n)
        {
            Console.Write("<");

            AnyType type = n.TypeNamePairs[0].Item1;
            type.Accept(this);

            string name = n.TypeNamePairs[0].Item2;
            if (!name.Equals(""))
                Console.Write(" " + name);

            for (int i = 1; i < n.TypeNamePairs.Count; i++)
            {
                Console.Write(", ");

                type = n.TypeNamePairs[i].Item1;
                type.Accept(this);

                name = n.TypeNamePairs[i].Item2;
                if (!name.Equals(""))
                    Console.Write(" " + name);
            }
            Console.Write(">");
        }


        private void endStatement()
        {
            if (nestedPrints == 0)
                Console.WriteLine();
        }
    }
}
