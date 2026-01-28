# BubblGum Compiler - Quick Reference

## Building the Compiler

```bash
cd MainBBGM
dotnet build BubblGum.sln
```

## Running the Compiler

```bash
# Full compilation (semantic analysis + LLVM codegen)
dotnet run --project Main/Main.csproj run -A program.bbgm

# LLVM code generation only  
dotnet run --project Main/Main.csproj run -C program.bbgm

# Semantic analysis only
dotnet run --project Main/Main.csproj run -T program.bbgm

# Parse to AST only
dotnet run --project Main/Main.csproj run -P program.bbgm

# Show all imported files
dotnet run --project Main/Main.csproj run -H program.bbgm
```

## BubblGum Language Syntax

### Types
```bubblegum
sugar x     # 64-bit integer
carb y      # double-precision float
yum flag    # boolean
cal ch      # character (8-bit)
kcal str    # string (pointer)
```

### Variables
```bubblegum
sugar x :: 42                    # declare and initialize
sugar a, b, c                    # declare multiple
(sugar x, carb y) :: (1, 2.5)   # tuple-style declaration
```

### Functions
```bubblegum
recipe: functionName(sugar param1, carb param2) sugar {
    pop param1 + param2   # return statement
}

recipe: main() sugar {
    sugar result :: functionName(10, 3.14)
    pop 0
}
```

### Control Flow
```bubblegum
# If statement
a : x > 5 : {
    (x)!
}

# If-elif-else
a : x > 5 : {
    ("greater")!
} ae : x < 5 : {
    ("less")!  
} e : {
    ("equal")!
}

# While loop
a : x > 0 : {
    (x)!
    x :: x - 1
}

# Repeat loop (count up)
i: repeatUp(0, 10): {
    (i)!
}

# Repeat loop (count down)  
i: repeatDown(10, 0): {
    (i)!
}
```

### Operators
```bubblegum
# Arithmetic
x :: a + b    # addition
x :: a - b    # subtraction
x :: a * b    # multiplication
x :: a / b    # division
x :: a % b    # modulo

# Comparison
a < b         # less than
a <= b        # less than or equal
a > b         # greater than
a >= b        # greater than or equal
a <: b        # equal to
a :> b        # not equal to

# Logical
a and b       # logical AND
a or b        # logical OR
not a         # logical NOT
a xor b       # exclusive OR
a xnor b      # exclusive NOR

# Bitwise
a :< b        # left shift
a :> b        # right shift

# Assignment
x :: value    # assign
x +: amount   # increment by
x -: amount   # decrement by
```

### I/O
```bubblegum
(expression)!     # print with newline
(expression)!!    # print without newline
```

### Comments
```bubblegum
# This is a single-line comment
```

## Complete Example

```bubblegum
# Fibonacci calculator
recipe: fibonacci(sugar n) sugar {
    a : n <= 1 : {
        pop n
    }
    pop fibonacci(n - 1) + fibonacci(n - 2)
}

recipe: main() sugar {
    # Calculate first 10 Fibonacci numbers
    i: repeatUp(0, 10): {
        sugar result :: fibonacci(i)
        (result)!
    }
    pop 0
}
```

## Compiling to Executable

```bash
# Step 1: Generate LLVM IR
dotnet run --project Main/Main.csproj run -C fibonacci.bbgm
# Creates fibonacci.ll

# Step 2: Compile runtime library (one time)
cd CodeGeneration/Runtime
clang -c runtime.c -o runtime.o
cd ../..

# Step 3: Link and create executable
clang fibonacci.ll CodeGeneration/Runtime/runtime.o -o fibonacci

# Step 4: Run!
./fibonacci
```

## Project Structure

```
MainBBGM/
├── AST/                    # AST node definitions
├── Lexer_Parser/           # ANTLR grammar & generated parser
├── Preprocessing/          # Import resolution
├── BubblGum/              # Main compiler logic
├── CodeGeneration/        # LLVM backend
│   ├── Visitors/          # LLVMCodeGenerator
│   ├── TypeMapper.cs
│   ├── SymbolResolver.cs
│   └── Runtime/           # C runtime library
├── Main/                  # CLI entry point
└── Tests/                 # Unit tests
    ├── Lexer_Parser/
    ├── Code_Generation/
    └── ...
```

## Adding New Features

### To add a new operator:

1. Add to grammar (`Lexer_Parser/Parser/BubblGum.g4`)
2. Add AST node (`AST/NewOperator.cs`)
3. Update parser mapping (`Lexer_Parser/Parser/CreateAST.cs`)
4. Add type checking (`AST/Visitors/TypeCheckingPass.cs`)
5. Add code generation (`CodeGeneration/Visitors/LLVMCodeGenerator.cs`)

### To add a new statement type:

1. Add to grammar
2. Create AST node implementing `Statement` interface
3. Update CreateAST visitor
4. Add type checking logic
5. Add `Visit(NewStatement n)` method to LLVMCodeGenerator

## Testing

```bash
# Run all tests
dotnet test BubblGum.sln

# Run specific test class
dotnet test --filter BasicCodeGenTests

# Run specific test
dotnet test --filter TestSimpleFunction
```

## Troubleshooting

### "Unable to load DLL 'libLLVM'"
Install LLVM 20.x. See `LLVM_SETUP.md` for instructions.

### Compilation errors
Check `dotnet build BubblGum.sln` output for specific errors.

### Generated IR looks wrong
Check the AST structure matches what you expect in the Visit methods.

### Runtime crashes
Make sure runtime.o is linked when creating the executable.

## Resources

- **LLVM Documentation**: https://llvm.org/docs/
- **LLVMSharp**: https://github.com/dotnet/LLVMSharp
- **ANTLR**: https://www.antlr.org/

## Notes

- All BubblGum files must have `.bbgm` extension
- Main function should return `sugar` (int) with code 0 for success
- LLVM IR files have `.ll` extension
- Generated executables link with C runtime library

---

**Happy compiling! 🎉**
