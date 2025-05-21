using UnityEngine;

[System.Serializable]

// Абстрактный класс целей квеста
public abstract class QuestObjective
{
    public string description;
    public bool isOptional;
    public bool IsComplete { get; protected set; }

    public abstract void Initialize();
    public abstract void CheckProgress();
    public abstract string GetProgressText();
}

// Пример конкретной цели - собрать предметы
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
        // Подписываемся на событие получения предмета
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
        // Можно добавить дополнительную логику проверки
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentAmount}/{requiredAmount})";
    }
}

public class InteractObjective : QuestObjective
{
    public string interactableId; // Переименовано для соответствия
    public int requiredInteractions = 1; // Переименовано для ясности
    public int currentInteractions;

    public override void Initialize()
    {
        Debug.Log("HandleInteraction init ");

        currentInteractions = 0;
        IsComplete = false;

        // Подписываемся на статическое событие
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
        // Дополнительная логика при необходимости
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentInteractions}/{requiredInteractions})";
    }

    // Важно отписаться при уничтожении
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
        // Можно добавить дополнительную логику проверки
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentAmount}/{requiredAmount})";
    }
}

// Пример цели - убить врагов
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
        // Дополнительная логика проверки
    }

    public override string GetProgressText()
    {
        return $"{description} ({currentKills}/{requiredKills})";
    }
}