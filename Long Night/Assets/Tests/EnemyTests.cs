using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class EnemyStatsTests
{
    private GameObject enemyGameObject;
    private EnemyStats enemyStats;
    private EnemyTemplate dummyTemplate;

    // ����� SetUp ����������� ����� ������ ������
    // ����� �� �������������� �������� ���������
    [SetUp]
    public void SetUp()
    {
        // ������� mock-������ EnemyTemplate (�������� ������ �����)
        // ScriptableObject.CreateInstance ��������� ������� ��������� ScriptableObject � ������
        dummyTemplate = ScriptableObject.CreateInstance<EnemyTemplate>();
        dummyTemplate.enemyName = "Test Enemy";
        dummyTemplate.maxHP = 100;       // ������������� ������������ HP ��� ������
        dummyTemplate.attackPower = 10;  // ������������� ���� ����� ��� ������

        // ������� � ����������� mock ��� EnemyDataTransfer
        // (������� �������� ������ ����� �������)
        var dataTransferGO = new GameObject();
        var dataTransfer = dataTransferGO.AddComponent<EnemyDataTransfer>();
        dataTransfer.currentEnemyTemplate = dummyTemplate;
        EnemyDataTransfer.Instance = dataTransfer;

        // ������� ������� ������ ����� � ��������� ��������� EnemyStats
        // ��� ��� ����������� ������
        enemyGameObject = new GameObject();
        enemyStats = enemyGameObject.AddComponent<EnemyStats>();
    }

    // ����� TearDown ����������� ����� ������� �����
    // ����� �� ������� �������, ����� ����� �� ������ ���� �� �����
    [TearDown]
    public void TearDown()
    {
        // ���������� ��������� �������
        Object.DestroyImmediate(enemyGameObject);
        Object.DestroyImmediate(dummyTemplate);
        Object.DestroyImmediate(EnemyDataTransfer.Instance.gameObject);
    }

    /// ���������, ��� ����� TakeDamage ��������� ��������� HP �����
    [Test]
    public void TakeDamage_ReducesCurrentHP()
    {
        // Arrange (����������)
        // ���������� ��������� ���������� HP
        int initialHP = enemyStats.currentHP;
        int damage = 20; // ����, ������� ����� ��������

        // Act (��������)
        // ������� ���� �����
        enemyStats.TakeDamage(damage);

        // Assert (��������)
        // ���������, ��� HP ����������� ����� �� �������� �����
        Assert.AreEqual(initialHP - damage, enemyStats.currentHP,
            "HP ������ ����������� �� �������� ����������� �����");
    }

    /// ���������, ��� HP �� ����� ����� ������ 0 ��� ��������� �����
    [Test]
    public void TakeDamage_DoesNotReduceHPBelowZero()
    {
        // Arrange (����������)
        // ����, ������� ������ ������������� HP �����
        int excessiveDamage = enemyStats.maxHP + 50;

        // Act (��������)
        enemyStats.TakeDamage(excessiveDamage);

        // Assert (��������)
        // ���������, ��� HP ����� 0, � �� ������������� �����
        Assert.AreEqual(0, enemyStats.currentHP,
            "HP �� ������ ���������� ���� 0, ���� ���� ���� ��������� ������� HP");
    }

    /// ���������, ��� ����� IsDead ���������� true, ����� HP ����� ����� 0
    [Test]
    public void IsDead_ReturnsTrueWhenHPZero()
    {
        // Arrange (����������)
        // ������� ����, ������ ������������� HP, ����� ����� �����
        enemyStats.TakeDamage(enemyStats.maxHP);

        // Act (��������)
        bool isDead = enemyStats.IsDead();

        // Assert (��������)
        Assert.IsTrue(isDead,
            "IsDead ������ ���������� true, ����� HP ����� 0");
    }

    /// ���������, ��� ����� IsDead ���������� false, ����� � ����� �������� HP
    [Test]
    public void IsDead_ReturnsFalseWhenHPAboveZero()
    {
        // Arrange (����������)
        // ������� ���� �� 1 ������ ������������� HP
        // ����� � ����� �������� 1 HP
        enemyStats.TakeDamage(enemyStats.maxHP - 1);

        // Act (��������)
        bool isDead = enemyStats.IsDead();

        // Assert (��������)
        Assert.IsFalse(isDead,
            "IsDead ������ ���������� false, ����� HP ������ 0");
    }
}