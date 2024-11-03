/*
    EventNode represents an event that occurs in the dialogue tree.
    e.g. giving the player a reward before continuing the conversation

    EventNode node = new EventNode(() => {
        player.AddExp(100);
        player.AddGold(100);
    }, 100);
*/

using System;

public class EventNode : BaseNode {
    private string _nextNodeKey = "";

    private Action _eventAction;

    public EventNode(Action eventAction, string nextNodeKey) {
        _eventAction = eventAction;
        _nextNodeKey = nextNodeKey;
    }

    public override string GetNextNodeKey() {
        _eventAction();
        return _nextNodeKey;
    }
}