using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string enemyName;
    public EnemyType enemyType;
    public int maxHP;
    public int attackPower;
    public int defensePower;
    public int manaPool;
    public float strongAttackChance = 0.3f;
    public float healThreshold = 0.4f;
    public float playerFinishThreshold = 0.3f;
}

public enum EnemyType
{
    Aggressive,  // Чаще атакует, сильные удары
    Healer       // Лечит себя, выносливый
}
