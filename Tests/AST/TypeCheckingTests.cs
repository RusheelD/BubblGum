using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AST;

namespace Tests.AST
{
    [TestClass]
    public class TypeCheckingTests
    {
        private const string TEST_DIR = "./TypeChecking_Tests";

        private void CreateTestFile(string filename, string content)
        {
            Directory.CreateDirectory(TEST_DIR);
            string fullPath = Path.Combine(TEST_DIR, filename);
            File.WriteAllText(fullPath, content);
        }

        private bool ExecuteTypeChecking(string filename, out string errors)
        {
            string fullPath = Path.Combine(TEST_DIR, filename);
            var errorStream = new StringWriter();
            var oldError = Console.Error;
            Console.SetError(errorStream);

            // Reset error count
            Errors.numErrors = 0;

            bool success = BubblGum.ExecuteSemanticAnalysis(fullPath, new StreamWriter(Stream.Null));

            Console.SetError(oldError);
            errors = errorStream.ToString();
            return success;
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(TEST_DIR))
            {
                Directory.Delete(TEST_DIR, true);
            }
        }

        [TestMethod]
        public void TestValidPrimitiveTypes()
        {
            string code = @"
recipe: AddNumbers(sugar a, sugar b) sugar {
    sugar result :: a + b
    pop result
}
";
            CreateTestFile("test_valid_primitives.bbgm", code);
            bool success = ExecuteTypeChecking("test_valid_primitives.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid primitive types should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestInvalidAddition()
        {
            string code = @"
recipe: BadAdd(sugar a, kcal b) sugar {
    sugar result :: a + b
    pop result
}
";
            CreateTestFile("test_invalid_add.bbgm", code);
            bool success = ExecuteTypeChecking("test_invalid_add.bbgm", out string errors);
            Assert.IsFalse(success, "Adding sugar and kcal should fail type checking");
            Assert.IsTrue(errors.Contains("Incompatible types") || errors.Contains("Type mismatch"),
                $"Expected type error, got: {errors}");
        }

        [TestMethod]
        public void TestImmutableAssignment()
        {
            string code = @"
recipe: TestImmutable() {
    $sugar x :: 5
    x :: 10
}
";
            CreateTestFile("test_immutable.bbgm", code);
            bool success = ExecuteTypeChecking("test_immutable.bbgm", out string errors);
            Assert.IsFalse(success, "Assigning to immutable variable should fail");
            Assert.IsTrue(errors.Contains("immutable") || errors.Contains("Immutable"),
                $"Expected immutable error, got: {errors}");
        }

        [TestMethod]
        public void TestNonBooleanCondition()
        {
            string code = @"
recipe: BadCondition() {
    sugar x :: 5
    if (x):
        (x) !!
}
";
            CreateTestFile("test_bad_condition.bbgm", code);
            bool success = ExecuteTypeChecking("test_bad_condition.bbgm", out string errors);
            Assert.IsFalse(success, "Non-boolean condition should fail");
            Assert.IsTrue(errors.Contains("boolean") || errors.Contains("Boolean"),
                $"Expected boolean error, got: {errors}");
        }

        [TestMethod]
        public void TestValidBooleanCondition()
        {
            string code = @"
recipe: GoodCondition() {
    yum flag :: yup
    if (flag):
        (""true"") !!
}
";
            CreateTestFile("test_good_condition.bbgm", code);
            bool success = ExecuteTypeChecking("test_good_condition.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid boolean condition should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestArithmeticOperations()
        {
            string code = @"
recipe: ArithmeticTest() {
    sugar a :: 10
    sugar b :: 20
    sugar sum :: a + b
    sugar diff :: a - b
    sugar product :: a * b
    sugar quotient :: a / b
    sugar remainder :: a % b
}
";
            CreateTestFile("test_arithmetic.bbgm", code);
            bool success = ExecuteTypeChecking("test_arithmetic.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid arithmetic operations should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestComparisonOperations()
        {
            string code = @"
recipe: ComparisonTest() {
    sugar a :: 10
    sugar b :: 20
    yum less :: a < b
    yum lessEq :: a <= b
    yum greater :: a > b
    yum greaterEq :: a >= b
    yum equal :: a = b
    yum notEqual :: ~(a = b)
}
";
            CreateTestFile("test_comparison.bbgm", code);
            bool success = ExecuteTypeChecking("test_comparison.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid comparison operations should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestLogicalOperations()
        {
            string code = @"
recipe: LogicalTest() {
    yum a :: yup
    yum b :: nope
    yum andResult :: a & b
    yum orResult :: a | b
    yum notResult :: ~a
}
";
            CreateTestFile("test_logical.bbgm", code);
            bool success = ExecuteTypeChecking("test_logical.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid logical operations should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestInvalidLogicalOperation()
        {
            string code = @"
recipe: BadLogical() {
    sugar a :: 5
    yum b :: yup
    yum result :: a & b
}
";
            CreateTestFile("test_bad_logical.bbgm", code);
            bool success = ExecuteTypeChecking("test_bad_logical.bbgm", out string errors);
            Assert.IsFalse(success, "Logical operation on non-boolean should fail");
        }

        [TestMethod]
        public void TestArrayCreation()
        {
            string code = @"
recipe: ArrayTest() {
    [sugar] numbers :: [sugar](10)
    [carb] doubles :: [1.1, 2.2, 3.3]
}
";
            CreateTestFile("test_array.bbgm", code);
            bool success = ExecuteTypeChecking("test_array.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid array creation should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestLoopTypeChecking()
        {
            string code = @"
recipe: LoopTest() {
    i :: repeatUp(0, 10) {
        (i) !!
    }
    
    while (i < 100) {
        i +: 1
    }
}
";
            CreateTestFile("test_loops.bbgm", code);
            bool success = ExecuteTypeChecking("test_loops.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid loop constructs should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestInvalidLoopBound()
        {
            string code = @"
recipe: BadLoop() {
    i :: repeatUp(""hello"", 10) {
        (i) !!
    }
}
";
            CreateTestFile("test_bad_loop.bbgm", code);
            bool success = ExecuteTypeChecking("test_bad_loop.bbgm", out string errors);
            Assert.IsFalse(success, "Loop with non-numeric bound should fail");
        }

        [TestMethod]
        public void TestTypeCasting()
        {
            string code = @"
recipe: CastTest() {
    carb x :: 5.5
    sugar y :: (x => sugar)
}
";
            CreateTestFile("test_cast.bbgm", code);
            bool success = ExecuteTypeChecking("test_cast.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid type casting should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestMultipleTypeErrors()
        {
            string code = @"
recipe: ManyErrors() {
    sugar x :: ""not a number""
    yum flag :: 42
    if (x):
        x :: 99
}
";
            CreateTestFile("test_multiple_errors.bbgm", code);
            bool success = ExecuteTypeChecking("test_multiple_errors.bbgm", out string errors);
            Assert.IsFalse(success, "Multiple type errors should be detected");
            Assert.IsTrue(Errors.numErrors > 1, "Should detect multiple errors");
        }

        [TestMethod]
        public void TestUndefinedVariable()
        {
            string code = @"
recipe: UndefinedVar() {
    sugar x :: y + 5
}
";
            CreateTestFile("test_undefined.bbgm", code);
            bool success = ExecuteTypeChecking("test_undefined.bbgm", out string errors);
            Assert.IsFalse(success, "Using undefined variable should fail");
            Assert.IsTrue(errors.Contains("Undefined") || errors.Contains("undefined"),
                $"Expected undefined variable error, got: {errors}");
        }

        [TestMethod]
        public void TestValidFunction()
        {
            string code = @"
recipe: Helper(sugar x) sugar {
    pop x * 2
}

recipe: Main() {
    sugar result :: Helper(5)
}
";
            CreateTestFile("test_function.bbgm", code);
            bool success = ExecuteTypeChecking("test_function.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid function calls should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestBitwiseOperations()
        {
            string code = @"
recipe: BitwiseTest() {
    sugar a :: 5
    sugar b :: 3
    sugar leftShift :: a << b
    sugar rightShift :: a >> b
    sugar xorResult :: a ^ b
}
";
            CreateTestFile("test_bitwise.bbgm", code);
            bool success = ExecuteTypeChecking("test_bitwise.bbgm", out string errors);
            Assert.IsTrue(success, $"Valid bitwise operations should pass. Errors: {errors}");
        }

        [TestMethod]
        public void TestInvalidBitwiseOperation()
        {
            string code = @"
recipe: BadBitwise() {
    carb a :: 5.5
    carb b :: 3.3
    carb result :: a << b
}
";
            CreateTestFile("test_bad_bitwise.bbgm", code);
            bool success = ExecuteTypeChecking("test_bad_bitwise.bbgm", out string errors);
            Assert.IsFalse(success, "Bitwise operation on non-integer should fail");
        }
    }
}
