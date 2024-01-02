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
    public class GlobalSymbolTable
    {
        public Dictionary<string, WrapperTable> WrapperTables; // interfaces
        public Dictionary<string, GumTable> GumTables; // classes
        public Dictionary<string, CandyTable> CandyTables; // structs

        public Dictionary<string, RecipeTable> RecipeTables; // global methods
        public Dictionary<string, FlavorInfo> FlavorInfos; // global vars

        public GlobalSymbolTable()
        {
            WrapperTables = new();
            GumTables = new();
            CandyTables = new();
            RecipeTables = new();
            FlavorInfos = new();
        }
    }
}
