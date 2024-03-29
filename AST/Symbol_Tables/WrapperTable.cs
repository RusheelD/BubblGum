﻿using System;
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
        public Dictionary<string, Dictionary<string, GumRecipeTable>> Functions; // key = recipe name, second_key = recipe key
        public Dictionary<string, GumFlavorInfo> Flavors; 
        
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

        public WrapperTable(Visbility visibility, bool isStatic, string name, int line, int col)
        {
            Visibility  = visibility;
            IsStatic = isStatic;
            Name = name;

            Functions = new();
            Flavors = new();
            
            LineNum = line;
            Column = col;
        }
    }

}
