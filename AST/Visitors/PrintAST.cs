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
        private bool enableBoldSet;
        private bool enableSubtleSet;
        private int numTabs = 0;

        // program + program piecesend

        public void Visit(Program n)
        {
            foreach (var piece in n.Pieces)
                piece.Accept(this);
        }

        public void Visit(Class n)
        {
            tab();
            if (n.IsSticky)
                Console.Write("sticky ");

            Console.Write(n.Visbility.ToString().ToLower());
            Console.Write(" Gum " + n.Name);

            if (n.InterfacesAndParentClasses.Count != 0)
                Console.Write($" : {n.InterfacesAndParentClasses[0]}");

            for (int i = 1; i < n.InterfacesAndParentClasses.Count; i++)
                Console.Write($", {n.InterfacesAndParentClasses[i]}");

            Console.Write(" {");

            if (n.ClassMemberInfo.Count != 0)
                Console.WriteLine();

                numTabs++;
            for (int i = 0; i < n.ClassMemberInfo.Count; i++)
            {
                tab();

                (bool isSticky, Visbility get, Visbility set, AstNode node) = n.ClassMemberInfo[i];
                if (isSticky)
                    Console.Write("sticky ");
                Console.Write(get.ToString().ToLower() + " ");

                switch (set)
                {
                    case Visbility.Bold:
                        enableBoldSet = true;
                        break;
                    case Visbility.Subtle:
                        enableSubtleSet = true;
                        break;
                    default:
                        break;
                }

                node.Accept(this);
            }
            numTabs--;

            Console.WriteLine("}\n");
        }

        public void Visit(Function n)
        {
            n.Header.Accept(this);
            acceptList(n.Statements);
            Console.WriteLine();
        }

        public void Visit(FunctionHeader n)
        {
            Console.Write($"recipe : {n.Name}(");

            if (n.Params.Count != 0)
            {
                (bool isImmut, AnyType type, string name, bool isEllipses) = n.Params[0];
                if (isImmut)
                    Console.Write("$");
                type.Accept(this);
                Console.Write($" {name}{(isEllipses ? "..." : "")}");
            }

            for (int i = 1; i < n.Params.Count; i++)
            {
                Console.Write(", ");
                (bool isImmut, AnyType type, string name, bool isEllipses) = n.Params[i];
                if (isImmut)
                    Console.Write("$");
                type.Accept(this);
                Console.Write($" {name}{(isEllipses ? "..." : "")}");
            }

            Console.Write(") ");
            if (n.Outputs.Count == 1 && n.Outputs[0].Item2.Equals("") && !n.Outputs[0].Item3) {
                n.Outputs[0].Item1.Accept(this);
                Console.Write(" ");
            }
            else if (n.Outputs.Count > 0)
            {
                Console.Write("<");
                n.Outputs[0].Item1.Accept(this);
                if (!n.Outputs[0].Item2.Equals(""))
                    Console.Write(" ");
                Console.Write($"{n.Outputs[0].Item2}{(n.Outputs[0].Item3 ? "..." : "")}");

                for (int i = 1; i < n.Outputs.Count; i++)
                {
                    Console.Write(", ");
                    n.Outputs[i].Item1.Accept(this);
                    if (!n.Outputs[i].Item2.Equals(""))
                        Console.Write(" ");
                    Console.Write($"{n.Outputs[i].Item2}{(n.Outputs[i].Item3 ? "..." : "")}");
                }
                Console.Write("> ");
            }
        }

        public void Visit(Struct n)
        {
            tab();
            Console.Write($"candy : {n.Name} ");

            acceptList(n.Statements);
            Console.WriteLine();
        }

        public void Visit(Interface n)
        {
            tab();
            if (n.IsSticky)
                Console.Write("sticky ");

            Console.Write(n.Visbility.ToString().ToLower());
            Console.Write(" Wrapper " + n.Name);

            if (n.ParentInterfaces.Count != 0)
                Console.Write($" : {n.ParentInterfaces[0]}");

            for (int i = 1; i < n.ParentInterfaces.Count; i++)
                Console.Write($", {n.ParentInterfaces[i]}");

            Console.Write(" {");

            if (n.InterfaceMemberInfo.Count != 0)
                Console.WriteLine();

            numTabs++;
            for (int i = 0; i < n.InterfaceMemberInfo.Count; i++)
            {
                tab();
                (bool isSticky, Visbility get, Visbility set, AstNode node) = n.InterfaceMemberInfo[i];
                if (isSticky)
                    Console.Write("sticky ");
                Console.Write(get.ToString().ToLower() + " ");

                switch (set)
                {
                    case Visbility.Bold:
                        enableBoldSet = true;
                        break;
                    case Visbility.Subtle:
                        enableSubtleSet = true;
                        break;
                    default:
                        break;
                }
                node.Accept(this);
            }
            numTabs--;

            Console.WriteLine("}\n");
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
            Console.Write(n.TypeInfo.ToString().ToLower() + " ");

            if (n.Variables.Count != 0)
                Console.Write(n.Variables[0]);

            for (int i = 1; i < n.Variables.Count; i++)
                Console.Write(", " + n.Variables[i]);

            endStatement();
        }

        public void Visit(PrimitiveDeclaration2 n)
        {
            if (n.TypeVarPair.Count != 0)
            {
                Console.Write(n.TypeVarPair[0].Item1.ToString().ToLower());
                Console.Write(" " + n.TypeVarPair[0].Item2);
            }

            for (int i = 1; i < n.TypeVarPair.Count; i++)
            {
                Console.Write(", " + n.TypeVarPair[i].Item1.ToString().ToLower());
                Console.Write(" " + n.TypeVarPair[i].Item2);
            }

            endStatement();
        }


        // pop statements

        public void Visit(Pop n)
        {
            Console.Write("pop");
            endStatement();
        }

        public void Visit(PopLoop n)
        {
            Console.Write($"pop flavors {n.VarName} in (");
            n.Exp.Accept(this);
            Console.Write(") => ");

            acceptList(n.Statements);
            endStatement();

        }

        public void Visit(PopStream n)
        {
            Console.Write("pop ");
            n.Var.Accept(this);
            Console.Write(" => popstream");

            if (n.HasOutputIdx)
            {
                Console.Write("(");
                n.OutputIdx.Accept(this);
                Console.Write(")");
            }

            endStatement();
        }

        public void Visit(PopVar n)
        {
            Console.Write("pop ");
            n.Var.Accept(this);

            if (n.UseOutput)
            {
                Console.Write(" => ");
                n.Output.Accept(this);
            }

            endStatement();
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
            Console.Write("if (");
            n.Cond.Accept(this);
            Console.Write(") ");
            acceptList(n.Statements);
            endStatement();
        }

        public void Visit(MultiIf n)
        {
            //  public Exp Cond;
            //public List<Statement> Statements;
            //public List<(Exp, List<Statement>)> Elifs;
            //public List<Statement> Else;

            Console.Write("if (");
            n.Cond.Accept(this);
            Console.Write(") ");

            acceptList(n.Statements);
            Console.Write(" ");

            for (int i = 0; i < n.Elifs.Count; i++)
            {
                Console.Write("elif (");
                n.Elifs[i].Item1.Accept(this);
                Console.Write(") ");
                acceptList(n.Elifs[i].Item2);
                Console.Write(" ");
            }

            if (n.Else.Count != 0)
            {
                Console.Write("else ");
                acceptList(n.Else);
            }

            endStatement();
        }

        public void Visit(IncDec n)
        {
            n.E1.Accept(this);
            Console.Write(n.ShouldIncrement ? " +: " : " -: ");
            n.E2.Accept(this);
            endStatement();
        }

        public void Visit(RepeatLoop n)
        {
            Console.Write(n.VarName + ":");
            Console.Write(n.IsUp ? " repeatUp(" : " repeatDown(");
            n.Start.Accept(this);
            Console.Write(", ");
            n.End.Accept(this);
            Console.Write(") ");

            acceptList(n.Statements);
            endStatement();
        }

        public void Visit(While n)
        {
            Console.Write("while (");
            n.Cond.Accept(this);
            Console.Write(") ");

            acceptList(n.Statements);
            endStatement();
        }


        // access expressions

        public void Visit(GlobalAccess n)
        {
            Console.Write("sweets->");
            n.E1.Accept(this);
        }

        public void Visit(MemberAccess n)
        {
            n.E1.Accept(this);
            Console.Write("->");
            n.E2.Accept(this);
        }

        public void Visit(MethodCall n)
        {
            n.Lhs.Accept(this);
            Console.Write("(");

            if (n.Args.Count != 0)
                n.Args[0].Accept(this);

            for (int i = 1; i < n.Args.Count; i++)
            {
                Console.Write(", ");
                n.Args[i].Accept(this);
            }
            Console.Write(")");
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
            Console.Write($"{"\""}{n.Value}{"\""}");
        }

        public void Visit(Mintpack n)
        {
            Console.Write("mintpack");
        }

        public void Visit(Input n)
        {
            Console.Write("Input");
        }

        public void Visit(Cast n)
        {
            n.CastType.Accept(this);
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(")");
        }

        // comparison expressions

        public void Visit(NotEquals n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" ~= ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Equals n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" = ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(GreaterThan n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" > ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(GreaterThanEquals n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" >= ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(LessThan n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" < ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(LessThanEquals n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" <= ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Is n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" is ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(SubClassOf n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" :< ");
            n.E2.Accept(this);
            Console.Write(")");
        }


        // boolean expressions

        public void Visit(Not n)
        {
            Console.Write("(");
            Console.Write("~");
            n.E1.Accept(this);
            Console.Write(")");
        }

        public void Visit(And n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" & ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Or n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" | ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        // arithmetic expressions

        public void Visit(Plus n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" + ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Minus n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" - ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Multiply n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" * ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Divide n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" / ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Modulo n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" % ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Power n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" ** ");
            n.E2.Accept(this);
            Console.Write(")");
        }


        // bit expressions

        public void Visit(LeftShift n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" <: ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(RightShift n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" :> ");
            n.E2.Accept(this);
            Console.Write(")");
        }

        public void Visit(Xor n)
        {
            Console.Write("(");
            n.E1.Accept(this);
            Console.Write(" ^ ");
            n.E2.Accept(this);
            Console.Write(")");
        }


        // creation expressions and pack expressions

        public void Visit(NewPack n)
        {
            n.PackType.Accept(this);
            Console.Write("(");
            n.Exp.Accept(this);
            Console.Write(")");
        }

        public void Visit(NewTuple n)
        {
            Console.Write("<");
            if (n.Exps.Count != 0)
                n.Exps[0].Accept(this);

            for (int i = 1; i < n.Exps.Count; i++)
            {
                Console.Write(", ");
                n.Exps[i].Accept(this);
            }

            Console.Write(">");
        }

        public void Visit(IdentifierExp n)
        {
            Console.Write(n.Value);
        }

        public void Visit(ObjectEmpty n)
        {
            n.E1.Accept(this);
            Console.Write("->empty");
        }

        public void Visit(PackAccess n)
        {
            n.E1.Accept(this);
            Console.Write("[");
            n.E2.Accept(this);
            Console.Write("]");
        }

        public void Visit(PackSize n)
        {
            n.E1.Accept(this);
            Console.Write("->size");
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
            Console.Write(n.Name);

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
            if (enableBoldSet)
                Console.Write("!");
            else if (enableSubtleSet)
                Console.Write("?");

            enableBoldSet = false;
            enableSubtleSet = false;

            if (nestedPrints == 0)
                Console.WriteLine();
        }

        // write tabs
        private void tab()
        {
            for (int i = 0; i < numTabs; i++)
                Console.Write("    ");
        }

        private void acceptList(List<AstNode> nodes)
        {
            Console.Write("{");

            if (nodes.Count != 0)
            {
                Console.WriteLine();

                numTabs++;
                foreach (var node in nodes)
                {
                    tab();
                    node.Accept(this);
                }
                numTabs--;
                tab();
            }

            Console.WriteLine("}");
        }

        private void acceptList(List<Statement> nodes)
        {
            Console.Write("{");

            if (nodes.Count != 0)
            {
                Console.WriteLine();

                numTabs++;
                foreach (var node in nodes)
                {
                    tab();
                    node.Accept(this);
                }
                numTabs--;
                tab();
            }

            Console.WriteLine("}");
        }
    }
}
