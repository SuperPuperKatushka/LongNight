using System;
using TMPro;
using UnityEngine;
using UnityEngine;

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{

    public DialogueData dialogue; // Подключаем ScriptableObject
    private DialogueManager dialogueManager;
    public string npcId;
    private bool isPlayerInRange = false;

    private void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager не найден на сцене!");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialogueManager.IsDialogueActive())
            {
                dialogueManager.StartDialogue(dialogue, npcId);
        }
    }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
