public class TextNode : BaseNode {
	private string _name;
	private string _icon;
	private string _text;
	private string _nextNodeKey = "";

	public TextNode(string name, string icon, string text, string nextNodeKey) {
		_name = name;
		_icon = icon;
		_text = text;
		_nextNodeKey = nextNodeKey;
	}

	public string GetName() {
		return _name;
	}

	public string GetText() {
		return _text;
	}

	public string GetIcon() {
		return _icon;
	}

	public override string GetNextNodeKey() {
		return _nextNodeKey;
	}
}
