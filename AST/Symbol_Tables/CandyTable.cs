using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{

    /// <summary> Stores info about a struct's symbols </summary>   
    public class CandyTable : Info
    {
        public string Name;
        public Visbility Visibility;
        public Dictionary<string, FlavorInfo> Variables;
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

        public CandyTable(string name, Visbility visibility, int line, int column)
        {
            Variables = new();
            
            Name = name;
            Visibility = visibility;
            LineNum = line;
            Column = column;
        }
    }

}
