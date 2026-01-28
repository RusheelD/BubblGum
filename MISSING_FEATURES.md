# Missing Code Generation Features

This document lists language features that are **parsed and in the AST** but **not yet code-generated**.

---

## 🔴 Critical Missing Features

### 1. **Variadic Functions (Ellipses `...`)**
**Status:** Parsed ✅ | Code Generated ❌

**Grammar Support:**
```python
recipe: sum(sugar nums...) sugar {
    # nums is variadic - can accept any number of arguments
}

recipe: printf(kcal format, flavor args...) {
    # Variable arguments like C printf
}
```

**What's Missing:**
- No LLVM va_list implementation
- Can't generate code to iterate over variable arguments
- Return value ellipses `<sugar...>` not supported

**AST:** `FunctionHeader.Params[i].Item4` (hasEllipses flag)

---

### 2. **Immutable Variables (`$` prefix)**
**Status:** Parsed ✅ | Code Generated ❌

**Grammar Support:**
```python
$carb pi :: 3.14159           # Immutable variable
recipe: foo($sugar x) {       # Immutable parameter
    x :: 5                    # Should ERROR - x is immutable
}
```

**What's Missing:**
- No enforcement of immutability
- Should reject assignments to `$` variables after initialization
- Parameters marked immutable can still be modified

**AST:** 
- `AssignDeclLHS.IsImmutable`
- `FunctionHeader.Params[i].Item1` (isImmutable flag)

**Tracking:** `FlavorInfo.IsImmutable` in symbol tables

---

### 3. **Multiple Return Values (Named Outputs)**
**Status:** Parsed ✅ | Code Generated ❌

**Grammar Support:**
```python
# Named outputs
recipe: divide(carb a, carb b) <carb quotient, carb remainder> {
    pop a/b => quotient         # Return by name
    pop a%b => remainder
}

# Indexed outputs  
recipe: coords() <carb, carb, carb> {
    pop 10.0 => 0              # Return to index 0
    pop 20.0 => 1              # Return to index 1
    pop 30.0 => 2              # Return to index 2
}
```

**What's Missing:**
- Functions currently only return single value
- No tuple return implementation
- `pop value => name` and `pop value => idx` not code-generated

**AST:** 
- `FunctionHeader.Outputs` - list of (type, name, hasEllipses)
- `Pop.UseIdx`, `Pop.UseName` for output targeting

---

### 4. **Debug Statement (`?` operator)**
**Status:** Parsed ✅ | Code Generated ❌

**Grammar Support:**
```python
(x + y)?              # Debug print - single ?
(myArray)?            # Debug print array
(complexExpr)??       # Double ?? for different format
```

**What's Missing:**
- No LLVM code generation for Debug AST node
- Should output to stderr or debug stream
- Could include file/line information

**AST:** `Debug` node (similar to `Print`)

---

## 🟡 Important Missing Features

### 5. **`gum` Keyword (This Pointer)**
**Status:** Parsed ✅ | Partially Generated ⚠️

**Grammar Support:**
```python
Gum Point2D {
    bold carb x!
    
    bold recipe: setX(carb newX) {
        gum->x :: newX        # Access this->x
    }
}
```

**What's Missing:**
- `gum` should map to LLVM `this` pointer in methods
- Member access through `gum->` not implemented
- Need proper struct pointer handling

**AST:** `THIS` token maps to identifier "gum"

---

### 6. **Static/Sticky Members**
**Status:** Parsed ✅ | Code Generated ❌

**Grammar Support:**
```python
Gum Counter {
    sticky sugar count :: 0    # Static variable shared by all instances
    
    sticky recipe: increment() {
        count +: 1
    }
}
```

**What's Missing:**
- No LLVM global variable generation for static fields
- Static methods not differentiated from instance methods
- No handling of static initialization

**AST:** 
- Classes: `Class.IsSticky` per member
- Symbol tables: `GumFlavorInfo.IsSticky`, `GumRecipeTable.IsSticky`

---

### 7. **Visibility Modifiers Enforcement**
**Status:** Parsed ✅ | Not Enforced ❌

**Grammar Support:**
```python
Gum MyClass {
    bold sugar public!         # Public read/write
    subtle sugar protected?    # Protected read, protected write
    bland sugar private        # Private read, no write modifier
}
```

**Modifiers:**
- `bold` = public (any class can read)
- `subtle` = protected (derived classes can read)
- `bland` = private (only this class can read)
- `!` suffix = public write
- `?` suffix = protected write

**What's Missing:**
- No access checking during member access
- All members effectively public in code generation
- Need scope-based validation

**AST:** `Visbility` enum: `Bold`, `Subtle`, `Bland`

---

## 🟢 Minor/Future Features

### 8. **Pop Stream Operations**
**Status:** Partial implementation

**What's Missing:**
- `popstream` keyword for streaming outputs
- Runtime stream support needed

### 9. **Mintpack (Args Array)**
**Status:** Basic stub exists

**Grammar:** `mintpack` keyword for varargs packing
**Implementation:** Creates empty array - needs proper varargs integration

### 10. **Pure (Unsigned) Types**
**Status:** Parsed but not differentiated

**Grammar:** `pure sugar` for unsigned int
**Missing:** Type system doesn't distinguish signed/unsigned

---

## 📋 Implementation Priority

### High Priority (Core Features)
1. **Multiple Return Values** - Critical for language design
2. **Immutability Enforcement** - Important for safety
3. **Debug Statements** - Essential for development

### Medium Priority (OOP Features)  
4. **`gum` This Pointer** - Needed for proper OOP
5. **Static/Sticky Members** - Common requirement
6. **Visibility Enforcement** - Security/encapsulation

### Low Priority (Advanced)
7. **Variadic Functions** - Complex, less common
8. **Pop Stream** - Requires runtime design
9. **Pure Types** - Type system enhancement

---

## 🛠️ Implementation Notes

### For Multiple Returns:
- Use LLVM struct returns
- Map output names to struct field indices
- `pop value => name` stores to struct field
- Function returns entire struct

### For Immutability:
- Add checks in `Visit(Assignment)` 
- Look up variable in symbol table
- If `IsImmutable` and already initialized, error
- Similar to const in C++

### For Variadic Functions:
- Use LLVM va_list intrinsics
- `llvm.va_start`, `llvm.va_arg`, `llvm.va_end`
- Requires C calling convention
- Platform-specific implementation

### For Debug:
- Generate call to debug runtime function
- Include source location metadata
- Could use LLVM debug intrinsics
- Different formatting than print

---

## 📊 Current Status Summary

| Feature | Lexer | Parser | AST | Symbol Table | Code Gen |
|---------|-------|--------|-----|--------------|----------|
| Variadic (`...`) | ✅ | ✅ | ✅ | ✅ | ❌ |
| Immutable (`$`) | ✅ | ✅ | ✅ | ✅ | ❌ |
| Multiple Returns | ✅ | ✅ | ✅ | ✅ | ❌ |
| Debug (`?`) | ✅ | ✅ | ✅ | ⚠️ | ❌ |
| `gum` (this) | ✅ | ✅ | ✅ | ✅ | ⚠️ |
| `sticky` (static) | ✅ | ✅ | ✅ | ✅ | ❌ |
| Visibility | ✅ | ✅ | ✅ | ✅ | ❌ |
| `pure` (unsigned) | ✅ | ✅ | ⚠️ | ❌ | ❌ |

**Legend:**
- ✅ = Fully implemented
- ⚠️ = Partially implemented  
- ❌ = Not implemented

---

## 🎯 Next Steps

To reach 100% language feature parity:

1. Implement multiple return values (struct returns)
2. Add immutability checks in assignment codegen
3. Generate debug statement LLVM IR
4. Implement `gum` this pointer in class methods
5. Add static member support (globals for sticky fields)
6. Enforce visibility during member access
7. Implement variadic function support (va_list)

**Estimated remaining work:** ~20-30% of core language features
