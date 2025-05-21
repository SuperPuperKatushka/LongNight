using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static GameManager;

// Класс менеджера квестов 
// Является синглотном, отлетает в GameManager сохраненными данными 

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem Instance { get; private set; }
    public event System.Action OnQuestStateChanged;


    private Dictionary<string, Quest> quests = new Dictionary<string, Quest>();
    private Dictionary<string, RuntimeChainQuest> chainQuests = new Dictionary<string, RuntimeChainQuest>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void RegisterChainQuest(RuntimeChainQuest chainQuest)
    {
        if (!chainQuests.ContainsKey(chainQuest.chainID))
        {
            chainQuests.Add(chainQuest.chainID, chainQuest);
            Quest currentQuest = chainQuest.GetCurrentQuest();
            RegisterQuest(currentQuest);
            currentQuest.UpdateState(); // ВРЕМЕННО

            AcceptQuest(currentQuest.questID);

        }
    }

    public void AdvanceChainQuest(string chainID)
    {
        if (chainQuests.TryGetValue(chainID, out RuntimeChainQuest chain))
        {
            // 1. Завершаем текущий квест
            CompleteQuest(chain.GetCurrentQuest().questID);

            // 2. Переходим к следующему
            chain.AdvanceToNextQuest();

            if (!chain.IsChainComplete())
            {
                // 3. Получаем следующий квест
                Quest nextQuest = chain.GetCurrentQuest();

                // 4. Важно: сбрасываем состояние нового квеста
                nextQuest.state = Quest.QuestState.Available;

                // 5. Регистрируем и активируем
                RegisterQuest(nextQuest);
                AcceptQuest(nextQuest.questID); // Это вызовет Initialize() для целей
            }
        }

    }

    public void RegisterQuest(Quest quest)
    {
        Debug.Log("RegisterQuest " + quest.title + quest.state);
        if (!quests.ContainsKey(quest.questID))
        {
            quests.Add(quest.questID, quest);

        }
        OnQuestStateChanged?.Invoke();

    }

    public static Quest GetQuest(string questID)
    {
        if (Instance.quests.TryGetValue(questID, out Quest quest))
        {
            return quest;
        }
        return null;
    }
    public bool ChainExists(string chainID)
    {
        return chainQuests.ContainsKey(chainID);
    }
    public void AcceptQuest(string questID)
    {
        Quest quest = GetQuest(questID);
        if (quest != null)
        {
            if (quest.state == Quest.QuestState.Available)
            {
                quest.state = Quest.QuestState.Active;
                InitializeQuestObjectives(quest);
            }
            else
            {
            }
        }
        OnQuestStateChanged?.Invoke();
    }

    public void CompleteQuest(string questID)
    {
        Quest quest = GetQuest(questID);
        if (quest != null)
        {
            if (quest.state == Quest.QuestState.Completed)
            {
                GiveRewards(quest);
                quest.state = Quest.QuestState.Rewarded;

            }

        }
        OnQuestStateChanged?.Invoke();
    }

    private void InitializeQuestObjectives(Quest quest)
    {
        foreach (var objective in quest.objectives)
        {
            objective.Initialize();
        }
    }

    private void GiveRewards(Quest quest)
    {
        foreach (var reward in quest.rewards)
        {
            reward.GiveReward();
        }
    }

    public void UpdateQuests()
    {

        foreach (var quest in quests.Values.ToList())
        {

            quest.UpdateState();
            CheckQuestCompletion(quest);
            OnQuestStateChanged?.Invoke();

        }
    }

    private void CheckQuestCompletion(Quest quest)
    {
        if (quest.IsComplete())
        {
            // Проверяем, является ли квест частью цепочки
            foreach (var chain in chainQuests.Values)
            {
                if (chain.GetCurrentQuest()?.questID == quest.questID)
                {
                    AdvanceChainQuest(chain.chainID);
                    return;
                }
            }
            CompleteQuest(quest.questID);
        }
    }
    public bool IsQuestCompleted(string questId)
    {
        if (quests.TryGetValue(questId, out Quest quest))
        {
            return quest.state == Quest.QuestState.Rewarded;
        }
        return false;
    }
    public List<Quest> GetActiveQuests()
    {
        var activeQuests = new List<Quest>();

        // Собираем уникальные квесты
        var seenIds = new HashSet<string>();

        // Обычные квесты
        foreach (var quest in quests.Values)
        {
            if (quest.state == Quest.QuestState.Active && !seenIds.Contains(quest.questID))
            {
                activeQuests.Add(quest);
                seenIds.Add(quest.questID);
            }
        }

        // Квесты из цепочек
        foreach (var chain in chainQuests.Values)
        {
            var current = chain.GetCurrentQuest();
            if (current != null && current.state == Quest.QuestState.Active && !seenIds.Contains(current.questID))
            {
                activeQuests.Add(current);
                seenIds.Add(current.questID);
            }
        }

        return activeQuests;
    }
    public List<GameData.QuestSave> GetQuestsSaveData()
    {

        var result = new List<GameData.QuestSave>();

        foreach (var q in quests.Values)
        {

            var questSave = new GameData.QuestSave
            {
                questID = q.questID,
                state = q.state,
                objectives = new List<GameData.ObjectiveSave>()
            };

            if (q.objectives != null)
            {
                foreach (var o in q.objectives)
                {

                    var objectiveSave = new GameData.ObjectiveSave();
                    objectiveSave.isComplete = o.IsComplete;

                    switch (o)
                    {
                        case CollectObjective co:
                            objectiveSave.type = "Collect";
                            objectiveSave.currentProgress = co.currentAmount;
                            break;

                        case KillObjective ko:
                            objectiveSave.type = "Kill";
                            objectiveSave.currentProgress = ko.currentKills;
                            break;

                        case InteractObjective io:
                            objectiveSave.type = "Interact";
                            objectiveSave.currentProgress = io.currentInteractions;
                            break;

                        case TalkObjective to:
                            objectiveSave.type = "Talk";
                            objectiveSave.currentProgress = to.currentAmount;
                            break;

                        default:
                            break;
                    }

                    questSave.objectives.Add(objectiveSave);
                }
            }
            else
            {
            }

            result.Add(questSave);
        }

        return result;
    }

    public List<GameData.ChainQuestSave> GetChainQuestsSaveData()
    {

        var result = new List<GameData.ChainQuestSave>();

        foreach (var c in chainQuests.Values)
        {

            result.Add(new GameData.ChainQuestSave
            {
                chainID = c.chainID,
                currentQuestIndex = c.currentQuestIndex
            });
        }

        return result;
    }

    public void LoadQuests(List<GameData.QuestSave> questSaves)
    {
        if (questSaves == null)
        {
            return;
        }


        foreach (var save in questSaves)
        {

            if (!quests.TryGetValue(save.questID, out Quest quest))
            {
                Debug.LogWarning($"[QuestSystem] Квест {save.questID} не найден в системе!");
                continue;
            }

            quest.state = save.state;

            if (save.objectives != null && quest.objectives != null)
            {
                int minCount = Mathf.Min(save.objectives.Count, quest.objectives.Count);

                for (int i = 0; i < minCount; i++)
                {
                    var objective = quest.objectives[i];
                    var objectiveSave = save.objectives[i];


                    switch (objective)
                    {
                        case CollectObjective co when objectiveSave.type == "Collect":
                            co.currentAmount = objectiveSave.currentProgress;
                            break;

                        case KillObjective ko when objectiveSave.type == "Kill":
                            ko.currentKills = objectiveSave.currentProgress;
                            break;

                        case InteractObjective io when objectiveSave.type == "Interact":
                            io.currentInteractions = objectiveSave.currentProgress;
                            break;

                        case TalkObjective to when objectiveSave.type == "Talk":
                            to.currentAmount = objectiveSave.currentProgress;
                            break;

                        default:
                            break;
                    }

                    if (quest.state == Quest.QuestState.Active)
                    {
                        objective.Initialize();
                    }
                }
            }
            else
            {
                Debug.LogWarning($"[QuestSystem] Проблемы со списками целей (save: {save.objectives != null}, quest: {quest.objectives != null})");
            }
        }
    }

    public void LoadChainQuests(List<GameData.ChainQuestSave> chainSaves)
    {
        if (chainSaves == null)
        {
            return;
        }


        foreach (var save in chainSaves)
        {

            if (!chainQuests.TryGetValue(save.chainID, out RuntimeChainQuest chain))
            {
                continue;
            }

            chain.currentQuestIndex = save.currentQuestIndex;

            var currentQuest = chain.GetCurrentQuest();
            if (currentQuest != null)
            {
                currentQuest.state = Quest.QuestState.Active;

                // ✅ Регистрируем квест в основной словарь, чтобы он обновлялся
                RegisterQuest(currentQuest);

                if (currentQuest.state == Quest.QuestState.Active)
                {
                    InitializeQuestObjectives(currentQuest);
                }
            }
            else
            {
                Debug.LogWarning($"[QuestSystem] Текущий квест цепочки {save.chainID} не найден!");
            }
        }
    }



    public void Reset()
    {
        // Сброс квестов
        foreach (var quest in quests.Values)
        {
            quest.state = Quest.QuestState.Available;

            if (quest.objectives != null)
            {
                foreach (var objective in quest.objectives)
                {
                    switch (objective)
                    {
                        case CollectObjective co:
                            co.currentAmount = 0;
                            break;

                        case KillObjective ko:
                            ko.currentKills = 0;
                            break;

                        case InteractObjective io:
                            io.currentInteractions = 0;
                            break;

                        case TalkObjective to:
                            to.currentAmount = 0;
                            break;

                        default:
                            break;
                    }
                }
            }
        }

        // Сброс цепочек
        foreach (var chain in chainQuests.Values)
        {
            chain.currentQuestIndex = 0;

            var currentQuest = chain.GetCurrentQuest();
            if (currentQuest != null)
            {
                currentQuest.state = Quest.QuestState.Available;

                if (currentQuest.objectives != null)
                {
                    foreach (var objective in currentQuest.objectives)
                    {
                        switch (objective)
                        {
                            case CollectObjective co:
                                co.currentAmount = 0;
                                break;

                            case KillObjective ko:
                                ko.currentKills = 0;
                                break;

                            case InteractObjective io:
                                io.currentInteractions = 0;
                                break;

                            case TalkObjective to:
                                to.currentAmount = 0;
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        // Полная очистка всех квестов и цепочек
        quests.Clear();
        chainQuests.Clear();

        // Вызываем событие обновления
        OnQuestStateChanged?.Invoke();

        Debug.Log("[QuestSystem] Система квестов сброшена.");
    }



}