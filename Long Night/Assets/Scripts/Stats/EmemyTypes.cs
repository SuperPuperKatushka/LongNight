using UnityEngine;

[System.Serializable]
public class EnemyData
{
    public string enemyName;
    public int maxHP;
    public int attackPower;
    public EnemyType enemyType;
}

public enum EnemyType
{
    Base,
    Middle,
    Strong,
    Boss
}
