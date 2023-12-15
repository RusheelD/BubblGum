
using System;
using System.IO;

[TestClass]
public class TestBubblGumParser
{
    public const string TEST_FILES_LOCATION = "./ParserTests";
    public const string TE

    // tests BubblGum parser. Takes in a file name and wehther or not
    // scanning should succeed with 0 errors
    private void parse(string testCaseName, boolean shouldSucceed)
    {
        BubblGum bG = new BubblGum();
        bG.Execute(CompilerMode.Parser, "./Tests/HelloWorld.bbgm");

        StringBuilder sb = TEST_FILES_LOCATION;
        sb.Append(testCaseName);
        sb.Append(TestConstants.FILE_EXTENSION)
        string inputFile = sb.ToString();

        sb.Clear();
        sb.Append(testCaseName);
        sb.Append(TestConstants.FILE_EXTENSION)
        string inputFile = sb.ToString();

        Asser
    }


    [TestMethod]
    public void testHelloWorld() {

    }
}
