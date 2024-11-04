using System;

public class QuestionNode : BaseNode {
    private string _name;
    private string _icon;
    private string _question;
    private string[] _answerKeys;
    private string[] _nextNodeKeys;
    private string _nextNodeKey = "";

    public QuestionNode(string name, string icon, string question, string[] answerKeys, string[] nextNodeKeys) {
        _name = name;
        _icon = icon;
        _question = question;
        _answerKeys = answerKeys;
        _nextNodeKeys = nextNodeKeys;
    }

    public string GetName() {
        return _name;
    }

    public string GetIcon() {
        return _icon;
    }

    public string GetQuestion() {
        return _question;
    }

    public string[] GetAnswers() {
        return _answerKeys;
    }

    public void SetNextNode(int answerIndex) {
        _nextNodeKey = _nextNodeKeys[answerIndex];
    }

    public override string GetNextNodeKey() {
        if (_nextNodeKey == null) {
            throw new Exception("Next node key has not been set.");
        }
        return _nextNodeKey;
    }
}