using UnityEngine;

[System.Serializable]

// ����������� ����� ����� ������
public abstract class QuestObjective
{
    public string description;
    public bool isOptional;
    public bool IsComplete { get; protected set; }

    public abstract void Initialize();
    public abstract void CheckProgress();
    public abstract string GetProgressText();
}

// ������ ���������� ���� - ������� ��������
[System.Serializable]
public class CollectObjective : QuestObjective
{
    public string itemID;
    public int requiredAmount;
    public int currentAmount;

    public override void Initialize()
    {
        currentAmount = 0;
        IsComplete = false;
        // ������������� �� ������� ��������� ��������
        Inventory.OnItemPickUp += OnItemCollected;
    }

 
    private void OnItemCollected(string collectedItemID)
    {

        if (collectedItemID == itemID)
        {
            currentAmount++;
            if (currentAmount >= requiredAmount)
            {
                IsComplete = true;
            }
            CheckProgress();
        }
    }
    public override void CheckProgress()
    {
        // ����� �������� �������������� ������ ��������
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentAmount}/{requiredAmount})";
    }
}

public class InteractObjective : QuestObjective
{
    public string interactableId; // ������������� ��� ������������
    public int requiredInteractions = 1; // ������������� ��� �������
    public int currentInteractions;

    public override void Initialize()
    {
        Debug.Log("HandleInteraction init ");

        currentInteractions = 0;
        IsComplete = false;

        // ������������� �� ����������� �������
        InteractableObject.OnInteracted += HandleInteraction;
    }

    private void HandleInteraction(string interactedId)
    {
        if (interactedId == interactableId)
        {
            currentInteractions++;
            if (currentInteractions >= requiredInteractions)
            {
                currentInteractions = requiredInteractions;
                IsComplete = true;
            }
            CheckProgress();
        }
    }

    public override void CheckProgress()
    {
        // �������������� ������ ��� �������������
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentInteractions}/{requiredInteractions})";
    }

    // ����� ���������� ��� �����������
    private void OnDestroy()
    {
        //InteractableObject.OnInteracted -= HandleInteraction;
    }
}

public class TalkObjective : QuestObjective
{
    public string npcID;
    public int requiredAmount;
    public int currentAmount;

    public override void Initialize()
    {

        currentAmount = 0;
        IsComplete = false;
        DialogueManager.OnTalkEnd -= OnTalkWithNPC;
        DialogueManager.OnTalkEnd += OnTalkWithNPC;
    }

    private void OnTalkWithNPC(string talkNpcID)
    {

        if (talkNpcID == npcID)
        {
            currentAmount++;
            if (currentAmount >= requiredAmount)
            {
                IsComplete = true;
            }
            CheckProgress();
        }
    }
    public override void CheckProgress()
    {
        // ����� �������� �������������� ������ ��������
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentAmount}/{requiredAmount})";
    }
}

// ������ ���� - ����� ������
[System.Serializable]
public class KillObjective : QuestObjective
{
    public string enemyID;
    public int requiredKills;
    public int currentKills;

    public override void Initialize()
    {
        currentKills = 0;
        IsComplete = false;
        //Enemy.OnEnemyDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath(string killedEnemyID)
    {
        if (killedEnemyID == enemyID)
        {
            currentKills++;
            if (currentKills >= requiredKills)
            {
                IsComplete = true;
            }
            CheckProgress();
        }
    }

    public override void CheckProgress()
    {
        // �������������� ������ ��������
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentKills}/{requiredKills})";
    }
}