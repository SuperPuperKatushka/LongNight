using NUnit.Framework;
using UnityEngine;

public class PlayerStatsTests
{
    // ������� �������� ��������� PlayerStats � ���������� �����������:
    // - ������� 1
    // - 0 �����
    // - 100 ����� ��� ��������� ������
    // - ������������ ������� 10
    private PlayerStats CreateTestPlayerStats()
    {
        GameObject obj = new GameObject();
        PlayerStats stats = obj.AddComponent<PlayerStats>();
        stats.level = 1;
        stats.currentEXP = 0;
        stats.expToLevelUp = 100;
        stats.maxLevel = 10;
        stats.ApplyLevelStats(); // �������������� �������������� �������� ������
        return stats;
    }

    [Test]
    public void GainEXP_IncreasesCurrentEXP()
    {
        // Arrange: ���������� ��������� �������
        PlayerStats stats = CreateTestPlayerStats();
        int initialEXP = stats.currentEXP; // ���������� ��������� �������� �����

        // Act: ��������� �������� - ��������� ����
        stats.GainEXP(50);

        // Assert: ���������, ��� ������� ���� ���������� �� 50
        Assert.AreEqual(initialEXP + 50, stats.currentEXP);
    }

    [Test]
    public void LevelUp_WhenEXPThresholdReached()
    {
        // Arrange: ���������� ��������� �������
        PlayerStats stats = CreateTestPlayerStats();
        int initialLevel = stats.level; // ���������� ��������� �������
        int initialHP = stats.maxHP; // ���������� ��������� ��������
        int initialAttack = stats.attackPower; // ���������� ��������� �����

        // Act: ��������� ����, ����������� ����� ������
        stats.GainEXP(150); // ���������� ��� ��������� ������ (100 �����)

        // Assert: ��������� ��������� ����� ��������� ������
        Assert.AreEqual(initialLevel + 1, stats.level); // ������� ������ ����������� �� 1
        Assert.AreEqual(250, stats.maxHP); // ��������� ����� �������� HP: 200 + (2-1)*50
        Assert.AreEqual(15, stats.attackPower); // ��������� ����� �����: 10 + (2-1)*5
        Assert.AreEqual(50, stats.currentEXP); // ��������� ������� �����: 150 - 100 = 50
    }

    [Test]
    public void LevelUp_StopsAtMaxLevel()
    {
        // Arrange: ���������� ��������� ������� � ������������ �������
        PlayerStats stats = CreateTestPlayerStats();
        stats.level = 10; // ������������� ������������ �������
        stats.currentEXP = 0; // �������� ����

        // Act: �������� �������� ������� ���������� �����
        stats.GainEXP(1000);

        // Assert: ���������, ��� ������� �� ���������
        Assert.AreEqual(10, stats.level);
        // ���������, ��� ���� �� ��� �������� �� ��������� ������
        Assert.AreEqual(1000, stats.currentEXP);
    }
}