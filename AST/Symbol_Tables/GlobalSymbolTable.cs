using System;
using System.Collections.Generic;
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

        string, List<AnyType>
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


       // public List<(bool, AnyType, string, bool)> Params; // isImmutable, type, name, isEllipses
       
        // biginteger or string is a better key (To be tested)
        private string generateRecipeKey(string name, List<(bool, AnyType, string, bool)> inputs)
        {
            var sb = new StringBuilder();
            sb.Append(name);

            // unique name for primitive types (it's just a unique ascii value not part of identifiers)
            int size = Enum.GetNames(typeof(TypeBI)).Length;
            foreach (var input in inputs)
            {
                if (input.Item2 is PrimitiveType)
                {
                    var type = ((PrimitiveType)(input.Item2)).Type;
                    sb.Append('\0' + type);;
                }
                else if (input.Item2 is PackType)
                {
                    var type = ((PackType)(input.Item2)).Type;
                    sb.Append('\0' + size + type);
                }
                else if (input.Item2 is ObjectType)
                {
                    var objName = ((ObjectType)(input.Item2)).Name;
                    sb.Append(objName);
                }

                if (input.Item4)
                    sb.Append('~');

                sb.Append("*");
            }

            return sb;
        }
    }

}
