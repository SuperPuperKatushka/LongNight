using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    public static event System.Action<string> OnTalkEnd;

    [Header("UI Elements")]
    public GameObject dialogObject;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;
    private GameObject currentAvatarInstance;
    private string dialogueNpcId;

    public Transform avatarContainer;

    [Header("Events")]
    public UnityEvent onDialogueEnd;

    private DialogueData currentDialogue;
    private int currentNodeIndex = 0;
    private bool isDialogueActive = false;
    private Coroutine typingCoroutine;

    public bool IsDialogueActive() => isDialogueActive;

    public void StartDialogue(DialogueData dialogue, string npcId)
    {
        // Если уже есть активный диалог - прерываем его
        if (isDialogueActive)
        {
            ForceEndDialogue();
        }
        dialogueNpcId = npcId;
        currentDialogue = dialogue;
        currentNodeIndex = 0;
        isDialogueActive = true;
        dialogObject.SetActive(true);

        ShowCurrentNode();
    }

    private void ShowCurrentNode()
    {
        ClearChoices();

        // Получаем актуальные узлы диалога с учетом квестов
        DialogueNode[] nodesToUse = GetCurrentNodes();

        if (currentNodeIndex >= nodesToUse.Length)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = nodesToUse[currentNodeIndex];
        nameText.text = node.speaker.speakerName;

        // Обновляем аватар
        if (currentAvatarInstance != null)
        {
            Destroy(currentAvatarInstance);
        }

        if (node.speaker.avatar != null)
        {
            avatarContainer.gameObject.SetActive(true);
            currentAvatarInstance = Instantiate(node.speaker.avatar, avatarContainer);
            currentAvatarInstance.transform.localPosition = Vector3.zero;
        }
        else
        {
            avatarContainer.gameObject.SetActive(false);
        }

        // Печатаем текст
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeSentence(node.sentence));

        // Обрабатываем выборы
        if (node.choices != null && node.choices.Length > 0)
        {
            foreach (DialogueChoice choice in node.choices)
            {
                CreateChoiceButton(choice);
            }
        }
        else
        {
            StartCoroutine(WaitForContinue());
        }
    }

    private DialogueNode[] GetCurrentNodes()
    {
        List<DialogueOverride> validOverrides = new List<DialogueOverride>();

        // Собираем все подходящие варианты
        foreach (var overrideData in currentDialogue.overrides)
        {
            if (CheckConditions(overrideData.conditions))
            {
                validOverrides.Add(overrideData);
            }
        }

        // Если есть подходящие варианты - выбираем с максимальным приоритетом
        if (validOverrides.Count > 0)
        {
            DialogueOverride bestOverride = validOverrides[0];
            foreach (var overrideData in validOverrides)
            {
                if (overrideData.priority > bestOverride.priority)
                {
                    bestOverride = overrideData;
                }
            }
            return bestOverride.nodes;
        }

        // Возвращаем диалог по умолчанию
        return currentDialogue.defaultNodes;
    }

    private bool CheckConditions(DialogueCondition[] conditions)
    {
        if (QuestSystem.Instance == null) return false;

        foreach (var condition in conditions)
        {
            Quest quest = QuestSystem.GetQuest(condition.questId);
            if (quest == null || quest.state != condition.requiredState)
            {
                return false;
            }
        }
        return true;
    }

    private void CreateChoiceButton(DialogueChoice choice)
    {
        GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
        buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = choice.choiceText;
        buttonObj.GetComponent<Button>().onClick.AddListener(() => OnChoiceSelected(choice.nextNodeIndex));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return null;
        }
        typingCoroutine = null;
    }

    IEnumerator WaitForContinue()
    {
        yield return new WaitUntil(() => !Input.GetKey(KeyCode.Space));
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));

        currentNodeIndex++;
        ShowCurrentNode();
    }

    private void OnChoiceSelected(int nextNode)
    {
        DialogueNode[] nodesToUse = GetCurrentNodes();

        if (nextNode < 0 || nextNode >= nodesToUse.Length)
        {
            EndDialogue();
        }
        else
        {
            currentNodeIndex = nextNode;
            ShowCurrentNode();
        }
    }

    private void ClearChoices()
    {
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    public void ForceEndDialogue()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        EndDialogue();

    }

    public void EndDialogue()
    {
        if (currentDialogue != null)
        {
            currentDialogue.onDialogueEnd?.Invoke();
            OnTalkEnd?.Invoke(currentDialogue.name);
        }

        if (currentAvatarInstance != null)
        {
            Destroy(currentAvatarInstance);
        }

        isDialogueActive = false;
        dialogObject.SetActive(false);
        ClearChoices();
        onDialogueEnd.Invoke();
        OnTalkEnd?.Invoke(dialogueNpcId);
    }
}