using UnityEngine;

public class Barrier : MonoBehaviour
{
    [SerializeField] private string targetQuestID = "go_to_boss";
    [SerializeField] private string saveKey = "BarrierDestroyed";
    [SerializeField] private GameObject cloudsBarrier;

    private bool isDestroyed = false;

    private void Start()
    {
        // Загружаем состояние при старте
        isDestroyed = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (isDestroyed)
        {
            DestroyBarrier();
            return;
        }

        // Проверяем квест каждый кадр (можно заменить на событие)
        InvokeRepeating(nameof(CheckQuest), 0.1f, 0.5f);
    }

    private void CheckQuest()
    {
        if (isDestroyed) return;

        // Если квест активен - разрушаем барьер
        if (IsQuestActive())
        {
            DestroyBarrier();
            SaveState();
        }
    }

    private bool IsQuestActive()
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

    private void DestroyBarrier()
    {

        // Отключаем коллайдер и рендеринг
        if (TryGetComponent<CapsuleCollider2D>(out var collider))
            collider.enabled = false;

        cloudsBarrier.SetActive(false);

        isDestroyed = true;
        CancelInvoke(nameof(CheckQuest)); // Останавливаем проверку
    }

    private void SaveState()
    {
        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();
    }

    [ContextMenu("Reset Barrier State")]
    public void ResetBarrier()
    {
        PlayerPrefs.DeleteKey(saveKey);

        // Восстанавливаем барьер (для тестирования)
        if (TryGetComponent<Collider>(out var collider))
            collider.enabled = true;

        cloudsBarrier.SetActive(true);

        isDestroyed = false;
        Debug.Log("Состояние барьера сброшено");
    }
}