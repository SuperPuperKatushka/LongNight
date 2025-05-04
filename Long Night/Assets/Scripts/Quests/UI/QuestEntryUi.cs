using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using log4net.Layout;
using System;

public class QuestUIEntry : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Transform objectivesContainer;
    public GameObject objectiveTextPrefab;


    public void Setup(QuestStateMachine quest)
    {
        titleText.text = quest.data.questName;
        descriptionText.text = quest.data.description;

        // Удаляем старые цели
        foreach (Transform child in objectivesContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var obj in quest.data.objectives)
        {
            GameObject textObj = Instantiate(objectiveTextPrefab, objectivesContainer);
            textObj.SetActive(true);
            var txt = textObj.GetComponent<TMP_Text>();

            txt.text = $"• {obj.objectiveName} ({obj.currentAmount}/{obj.requiredAmount})";
            txt.color = obj.isCompleted ? Color.green : Color.white;
        }

        // 💥 Ключевая строка:
        LayoutRebuilder.ForceRebuildLayoutImmediate(objectivesContainer.GetComponent<RectTransform>());
    }


}
