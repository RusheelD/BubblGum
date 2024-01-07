using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AST
{
    public static class RecipeKeys
    {
        private static char ellipses = '~';
        private static char seperator = '|';
        private static char weakSeparator = '\\';
        private static char pack = '}';
        private static char flavor = '{';
        private static char tuple = '`';
        private static char array = '^';

        // Requires the recipe/method name, and its list of input parameters
        // Returns a unique string representing that recipe/method
        public static string Generate(string name, List<(bool, AnyType, string, bool)> parameters)
        {
            var sb = new StringBuilder();
            sb.Append(name);

            int size = Enum.GetNames(typeof(TypeBI)).Length;
            foreach (var p in parameters)
                encodeVar(p.Item2, p.Item4, sb, size, true);

            return sb.ToString();
        }


        // Encodes the provided type (and whether it has ellipses) into the provided string builder
        // Requires the size of the TypeBI enum, and whether the var is an outer parameter (instead of nested inside a parameter)
        private static void encodeVar(AnyType type, bool hasEllipses, StringBuilder sb, int size, bool outerElement)
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
                    encodeVar(typeInfo.Item1, false, sb, size, false);
            }
            else if (type is ObjectType)
            {
                // starts with _ or letter
                var obj = (ObjectType)type;
                sb.Append(obj.Name);

                if (obj.IsPack)
                    sb.Append(pack);
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
                    encodeVar(arrayType, false, sb, size, false);
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
                encodeVar(((ArrayType)type).TupleType, false, sb, size, false);
            }
            else if (type is FlavorType)
            {
                // starts with flavor char
                sb.Append(flavor);
            }

            if (hasEllipses)
                sb.Append(ellipses);

            sb.Append(outerElement ? seperator : weakSeparator);
        }
    }

}
