using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject dialogObject;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public Transform choicesContainer;
    public GameObject choiceButtonPrefab;

    private DialogueData currentDialogue;
    private int currentNodeIndex = 0;
    private bool isDialogueActive = false;

    public bool IsDialogueActive() => isDialogueActive;


    private void Start()
    {
        isDialogueActive = false;
        dialogObject.SetActive(false);

    }


    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentNodeIndex = 0;
        isDialogueActive = true;
        dialogObject.SetActive(true);
        nameText.text = dialogue.npcName;

        ShowCurrentNode();
    }

    private void ShowCurrentNode()
    {
        ClearChoices();

        if (currentNodeIndex >= currentDialogue.nodes.Length)
        {
            EndDialogue();
            return;
        }

        DialogueNode node = currentDialogue.nodes[currentNodeIndex];
        StopAllCoroutines();
        StartCoroutine(TypeSentence(node.sentence));

        // Если есть варианты ответа — показать
        if (node.choices != null && node.choices.Length > 0)
        {
            foreach (DialogueChoice choice in node.choices)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choicesContainer);
                TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = choice.choiceText;

                Button button = buttonObj.GetComponent<Button>();
                int nextIndex = choice.nextNodeIndex;
                button.onClick.AddListener(() => OnChoiceSelected(nextIndex));
            }
        }
        else
        {
            // Если нет выбора — нажми Space для продолжения
            StartCoroutine(WaitForContinue());
        }
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence)
        {
            dialogueText.text += letter;
            yield return null; // можно задержку: yield return new WaitForSeconds(0.02f);
        }
    }

    IEnumerator WaitForContinue()
    {
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        currentNodeIndex++;
        ShowCurrentNode();
    }

    private void OnChoiceSelected(int nextNode)
    {
        if (nextNode < 0 || nextNode >= currentDialogue.nodes.Length)
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

    public void EndDialogue()
    {
        isDialogueActive = false;
        dialogObject.SetActive(false);
        ClearChoices();
    }
}