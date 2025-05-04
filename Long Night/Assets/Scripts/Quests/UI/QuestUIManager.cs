using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    public GameObject questPanel;
    public GameObject questEntryPrefab;

    private List<GameObject> questEntries = new();

    private void Start()
    {
        GameStateManager.Instance.OnQuestUpdated += UpdateUI; // 👈 подписка
        UpdateUI(); // 👈 начальное обновление
    }

    public void UpdateUI()
    {
        Clear();

        foreach (var quest in GameStateManager.Instance.activeQuests)
        {
            if (quest.currentState != QuestState.InProgress)
                continue;

            GameObject entry = Instantiate(questEntryPrefab, questPanel.transform);
            QuestUIEntry ui = entry.GetComponent<QuestUIEntry>();
            ui.Setup(quest);
            questEntries.Add(entry);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(questPanel.GetComponent<RectTransform>());
        
    }

    private void Clear()
    {
        foreach (var entry in questEntries)
            Destroy(entry);

        questEntries.Clear();
    }
}
