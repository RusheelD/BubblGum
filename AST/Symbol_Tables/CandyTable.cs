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
        public Dictionary<string, FlavorInfo> Variables;
        public int LineNum {get; set;} = 0;
        public int Column {get; set;} = 0;

        public CandyTable(string name, int line, int column)
        {
            Variables = new();
            Name = name;
            LineNum = line;
            Column = column;
        }
    }

}
