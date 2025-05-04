using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    public List<QuestStateMachine> activeQuests = new();

    public event Action OnQuestUpdated; // 👈 событие

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }

    public void AddQuest(QuestData data)
    {
        if (activeQuests.Exists(q => q.QuestId == data.questId))
        {
            Debug.LogWarning($"[GameState] Quest already active: {data.questName}");
            return;
        }

        var quest = new QuestStateMachine(data);
        activeQuests.Add(quest);
        quest.StartQuest();

        NotifyQuestUpdated(); // 👈 обновляем UI при добавлении квеста
    }

    public QuestStateMachine GetQuest(string questId)
    {
        return activeQuests.Find(q => q.QuestId == questId);
    }

    public void NotifyQuestUpdated() // 👈 вызывается при изменении квеста
    {
        OnQuestUpdated?.Invoke();
    }
}
