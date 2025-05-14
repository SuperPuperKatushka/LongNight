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

// ������ ��� ������ (���������� ������)
[System.Serializable]
public class QuestObjectiveData
{
    public enum ObjectiveType { Kill, Collect, Talk, Interact }

    public ObjectiveType type;
    public string description;
    public string targetID; // ID �����, �������� ��� NPC
    public int requiredAmount;
}

[System.Serializable]
public class QuestRewardData
{
    public enum RewardType { Item, Experience, Currency }

    public RewardType type;
    public string itemID; // ��� ���������
    public int amount;
}

[System.Serializable]
public class QuestRequirementData
{
    public enum RequirementType { Level, Quest }

    public RequirementType type;
    public int requiredLevel; // ��� ������
    public string requiredQuestID; // ��� �������
    public Quest.QuestState requiredQuestState;
}