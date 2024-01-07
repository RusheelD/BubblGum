using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class RecipeTable
    {
        public string Name;
        public Origin Origin;
        public List<FlavorInfo> Parameters;
        public Dictionary<string, FlavorInfo> Flavors;
        public List<FlavorInfo> Outputs;
        
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

       public RecipeTable(string name,List<FlavorInfo> parameters,
             Dictionary<string, FlavorInfo> flavors, 
            List<FlavorInfo> outputs, int line, int col)
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
