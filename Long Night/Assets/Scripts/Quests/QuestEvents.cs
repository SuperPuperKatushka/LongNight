using System;
using UnityEngine;

public class QuestEvents : MonoBehaviour
{
    public static QuestEvents Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public event Action<string> OnEnemyKilled;
    public event Action<string> OnItemCollected;
    public event Action<string> OnNpcTalked;

    public void EnemyKilled(string enemyId) => OnEnemyKilled?.Invoke(enemyId);
    public void ItemCollected(string itemId) => OnItemCollected?.Invoke(itemId);
    public void NpcTalked(string npcId) => OnNpcTalked?.Invoke(npcId);
}
