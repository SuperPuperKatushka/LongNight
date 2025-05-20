using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestUIEntry : MonoBehaviour
{
    public TMP_Text titleText;
    public TMP_Text descriptionText;
    public Transform objectivesContainer;
    public GameObject objectiveTextPrefab;

    public void Setup(Quest quest)
    {
        titleText.text = quest.title;
        descriptionText.text = quest.description;

        // Очищаем предыдущие цели
        foreach (Transform child in objectivesContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые цели
        foreach (var objective in quest.objectives)
        {
            GameObject textObj = Instantiate(objectiveTextPrefab, objectivesContainer);
            textObj.SetActive(true);
            var txt = textObj.GetComponent<TMP_Text>();

            txt.text = $"• {objective.GetProgressText()}";
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(objectivesContainer.GetComponent<RectTransform>());
    }
}
