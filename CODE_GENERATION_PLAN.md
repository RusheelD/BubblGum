# Code Generation Requirements for BubblGum

## Overview
With the TypeCheckingPass complete, the compiler now has:
- ✅ **Lexical & Syntax Analysis** (ANTLR-generated)
- ✅ **AST Construction** (Custom condensed tree)
- ✅ **Symbol Table Creation** (GST Pass 1 & 2)
- ✅ **Type Checking** (TypeCheckingPass)

**Next Phase: Code Generation & Execution**

---

## 1. Backend Options

### Option A: LLVM IR Backend (Recommended)
**Pros:**
- Industry-standard, highly optimized
- Cross-platform support (Windows, Linux, macOS)
- Built-in optimization passes
- Strong tooling ecosystem

**Cons:**
- Steeper learning curve
- Requires LLVM bindings for C#

**Implementation:**
- Use **LLVMSharp** NuGet package
- Generate LLVM IR from AST
- Leverage LLVM optimization pipeline
- Compile to native code via LLVM backend

### Option B: C Transpilation
**Pros:**
- Simpler to implement
- Leverages existing C compilers
- Easy debugging (read generated C)
- No additional dependencies

**Cons:**
- Less optimized than LLVM
- Requires C compiler on target machine
- Manual optimization needed

**Implementation:**
- Visitor pattern to generate C code
- Map BubblGum types to C types
- Generate runtime library in C
- Use gcc/clang for final compilation

### Option C: .NET IL Backend
**Pros:**
- Native C# support
- No external dependencies
- JIT compilation benefits
- Easy interop with .NET libraries

**Cons:**
- Locks into .NET ecosystem
- Different optimization model
- May not align with language goals

**Implementation:**
- Use System.Reflection.Emit
- Generate IL from AST
- Create assemblies dynamically

### Option D: Custom Bytecode VM
**Pros:**
- Full control over execution
- Easier to implement than native code
- Portable

**Cons:**
- Slower execution
- More work to optimize
- Need to build entire VM

---

## 2. Core Components Needed

### 2.1 Code Generation Visitor
Following the existing visitor pattern:

```csharp
namespace AST
{
    public class CodeGenerator : Visitor
    {
        private IRBuilder builder;  // or C code builder
        private GlobalSymbolTable gst;
        private Dictionary<string, object> symbolMap;
        
        public void Execute(string filePath, Program n, GlobalSymbolTable gst)
        {
            // Initialize code generation
        }
        
        public void Visit(Program n) { }
        public void Visit(Function n) { }
        public void Visit(Class n) { }
        // ... implement all Visit methods
    }
}
```

### 2.2 Type Mapping System
Map BubblGum types to target representation:

```csharp
public class TypeMapper
{
    // For LLVM: Map to LLVM types
    public LLVMTypeRef MapType(AnyType type) { }
    
    // For C: Map to C type strings
    public string MapTypeToC(AnyType type) 
    {
        if (type is PrimitiveType pt)
        {
            switch (pt.Type)
            {
                case TypeBI.Sugar: return "int64_t";
                case TypeBI.Carb: return "double";
                case TypeBI.Cal: return "char";
                case TypeBI.Kcal: return "char*";
                case TypeBI.Yum: return "bool";
                default: return "void*";
            }
        }
        // ... handle complex types
    }
}
```

### 2.3 Runtime Library
Implement built-in functionality:

**Memory Management:**
- Garbage collector OR reference counting
- Array allocation/deallocation
- String management

**Built-in Functions:**
```c
// Example runtime functions for C backend
void* bbgm_alloc(size_t size);
void bbgm_free(void* ptr);
void bbgm_print(void* value, int type);
int64_t bbgm_repeatUp(int64_t start, int64_t end, void* body);
```

**Standard Library:**
- I/O operations (print, read)
- String operations
- Array operations
- Math functions

### 2.4 Symbol Resolution
Map identifiers to their runtime locations:

```csharp
public class SymbolResolver
{
    // Map variable names to stack slots / registers / heap locations
    private Dictionary<string, Location> locals;
    private Dictionary<string, Location> globals;
    
    public Location ResolveVariable(string name) { }
    public Location ResolveFunction(string name, string key) { }
}
```

---

## 3. Implementation Phases

### Phase 1: Basic Expression & Statement Generation
**Priority: HIGH**
- Literals (integers, doubles, strings, booleans)
- Binary operations (+, -, *, /, %, **)
- Variable declarations and assignments
- Print statements
- Function definitions (without calls initially)

**Estimated Work:** 40-60 hours

### Phase 2: Control Flow
**Priority: HIGH**
- If/elif/else statements
- While loops
- RepeatUp/RepeatDown loops
- Pop (return) statements
- Short-circuit boolean evaluation

**Estimated Work:** 30-40 hours

### Phase 3: Functions & Calls
**Priority: HIGH**
- Function calls with arguments
- Return value handling
- Multiple return values (tuples)
- Recursion support
- Function overloading resolution

**Estimated Work:** 40-50 hours

### Phase 4: Arrays & Collections
**Priority: MEDIUM**
- Array allocation
- Array indexing
- Array initialization
- Tuple creation/access
- Pop loops (foreach)

**Estimated Work:** 30-40 hours

### Phase 5: Classes & Objects
**Priority: MEDIUM**
- Class definitions
- Object instantiation
- Method calls
- Member access
- Inheritance support
- Constructor/destructor

**Estimated Work:** 50-70 hours

### Phase 6: Advanced Features
**Priority: LOW**
- Interfaces
- Structs
- Type casting
- Global namespace access
- Flavorless (null) handling

**Estimated Work:** 40-60 hours

### Phase 7: Optimizations
**Priority: LOW**
- Constant folding
- Dead code elimination
- Inline small functions
- Loop unrolling
- Tail call optimization

**Estimated Work:** 60-80 hours

---

## 4. Recommended Approach

### Step 1: Choose Backend (C Transpilation Recommended for MVP)
Start with C transpilation because:
- Simpler to implement and debug
- Can read and verify generated code
- Leverages mature C compilers
- Easier to integrate runtime library

### Step 2: Create Minimal Runtime
```c
// runtime.h
#include <stdint.h>
#include <stdbool.h>
#include <stdio.h>
#include <stdlib.h>

typedef int64_t Sugar;
typedef double Carb;
typedef char Cal;
typedef char* Kcal;
typedef bool Yum;

void bbgm_print_sugar(Sugar value);
void bbgm_print_carb(Carb value);
void bbgm_print_kcal(Kcal value);
// ... more runtime functions
```

### Step 3: Implement CodeGenVisitor
```csharp
public class CCodeGenerator : Visitor
{
    private StringBuilder code;
    private int indentLevel;
    
    public string GenerateCode(Program program, GlobalSymbolTable gst)
    {
        code = new StringBuilder();
        code.AppendLine("#include \"runtime.h\"");
        code.AppendLine();
        Visit(program);
        return code.ToString();
    }
}
```

### Step 4: Test Incrementally
- Start with simple programs (arithmetic, print)
- Gradually add features
- Test each feature thoroughly before moving on

### Step 5: Build & Execute Pipeline
```csharp
public static bool CompileAndRun(string filePath)
{
    // 1. Parse to AST
    // 2. Build GST
    // 3. Type check
    // 4. Generate C code
    var codeGen = new CCodeGenerator();
    string cCode = codeGen.GenerateCode(program, gst);
    
    // 5. Write C file
    File.WriteAllText("output.c", cCode);
    
    // 6. Compile with gcc
    Process.Start("gcc", "output.c runtime.c -o output");
    
    // 7. Run executable
    Process.Start("./output");
}
```

---

## 5. File Structure for Code Generation

```
MainBBGM/
├── CodeGeneration/
│   ├── CodeGeneration.csproj
│   ├── Visitors/
│   │   ├── CCodeGenerator.cs          # C transpiler
│   │   ├── LLVMCodeGenerator.cs       # LLVM backend (future)
│   │   └── OptimizationPass.cs        # Optimizations
│   ├── Runtime/
│   │   ├── runtime.h                  # C runtime header
│   │   ├── runtime.c                  # C runtime implementation
│   │   └── memory.c                   # Memory management
│   ├── TypeMapper.cs
│   ├── SymbolResolver.cs
│   └── OutputBuilder.cs
├── Tests/
│   └── Code_Generation/
│       ├── BasicExpressionTests.cs
│       ├── ControlFlowTests.cs
│       ├── FunctionTests.cs
│       └── IntegrationTests.cs
```

---

## 6. Example Code Generation

### Input (BubblGum):
```python
recipe: Factorial(sugar n) sugar {
    if (n <= 1):
        pop 1
    pop n * Factorial(n - 1)
}
```

### Output (C):
```c
#include "runtime.h"

Sugar Factorial(Sugar n) {
    if (n <= 1) {
        return 1;
    }
    return n * Factorial(n - 1);
}

int main() {
    return 0;
}
```

---

## 7. Testing Strategy

### Unit Tests
- Test each AST node's code generation
- Verify correct C/LLVM output
- Test runtime library functions

### Integration Tests
```csharp
[TestMethod]
public void TestFactorial()
{
    string bbgmCode = @"
        recipe: Factorial(sugar n) sugar {
            if (n <= 1):
                pop 1
            pop n * Factorial(n - 1)
        }
    ";
    
    var output = CompileAndRun(bbgmCode);
    Assert.AreEqual(120, Factorial(5));
}
```

### End-to-End Tests
- Full programs with multiple files
- Import/namespace resolution
- Complex type interactions

---

## 8. Performance Considerations

### Memory Management
- Start with simple malloc/free
- Later: implement GC or reference counting
- Track object lifetimes during codegen

### Optimization Opportunities
1. **Constant folding**: `5 + 3` → `8`
2. **Dead code elimination**: Remove unreachable code
3. **Inline small functions**: Avoid function call overhead
4. **Loop optimization**: Unroll small loops
5. **String interning**: Reuse string constants

---

## 9. Immediate Next Steps

1. **Create CodeGeneration project** (~2 hours)
2. **Implement minimal C runtime** (~8 hours)
3. **Implement CCodeGenerator for expressions** (~20 hours)
4. **Implement CCodeGenerator for statements** (~20 hours)
5. **Add function generation** (~15 hours)
6. **Test with simple programs** (~10 hours)
7. **Iterate and expand** (ongoing)

**Total Estimated Time for MVP:** 100-150 hours

---

## 10. Success Criteria

A working MVP should be able to compile and run:

```python
recipe: Main() {
    sugar a :: 10
    sugar b :: 20
    sugar sum :: a + b
    (sum) !!
    
    if (sum > 25):
        ("Greater than 25") !!
}
```

And produce executable output that prints:
```
30
Greater than 25
```
