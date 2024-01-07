using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class RecipeTable : Info
    {
        public string Name;
        public Origin Origin;
        public List<RecipeFlavorInfo> Parameters;
        public Dictionary<string, FlavorInfo> Flavors;
        public List<RecipeFlavorInfo> Outputs;
        
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

       public RecipeTable(string name,List<RecipeFlavorInfo> parameters,
             Dictionary<string, FlavorInfo> flavors, 
            List<RecipeFlavorInfo> outputs, int line, int col)
        {
            Name = name;

            Parameters = parameters;
            Flavors = flavors;
            Outputs = outputs;
            
            LineNum = line;
            Column = col;
        }

    }

}
