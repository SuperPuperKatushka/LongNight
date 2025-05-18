using NUnit.Framework;
using UnityEngine;

public class PlayerStatsTests
{
    // Создает тестовый экземпляр PlayerStats с начальными параметрами:
    // - уровень 1
    // - 0 опыта
    // - 100 опыта для повышения уровня
    // - максимальный уровень 10
    private PlayerStats CreateTestPlayerStats()
    {
        GameObject obj = new GameObject();
        PlayerStats stats = obj.AddComponent<PlayerStats>();
        stats.level = 1;
        stats.currentEXP = 0;
        stats.expToLevelUp = 100;
        stats.maxLevel = 10;
        stats.ApplyLevelStats(); // Инициализирует характеристики согласно уровню
        return stats;
    }

    [Test]
    public void GainEXP_IncreasesCurrentEXP()
    {
        // Arrange: Подготовка тестового объекта
        PlayerStats stats = CreateTestPlayerStats();
        int initialEXP = stats.currentEXP; // Запоминаем начальное значение опыта

        // Act: Выполняем действие - добавляем опыт
        stats.GainEXP(50);

        // Assert: Проверяем, что текущий опыт увеличился на 50
        Assert.AreEqual(initialEXP + 50, stats.currentEXP);
    }

    [Test]
    public void LevelUp_WhenEXPThresholdReached()
    {
        // Arrange: Подготовка тестового объекта
        PlayerStats stats = CreateTestPlayerStats();
        int initialLevel = stats.level; // Запоминаем начальный уровень
        int initialHP = stats.maxHP; // Запоминаем начальное здоровье
        int initialAttack = stats.attackPower; // Запоминаем начальную атаку

        // Act: Добавляем опыт, превышающий порог уровня
        stats.GainEXP(150); // Достаточно для повышения уровня (100 опыта)

        // Assert: Проверяем изменения после повышения уровня
        Assert.AreEqual(initialLevel + 1, stats.level); // Уровень должен увеличиться на 1
        Assert.AreEqual(250, stats.maxHP); // Проверяем новое значение HP: 200 + (2-1)*50
        Assert.AreEqual(15, stats.attackPower); // Проверяем новую атаку: 10 + (2-1)*5
        Assert.AreEqual(50, stats.currentEXP); // Проверяем остаток опыта: 150 - 100 = 50
    }

    [Test]
    public void LevelUp_StopsAtMaxLevel()
    {
        // Arrange: Подготовка тестового объекта с максимальным уровнем
        PlayerStats stats = CreateTestPlayerStats();
        stats.level = 10; // Устанавливаем максимальный уровень
        stats.currentEXP = 0; // Обнуляем опыт

        // Act: Пытаемся добавить большое количество опыта
        stats.GainEXP(1000);

        // Assert: Проверяем, что уровень не изменился
        Assert.AreEqual(10, stats.level);
        // Проверяем, что опыт не был потрачен на повышение уровня
        Assert.AreEqual(1000, stats.currentEXP);
    }
}