using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class EnemyStatsTests
{
    private GameObject enemyGameObject;
    private EnemyStats enemyStats;
    private EnemyTemplate dummyTemplate;

    // Метод SetUp выполняется перед каждым тестом
    // Здесь мы подготавливаем тестовое окружение
    [SetUp]
    public void SetUp()
    {
        // Создаем mock-объект EnemyTemplate (тестовый шаблон врага)
        // ScriptableObject.CreateInstance позволяет создать экземпляр ScriptableObject в памяти
        dummyTemplate = ScriptableObject.CreateInstance<EnemyTemplate>();
        dummyTemplate.enemyName = "Test Enemy";
        dummyTemplate.maxHP = 100;       // Устанавливаем максимальное HP для тестов
        dummyTemplate.attackPower = 10;  // Устанавливаем силу атаки для тестов

        // Создаем и настраиваем mock для EnemyDataTransfer
        // (системы передачи данных между сценами)
        var dataTransferGO = new GameObject();
        var dataTransfer = dataTransferGO.AddComponent<EnemyDataTransfer>();
        dataTransfer.currentEnemyTemplate = dummyTemplate;
        EnemyDataTransfer.Instance = dataTransfer;

        // Создаем игровой объект врага и добавляем компонент EnemyStats
        // Это наш тестируемый объект
        enemyGameObject = new GameObject();
        enemyStats = enemyGameObject.AddComponent<EnemyStats>();
    }

    // Метод TearDown выполняется после каждого теста
    // Здесь мы очищаем ресурсы, чтобы тесты не влияли друг на друга
    [TearDown]
    public void TearDown()
    {
        // Уничтожаем созданные объекты
        Object.DestroyImmediate(enemyGameObject);
        Object.DestroyImmediate(dummyTemplate);
        Object.DestroyImmediate(EnemyDataTransfer.Instance.gameObject);
    }

    /// Проверяет, что метод TakeDamage корректно уменьшает HP врага
    [Test]
    public void TakeDamage_ReducesCurrentHP()
    {
        // Arrange (Подготовка)
        // Запоминаем начальное количество HP
        int initialHP = enemyStats.currentHP;
        int damage = 20; // Урон, который будем наносить

        // Act (Действие)
        // Наносим урон врагу
        enemyStats.TakeDamage(damage);

        // Assert (Проверка)
        // Проверяем, что HP уменьшилось ровно на величину урона
        Assert.AreEqual(initialHP - damage, enemyStats.currentHP,
            "HP должно уменьшиться на величину полученного урона");
    }

    /// Проверяет, что HP не может стать меньше 0 при получении урона
    [Test]
    public void TakeDamage_DoesNotReduceHPBelowZero()
    {
        // Arrange (Подготовка)
        // Урон, который больше максимального HP врага
        int excessiveDamage = enemyStats.maxHP + 50;

        // Act (Действие)
        enemyStats.TakeDamage(excessiveDamage);

        // Assert (Проверка)
        // Проверяем, что HP равно 0, а не отрицательное число
        Assert.AreEqual(0, enemyStats.currentHP,
            "HP не должно опускаться ниже 0, даже если урон превышает текущее HP");
    }

    /// Проверяет, что метод IsDead возвращает true, когда HP врага равно 0
    [Test]
    public void IsDead_ReturnsTrueWhenHPZero()
    {
        // Arrange (Подготовка)
        // Наносим урон, равный максимальному HP, чтобы убить врага
        enemyStats.TakeDamage(enemyStats.maxHP);

        // Act (Действие)
        bool isDead = enemyStats.IsDead();

        // Assert (Проверка)
        Assert.IsTrue(isDead,
            "IsDead должен возвращать true, когда HP равно 0");
    }

    /// Проверяет, что метод IsDead возвращает false, когда у врага осталось HP
    [Test]
    public void IsDead_ReturnsFalseWhenHPAboveZero()
    {
        // Arrange (Подготовка)
        // Наносим урон на 1 меньше максимального HP
        // чтобы у врага осталось 1 HP
        enemyStats.TakeDamage(enemyStats.maxHP - 1);

        // Act (Действие)
        bool isDead = enemyStats.IsDead();

        // Assert (Проверка)
        Assert.IsFalse(isDead,
            "IsDead должен возвращать false, когда HP больше 0");
    }
}