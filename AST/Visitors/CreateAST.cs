﻿using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
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
    public class CreateAST
    {

       public Program Visit(ProgramContext n)
        {
            Console.WriteLine($"{n.children.Count}");

            var programPieces = new List<ProgramPiece>();
            for (int i = 0; i < n.children.Count-1; i++)
            {
                AstNode node = visit((dynamic)n.children[i]);

                if (node is StatementList)
                    programPieces.AddRange(((StatementList)node).Statements);
                else if (node is ProgramPiece)
                    programPieces.Add((ProgramPiece)node);
                else
                    Console.Error.WriteLine($"node of type {node.GetType()} isn't handled");
            }

            return new Program(programPieces, 0, 0);
       }

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
                AstNode node = visit((StatementContext)n.children[i]);

                if (node is StatementList)
                    statements.AddRange(((StatementList)node).Statements);
                else if (node is Statement)
                    statements.Add((Statement)node);
                else
                    Console.Error.WriteLine($"node of type {node.GetType()} isn't handled");
            }

            return new StatementList(statements, 0, 0);
        }


        private Statement visit(Single_statementContext n) => visit((dynamic)n.children[0]);

        private Statement visit(Base_statementContext n) => visit((dynamic)n.children[0]);

        private Assignment visit(AssignmentContext n)
        {
            var lhs = new List<AssignLHS>();
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
                    else if (child.ChildCount == 0)
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
                    else if (child.ChildCount == 0)
                    {
                        IToken token = (IToken)child.Payload;
                        if (token.Type == FLAVOR)
                        {
                            type = new FlavorType();
                            state = 2;
                        }
                        else if (token.Type == IDENTIFIER)
                        {
                            lhs.Add(new Identifier(token.Text, lineNum, col));
                            state = 0;
                        }
                    }
                }

                else if (state == 2)
                {
                    if (child.ChildCount == 0)
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

            return new Assignment(lhs, result, 0, 0);
        }

        private Print visit(Print_statementContext n)
        {
            dynamic child = n.children[1];

            var token = (IToken)n.LEFT_PAREN().Payload;
            int lineNum = token.Line;
            int col = token.Column;
            bool useNewLine = (n.PRINT().Length == 1);
           
            Printable thing = visit(child);

            return new Print(thing, useNewLine, lineNum, col);
        }

        private Exp visit(ExpressionContext n)
        {
            return null;
        }

        private (AnyType, int, int) visit(TypeContext n)
        {
            return (null, 0, 0);
        }
    }
}
