using System;

public class BooleanNode : BaseNode {
    string _trueNodeKey = "";
    string _falseNodeKey = "";
    Func<bool> _evaluation;

    public BooleanNode(string trueNodeKey, string falseNodeKey, Func<bool> evaluation) {
        _trueNodeKey = trueNodeKey;
        _falseNodeKey = falseNodeKey;
        _evaluation = evaluation;
    }

    public override string GetNextNodeKey() {
        return _evaluation() ? _trueNodeKey : _falseNodeKey;
    }
}