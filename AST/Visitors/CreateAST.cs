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

        private Struct visit(StructContext n)
        {
            IToken candy = (IToken)n.children[0];
            IToken identifier = (IToken)n.children[2];
            string name = identifier.Text;
            int lineNumber = candy.Line;
            int startCol = candy.Column;
            var statements = new List<AstNode>();
            
            for(int i = 4; i < n.ChildCount; i++)
            {
                dynamic childi = n.children[i];
                if(childi is Primitive_declarationContext || childi is AssignmentContext)
                {
                    statements.Add(visit(childi));
                }
            }

            return new Struct(name, statements, lineNumber, startCol);
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

        private Statement visit(Primitive_declarationContext n)
        {
            (AnyType type, int line, int col) = visit((PrimitiveContext)n.children[0]);

            if (n.primitive().Length == 1)
            {
                TypeBI primitiveType = ((PrimitiveType)type).Type;
                var variables = new List<string>();

                for (int i = 3; i < n.children.Count; i++)
                {
                    dynamic childi = n.children[i];
                    if (childi is IToken)
                    {
                        IToken tokeni = (IToken)childi.Payload;
                        if (tokeni.Type == IDENTIFIER)
                            variables.Add(tokeni.Text);
                    }
                }

                return new PrimitiveDeclaration1(primitiveType, variables, line, col);
            }
            else
            {
                var variables = new List<(TypeBI, string)>();
                TypeBI primitiveType = 0;
                for (int i = 0; i < n.children.Count; i++)
                {
                    dynamic childi = n.children[i];
                    if (childi is PrimitiveContext)
                    {
                        AnyType typei = visit((PrimitiveContext)n.children[i]).Item1;
                        primitiveType = ((PrimitiveType)typei).Type;
                    }
                    else if (childi is IToken)
                    {
                        IToken tokeni = (IToken)childi.Payload;
                        if (tokeni.Type == IDENTIFIER)
                            variables.Add((primitiveType, tokeni.Text));
                    }
                }

                return new PrimitiveDeclaration2(variables, line, col);
            }
        }

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

        private Statement visit(Variable_inc_decContext n)
        {
            bool shouldIncrement = n.PLUS_COLON() != null;
            Exp e1 = visit((dynamic)n.children[0]);
            Exp e2 = visit((dynamic)n.children[2]);
            return new IncDec(e1, e2, shouldIncrement, e1.LineNumber, e1.StartCol);
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

            dynamic child1 = n.children[1];

            if (child1.Payload is IToken)
            {
                IToken token = (IToken)child1.Payload;
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
                else if (token.Type == LEFT_SHIFT)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new LeftShift(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == RIGHT_SHIFT)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new RightShift(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == POWER)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Power(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == MODULO)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Modulo(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == AND | token.Type == AND_OP)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new And(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == OR | token.Type == OR_OP)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Or(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == XOR | token.Type == XOR_OP)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Xor(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == EQUALS)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Equals(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == NOT_EQ_1 | token.Type == NOT_EQ_2)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new NotEquals(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == GT_EQ)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new GreaterThanEquals(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == LT_EQ)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new LessThanEquals(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == RIGHT_ANGLE_BRACKET)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new GreaterThan(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == LEFT_ANGLE_BRACKET)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new LessThan(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == IS)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new Is(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == SUBCLASS_OF)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new SubClassOf(e1, e2, e1.LineNumber, e1.StartCol);
                }
                else if (token.Type == THIN_ARROW)
                {
                    dynamic child0 = n.children[0];
                    dynamic child2 = n.children[2];

                    if (child0.Payload is IToken)
                    {
                        IToken token0 = (IToken)child0.Payload;
                        return new GlobalAccess(visit(child2), token0.Line, token0.Column);
                    }
                    else if (child2 is ExpressionContext)
                    {
                        Exp e1 = visit(child0);
                        return new MemberAccess(e1, visit(child2), e1.LineNumber, e1.StartCol);
                    }
                    else if (child2.Payload is IToken)
                    {
                        IToken token2 = (IToken)child2.Payload;
                        Exp e1 = visit(child0);
                        if (token2.Type == SIZE)
                            return new PackSize(e1, e1.LineNumber, e1.StartCol);
                        else if (token2.Type == EMPTY)
                            return new ObjectEmpty(e1, e1.LineNumber, e1.StartCol);
                    }
                }
                else if (token.Type == LEFT_PAREN)
                {
                    dynamic child0 = n.children[0];

                    if (child0 is ExpressionContext)
                    {
                        Exp lhs = visit((ExpressionContext)child0);
                        var args = new List<Exp>();
                        for (int i = 2; i < n.children.Count; i++)
                        {
                            dynamic childi = n.children[i];
                            if (childi is ExpressionContext)
                                args.Add(visit(childi));
                        }

                        return new MethodCall(lhs, args, lhs.LineNumber, lhs.StartCol);
                    }
                    else if (child0 is ArrayContext)
                    {
                        (AnyType type, int line, int col) = visit((ArrayContext)child0);
                        Exp e1 = visit((ExpressionContext)n.children[2]);
                        return new NewPack(type, e1, line, col);
                    }
                }
                else if (token.Type == LEFT_SQUARE_BRACKET)
                {
                    Exp e1 = visit((dynamic)n.children[0]);
                    Exp e2 = visit((dynamic)n.children[2]);
                    return new PackAccess(e1, e2, e1.LineNumber, e1.StartCol);
                }
            }
            else if (child1 is ExpressionContext)
            {
                dynamic child0 = n.children[0];

                if (child0.Payload is IToken)
                {
                    IToken token0 = (IToken)child0.Payload;

                    if (token0.Type == LEFT_PAREN)
                        return visit(child1);
                    else if (token0.Type == LEFT_ANGLE_BRACKET)
                    {
                        var exps = new List<Exp>();
                        for (int i = 1; i < n.children.Count; i++)
                        {
                            dynamic childi = n.children[i];
                            if (childi is ExpressionContext)
                                exps.Add(visit(childi));
                        }
                        return new NewTuple(exps, token0.Line, token0.Column);
                    }
                    else if (token0.Type == NOT | token0.Type == NOT_OP)
                        return new Not(visit(child1), token0.Line, token0.Column);
                }
            }

           throw new Exception($"Invalid type {child1.GetType()} detected");
        }

        private IdentifierExp visit(IdentifierContext n)
        {
            int lineNum, col;
            IToken token = (IToken)n.children[0].Payload;
            lineNum = token.Line;
            col = token.Column;
            return new IdentifierExp(token.Text, lineNum, col);
        }

        private Statement visit(Return_statementContext n)
        {
            IToken pop = (IToken)n.children[0].Payload;
            if(n.ChildCount == 1)
            {
                return new Pop(pop.Line, pop.Column);
            }

            Exp var = visit((dynamic)n.children[1]);

            if(n.POPSTREAM() != null)
            {
                IToken lastToken = (IToken)(n.children[n.ChildCount - 1].Payload);
                if (lastToken.Type == POPSTREAM)
                {
                    return new PopStream(var, false, new Integer(0, lastToken.Line, lastToken.Column), pop.Line, pop.Column);
                } else if (lastToken.Type == RIGHT_PAREN)
                {
                    Exp outputIdx = visit((dynamic)n.children[n.ChildCount - 2]);
                    return new PopStream(var, true, outputIdx, pop.Line, pop.Column);
                }
            }

            if(n.THICK_ARROW() != null)
            {
                Exp outputIdx = visit((dynamic)n.children[n.ChildCount - 1]);
                return new PopVar(var, true, outputIdx, pop.Line, pop.Column);
            }

            return new PopVar(var, false, new Integer(0, var.LineNumber, var.StartCol), pop.Line, pop.Column);
        }

        private Statement visit(If_statementContext n)
        {
            dynamic child = n.children[n.ChildCount - 1];
            Exp cond = visit((dynamic)n.children[1]);
            IToken If = (IToken)n.children[0].Payload;
            var statements = new List<Statement>();

            if(child is Single_statementContext || child is Scope_bodyContext)
            {
                Statement statement = visit(child);
                if (statement is StatementList)
                    statements.AddRange(((StatementList)statement).Statements);
                else
                    statements.Add(statement);
                return new SingleIf(cond, statements, If.Line, If.Column);
            } else
            {
                var elifs = new List<(Exp, List<Statement>)>();
                var Else = new List<Statement>();

                for (int i = 2; i < n.ChildCount; i++)
                {
                    dynamic childi = n.children[i];

                    if (childi is Single_statementContext || childi is Scope_bodyContext)
                    {
                        Statement statement = visit(childi);
                        if (statement is StatementList)
                            statements.AddRange(((StatementList)statement).Statements);
                        else
                            statements.Add(statement);
                    }
                    else if(childi is Elif_statementContext)
                        elifs.Add(visit(childi));
                    else if(childi is Else_statementContext)
                        Else.AddRange(visit(childi));
                    else
                        throw new Exception($"Invalid type {childi.GetType()} detected");
                }

                return new MultiIf(cond, statements, elifs, Else, If.Line, If.Column);
            }

            
        }
        private (Exp, List<Statement>) visit(Elif_statementContext n)
        {
            dynamic child = n.children[n.ChildCount - 1];
            Exp cond = visit((dynamic)n.children[1]);
            var statements = new List<Statement>();

            Statement statement = visit(child);
            if (statement is StatementList)
                statements.AddRange(((StatementList)statement).Statements);
            else
                statements.Add(statement);
            return (cond, statements);
        }
        private List<Statement> visit(Else_statementContext n)
        {
            dynamic child = n.children[n.ChildCount - 1];
            var statements = new List<Statement>();

            Statement statement = visit(child);
            if (statement is StatementList)
                statements.AddRange(((StatementList)statement).Statements);
            else
                statements.Add(statement);
            return statements;
        }

        private Statement visit(LoopContext n)
        {
            return visit((dynamic)n.children[0]);
        }
        private Statement visit(While_loopContext n)
        {
            Exp condition;
            List<Statement> statements = new List<Statement>();
            int lineNumber, startCol;

            IToken identifier = (IToken)n.children[0].Payload;
            lineNumber = identifier.Line;
            startCol = identifier.Column;
            condition = visit((dynamic)n.children[1]);
            Statement stat = visit((dynamic)n.children[n.ChildCount - 1]);
            if (stat is StatementList)
                statements.AddRange(((StatementList)stat).Statements);
            else
                statements.Add(stat);

            return new While(condition, statements, lineNumber, startCol);
        }
        private Statement visit(Repeat_loopContext n)
        {
            string varName;
            bool isUp;
            Exp start, end;
            List<Statement> statements = new List<Statement>();
            int lineNumber, startCol;

            IToken identifier = (IToken)n.children[0].Payload;
            IToken repeat = (IToken)n.children[2].Payload;

            varName = identifier.Text;
            lineNumber = identifier.Line; 
            startCol = identifier.Column;
            isUp = repeat.Type == REPEAT_UP;
            start = visit((dynamic)n.children[4]);
            end = visit((dynamic)n.children[6]);
            Statement stat = visit((dynamic)n.children[n.ChildCount - 1]);
            if (stat is StatementList)
                statements.AddRange(((StatementList)stat).Statements);
            else
                statements.Add(stat);

            return new RepeatLoop(varName, isUp, start, end, statements, lineNumber, startCol);
        }
        private Statement visit(Pop_loopContext n)
        {
            string varName;
            Exp exp;
            List<Statement> statements = new List<Statement>();
            int lineNumber, startCol;

            IToken pop = (IToken)n.children[0].Payload;
            IToken identifier = (IToken)n.children[2].Payload;

            varName = identifier.Text;
            lineNumber = pop.Line;
            startCol = pop.Column;
            exp = visit((dynamic)n.children[4]);
            Statement stat = visit((dynamic)n.children[n.ChildCount - 1]);
            if (stat is StatementList)
                statements.AddRange(((StatementList)stat).Statements);
            else
                statements.Add(stat);

            return new PopLoop(varName, exp, statements, lineNumber, startCol);
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
