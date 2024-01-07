using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public class GumRecipeTable : RecipeTable
    {
        public Visbility Visibility;
        public bool IsSticky;

        public GumRecipeTable ( Visbility visibility, bool isSticky,
            string name,List<RecipeFlavorInfo> parameters,
             Dictionary<string, FlavorInfo> flavors, 
            List<RecipeFlavorInfo> outputs, int line, int col) 
        : base(name, parameters, flavors, outputs, line, col )
        {
            Visibility = visibility;
            IsSticky = isSticky;
        }

    }

}
