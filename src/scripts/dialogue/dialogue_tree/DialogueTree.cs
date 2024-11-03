using System.Collections.Generic;

public class DialogueTree {
    private Dictionary<string, BaseNode> _nodeDict = new Dictionary<string, BaseNode>();
    private string _rootKey = "";

    public DialogueTree(string rootKey) {
        _rootKey = rootKey;
    }

    public void AddNode(string nodeKey, BaseNode node) {
        _nodeDict.Add(nodeKey, node);
    }

    public BaseNode GetRootNode() {
        return _nodeDict.GetValueOrDefault(_rootKey, null);
    }

    public BaseNode GetNodeByKey(string key) {
        return _nodeDict.GetValueOrDefault(key, null);
    }

    // TODO: Change to handle complex logic involving events
    public BaseNode Evaluate() {
        BaseNode current = _nodeDict.GetValueOrDefault(_rootKey, null);
        while (current is BooleanNode) {
            current = _nodeDict.GetValueOrDefault(current.GetNextNodeKey(), null);
        }
        return current;
    }
}
