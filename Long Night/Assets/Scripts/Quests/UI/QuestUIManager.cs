using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    public GameObject questPanel;
    public GameObject questEntryPrefab;
    public Button toggleButton;

    private List<GameObject> questEntries = new List<GameObject>();

    private void Start()
    {

        // Подписываемся на события
        QuestSystem.Instance.OnQuestStateChanged += UpdateUI;

        UpdateUI();
    }


    private void OnDestroy()
    {
        QuestSystem.Instance.OnQuestStateChanged -= UpdateUI;
    }

    public void UpdateUI()
    {
        Clear();
        //Debug.Log(" GetActiveQuests " + QuestSystem.Instance.GetActiveQuests());
        // Отображаем только активные квесты
        foreach (var quest in QuestSystem.Instance.GetActiveQuests())
        {
            
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
        {
            Destroy(entry);
        }
        questEntries.Clear();
    }

   
}