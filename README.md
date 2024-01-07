*Note: Development is still on-going. Currently underway with type checking.

# BubblGum Completion Goal:
**BubblGum is a type-safe compiled language with strong null safety**, so no more null reference errors. It supports both classless coding development and object-oriented development (pick your poison). Explore it's rich bubblegum-inspired syntax, and fast execution due to countless, robust code generation optimizations (documented below). BubblGum supports all 3 major platforms: Windows, Linux and MacOS. It comes with syntax-highlighting, and we've thoroughly unit-tested every provided feature.
 
### Strong Null Safety:
TO-DO

### [Rich Documentation](https://www.youtube.com/watch?v=dQw4w9WgXcQ)
TO-DO

### Speed Tests
TO-DO

## Development
The compiler is broken down into the following parts. It was written primarily in C#, alongside Assembly and ANTLR:
- **Lexical and Syntax Analysis :** These parts of the compiler were written using [ANTLR4](https://github.com/antlr/antlr4), a powerful parser generator. Input is broken down into tokens, and a parse tree is generated based off grammar rules. We then convert the Parse Tree into a condensed Abstract Syntax Tree (AST) for the next stage.
- **Semantic Analysis:** Type checking and other semantic analysis.