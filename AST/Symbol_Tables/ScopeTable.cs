using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    /// <summary>
    /// Stores info for variables in the same scope. Contains a list of relatively nested scopes
    /// </summary>
    public class ScopeTable
    {
        public List<ScopeTable> NestedScopes;
        public Dictionary<string, FlavorInfo> Vars;

        public ScopeTable()
        {
            Vars = new();
            NestedScopes = new();
        }
    }
}
