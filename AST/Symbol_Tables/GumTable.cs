using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    /// <summary> Stores info for a class' symbols </summary>
    public class GumTable : Info
    {
        public string Name;
        
        public Visbility Visibility;

        public bool IsStatic;

        public GumTable? ParentGum;
        public HashSet<WrapperTable> ParentWrappers;
        
        public Dictionary<string, GumRecipeTable> Recipes;

        public Dictionary<string, GumFlavorInfo> Flavors;
        
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

        public GumTable(Visbility visibility, bool isStatic,
            string name, int line, int col)
        {
            Visibility  = visibility;
            IsStatic = isStatic;
            Name = name;

            ParentWrappers = new();
            Recipes = new();
            Flavors = new();
            
            LineNum = line;
            Column = col;
        }
    }
}
