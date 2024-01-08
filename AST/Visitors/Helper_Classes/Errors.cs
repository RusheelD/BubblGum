﻿using Antlr4.Runtime;
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

       public static void DuplicateClassInNamespace(string name, string namespaceName, string fileName, int line) {
         if (namespaceName != "")
            Console.Error.WriteLine($"Class {name} matches another object name in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Class {name} matches another object name in global namespace at file {fileName} on line {line}");
         
         numErrors++;
       }

       public static void DuplicateInterfaceInNamespace(string name, string namespaceName, string fileName, int line) {
         if (namespaceName != "")
            Console.Error.WriteLine($"Interface {name} matches another object name in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Interface {name} matches another object name in global namespace at file {fileName} on line {line}");
         
         numErrors++;
       }

      public static void DuplicateStructInNamespace(string name, string namespaceName, string fileName, int line) {
         if (namespaceName != "")
            Console.Error.WriteLine($"Struct {name} matches another object name in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Struct {name} matches another object name in global namespace at file {fileName} on line {line}");
         
         numErrors++;
       }

       public static void DuplicateRecipeInNamespace(string name, string namespaceName, string fileName, int line) {
         if (namespaceName != "")
            Console.Error.WriteLine($"Duplicate recipe {name} in namespace {namespaceName} at file {fileName} on line {line}");
         else
            Console.Error.WriteLine($"Duplicate recipe {name}  in global namespace at file {fileName} on line {line}");
         
         numErrors++;
       }

      public static void DuplicateRecipeInClass(string name, string className, string fileName, int line) {
         Console.Error.WriteLine($"Duplicate recipe {name} in {className} at file {fileName} on line {line}");
         numErrors++;
      }

      public static void DuplicateVarInScope(string name, string fileName, int line) {
         Console.Error.WriteLine($"Duplicate variable {name} in scope at file {fileName} on line {line}");
         numErrors++;
      }
    }
}
