namespace tests;

[TestClass]
public class DialogueTreeTest {
    [TestMethod]
    public void DialogueTreeInstantiatedCorrectlyTest() {
        ExampleDialogueTree tree = new ExampleDialogueTree();
        Assert.AreEqual(tree.playerGold, 10);
    }
}