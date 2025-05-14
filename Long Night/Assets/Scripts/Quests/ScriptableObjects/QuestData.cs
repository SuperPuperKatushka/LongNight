using UnityEngine;

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quests/Quest Data")]
public class QuestData : ScriptableObject
{
    public string questID;
    public string title;
    [TextArea] public string description;

    public QuestObjectiveData[] objectives;
    public QuestRewardData[] rewards;
    public QuestRequirementData[] requirements;
}

// Классы для данных (упрощённые версии)
[System.Serializable]
public class QuestObjectiveData
{
    public enum ObjectiveType { Kill, Collect, Talk, Interact }

    public ObjectiveType type;
    public string description;
    public string targetID; // ID врага, предмета или NPC
    public int requiredAmount;
}

[System.Serializable]
public class QuestRewardData
{
    public enum RewardType { Item, Experience, Currency }

    public RewardType type;
    public string itemID; // Для предметов
    public int amount;
}

[System.Serializable]
public class QuestRequirementData
{
    public enum RequirementType { Level, Quest }

    public RequirementType type;
    public int requiredLevel; // Для уровня
    public string requiredQuestID; // Для квестов
    public Quest.QuestState requiredQuestState;
}