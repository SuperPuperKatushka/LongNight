using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy Template")]
public class EnemyTemplate : ScriptableObject
{
    public string enemyName;
    public EnemyType enemyType;
    public int maxHP;
    public int attackPower;
    public int defensePower;
    public int manaPool;
    public Sprite battleSprite;

    [Header("AI Behavior")]
    public float strongAttackChance = 0.3f;
    public float healThreshold = 0.4f;
    public float playerFinishThreshold = 0.3f;
}
