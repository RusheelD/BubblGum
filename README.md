*Note: Development is still on-going (semantic analysis stage)

# BubblGum Goal:
**BubblGum is a type-safe compiled language with strong null safety**, so no more null reference errors. It supports both classless coding development and object-oriented development (pick your poison). Explore it's rich bubblegum-inspired syntax, and fast execution due to countless, robust code generation optimizations (documented below). BubblGum supports all 3 major platforms: Windows, Linux and MacOS. It comes with syntax-highlighting, and we've thoroughly unit-tested every provided feature.
 
## Features Currently Implemented
- Config files with external directory imports
- Namespaces
   - Child nested namespaces
- Imports
   - Valid import scanning
   - Import specific namespaces OR individual files
- Global Symbol Table with nested scope tracking
- Classes, Functions, Struct, Interfaces
- Standalone functions/vars/statements (Classless development)
- Tuples, Objects, Arrays, Primitive types
    - directly or indirectly referenced through a namespace
- RepeatUp and RepeatDown loops (ie. for loops)
- Pop loops (ie. foreach loops)
- While loops, Single If, Multi If statements
- Pop, Pop stream, Pop a from b (multi-output returns)

## Tools and Proccess Used
The compiler is written primarily in C# and ANTLR:
- **Lexical and Syntax Analysis :** These parts of the compiler were written using [ANTLR4](https://github.com/antlr/antlr4), a powerful parser generator. Input is broken down into tokens, and a parse tree is generated based off grammar rules. We then convert the Parse Tree into a condensed, custom Abstract Syntax Tree (AST).
- **Semantic Analysis:** We visit the AST, storing useful book-keeping info in the AST in a Global Symbol Table (GST). With this, we can perform type checking and the rest of semantic analysis on the AST.
