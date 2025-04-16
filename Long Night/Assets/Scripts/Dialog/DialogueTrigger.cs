using System;
using UnityEngine;
using UnityEngine;

using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public DialogueData dialogue; // ���������� ScriptableObject
    private DialogueManager dialogueManager;
    private bool isPlayerInRange = false;

    private void Start()
    {
        dialogueManager = FindFirstObjectByType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager �� ������ �� �����!");
        }
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialogueManager.IsDialogueActive())
            {
                dialogueManager.StartDialogue(dialogue);
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
