using AST;
using LLVMSharp.Interop;

namespace CodeGeneration
{
    public class TypeMapper
    {
        private readonly LLVMContextRef context;

        public TypeMapper(LLVMContextRef context)
        {
            this.context = context;
        }

        /// <summary>
        /// Maps a BubblGum AnyType to an LLVM type
        /// </summary>
        public LLVMTypeRef MapType(AnyType type)
        {
            if (type is PrimitiveType primType)
            {
                return MapPrimitiveType(primType.Type);
            }
            else if (type is ArrayType arrayType)
            {
                // Arrays are represented as pointers to structs containing length + data
                // struct { i64 length; T* data; }
                var elementType = MapType(arrayType.TupleType.TypeNamePairs[0].Item1);
                return LLVMTypeRef.CreateStruct(new[]
                {
                    LLVMTypeRef.Int64,  // length
                    LLVMTypeRef.CreatePointer(elementType, 0)  // data
                }, false);
            }
            else if (type is TupleType tupleType)
            {
                // Tuples are represented as LLVM structs
                var fieldTypes = new LLVMTypeRef[tupleType.TypeNamePairs.Count];
                for (int i = 0; i < tupleType.TypeNamePairs.Count; i++)
                {
                    fieldTypes[i] = MapType(tupleType.TypeNamePairs[i].Item1);
                }
                return LLVMTypeRef.CreateStruct(fieldTypes, false);
            }
            else if (type is ObjectType || type is NamespaceObjectType)
            {
                // Classes, structs, and interfaces are represented as opaque pointers
                // We'll resolve the actual structure later during class generation
                return LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
            }
            else if (type is FlavorType || type is FlavorpackType)
            {
                // Flavorless (null/void) types
                return LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
            }
            else
            {
                // Unknown type - treat as opaque pointer
                return LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0);
            }
        }

        /// <summary>
        /// Maps a BubblGum primitive type to an LLVM type
        /// </summary>
        private LLVMTypeRef MapPrimitiveType(TypeBI typeBI)
        {
            return typeBI switch
            {
                TypeBI.Sugar => LLVMTypeRef.Int64,      // int64
                TypeBI.Carb => LLVMTypeRef.Double,      // double
                TypeBI.Cal => LLVMTypeRef.Int8,         // char
                TypeBI.Kcal => LLVMTypeRef.CreatePointer(LLVMTypeRef.Int8, 0),  // string (char*)
                TypeBI.Yum => LLVMTypeRef.Int1,         // bool
                TypeBI.PureSugar => LLVMTypeRef.Int64,  // pure sugar (also int64)
                _ => LLVMTypeRef.Void
            };
        }

        /// <summary>
        /// Gets the default value for a given LLVM type
        /// </summary>
        public LLVMValueRef GetDefaultValue(LLVMTypeRef type)
        {
            if (type.Kind == LLVMTypeKind.LLVMIntegerTypeKind)
            {
                return LLVMValueRef.CreateConstInt(type, 0, false);
            }
            else if (type.Kind == LLVMTypeKind.LLVMDoubleTypeKind)
            {
                return LLVMValueRef.CreateConstReal(type, 0.0);
            }
            else if (type.Kind == LLVMTypeKind.LLVMPointerTypeKind)
            {
                return LLVMValueRef.CreateConstPointerNull(type);
            }
            else if (type.Kind == LLVMTypeKind.LLVMStructTypeKind)
            {
                return LLVMValueRef.CreateConstNull(type);
            }
            else
            {
                return LLVMValueRef.CreateConstNull(type);
            }
        }

        /// <summary>
        /// Checks if a type is numeric (for arithmetic operations)
        /// </summary>
        public bool IsNumericType(LLVMTypeRef type)
        {
            return type.Kind == LLVMTypeKind.LLVMIntegerTypeKind ||
                   type.Kind == LLVMTypeKind.LLVMDoubleTypeKind;
        }

        /// <summary>
        /// Checks if a type is an integer type
        /// </summary>
        public bool IsIntegerType(LLVMTypeRef type)
        {
            return type.Kind == LLVMTypeKind.LLVMIntegerTypeKind;
        }

        /// <summary>
        /// Checks if a type is a floating-point type
        /// </summary>
        public bool IsFloatType(LLVMTypeRef type)
        {
            return type.Kind == LLVMTypeKind.LLVMDoubleTypeKind;
        }
    }
}
