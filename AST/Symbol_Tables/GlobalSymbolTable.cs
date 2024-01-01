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


        public static string GenerateRecipeKey(string name, List<(bool, AnyType, string, bool)> inputs)
        {
            var sb = new StringBuilder();
            sb.Append(name);

            // unique name for primitive types (it's just a unique ascii value not part of identifiers)
            int size = Enum.GetNames(typeof(TypeBI)).Length;
            foreach (var input in inputs)
                append(input.Item2, input.Item4, size, sb, true);

            Console.WriteLine(sb);
            return sb.ToString();
        }

        private static char ellipses = '~';
        private static char seperator = '|';
        private static char weakSeparator = '\\';
        private static char pack = '}';
        private static char flavor = '{';
        private static char tuple = '`';
        private static char array = '^';

        private static void append(AnyType type, bool hasEllipses, int size, StringBuilder sb, bool outerElement)
        {
            if (type is PrimitiveType)
            {
                // starts with digit
                int typeInt = (int)((PrimitiveType)type).Type;
                sb.Append(typeInt);
            }
            else if (type is TupleType)
            {
                // starts with tuple char
                sb.Append(tuple);
                foreach (var typeInfo in ((TupleType)type).TypeNamePairs)
                    append(typeInfo.Item1, false, size, sb, false);
            }
            else if (type is ObjectType)
            {
                // starts with _ or letter
                var obj = (ObjectType)type;
                sb.Append(obj.Name);

                if (obj.IsPack)
                    sb.Append(pack);
            }
            else if (type is FlavorType)
            {
                // starts with flavor char
                sb.Append(flavor);
            }
            else if (type is SingularArrayType)
            {
                // [primitive] should output same code as primitive pack
                var arrayType = ((SingularArrayType)type).Type;
                if (arrayType is PrimitiveType)
                {
                    int typeInt = (int)((PrimitiveType)arrayType).Type;
                    sb.Append(typeInt + size);
                }
                // [object] should output same code as object pack
                else
                {
                    append(arrayType, false, size, sb, false);
                    sb.Append(pack);
                }
            }
            else if (type is PackType)
            {
                // starts with digit
                int typeInt = ((int)((PackType)type).Type);
                sb.Append(typeInt + size);
            }
            else if (type is ArrayType)
            {
                // start with array char
                sb.Append(array);
                append(((ArrayType)type).TupleType, false, size, sb, false);
            }

            if (hasEllipses)
                sb.Append(ellipses);

            sb.Append(outerElement ? seperator : weakSeparator);
        }
    }
}
