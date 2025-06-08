using UnityEngine;
using UnityEngine.Events;

public class StatueCoin : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string targetQuestID;
    [SerializeField] private string targetItemID;
    [SerializeField] private string saveKey = "StatueCoin_Collected";

    [Header("Диалоги")]
    public DialogueData questNotActiveDialogue;
    public DialogueData questActiveDialogue;
    public DialogueData alreadyCollectedDialogue;

    [Header("Компоненты")]
    public Animator animator;
    public string collectTrigger = "Collected";

    [Header("События")]
    public UnityEvent onCoinCollected;

    private DialogueManager dialogueManager;
    private bool isCoinCollected;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        LoadCoinState();
    }

    public void Interact()
    {
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager не найден!");
            return;
        }

        if (isCoinCollected)
        {
            dialogueManager.StartDialogue(alreadyCollectedDialogue, "statue");
            return;
        }

        if (!IsTargetQuestActive())
        {
            dialogueManager.StartDialogue(questNotActiveDialogue, "statue");
            return;
        }

        // Если квест активен и монетка не собрана
        dialogueManager.StartDialogue(questActiveDialogue, "statue");
        GiveCoin();
    }

    private bool IsTargetQuestActive()
    {
        if (QuestSystem.Instance == null)
        {
            Debug.LogWarning("QuestSystem.Instance не найден!");
            return false;
        }

        foreach (Quest quest in QuestSystem.Instance.GetActiveQuests())
        {
            if (quest.questID == targetQuestID)
                return true;
        }

        return false;
    }

    private void GiveCoin()
    {
        if (ItemGiver.Instance == null)
        {
            Debug.LogError("ItemGiver.Instance не найден!");
            return;
        }

        ItemGiver.Instance.GiveItem(targetItemID);
        isCoinCollected = true;
        SaveCoinState();

        // Анимация
        if (animator != null)
        {
            animator.SetTrigger(collectTrigger);
        }

        onCoinCollected.Invoke();
    }

    private void SaveCoinState()
    {
        PlayerPrefs.SetInt(saveKey, isCoinCollected ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadCoinState()
    {
        isCoinCollected = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (isCoinCollected && animator != null)
        {
            animator.SetTrigger(collectTrigger);
        }
    }

    // Для отладки
    [ContextMenu("Сбросить состояние монетки")]
    public void ResetCoinState()
    {
        isCoinCollected = false;
        PlayerPrefs.DeleteKey(saveKey);
        if (animator != null)
        {
            animator.ResetTrigger(collectTrigger);
        }
    }
}