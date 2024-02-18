using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    /// <summary>
    /// Stores symbol info for a standalone function (non-object oriented development)
    /// </summary>
    public class RecipeTable : Info
    {
        public string Name;
        public Origin Origin;
        
        public List<RecipeFlavorInfo> Parameters;
        public ScopeTable OutermostScope;
        public List<RecipeFlavorInfo> Outputs;
        
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

       public RecipeTable(string name, List<RecipeFlavorInfo> parameters,
            List<RecipeFlavorInfo> outputs, int line, int col)
        {
            Name = name;

            Parameters = parameters;
            OutermostScope = new();
            Outputs = outputs;
            
            LineNum = line;
            Column = col;
        }

    }

}
