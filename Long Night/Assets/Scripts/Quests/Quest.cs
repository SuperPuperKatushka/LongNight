using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

// ����� ����������� �����
public class Quest
{
    public enum QuestState
    {
        NotAvailable, // ����� ��� �� ��������
        Available,    // ����� �������� ��� ��������
        Active,       // ����� �������
        Completed,    // ����� ��������
        Rewarded      // ������� ��������
    }

    public string questID;
    public string title;
    public string description;
    public QuestState state = QuestState.NotAvailable;

    public List<QuestObjective> objectives = new List<QuestObjective>();
    public List<QuestReward> rewards = new List<QuestReward>();

    // ���������� ��� ��������� ������
    public List<QuestRequirement> requirements = new List<QuestRequirement>();

    // ���������, �������� �� �����
    public bool IsComplete()
    {
        foreach (var objective in objectives)
        {
            if (!objective.IsComplete)
                return false;
        }
        return true;
    }

    // ���������, �������� �� ����� ��� ��������
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

    // ��������� ������ ������
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