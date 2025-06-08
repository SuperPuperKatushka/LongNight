using UnityEngine;

public class Fountain : MonoBehaviour
{
    [SerializeField] private string targetQuestID;
    [SerializeField] private string targetItemID;
    private const string SAVE_KEY = "Fountain_CoinGiven"; // Базовый ключ

    [Header("Dialogue Data")]
    public DialogueData notStartQuestDialogue;
    public DialogueData startQuestDialogue;
    public DialogueData hasCoinDialogue;

    private DialogueManager dialogueManager;
    private bool hasCoinGiven = false;

    private void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        LoadState();
    }

    public void TryGiveItem()
    {
        if (dialogueManager == null) return;

        if (hasCoinGiven)
        {
            dialogueManager.StartDialogue(hasCoinDialogue, "fountain");
            return;
        }

        if (!IsTargetQuestActive())
        {
            dialogueManager.StartDialogue(notStartQuestDialogue, "fountain");
            return;
        }

        GiveCoin();
    }

    private void GiveCoin()
    {
        if (ItemGiver.Instance != null)
        {
            ItemGiver.Instance.GiveItem(targetItemID);
        }
        hasCoinGiven = true;
        SaveState();
        dialogueManager.StartDialogue(startQuestDialogue, "fountain");
    }

    private bool IsTargetQuestActive()
    {
        if (QuestSystem.Instance == null) return false;

        foreach (Quest quest in QuestSystem.Instance.GetActiveQuests())
        {
            if (quest.questID == targetQuestID)
                return true;
        }
        return false;
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt(SAVE_KEY, hasCoinGiven ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        hasCoinGiven = PlayerPrefs.GetInt(SAVE_KEY, 0) == 1;
    }

    [ContextMenu("Reset Fountain State")]
    private void ResetState()
    {
        hasCoinGiven = false;
        PlayerPrefs.DeleteKey(SAVE_KEY);
        Debug.Log($"Fountain state reset for {SAVE_KEY}");
    }
}