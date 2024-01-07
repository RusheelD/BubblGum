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
    public class StockTable
    {
        public StockTable ParentTable;
        public string Name; // namespace name

        public Dictionary<string, WrapperTable> Interfaces; // interfaces
        public Dictionary<string, GumTable> Classes; // classes
        public Dictionary<string, CandyTable> Structs; // structs
        public Dictionary<string, RecipeTable> Functions; // global methods
        public Dictionary<string, FlavorInfo> Vars; // global vars


        public StockTable(string name)
        {
            Name = name;
            Interfaces = new();
            Classes = new();
            Structs = new();
            Functions = new();
            Vars = new();
        }
    }
}
