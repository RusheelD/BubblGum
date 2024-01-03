using System;
using System.IO;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.RegularExpressions;

[TestClass]
public class TestParser
{
    public const string TEST_FILES_LOCATION = "./Lexer_Parser_Tests";
    public const string TEMP_TEST_FILE_NAME = "ParserOutput.bbgm";

    // Tests BubblGum parser. Takes in a file name and whether
    // scanning should succeed with 0 errors
    private void parse(string testCaseName, bool shouldSucceed)
    {
        var sb = new StringBuilder(TEST_FILES_LOCATION);
        sb.Append(testCaseName);
        sb.Append(Constants.FILE_EXTENSION);
        string inputFile = Path.GetFullPath(sb.ToString());

        sb.Clear();
        sb.Append(testCaseName);
        sb.Append(Constants.FILE_EXTENSION);
        string expectedFile = Path.GetFullPath(sb.ToString());

        bool doTestFilesExist = File.Exists(inputFile) && (!shouldSucceed || File.Exists(expectedFile));
        if (!doTestFilesExist)
            Assert.Fail($"Files {inputFile} and {expectedFile} could not be found");

        //try parsing
        try
        {
            // parse test file and if successful, store output in memory stream
            var memoryStream = new MemoryStream();
            var outputStream = new StreamWriter(TEMP_TEST_FILE_NAME, false);
            bool isSuccess = BubblGum.ExecuteParser(new List<string>() { inputFile }, outputStream);
            Assert.AreEqual(shouldSucceed, isSuccess);

            // compare received output to expected output
            var outputText = new StreamReader(TEMP_TEST_FILE_NAME).ReadToEnd();
            string[] output = Regex.Split(outputText, "[\n (\r\n)\r]");

            string expectedText = File.ReadAllText(expectedFile);
            string[] expected = Regex.Split(expectedText, "[\n (\r\n)\r]");

            Assert.AreEqual(output.Length, expected.Length);
            for (int i = 0; i < output.Length; i++)
            {
                if (!output[i].Equals(expected[i]))
                    Assert.Fail($"expected {expected[i]} but got {output[i]}");
            }

            Assert.AreEqual(shouldSucceed, isSuccess);

        }
        catch
        {
            Assert.IsFalse(shouldSucceed);
        }
    }


    [TestMethod]
    public void testIfStatements()
    {

    }
}
