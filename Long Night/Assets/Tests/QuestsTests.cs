using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

// Класс для тестирования системы квестов
public class QuestSystemTests
{
    // Ссылки на тестируемые компоненты
    private QuestSystem questSystem;       // Система квестов
    private TestReward testReward;        // Тестовая награда
    private TestObjective testObjective;  // Тестовая цель квеста

    // Тестовый класс для награды
    public class TestReward : QuestReward
    {
        public bool WasGiven { get; private set; } // Флаг, была ли выдана награда

        // Метод выдачи награды
        public override void GiveReward()
        {
            WasGiven = true; // Устанавливаем флаг выдачи
        }
    }

    // Тестовый класс для цели квеста
    public class TestObjective : QuestObjective
    {
        public bool IsInitialized { get; private set; } // Флаг инициализации
        public bool ForceComplete { get; set; }         // Флаг принудительного завершения

        // Инициализация цели
        public override void Initialize()
        {
            IsInitialized = true;  // Отмечаем инициализацию
            IsComplete = false;    // Сбрасываем состояние выполнения
        }

        // Проверка прогресса выполнения
        public override void CheckProgress()
        {
            // Устанавливаем состояние выполнения согласно флагу
            IsComplete = ForceComplete;
        }

        // Текст прогресса выполнения
        public override string GetProgressText()
        {
            return $"Test objective: {(IsComplete ? "Complete" : "Incomplete")}";
        }
    }

    // Настройка перед каждым тестом
    [SetUp]
    public void Setup()
    {
        // Создаем новый QuestSystem для каждого теста
        var gameObject = new GameObject();
        questSystem = gameObject.AddComponent<QuestSystem>();

        // Инициализируем тестовые данные
        testReward = new TestReward();     // Создаем тестовую награду
        testObjective = new TestObjective(); // Создаем тестовую цель
    }

    // Очистка после каждого теста
    [TearDown]
    public void Teardown()
    {
        // Удаляем созданные объекты
        Object.DestroyImmediate(questSystem.gameObject);
    }

    // Тест принятия квеста
    [Test]
    public void AcceptQuest_ChangesStateToActive()
    {
        // Подготовка тестовых данных
        var quest = new Quest
        {
            questID = "test_quest",
            state = Quest.QuestState.Available,  // Начальное состояние - доступен
            objectives = new List<QuestObjective> { testObjective }
        };

        // Регистрируем квест в системе
        questSystem.RegisterQuest(quest);

        // Выполняем действие - принимаем квест
        questSystem.AcceptQuest("test_quest");

        // Проверяем результаты:
        // 1. Состояние должно измениться на Active
        Assert.AreEqual(Quest.QuestState.Active, quest.state);
        // 2. Цель должна быть инициализирована
        Assert.IsTrue(testObjective.IsInitialized);
    }

    // Тест завершения квеста
    [Test]
    public void CompleteQuest_ChangesStateToRewarded_AndGivesRewards()
    {
        // Подготовка тестовых данных
        var quest = new Quest
        {
            questID = "test_quest",
            state = Quest.QuestState.Completed,  // Начальное состояние - выполнен
            rewards = new List<QuestReward> { testReward }
        };

        // Регистрируем квест в системе
        questSystem.RegisterQuest(quest);

        // Выполняем действие - завершаем квест
        questSystem.CompleteQuest("test_quest");

        // Проверяем результаты:
        // 1. Состояние должно измениться на Rewarded
        Assert.AreEqual(Quest.QuestState.Rewarded, quest.state);
        // 2. Награда должна быть выдана (флаг WasGiven = true)
        Assert.IsTrue(testReward.WasGiven);
    }

    // Тест проверки завершенности квеста
    [Test]
    public void IsQuestCompleted_ReturnsTrue_OnlyForRewardedQuests()
    {
        // Подготовка тестовых данных
        var quest = new Quest
        {
            questID = "test_quest",
            state = Quest.QuestState.Rewarded  // Начальное состояние - награда получена
        };

        // Регистрируем квест в системе
        questSystem.RegisterQuest(quest);

        // Проверяем результаты:
        // 1. Для квеста в состоянии Rewarded должно возвращаться true
        Assert.IsTrue(questSystem.IsQuestCompleted("test_quest"));
        // 2. Для несуществующего квеста должно возвращаться false
        Assert.IsFalse(questSystem.IsQuestCompleted("non_existent_quest"));
    }
}

