using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BubblGumParser;

namespace AST
{
   public class Errors
   {
      public static int numErrors = 0;

      public static void DuplicateClassInNamespace(string name, string namespaceName, string fileName, int line)
      {
         if (namespaceName != "")
            Console.Error.WriteLine($"Class {name} matches another object name in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Class {name} matches another object name in global namespace at file {fileName} on line {line}");

         numErrors++;
      }

      public static void ClassNotFound(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Class {name} not found at file {fileName} on line {line}");

         numErrors++;
      }

      public static void ClassOrInterfaceNotFound(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Class or Interface {name} not found at file {fileName} on line {line}");

         numErrors++;
      }

      public static void InterfaceNotFound(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Interface {name} not found at file {fileName} on line {line}");

         numErrors++;
      }

      public static void NamespaceNotFound(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Namespace {name} not found at file {fileName} on line {line}");

         numErrors++;
      }

      public static void FileNotFound(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"File {name} not found at file {fileName} on line {line}");

         numErrors++;
      }

      public static void MultiInheritanceNotAllowed(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Multi-Inheritance not allowed for class {name} at file {fileName} on line {line}");

         numErrors++;
      }

      public static void DuplicateInterfaceInNamespace(string name, string namespaceName, string fileName, int line)
      {
         if (namespaceName != "")
            Console.Error.WriteLine($"Interface {name} matches another object name in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Interface {name} matches another object name in global namespace at file {fileName} on line {line}");

         numErrors++;
      }

      public static void DuplicateStructInNamespace(string name, string namespaceName, string fileName, int line)
      {
         if (namespaceName != "")
            Console.Error.WriteLine($"Struct {name} matches another object name in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Struct {name} matches another object name in global namespace at file {fileName} on line {line}");

         numErrors++;
      }

      public static void DuplicateRecipeInNamespace(string name, string namespaceName, string fileName, int line)
      {
         if (namespaceName != "")
            Console.Error.WriteLine($"Duplicate recipe {name} in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Duplicate recipe {name}  in global namespace at file {fileName} on line {line}");

         numErrors++;
      }

      public static void DuplicateRecipeInClass(string name, string className, string fileName, int line)
      {
         Console.Error.WriteLine($"Duplicate recipe {name} in {className} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void DuplicateVarInScope(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Duplicate variable {name} in scope at file {fileName} on line {line}");
         numErrors++;
      }

      public static void TypeMismatch(string expected, string actual, string fileName, int line)
      {
         Console.Error.WriteLine($"Type mismatch: expected {expected}, but got {actual} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void IncompatibleTypes(string type1, string type2, string operation, string fileName, int line)
      {
         Console.Error.WriteLine($"Incompatible types {type1} and {type2} for operation {operation} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void UndefinedVariable(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Undefined variable {name} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void UndefinedFunction(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Undefined function {name} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void ImmutableAssignment(string name, string fileName, int line)
      {
         Console.Error.WriteLine($"Cannot assign to immutable variable {name} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void ArgumentCountMismatch(string functionName, int expected, int actual, string fileName, int line)
      {
         Console.Error.WriteLine($"Function {functionName} expects {expected} arguments, but got {actual} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void InvalidCast(string fromType, string toType, string fileName, int line)
      {
         Console.Error.WriteLine($"Cannot cast from {fromType} to {toType} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void InvalidArrayAccess(string type, string fileName, int line)
      {
         Console.Error.WriteLine($"Cannot perform array access on type {type} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void InvalidMemberAccess(string memberName, string typeName, string fileName, int line)
      {
         Console.Error.WriteLine($"Type {typeName} does not have a member named {memberName} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void NonBooleanCondition(string type, string fileName, int line)
      {
         Console.Error.WriteLine($"Condition must be of boolean type, but got {type} at file {fileName} on line {line}");
         numErrors++;
      }
   }
}
