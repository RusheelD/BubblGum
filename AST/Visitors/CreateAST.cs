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
       public void Visit(ProgramContext n)
       {
            Console.WriteLine($"{n.children.Count}");
            for (int i = 0; i < n.children.Count-1; i++)
            {
                dynamic child = n.children[i];
                Visit(child);
            }
       }

       public void Visit(StatementContext n)
        {
            dynamic child = n.children[0];
            Visit(child);
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


        public void Visit(Base_statementContext n)
        {
            dynamic child = n.children[0];
            Visit(child);
        }

        public void Visit(AssignmentContext n)
        {
            Console.WriteLine($"assignment");
        }


        public void Visit(Print_statementContext n)
        {
            Console.WriteLine($"print");
            dynamic child = n.children[0];
           // Visit(child);
        }

        /*
       *    var payload = tree.Payload;
          Console.WriteLine(tree.Payload.GetType());

          if (payload is BubblGumParser.ExpressionContext)
          {
              var p = (BubblGumParser.ExpressionContext)payload;
              //if (p.expression() != null && p.expression().Count() > 0)

              if (p.PLUS() != null)
              {
                  Console.WriteLine($"{p.expression().Count()}");
              }
          }

       */
    }
}
