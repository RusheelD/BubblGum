using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BubblGumParser;

namespace AST
{
    public class CreateAST
    {

       public Program Visit(ProgramContext n)
        {
            var programPieces = new List<AstNode>();
            for (int i = 0; i < n.children.Count-1; i++)
            {
                AstNode node = visit((dynamic)n.children[i]);

                if (node is StatementList)
                    programPieces.AddRange(((StatementList)node).Statements);
                else if (node is ProgramPiece)
                    programPieces.Add(node);
                else
                    Console.Error.WriteLine($"node of type {node.GetType()} isn't handled");
            }

            return new Program(programPieces, 0, 0);
       }

        // may return statement or statement list
        private Statement visit(StatementContext n) => visit((dynamic)n.children[0]);

        private void visit(ClassContext n)
        {
           
        }

        private void visit(InterfaceContext n)
        {

        }

        private void visit(FunctionContext n)
        {

        }

        private void visit(StructContext n)
        {

        }

        private StatementList visit(Scope_bodyContext n) => visit((dynamic)n.children[1]);

        private StatementList visit(Statement_listContext n)
        {
            List<Statement> statements = new List<Statement>();
            for (int i = 0; i < n.children.Count; i++)
            {
                Statement node = visit((StatementContext)n.children[i]);

                if (node is StatementList)
                    statements.AddRange(((StatementList)node).Statements);
                else
                    statements.Add(node);
            }

            return new StatementList(statements, 0, 0);
        }


        private Statement visit(Single_statementContext n) => visit((dynamic)n.children[0]);

        private Statement visit(Base_statementContext n) => visit((dynamic)n.children[0]);

        private Assignment visit(AssignmentContext n)
        {
            var lhs = new List<AstNode>();
            var lastChild = n.children[n.children.Count - 1];
            Exp result = visit((ExpressionContext)lastChild);

            // state = 0 means start creating a new AssignLHS item,
            // state > 0 means add the nth terminal/non-terminal to the AssignLHS item being built
            int state = 0;
            bool isImmutable = false;
            AnyType type = new FlavorType();
            int lineNum = 0;
            int col = 0;

            for (int i = 0; i < n.children.Count-2; i++)
            {
                var child = n.children[i];

                if (child.Payload is IToken && ((IToken)child.Payload).Type == COMMA)
                    continue;

                if (state == 0)
                {
                    isImmutable = false;

                    if (child is TypeContext)
                    {
                        (type, lineNum, col) = visit((TypeContext)child);
                        state = 1;
                    }
                    else if (child is ExpressionContext)
                    {
                        lhs.Add(visit((ExpressionContext)child));
                        state = 0;
                    }
                    else if (child.Payload is IToken)
                    {
                        IToken token = (IToken)child.Payload;
                        lineNum = token.Line;
                        col = token.Column;

                        if (token.Type == IMMUTABLE)
                        {
                            isImmutable = true;
                            state = 1;
                        }
                        else if (token.Type == FLAVOR)
                        {
                            type = new FlavorType();
                            state = 1;
                        }
                        else if (token.Type == IDENTIFIER)
                        {
                            lhs.Add(new Identifier(token.Text, lineNum, col));
                            state = 0;
                        }
                    }
                }

                else if (state == 1)
                {
                    if (child is TypeContext)
                    {
                        type = visit((TypeContext)child).Item1;
                        state = 2;
                    }
                    else if (child.Payload is IToken)
                    {
                        IToken token = (IToken)child.Payload;
                        if (token.Type == FLAVOR)
                        {
                            type = new FlavorType();
                            state = 2;
                        }
                        else if (token.Type == IDENTIFIER)
                        {
                            lhs.Add(new AssignDeclLHS(isImmutable, type, token.Text, lineNum, col));
                            state = 0;
                        }
                    }
                }

                else if (state == 2)
                {
                    if (child.Payload is IToken)
                    {
                        IToken token = (IToken)child.Payload;
                        if (token.Type == IDENTIFIER)
                        {
                            lhs.Add(new AssignDeclLHS(isImmutable, type, token.Text, lineNum, col));
                            state = 0;
                        }
                    }
                }
            }

            int assignLineNum = lhs[0].LineNumber;
            int assignCol = lhs[0].StartCol;
            return new Assignment(lhs, result, assignLineNum, assignCol);
        }

        private Print visit(Print_statementContext n)
        {
            dynamic child = n.children[1];

            var token = (IToken)n.LEFT_PAREN().Payload;
            int lineNum = token.Line;
            int col = token.Column;
            bool useNewLine = n.PRINT().Length == 1;
           
            return new Print(visit(child), useNewLine, lineNum, col);
        }

        private Debug visit(Debug_statementContext n)
        {
            dynamic child = n.children[1];

            var token = (IToken)n.LEFT_PAREN().Payload;
            int lineNum = token.Line;
            int col = token.Column;
            bool useNewLine = n.DEBUG().Length == 1;

            return new Debug(visit(child), useNewLine, lineNum, col);
        }


        private Exp visit(ExpressionContext n)
        {
            if (n.children.Count == 1)
            {
                var loneChild = n.children[0];
                if (loneChild is IntContext)
                {
                    return visit((IntContext)loneChild);
                }
                else if (loneChild is DoubleContext)
                {
                    return visit((DoubleContext)loneChild);
                }
                else if (loneChild is BooleanContext)
                {
                    return visit((BooleanContext)loneChild);
                }
                else if (loneChild is IdentifierContext)
                {
                    return visit((IdentifierContext)loneChild);
                }
                else if (loneChild.Payload is IToken)
                {
                    var loneToken = (IToken)loneChild.Payload;
                    string text = loneToken.Text;
                    (int line, int col) = (loneToken.Line, loneToken.Column);
                    if (loneToken.Type == FLAVORLESS)
                    {
                        return new Flavorless(line, col);
                    }
                    else if (loneToken.Type == STRING_LITERAL)
                    {
                        return new StringLiteral(text.Substring(1, text.Length - 2), line, col);
                    }
                    else if (loneToken.Type == CHAR_LITERAL && text.Length == 3)
                    {
                        return new CharLiteral(text[1], line, col);
                    }
                    else
                        throw new Exception("Invalid type detected");
                }
                else
                    throw new Exception("Invalid type detected");
            }

            dynamic child = n.children[1];
            if (child.Payload is IToken)
            {
                IToken token = (IToken)child.Payload;
                if (token.Type == PLUS)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Plus(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == MINUS)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Minus(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == MULTIPLY)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Multiply(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == DIVIDE)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Divide(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else
                    throw new Exception("Invalid type detected");
            }
            else
                throw new Exception("Invalid type detected");
        }

        private IdentifierExp visit(IdentifierContext n)
        {
            int lineNum, col;
            IToken token = (IToken)n.children[0].Payload;
            lineNum = token.Line;
            col = token.Column;
            return new IdentifierExp(token.Text, lineNum, col);
        }

        private Bool visit(BooleanContext n)
        {
            IToken token = (IToken)n.children[0].Payload;
            bool value = token.Type == YUP;

            return new Bool(value, token.Line, token.Column);
        }

        // what about integer overflow??
        private Integer visit(IntContext n)
        {
            IToken token = (IToken)n.children[0].Payload;

            int integer = int.Parse(n.INTEGER_LITERAL().GetText());
            if (n.MINUS() != null)
                integer *= -1;

            return new Integer(integer, token.Line, token.Column);
        }

        private Double visit(DoubleContext n)
        {
            IToken token = (IToken)n.children[0].Payload;

            bool negative = n.MINUS() != null;
            double number;

            if (n.INTEGER_LITERAL().Length == 2)
            {
                number = double.Parse(n.INTEGER_LITERAL()[0].GetText() + "." + 
                    n.INTEGER_LITERAL()[1].GetText());
            }
            else
                number = int.Parse(n.INTEGER_LITERAL()[0].GetText());

            return new Double(number * (negative ? -1 : 1), token.Line, token.Column);
        }

        private (AnyType, int, int) visit(TypeContext n)
        {
            dynamic child = n.children[0];

            if (child.Payload is IToken)
            {
                IToken token = (IToken)child.Payload;
                return (new ObjectType(token.Text, false), token.Line, token.Column);
            }

            return visit(child);
        }

        private (AnyType, int, int) visit(ArrayContext n)
        {
            dynamic child = n.children[0];
            if (child is Primitive_packContext | child is Any_arrayContext)
                return visit(child);

            var token = (IToken)child.Payload;
            return (new ObjectType(token.Text, true), token.Line, token.Column);
        }


        private (AnyType, int, int) visit(TupleContext n)
        {
            int tupleLineNum = ((IToken)n.LEFT_ANGLE_BRACKET().Payload).Line;
            int tupleCol = ((IToken)n.LEFT_ANGLE_BRACKET().Payload).Column;

            int state = 0;
            AnyType type = new FlavorType();

            var typeNamePairs = new List<(AnyType, string)>();
            for (int i = 1; i < n.children.Count; i++)
            {
                var child = n.children[i];

                if (state == 0)
                {
                    if (child is TypeContext)
                    {
                        type = visit((TypeContext)child).Item1;
                        state = 1;
                    }
                    else if (child.Payload is IToken)
                    {
                        IToken token = (IToken)child.Payload;
                        if (token.Type == FLAVOR)
                        {
                            type = new FlavorType();
                            state = 1;
                        }
                    }
                }
                else if (state == 1)
                {
                    if (child.Payload is IToken)
                    {
                        IToken token = (IToken)child.Payload;
                        state = 0;
                        if (token.Type == IDENTIFIER)
                            typeNamePairs.Add((type, token.Text));
                        else if (token.Type == COMMA | token.Type == RIGHT_ANGLE_BRACKET)
                            typeNamePairs.Add((type, ""));
                    }
                }
            }

            return (new TupleType(typeNamePairs), tupleLineNum, tupleCol);

        }

        private (AnyType, int, int) visit(Any_arrayContext n)
        {
            int arrayLineNum = ((IToken)n.LEFT_SQUARE_BRACKET().Payload).Line;
            int arrayCol = ((IToken)n.LEFT_SQUARE_BRACKET().Payload).Column;
            
            int state = 0;
            AnyType type = new FlavorType();

            if (n.COMMA().Count() > 0)
            {
                var typeNamePairs = new List<(AnyType, string)>();
                for (int i = 1; i < n.children.Count; i++)
                {
                    var child = n.children[i];

                    if (state == 0)
                    {
                        if (child is TypeContext)
                        {
                            type = visit((TypeContext)child).Item1;
                            state = 1;
                        }
                        else if (child.Payload is IToken)
                        {
                            IToken token = (IToken)child.Payload;
                            if (token.Type == FLAVOR)
                            {
                                type = new FlavorType();
                                state = 1;
                            }
                        }
                    }
                    else if (state == 1)
                    {
                        if (child.Payload is IToken)
                        {
                            IToken token = (IToken)child.Payload;
                            state = 0;
                            if (token.Type == IDENTIFIER)
                                typeNamePairs.Add((type, token.Text));
                            else if (token.Type == COMMA | token.Type == RIGHT_SQUARE_BRACKET)
                                typeNamePairs.Add((type, ""));
                        }
                    }
                }

                return (new ArrayType(new TupleType(typeNamePairs)), arrayLineNum, arrayCol);
            }
            else
            {
                var child = n.children[1];
                if (child is TypeContext)
                {
                    type = visit((TypeContext)child).Item1;
                    return (new SingularArrayType(type), arrayLineNum, arrayCol);
                }
                else if (child.Payload is IToken)
                    return (new SingularArrayType(new FlavorType()), arrayLineNum, arrayCol);
                else
                    throw new Exception("Invalid type detected");
            }
        }

        private (AnyType, int, int) visit(PrimitiveContext n)
        {
            dynamic child = n.children[0];
            var token = (IToken)child.Payload;
            switch (token.Type)
            {
                case SUGAR:
                    return (new PrimitiveType(TypeBI.Sugar), token.Line, token.Column);
                case CARB:
                    return (new PrimitiveType(TypeBI.Carb), token.Line, token.Column);
                case CAL:
                    return (new PrimitiveType(TypeBI.Cal), token.Line, token.Column);
                case KCAL:
                    return (new PrimitiveType(TypeBI.Kcal), token.Line, token.Column);
                case YUM:
                    return (new PrimitiveType(TypeBI.Yum), token.Line, token.Column);
                case PURE:
                    return (new PrimitiveType(TypeBI.PureSugar), token.Line, token.Column);
                default:
                    throw new Exception("Invalid type detected");
            }
        }

        private (AnyType, int, int) visit(Primitive_packContext n)
        {
            dynamic child = n.children[0];
            var token = (IToken)child.Payload;
            switch (token.Type)
            {
                case SUGARPACK:
                    return (new PackType(TypePack.SugarPack), token.Line, token.Column);
                case CARBPACK:
                    return (new PackType(TypePack.CarbPack), token.Line, token.Column);
                case CALPACK:
                    return (new PackType(TypePack.CalPack), token.Line, token.Column);
                case KCALPACK:
                    return (new PackType(TypePack.KcalPack), token.Line, token.Column);
                case YUMPACK:
                    return (new PackType(TypePack.YumPack), token.Line, token.Column);
                case PURE:
                    return (new PackType(TypePack.PureSugarPack), token.Line, token.Column);
                default:
                    throw new Exception("Invalid type detected");
            }
        }
    }
}
