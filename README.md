*Note: Development is still on-going (semantic analysis stage). See features implemented checklist below.

# BubblGum Development Goal:
**BubblGum is a type-safe compiled language with strong null safety and immutability support.**. It supports both classless coding development and object-oriented development (pick your poison). Explore it's rich bubblegum-inspired syntax, and fast execution due to countless, robust code generation optimizations (documented below). BubblGum supports all 3 major platforms: Windows, Linux and MacOS. It comes with syntax-highlighting, and we've thoroughly unit-tested every provided feature.
 
## Syntax Sneakpeek

In Bubblgum, functions are called recipes. Here are some examples.
``` python
# (exp) !! is a quick way to print exp to outstream
recipe: FizzBuzz() {
    i :: repeatUp(0, 1000) {
       if (i % 3 = 0):
           ("fizz") !!
       if (i % 5 = 0):
           ("buzz") !!

       ("") !  # prints new line
   }
}

# carbs are equivalent to C's double type
recipe: GetSquareOfNumbers([carb] nums) <carb...> {
    # loop through array nums
    pop num from nums => {  
       (pop num * num) !
   }
}

# operator :: represents assignment, -> is used to access members of a var
recipe: MergeTwoSortedArrays([carb] l1, [carb] l2) [carb] {
    [carb] merged :: [carb](l1->size + l2->size) 
    sugar i, j, m

    while (i < l1->size & j < l2->size) {
        if (l1[i] <= l2[j]) {
            merged[m] :: l1[i]
            i +: 1
        }
        else {
            merged[m] :: l2[j]
            j +: 1
        }
        m +: 1
    }
    
    if (i < l1->size) {
        k :: repeatUp(0, l1->size-i):
           merged[m+k] :: l1[i+k]
    }
    elif (j < l2->size) {
        k :: repeatUp(0, l2->size-j):
           merged[m+k] :: l2[j+k]
    }
    pop merged
}

```

Scoll down for a more comprehensive syntax guide.

## Currently Added
- Config files with external directory imports
- Namespaces
   - Nested child namespaces
- Imports
   - Validation
   - Can import specific namespaces OR individual files
- Global Symbol Table with nested scope tracking
- Classes, Functions, Struct, Interfaces
- Standalone functions/vars/statements (Classless development)
- Tuples, Objects, Arrays, Primitive types
    - directly or indirectly referenced through a namespace
- RepeatUp and RepeatDown loops (ie. for loops)
- Pop loops (ie. foreach loops)
- While loops, Single If, Multi If statements
- Pop, Pop stream, Pop by var name or idx (function returns and outputs)
- Operators (arithmetic, bitwise, comparators)
- Expressions and Literals

## Tools and Proccess Used
The compiler is written primarily in C# and ANTLR:
- **Lexical and Syntax Analysis :** These parts of the compiler were written using [ANTLR4](https://github.com/antlr/antlr4), a powerful parser generator. Input is broken down into tokens, and a parse tree is generated based off grammar rules. We then convert the Parse Tree into a condensed, custom Abstract Syntax Tree (AST).
- **Semantic Analysis:** We visit the AST, storing useful book-keeping info in the AST in a Global Symbol Table (GST). With this, we can perform type checking and the rest of semantic analysis on the AST.

## Types and Pop Features
Primitive types:
```python
SUGAR: 'sugar';         # int
CARB: 'carb';           # double
CAL: 'cal';             # char
KCAL: 'kcal';           # string
YUM: 'yum';             # bool

YUP: 'yup';             # true value
NOPE: 'nope';           # false value
```

Bubblgum supports immutable vars and parameters (preceeded with $). This recipe outputs a tuple (surrounded by <>). 
```python

recipe: HelloWorld($[sugar] arr) <sugar a, sugar, sugar b> {

    # define immutable number
    $carb a :: 12.3

    # pop an output by idx
    pop arr[0] => 1

    # pop an output by name
    pop arr[arr->size - 1] => b 

    # cast 5.3 into a sugar type
    pop 4 + (5.3 => sugar) => a
}
```

You can define an array with [] notation or the keyword pack. Back pack is equivalent to [Back].
```python
recipe: HelloWorld($sugar a, $[BubbleGum, Back pack] d) { }
```

## Namespaces
Namespaces are a great way to organize code and imports. Here's a file that utilizes them.
```python

# import namespaces
chew MathX
chew Physics

# current file extends this namespace
stock Physics->Rigidbody

# requires an array of colliders. returns whether overlap was detected
# between any two colliders
recipe: DetectOverlap([Physics->Collisions.CircleCollider] cols) yum {
        # loop through colliders
        pop col1 from cols => {
           pop col2 from cols => {

             # if comparing two different colliders
              if (~(col1 is col2)) {
                  carb dist :: GetDistance(col1->center, col2->center)
                  carb radiiSum :: col1->radius + col2->radius
                 
                  if (dist < radiiSum):
                     pop yup
		     pop
              } 
           }
        }

        pop nope
}
```

## Classes
We use the Gum keyword to declare classes. By default, class variables can only be read and written to by the class they are declared in. To set a variable's read permissions, use the keywords bold, subtle or bland. These keywords are similar to C's public, protected and private keywords. To set a variable's write permissions, use the modifiers below after declaring the variable

Keywords:
```python
'bold'       // readable by any class
'subtle'     // readable by derived classes
'bland'      // readable by only this class
```
Modifiers:
```python
'!'          // writable by any class
'?'          // writeable by derived classes
```

Here is an example file that uses classes:
``` python
Gum Point2D {
	bold carb x! 
	bold carb y!
	
	bold recipe: Point2D(carb x, carb y) {
		gum->x :: x
		gum->y :: y
	}

	bold recipe: Distance(Point2D point) carb:
		pop (((gum->x - point->x)**(2) + (gum->y - point->y)**(2))**(1/2))
}

Gum Line2D {
	bold Point2D point1 :: nflv # we cover nflv in the next chapter
	bold Point2D point2 :: nflv

	bold recipe: Line2D(carb x1, carb x2, carb y1, carb y2) {
		point1 :: Point2D(x1, y1)
		point2 :: Point2D(x2, y2)
	}

	bold recipe: Length() carb:
		pop point1->Distance(point2)

	bold recipe: Slope() carb:
		pop (point1->y - point2->y) / (point1->x - point2->x)

	bold recipe: Points() <Point2D, Point2D> {
		pop point1
		pop point2
	}
}
```

## Flavorless
Bubblgum has no concept of null. If a reference should not be set yet, set it to flavorless or nflv (short for no flavor).

``` python
Cow c :: nflv
```

This cow actually points to a default Cow reference, and thus calling methods like Cow->moo will not throw an error. To check
if c is flavorless, one can use the variable c like a bool (yum), which will return true (yup) or false (nope).

``` python
Cow c :: nflv
if (c):
   c->moo        # won't run

c->moo           # will run and not throw error
```

The idea behind flavorless is to lower crashs due to null reference errors during runtime. However, since such errors are useful for catching mistakes,
the compiller will still throw warnings on debug mode when a flavorless reference is accessed.

Nflv assignment must be very intentional. Thus we disallow creating a reference without initializing it.

``` python
Cow c               # not allowed
Cow c :: nflv       # allowed
Cow c :: cow()      # allowed, creates a new cow object
```

## Other Features
This page was intended as a brief introduction to BubblGum. There are many other features Bubblgum supports, from interfaces (wrappers) and structs (candy), to literals, json config files, and more.
We plan to link to seperate, rigorous documentation once the compiler is finished.
