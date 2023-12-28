using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
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

            dynamic child = n.children[0];
            var programPieces = new List<ProgramPiece>() { Visit(child) };
            AstNode a = Visit(child);

            for (int i = 1; i < n.children.Count-1; i++)
            {
                child = n.children[i];
                programPieces.Add(Visit(child));
            }


            return new Program(programPieces, 0, 0);
       }

       public Statement Visit(StatementContext n)
        {
            dynamic child = n.children[0];
            Visit(child);

            return new Assignment(null, null, 0, 0);
        }

        public void Visit(ClassContext n)
        {

        }


        public void Visit(InterfaceContext n)
        {

        }

        public void Visit(FunctionContext n)
        {

        }

        public void Visit(StructContext n)
        {

        }

        public void Visit(Single_statementContext n)
        {
            dynamic child = n.children[0];
            Visit(child);
        }


        public Statement Visit(Base_statementContext n)
        {
            return new Assignment(null, null, 0, 0);
            dynamic child = n.children[0];
            Visit(child);
        }

        public Assignment Visit(AssignmentContext n)
        {
            return new Assignment(null, null, 0, 0);
        }


        public Print Visit(Print_statementContext n)
        {
            dynamic child = n.children[1];

            var token = (IToken)n.LEFT_PAREN().Payload;
            int lineNum = token.Line;
            int col = token.Column;
            bool useNewLine = (n.PRINT().Length == 1);
           
            Printable thing = Visit(child);

            return new Print(thing, useNewLine, lineNum, col);
        }
    }
}
