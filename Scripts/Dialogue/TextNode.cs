public class TextNode : DialogueNode {
	private string name;
	private string icon;
	private string text;
	private string nextNode;

	public string GetName() {
		return name;
	}

	public string GetText() {
		return text;
	}

	public string GetIcon() {
		return icon;
	}

    public override string GetNextNode() {
		return nextNode;
    }
}
