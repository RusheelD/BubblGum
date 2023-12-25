*Note: Development is on-going. We are currently underway with Semantic Analysis.
# BubblGum
**BubblGum is a type-safe compiled language with strong null safety**, so no more null reference errors. It supports both classless coding development and object-oriented development (pick your poison). Explore it's rich bubblegum-inspired syntax, and fast execution due to countless, robust code generation optimizations (documented below). BubblGum supports all 3 major platforms: Windows, Linux and MacOS. It comes with syntax-highlighting, and we've thoroughly unit-tested every provided feature.
 
## Usage and Download
Platform-dependent:
- On MacOS and Linux, run `pip install-bubblgum` in a terminal
- On Windows, run `npm install-bubblgum` in powershell or gitbas
  
## Useful Information
### Library Support:
To write programs in BubblGum, we offer numerous libraries: Mathf, LinearAlg, DataStructures (ex. PriorityQueues, Dictionaries), Physics3D, and more. Despite its low library support, BubblGum is the perfect coding language to have fun writing small, weekend projects with its concise candy-inspired syntax. Again, it's optimized for null safety and speed.

### Strong Null Safety:
TO-DO

### [Rich Documentation](https://www.youtube.com/watch?v=dQw4w9WgXcQ)
TO-DO note: Link to separate md for documentation, but provide some brief syntax heres and in other sections.

### Speed Tests
TO-DO

## Development
The compiler is broken down into the following parts. It was written primarily in C#, alongside Assembly and ANTLR:
- **Lexical and Syntax Analysis :** These parts of the compiler were written using [ANTLR4](https://github.com/antlr/antlr4), a powerful parser generator. Input is broken down into tokens, and a parse tree is generated based off grammar rules. We then convert the Parse Tree into a condensed Abstract Syntax Tree (AST) for the next stage.
- **Semantic Analysis:** Type checking and other semantic analysis.
- **[Intermediate Code Generation:](https://www.youtube.com/watch?v=dQw4w9WgXcQ)** Converts the code into an intermediate representation.
- **[Code Optimization:](https://www.youtube.com/watch?v=dQw4w9WgXcQ)** Optimizes the intermediate code for memory and efficiency.
- **Code Generation:** Translates the optimized code into assembly based off target platform.
- **Assembly Linking and Execution:** Combines assembly code files into an executable. Runs it on the target machine.

## Code Generation Optimizations
