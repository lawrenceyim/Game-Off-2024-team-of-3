public class ExampleDialogueTree {
    private DialogueTree _tree;
    public bool firstTimeMeeting = false;
    public bool questAssigned = false;
    public bool questFinished = false;
    public int playerRewardAmount = 100;
    public int assignedQuestnumber = -1;
    public int playerGold = 10;

    public ExampleDialogueTree() {
        string nullKey = "";
        string firstTimeMeetingKey = "first_time_meeting";
        string greetingKey = "greeting";
        string questAssignedKey = "quest_assigned";
        string questFinishedKey = "quest_finished";
        string assignQuestKey = "assign_quest";
        string quest1Key = "quest_1";
        string quest2Key = "quest_2";
        string quest3Key = "quest_3";
        string congratulationKey = "congratulation";
        string waitingKey = "waiting";
        string assignQuest1Key = "assign_quest_1";
        string assignQuest2Key = "assign_quest_2";
        string assignQuest3Key = "assign_quest_3";
        string givePlayerRewardKey = "give_player_reward";

        _tree = new DialogueTree(firstTimeMeetingKey);
        
        _tree.AddNode(firstTimeMeetingKey, new BooleanNode(greetingKey, questAssignedKey, () => firstTimeMeeting));
        
        _tree.AddNode(greetingKey, new TextNode(nullKey, nullKey, "Greetings player.", nullKey));
        _tree.AddNode(questAssignedKey, new BooleanNode(questFinishedKey, assignQuestKey, () => questAssigned));

        _tree.AddNode(assignQuestKey, new QuestionNode(nullKey, nullKey, "Which quest do you want to take?",
            new [] {"Quest 1", "Quest 2", "Quest 3"}, // This would use the key for the translated text in prod
            new [] {quest1Key, quest2Key, quest3Key}));
        _tree.AddNode(questFinishedKey, new BooleanNode(congratulationKey, waitingKey, () => questFinished));

        _tree.AddNode(assignQuest1Key, new EventNode(() => { questAssigned = true; assignedQuestnumber = 1; }, nullKey));
        _tree.AddNode(assignQuest2Key, new EventNode(() => { questAssigned = true; assignedQuestnumber = 2; }, nullKey));
        _tree.AddNode(assignQuest3Key, new EventNode(() => { questAssigned = true; assignedQuestnumber = 3; }, nullKey));
        _tree.AddNode(givePlayerRewardKey, new EventNode(() => playerGold += playerRewardAmount, nullKey));
    }
}