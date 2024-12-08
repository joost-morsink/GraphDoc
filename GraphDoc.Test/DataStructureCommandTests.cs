namespace GraphDoc.Test;

[TestClass]
public class DataStructureCommandTests : VerifyBase
{
    [TestMethod]
    public async Task SimpleDataStructureTest()
    {
        var str = CommandLineArguments.Parse(["datastructure", "GraphDoc.Test.dll", "Person"]);
        var result = await str.Execute();

        await Verify(result);
    }
    [TestMethod]
    public async Task VerboseSimpleDataStructureTest()
    {
        var str = CommandLineArguments.Parse(["datastructure", "GraphDoc.Test.dll", "Person", "-s", "mermaid"]);
        var result = await str.Execute();

        await Verify(result);
    }
    
}