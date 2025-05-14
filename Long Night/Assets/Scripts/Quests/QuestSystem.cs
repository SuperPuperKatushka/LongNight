using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ����� ��������� ������� 
// �������� ����������, �������� � GameManager ������������ ������� 

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
            currentQuest.UpdateState(); // ��������

            AcceptQuest(currentQuest.questID);

        }
    }

    public void AdvanceChainQuest(string chainID)
    {
        if (chainQuests.TryGetValue(chainID, out RuntimeChainQuest chain))
        {
            // 1. ��������� ������� �����
            CompleteQuest(chain.GetCurrentQuest().questID);

            // 2. ��������� � ����������
            chain.AdvanceToNextQuest();

            if (!chain.IsChainComplete())
            {
                // 3. �������� ��������� �����
                Quest nextQuest = chain.GetCurrentQuest();

                // 4. �����: ���������� ��������� ������ ������
                nextQuest.state = Quest.QuestState.Available;

                // 5. ������������ � ����������
                RegisterQuest(nextQuest);
                AcceptQuest(nextQuest.questID); // ��� ������� Initialize() ��� �����
            }
        }

    }

    public void RegisterQuest(Quest quest)
    {
        Debug.Log("RegisterQuest ");
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
            // ���������, �������� �� ����� ������ �������
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

        // �������� ���������� ������
        var seenIds = new HashSet<string>();

        // ������� ������
        foreach (var quest in quests.Values)
        {
            if (quest.state == Quest.QuestState.Active && !seenIds.Contains(quest.questID))
            {
                activeQuests.Add(quest);
                seenIds.Add(quest.questID);
            }
        }

        // ������ �� �������
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

}

