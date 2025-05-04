using UnityEngine;

public enum QuestState { NotStarted, InProgress, Completed }

public class QuestStateMachine
{
    public QuestData data;
    public QuestState currentState = QuestState.NotStarted;

    public string QuestId => data.questId;

    public QuestStateMachine(QuestData questData)
    {
        data = questData;
        Debug.Log($"[Quest] Created: {data.questName}");
    }

    public void StartQuest()
    {
        if (currentState != QuestState.NotStarted) return;

        currentState = QuestState.InProgress;
        Debug.Log($"[Quest] Started: {data.questName}");

        QuestEvents.Instance.OnEnemyKilled += HandleEnemyKilled;
        QuestEvents.Instance.OnItemCollected += HandleItemCollected;
        QuestEvents.Instance.OnNpcTalked += HandleNpcTalked;
    }

    private void HandleEnemyKilled(string id) => TryProgress(ObjectiveType.KillEnemy, id);
    private void HandleItemCollected(string id) => TryProgress(ObjectiveType.CollectItem, id);
    private void HandleNpcTalked(string id) => TryProgress(ObjectiveType.TalkToNpc, id);

    private void TryProgress(ObjectiveType type, string targetId)
    {
        Debug.Log("TryProgress");
        if (currentState != QuestState.InProgress) return;

        for (int i = 0; i < data.objectives.Length; i++)
        {
            var obj = data.objectives[i];
            if (obj.objectiveType != type || obj.targetId != targetId || obj.isCompleted) continue;

            if (data.sequential && !IsObjectiveActive(i))
            {
                Debug.Log($"[Quest] Objective '{obj.objectiveName}' is locked due to sequence.");
                return;
            }

            obj.currentAmount++;
            Debug.Log($"[Quest] Objective Progress: {obj.objectiveName} ({obj.currentAmount}/{obj.requiredAmount})");

            if (obj.isCompleted)
                Debug.Log($"[Quest] Objective Complete: {obj.objectiveName}");

            GameStateManager.Instance.NotifyQuestUpdated(); // 👈 обновить UI при прогрессе

            if (AllObjectivesComplete())
            {
                currentState = QuestState.Completed;
                Debug.Log($"[Quest] COMPLETED: {data.questName}");
                UnsubscribeEvents();
                GameStateManager.Instance.NotifyQuestUpdated(); // 👈 обновить UI при завершении
            }

            return;
        }

        Debug.Log($"[Quest] No matching objective found for type {type} and target '{targetId}'");
    }

    public bool IsObjectiveActive(int index)
    {
        if (!data.sequential) return true;

        for (int i = 0; i < index; i++)
            if (!data.objectives[i].isCompleted)
                return false;

        return true;
    }

    public bool AllObjectivesComplete()
    {
        foreach (var obj in data.objectives)
            if (!obj.isCompleted)
                return false;
        return true;
    }

    private void UnsubscribeEvents()
    {
        QuestEvents.Instance.OnEnemyKilled -= HandleEnemyKilled;
        QuestEvents.Instance.OnItemCollected -= HandleItemCollected;
        QuestEvents.Instance.OnNpcTalked -= HandleNpcTalked;
        Debug.Log($"[Quest] Unsubscribed events: {data.questName}");
    }
}
