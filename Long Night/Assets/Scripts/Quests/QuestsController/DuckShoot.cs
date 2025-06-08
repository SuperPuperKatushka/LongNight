using UnityEngine;

public class DuckShoot : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private string targetQuestID;
    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private Vector3 spawnOffset = new Vector3(-1f, -4f, 0f);
    private const string SAVE_KEY = "DuckShoot_Thrown";

    [Header("Анимация")]
    [SerializeField] private Animator animator;
    [SerializeField] private string shootAnimationTrigger = "DropCoin";

    private bool hasThrown = false;

    private void Awake()
    {
        LoadState();
    }

    public void TryShootItem()
    {
        if (hasThrown || !IsTargetQuestActive())
            return;

        ThrowItem();
        hasThrown = true;
        SaveState();
    }

    private bool IsTargetQuestActive()
    {
        if (QuestSystem.Instance == null)
        {
            Debug.LogWarning("QuestSystem.Instance is null!");
            return false;
        }

        foreach (Quest quest in QuestSystem.Instance.GetActiveQuests())
        {
            if (quest.questID == targetQuestID)
                return true;
        }

        return false;
    }

    private void ThrowItem()
    {
        if (itemPrefab == null)
        {
            Debug.LogWarning("Item prefab is not assigned in DuckShoot!");
            return;
        }

        if (animator != null)
        {
            animator.SetTrigger(shootAnimationTrigger);
        }

        Vector2 spawnPosition = transform.position + spawnOffset;
        Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt(SAVE_KEY, hasThrown ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        hasThrown = PlayerPrefs.GetInt(SAVE_KEY, 0) == 1;

        // Если предмет уже был выброшен, проигрываем анимацию
        if (hasThrown && animator != null)
        {
            animator.SetTrigger(shootAnimationTrigger);
        }
    }

    [ContextMenu("Reset DuckShoot State")]
    private void ResetState()
    {
        hasThrown = false;
        PlayerPrefs.DeleteKey(SAVE_KEY);
        if (animator != null)
        {
            animator.ResetTrigger(shootAnimationTrigger);
        }
        Debug.Log($"DuckShoot state reset for {SAVE_KEY}");
    }
}