using System;
using System.Collections.Generic;

public class DialogueTree {
    private Dictionary<string, BaseNode> _nodeDict = new Dictionary<string, BaseNode>();
    private string _rootKey = "";
    private int _expectedNodeCount;

    public DialogueTree(int expectedNodeCount, string rootKey) {
        _rootKey = rootKey;
        _expectedNodeCount = expectedNodeCount;
    }

    public void VerifyNodeCount() {
        if (_nodeDict.Count != _expectedNodeCount) {
            throw new Exception("The dialogue tree does not have the expected node count.");
        }
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
