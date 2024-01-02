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
using static BubblGumHeaderParser;

namespace AST
{
    public class CreateHeaderAST
    {
        public Program Visit(ProgramContext n)
        {
            var programPieces = new List<AstNode>();
            for (int i = 0; i < n.children.Count-1; i++)
            {
                AstNode node = null;
                var childi = n.children[i];

                if (childi is Define_stockContext)
                  node = visit((Define_stockContext)childi);
                else if (childi is Chew_importContext)
                  node = visit((Chew_importContext)childi);

                if (node != null)
                    programPieces.Add(node);
            }

            return new Program(programPieces, 0, 0);
        }

        private AstNode visit(Define_stockContext n)
        {
            IToken child0 = (IToken)n.children[0].Payload;

            var names = new List<string>();
            for (int i = 1; i < n.ChildCount; i++)
            {
                IToken childi = (IToken)n.children[i].Payload;
                if (childi.Type == IDENTIFIER)
                    names.Add(childi.Text);
            }

            return new Stock(names, child0.Line, child0.Column);
        }

        private AstNode visit(Chew_importContext n)
        {

            IToken child0 = (IToken)n.children[0].Payload;
            if (n.STRING_LITERAL() != null)
            {
                IToken child1 = (IToken)(n.children[1].Payload);
                string text = child1.Text;
                return new ChewPath(text.Substring(1, text.Length - 2), child0.Line, child0.Column);
            }

            var names = new List<string>();
            for (int i = 1; i < n.ChildCount; i++)
            {
                IToken childi = (IToken)n.children[i].Payload;
                if (childi.Type == IDENTIFIER)
                    names.Add(childi.Text);
            }
            return new ChewNames(names, child0.Line, child0.Column);
        }

    }
}
