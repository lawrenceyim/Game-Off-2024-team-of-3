public class QuestionNode : DialogueNode {
	private string name;
	private string icon;
	private string question;
	private string[] answers;
	private string[] possibleNextNodes;
	private string nextNode;

	public string GetName() {
		return name;
	}

	public string GetIcon() {
		return icon;
	}

	public string GetQuestion() {
		return question;
	}

	public string[] GetAnswers() {
		return answers;
	}

	public void SetNextNode(int answerIndex) {
		nextNode = possibleNextNodes[answerIndex];
	}

    public override string GetNextNode() {
		return nextNode;
    }
}