using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy/Enemy Template")]
public class EnemyTemplate : ScriptableObject
{
    public string enemyName;
    public EnemyType enemyType;
    public int maxHP;
    public int attackPower;
}
