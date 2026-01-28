# BubblGum Compiler - Usage Guide

## 🚀 Getting Started

### Prerequisites
- .NET 8.0 SDK (for building the compiler)
- .NET 8.0 Runtime (for running the compiler, if not using standalone build)
- LLVM (optional, for generating native executables)
  - Windows: `choco install llvm`
  - macOS: `brew install llvm`
  - Linux: `sudo apt-get install llvm`

---

## 🔨 Building the Compiler

### Option 1: Quick Build (For Development)

```bash
cd C:\code\github\Bubblegum\MainBBGM
dotnet build BubblGum.sln
```

### Option 2: Standalone Executable (Recommended for Distribution)

**Windows:**
```bash
.\build.bat
```

**Linux/macOS:**
```bash
chmod +x build.sh
./build.sh
```

This creates a standalone `bbgm.exe` (Windows) or `bbgm` (Linux/macOS) executable in `bin/Release/`.

### Installing System-Wide

**Windows:**
```bash
# Add to PATH or copy to a directory in PATH
copy bin\Release\bbgm.exe C:\Windows\System32\
```

**Linux/macOS:**
```bash
sudo cp bin/Release/bbgm /usr/local/bin/
```

---

## 📝 Using the Compiler

### Simple Usage

```bash
# Compile a BubblGum program
bbgm hello.bbgm

# Or explicitly use compile command
bbgm compile hello.bbgm
```

**Output:**
- `hello.ll` - LLVM intermediate representation  
- `hello.exe` - Native executable (if LLVM tools are installed)

### Running Your Program

```bash
# Run the compiled executable
./hello.exe
# or on Linux/macOS
./hello
```

---

## 📚 Writing Your First BubblGum Program

### Hello World

Create `hello.bbgm`:

```bubblegum
("Hello, World!") !
```

Compile and run:
```bash
bbgm hello.bbgm
./hello.exe
```

**Output:**
```
Hello, World!
```

---

## 🎯 Command-Line Reference

### Basic Commands

```bash
# Compile source file to executable
bbgm <file.bbgm>
bbgm compile <file.bbgm>
bbgm build <file.bbgm>

# Show help
bbgm --help
bbgm -h

# Show version
bbgm --version
bbgm -v
```

### Advanced Commands (Developer Mode)

```bash
# Full compilation (LLVM IR + executable)
bbgm run -C yourfile.bbgm

# Parse only (generate AST)
bbgm run -P yourfile.bbgm

# Semantic analysis only
bbgm run -T yourfile.bbgm

# Show all files used
bbgm run -H yourfile.bbgm
```

### Examples

```bash
# Simple compilation
bbgm hello.bbgm

# Compile with full output
bbgm compile myprogram.bbgm

# Get help
bbgm --help
```

---

## 🧪 Testing with Example Programs

### Quick Test

```bash
# Compile and run Hello World
bbgm Tests\Lexer_Parser\HelloWorld.bbgm
./Tests/Lexer_Parser/HelloWorld.exe
```

### Available Example Files

Located in `Tests/Lexer_Parser/`:
- `HelloWorld.bbgm` - Basic hello world
- `SimpleFunctions.bbgm` - Function examples
- `If.bbgm` - Conditional statements
- `Loops.bbgm` - All loop types
- `Arrays.bbgm` - Array operations
- `Classes.bbgm` - OOP examples
- `Operators.bbgm` - All operators
- `PrintDebug.bbgm` - Print/Debug statements

**Try them:**
```bash
bbgm Tests\Lexer_Parser\SimpleFunctions.bbgm
./Tests/Lexer_Parser/SimpleFunctions.exe
```

---

## 🔧 Troubleshooting

### Executable Not Generated

If you only get `.ll` files but no `.exe`:

**Cause:** LLVM tools (`llc`, `clang`) not installed or not in PATH

**Solution:**
```bash
# Windows
choco install llvm

# macOS
brew install llvm

# Linux
sudo apt-get install llvm clang

# Verify installation
llc --version
clang --version
```

### Cannot Run Executable

**Windows:** Make sure antivirus isn't blocking the .exe

**Linux/macOS:** Make executable:
```bash
chmod +x yourprogram
./yourprogram
```

### Build Errors

```bash
# Clean and rebuild
dotnet clean BubblGum.sln
dotnet build BubblGum.sln
```

---

## 📦 Distribution

### Sharing Your Compiler

1. **Build standalone executable:**
   ```bash
   ./build.bat  # Windows
   ./build.sh   # Linux/macOS
   ```

2. **Package files:**
   - `bin/Release/bbgm.exe` (or `bbgm` on Linux)
   - `README.md`
   - Example programs (optional)

3. **Users need:**
   - .NET 8.0 Runtime (download from microsoft.com/dotnet)
   - LLVM (for executable generation)

### Sharing Compiled Programs

Your compiled BubblGum programs are **native executables** - users don't need BubblGum or .NET to run them!

```bash
# Just distribute the .exe
hello.exe  # Runs anywhere!
```

---

## 🎯 Command-Line Usage

### Basic Commands

---

## 📚 Language Syntax Quick Reference

### Data Types

```bubblegum
sugar x :: 42              # int64
carb pi :: 3.14           # double
yum isTrue :: true        # bool
cal letter :: 'A'         # char
kcal name :: "BubblGum"   # string
```

### Functions (Recipes)

```bubblegum
# Simple function
recipe: greet(kcal name)
    ("Hello, ") !
    (name) !
pop

# Function with return value
recipe: add(sugar a, sugar b) sugar
    pop a + b
pop

# Multiple return values
recipe: divmod(sugar a, sugar b) <sugar quotient, sugar remainder>
    pop a / b => quotient
    pop a % b => remainder
pop
```

### Control Flow

#### If Statements

```bubblegum
if (x > 0):
    ("Positive") !
elif (x < 0):
    ("Negative") !
else:
    ("Zero") !
pop
```

#### While Loops

```bubblegum
sugar i :: 0
while (i < 10) {
    (i) !
    i +: 1
}
```

#### Repeat Loops (For Loops)

```bubblegum
# Count up from 0 to 9
i :: repeatUp(0, 10):
    (i) !
pop

# Count down from 10 to 1
i :: repeatDown(10, 1):
    (i) !
pop
```

#### Foreach Loops

```bubblegum
[sugar] numbers :: [1, 2, 3, 4, 5]
pop num from numbers {
    (num * 2) !
}
```

### Arrays

```bubblegum
# Array declaration
[sugar] arr :: [1, 2, 3, 4, 5]

# Array access
sugar first :: arr[0]

# Array type notation
sugarpack nums :: [10, 20, 30]  # sugarpack = [sugar]
```

### Tuples

```bubblegum
# Tuple creation
<sugar, carb, kcal> data :: <42, 3.14, "hello">

# Tuple unpacking (if implemented)
sugar x, carb y, kcal z :: data
```

### Classes (Gum)

```bubblegum
Gum Person {
    bold kcal name
    bold sugar age
    
    # Constructor-like method
    recipe: init(kcal n, sugar a)
        gum->name :: n    # 'gum' is 'this' pointer
        gum->age :: a
    pop
    
    # Method
    recipe: greet()
        ("Hello, I'm ") !
        (gum->name) !
    pop
}
pop

# Create instance (syntax depends on implementation)
Person p :: Person()
p->init("Alice", 30)
p->greet()
```

### Static Members

```bubblegum
Gum Counter {
    sticky bold sugar count :: 0    # Static member
    
    recipe: increment()
        Counter.count +: 1          # Access via ClassName.member
    pop
}
pop
```

### Immutable Variables

```bubblegum
$sugar pi :: 3.14159
# pi :: 3.14  # ERROR! Cannot reassign immutable variable
```

### Variadic Functions

```bubblegum
# Function accepting variable arguments
recipe: sum(sugar count, sugar nums...)
    # Implementation would use va_start/va_arg
    # LLVM handles va_list intrinsics
pop
```

### Operators

#### Arithmetic
- `+` Addition
- `-` Subtraction
- `*` Multiplication
- `/` Division
- `%` Modulo
- `**` Power

#### Comparison
- `=` Equal
- `<>` or `~=` Not equal
- `<` Less than
- `<=` Less than or equal
- `>` Greater than
- `>=` Greater than or equal

#### Logical
- `and` or `&` Logical AND
- `or` or `|` Logical OR
- `not` or `~` Logical NOT
- `xor` or `^` XOR
- `xnor` XNOR

#### Bitwise
- `<:` Left shift
- `:>` Right shift
- `&` Bitwise AND
- `|` Bitwise OR
- `^` Bitwise XOR

#### Special
- `::` Assignment
- `+:` Increment by
- `-:` Decrement by
- `->` Member access
- `=>` Type cast or output assignment
- `is` Identity check
- `:<` Subclass check

### Print and Debug

```bubblegum
# Print with newline
("Hello") !

# Print without newline
("Hello") !!

# Debug to stderr
(x) ?       # Debug with newline
(x) ??      # Debug without newline
```

### Comments

```bubblegum
# This is a single-line comment

# Multi-line comments use multiple # signs
# Like this
# And this
```

---

## 🗂️ Project Structure

### Using Imports

Create a config file or use the `chew` keyword:

```bubblegum
# Import a namespace
chew MyLibrary

# Use imported functions
MyLibrary.someFunction()
```

### Multiple Files

The compiler supports multi-file projects via `stock` declarations and `chew` imports (see `Imports.bbgm` for examples).

---

## 🔨 Example Programs

### Example 1: FizzBuzz

```bubblegum
recipe: fizzbuzz(sugar n)
    i :: repeatUp(1, n + 1) {
        if (i % 15 = 0):
            ("FizzBuzz") !
        elif (i % 3 = 0):
            ("Fizz") !
        elif (i % 5 = 0):
            ("Buzz") !
        else:
            (i) !
        pop
    }
pop

fizzbuzz(100)
```

### Example 2: Factorial

```bubblegum
recipe: factorial(sugar n) sugar
    if (n <= 1):
        pop 1
    else:
        pop n * factorial(n - 1)
    pop
pop

sugar result :: factorial(5)
(result) !  # Prints 120
```

### Example 3: Sum Array

```bubblegum
recipe: sumArray([sugar] arr) sugar
    sugar total :: 0
    pop num from arr {
        total +: num
    }
    pop total
pop

[sugar] numbers :: [1, 2, 3, 4, 5]
sugar sum :: sumArray(numbers)
(sum) !  # Prints 15
```

### Example 4: Class with Static Members

```bubblegum
Gum Counter {
    sticky bold sugar count :: 0
    
    recipe: increment()
        Counter.count +: 1
    pop
    
    recipe: getCount() sugar
        pop Counter.count
    pop
}
pop

Counter.increment()
Counter.increment()
Counter.increment()
(Counter.getCount()) !  # Prints 3
```

---

## 🧪 Testing

### Running Example Programs

```bash
# Navigate to test directory
cd Tests/Lexer_Parser

# Compile a test file
dotnet run --project ../../Main run -C HelloWorld.bbgm

# This generates HelloWorld.ll
```

### Available Test Files

Located in `Tests/Lexer_Parser/`:
- `HelloWorld.bbgm` - Basic hello world
- `Simple.bbgm` - Simple expressions
- `SimpleFunctions.bbgm` - Function examples
- `If.bbgm` - Conditional statements
- `Loops.bbgm` - Loop examples
- `Arrays.bbgm` - Array operations
- `Classes.bbgm` - Class definitions
- `Operators.bbgm` - Operator usage
- `PrintDebug.bbgm` - Print/Debug statements
- And many more...

---

## 🔧 Advanced Usage

### Viewing Generated LLVM IR

After compilation, open the `.ll` file to see the generated LLVM IR:

```bash
# On Windows
notepad yourfile.ll

# On Linux/Mac
cat yourfile.ll
```

### Compiling LLVM IR to Executable (Optional)

If you have LLVM installed:

```bash
# Compile IR to object file
llc yourfile.ll -o yourfile.o

# Link to executable (requires C runtime library)
clang yourfile.o -o yourfile.exe

# Run
./yourfile.exe
```

---

## 📖 Language Features Reference

### Visibility Modifiers
- `bold` - Public (accessible everywhere)
- `subtle` - Protected (accessible in class and subclasses)
- `bland` - Private (accessible only in same class)

### Write Modifiers
- `!` - Public write
- `?` - Protected write

### Special Keywords
- `gum` - This pointer in class methods
- `pop` - Return statement
- `nflv` - Flavorless (null-like default object)
- `sticky` - Static member
- `stock` - Namespace/module declaration
- `chew` - Import statement

### Type Casts

```bubblegum
carb x :: 3.14
sugar y :: x => sugar  # Cast double to int
```

---

## ❓ Troubleshooting

### Common Issues

1. **Build errors**: Make sure .NET 8.0 SDK is installed
2. **File not found**: Use absolute paths or run from correct directory
3. **Syntax errors**: Check that your `.bbgm` file uses correct BubblGum syntax

### Getting Help

- Check example files in `Tests/Lexer_Parser/`
- See `README.md` for language overview
- Review `FEATURE_COMPLETION_REPORT.md` for supported features

---

## 🎓 Next Steps

1. **Try the examples** in `Tests/Lexer_Parser/`
2. **Write your own programs** using the syntax guide above
3. **Explore advanced features** like classes, static members, and variadic functions
4. **Check the generated LLVM IR** to understand the compilation process

---

## 📝 Notes

- The compiler generates LLVM IR (`.ll` files)
- To get native executables, you need LLVM tools (`llc`, `clang`)
- All 94 language features are fully implemented
- The compiler enforces immutability, visibility, and type safety

---

**Happy coding with BubblGum! 🎉**

*For complete feature documentation, see [FEATURE_COMPLETION_REPORT.md](FEATURE_COMPLETION_REPORT.md)*
