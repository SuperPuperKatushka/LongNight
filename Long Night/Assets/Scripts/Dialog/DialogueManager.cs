using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;
    public GameObject dialogObject;
    

    private Queue<string> sentences;
    private bool isDialogueActive = false;

    private void Start()
    {
        sentences = new Queue<string>();
        dialogObject.SetActive(false);
        
    }
    private void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextSentences();
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        // Открытие диалога

        dialogObject.SetActive(true);
        isDialogueActive = true;
        nameText.text = dialogue.name;
        sentences.Clear();

        foreach (string sentence in dialogue.sentences)
        {
            sentences.Enqueue(sentence);
        }

    } 

    public void DisplayNextSentences()
    {
        if (sentences.Count == 0) {
            EndDialogue();
            return;
         }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));

    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return null;
        }
    }

    public void EndDialogue()
    {
        // Логика заверщения диалога
        isDialogueActive = false;
        dialogObject.SetActive(false);


    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

}
