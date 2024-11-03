namespace tests;

[TestClass]
public class DialogueTreeTest {
    [TestMethod]
    public void FirstTimeMeetingTrueTest() {
        ExampleDialogueTree tree = new ExampleDialogueTree();
        tree.firstTimeMeeting = true;

        BaseNode node = tree.GetEvaluation();

        Assert.IsInstanceOfType(node, typeof(TextNode), "Expected a TextNode if it is the first time meeting.");
    }

    [TestMethod]
    public void QuestAssignedButNotFinishedTest() {
        ExampleDialogueTree tree = new ExampleDialogueTree();
        tree.firstTimeMeeting = false;
        tree.questAssigned = true;
        tree.assignedQuestnumber = 1;
        tree.questFinished = false;

        BaseNode node = tree.GetEvaluation();

        Assert.IsInstanceOfType(node, typeof(TextNode), "Expected a TextNode if quest is assigned but not finished.");
    }

    [TestMethod] 
    public void QuestAssignedAndFinishedTest() {
        ExampleDialogueTree tree = new ExampleDialogueTree();
        tree.firstTimeMeeting = false;
        tree.questAssigned = true;
        tree.assignedQuestnumber = 1;
        tree.questFinished = true;

        BaseNode node = tree.GetEvaluation();
        Assert.IsInstanceOfType(node, typeof(TextNode), "Expected a TextNode if quest is assigned and finished.");
        node = tree.GetNextNodeByKey(node.GetNextNodeKey());
        Assert.IsInstanceOfType(node, typeof(EventNode), "Expected an EventNode after the TextNode.");
        Assert.AreEqual(tree.playerGold, 10, "Expected player to have 10 gold before receiving award.");
        node = tree.GetNextNodeByKey(node.GetNextNodeKey());
        Assert.IsNull(node, "Expected node to be null after the EventNode.");
        Assert.AreEqual(tree.playerGold, 110, "Expected player to have 110 after receiving reward.");
    }
}