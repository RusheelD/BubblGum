using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    /// <summary> Stores info for an interface's symbols </summary>
    public class WrapperTable : Info
    {
        public string Name;
        public Visbility Visibility;
        public bool IsStatic;
        public Dictionary<string, GumRecipeTable> Recipes; 
        public Dictionary<string, GumFlavorInfo> Flavors; 
        
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

        public WrapperTable(Visbility visibility, bool isStatic, string name,
            Dictionary<string, GumRecipeTable> recipes, 
            Dictionary<string, GumFlavorInfo> flavors, int line, int col)
        {
            Visibility  = visibility;
            IsStatic = isStatic;
            Name = name;

            Recipes = recipes;
            Flavors = flavors;
            
            LineNum = line;
            Column = col;
        }
    }

}
