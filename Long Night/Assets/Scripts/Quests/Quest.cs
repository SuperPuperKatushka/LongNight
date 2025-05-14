using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

// Класс описывающий квест
public class Quest
{
    public enum QuestState
    {
        NotAvailable, // Квест еще не доступен
        Available,    // Квест доступен для принятия
        Active,       // Квест активен
        Completed,    // Квест выполнен
        Rewarded      // Награда получена
    }

    public string questID;
    public string title;
    public string description;
    public QuestState state = QuestState.NotAvailable;

    public List<QuestObjective> objectives = new List<QuestObjective>();
    public List<QuestReward> rewards = new List<QuestReward>();

    // Требования для появления квеста
    public List<QuestRequirement> requirements = new List<QuestRequirement>();

    // Проверяет, выполнен ли квест
    public bool IsComplete()
    {
        foreach (var objective in objectives)
        {
            if (!objective.IsComplete)
                return false;
        }
        return true;
    }

    // Проверяет, доступен ли квест для принятия
    public bool IsAvailable()
    {
        foreach (var requirement in requirements)
        {
            if (!requirement.IsMet())
                return false;
        }
        return true;
    }

    public QuestState GetQuestState()
    {
        return state;
    }

    // Обновляет статус квеста
    public void UpdateState()
    {
        if (state == QuestState.NotAvailable && IsAvailable())
        {
            state = QuestState.Available;
        }
        else if (state == QuestState.Active && IsComplete())
        {
            state = QuestState.Completed;
        }
    }
}