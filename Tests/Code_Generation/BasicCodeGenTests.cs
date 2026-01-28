using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using CodeGeneration.Visitors;
using AST;

namespace AntlrCSharpTests.Code_Generation
{
    [TestClass]
    public class BasicCodeGenTests
    {
        private string testDir = "";

        [TestInitialize]
        public void Setup()
        {
            // Create test directory
            testDir = Path.Combine(Path.GetTempPath(), "BubblGumCodeGenTests_" + Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDir);
        }

        [TestCleanup]
        public void Cleanup()
        {
            // Clean up test directory
            if (Directory.Exists(testDir))
            {
                Directory.Delete(testDir, true);
            }
        }

        private void CreateTestFile(string filename, string content)
        {
            File.WriteAllText(Path.Combine(testDir, filename), content);
        }

        [TestMethod]
        public void TestSimpleFunctionGeneration()
        {
            string code = @"
stock:

recipe: Add(sugar a, sugar b) sugar {
    pop a + b
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            Assert.IsTrue(File.Exists(outputPath), "LLVM IR file should be generated");

            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("define"), "IR should contain function definition");
            Assert.IsTrue(ir.Contains("Add"), "IR should contain the Add function");
        }

        [TestMethod]
        public void TestPrintStatement()
        {
            string code = @"
stock:

recipe: Main() {
    sugar x :: 42
    (x) !!
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("bbgm_print_sugar"), "IR should call print function");
        }

        [TestMethod]
        public void TestArithmeticOperations()
        {
            string code = @"
stock:

recipe: Calculate(sugar a, sugar b) sugar {
    sugar sum :: a + b
    sugar diff :: a - b
    sugar prod :: a * b
    sugar quot :: a / b
    pop sum
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("add"), "IR should contain addition");
            Assert.IsTrue(ir.Contains("sub"), "IR should contain subtraction");
            Assert.IsTrue(ir.Contains("mul"), "IR should contain multiplication");
            Assert.IsTrue(ir.Contains("sdiv"), "IR should contain division");
        }

        [TestMethod]
        public void TestConditionalBranching()
        {
            string code = @"
stock:

recipe: Max(sugar a, sugar b) sugar {
    if (a > b):
        pop a
    pop b
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("br i1"), "IR should contain conditional branch");
            Assert.IsTrue(ir.Contains("if.then"), "IR should contain then block");
            Assert.IsTrue(ir.Contains("if.end"), "IR should contain end block");
        }

        [TestMethod]
        public void TestWhileLoop()
        {
            string code = @"
stock:

recipe: CountDown(sugar n) {
    while (n > 0):
        (n) !!
        n--
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("while.cond"), "IR should contain while condition block");
            Assert.IsTrue(ir.Contains("while.body"), "IR should contain while body block");
            Assert.IsTrue(ir.Contains("while.end"), "IR should contain while end block");
        }

        [TestMethod]
        public void TestRepeatLoop()
        {
            string code = @"
stock:

recipe: PrintNumbers() {
    repeatUp(0, 10):
        (""Counting"") !!
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("repeat.cond"), "IR should contain repeat condition block");
            Assert.IsTrue(ir.Contains("repeat.body"), "IR should contain repeat body block");
        }

        [TestMethod]
        public void TestComparisonOperations()
        {
            string code = @"
stock:

recipe: Compare(sugar a, sugar b) yum {
    yum result :: a == b
    pop result
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("icmp"), "IR should contain comparison");
        }

        [TestMethod]
        public void TestLogicalOperations()
        {
            string code = @"
stock:

recipe: LogicTest(yum a, yum b) yum {
    yum andResult :: a & b
    yum orResult :: a | b
    yum notResult :: !a
    pop andResult
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("and"), "IR should contain AND operation");
            Assert.IsTrue(ir.Contains("or"), "IR should contain OR operation");
        }

        [TestMethod]
        public void TestFloatingPoint()
        {
            string code = @"
stock:

recipe: Average(carb a, carb b) carb {
    carb sum :: a + b
    carb result :: sum / 2.0
    pop result
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("fadd"), "IR should contain floating-point addition");
            Assert.IsTrue(ir.Contains("fdiv"), "IR should contain floating-point division");
        }

        [TestMethod]
        public void TestStringLiteral()
        {
            string code = @"
stock:

recipe: Greet() {
    (""Hello, BubblGum!"") !!
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("Hello, BubblGum!"), "IR should contain string literal");
        }

        [TestMethod]
        public void TestMultipleFunctions()
        {
            string code = @"
stock:

recipe: Add(sugar a, sugar b) sugar {
    pop a + b
}

recipe: Multiply(sugar a, sugar b) sugar {
    pop a * b
}

recipe: Main() {
    sugar x :: Add(5, 3)
    sugar y :: Multiply(x, 2)
    (y) !!
}
";
            CreateTestFile("test.bbgm", code);
            var testPath = Path.Combine(testDir, "test.bbgm");

            var output = new StringWriter();
            bool success = BubblGum.CompileToLLVM(testPath, output);

            Assert.IsTrue(success, "Compilation should succeed");

            var outputPath = Path.Combine(testDir, "output.ll");
            var ir = File.ReadAllText(outputPath);
            Assert.IsTrue(ir.Contains("Add"), "IR should contain Add function");
            Assert.IsTrue(ir.Contains("Multiply"), "IR should contain Multiply function");
            Assert.IsTrue(ir.Contains("Main"), "IR should contain Main function");
            Assert.IsTrue(ir.Contains("call"), "IR should contain function calls");
        }
    }
}
