[System.Serializable]
public abstract class QuestRequirementAbstract
{
    public abstract bool IsMet();
}

[System.Serializable]
public class LevelRequirement : QuestRequirement
{
    public int requiredLevel;

    public override bool IsMet()
    {
        //return PlayerStats.Level >= requiredLevel;
        return true;
    }
}

[System.Serializable]
public class QuestRequirement : QuestRequirementAbstract
{
    public string requiredQuestID;
    public Quest.QuestState requiredState;

    public override bool IsMet()
    {
        Quest requiredQuest = QuestSystem.GetQuest(requiredQuestID);
        return requiredQuest != null && requiredQuest.state == requiredState;
    }
}