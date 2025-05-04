using System;

[Serializable]
public class QuestObjective
{
    public string objectiveName;
    public ObjectiveType objectiveType;
    public string targetId;
    public int requiredAmount = 1;
    public int currentAmount = 0;

    public int orderIndex = 0;

    public bool isCompleted => currentAmount >= requiredAmount;
}

public enum ObjectiveType
{
    KillEnemy,
    CollectItem,
    TalkToNpc
}
