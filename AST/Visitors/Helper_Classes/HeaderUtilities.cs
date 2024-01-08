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
    public static class HeaderUtilities
    {
        public static string GetNamespaceName(List<string> names) {
            if (names.Count == 0)
                return "";
            
            var sb = new StringBuilder(names[0]);
            for (int i = 1; i < names.Count; i++) {
                sb.Append("->");
                sb.Append(names[i]);
            }

            return sb.ToString();
        }
    }
}
