using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewChainQuest", menuName = "Quests/Chain Quest")]
public class ChainQuest : ScriptableObject
{
    public string chainID;
    public List<QuestData> questsInChain;
    public bool restartFromBeginning = false;

    [HideInInspector] public int currentQuestIndex = 0;

    public QuestData GetCurrentQuest()
    {
        if (currentQuestIndex < questsInChain.Count)
        {
            return questsInChain[currentQuestIndex];
        }
        return null;
    }

    public void AdvanceToNextQuest()
    {
        currentQuestIndex++;
        if (currentQuestIndex >= questsInChain.Count && restartFromBeginning)
        {
            currentQuestIndex = 0;
        }
    }

    public bool IsChainComplete()
    {
        return currentQuestIndex >= questsInChain.Count;
    }
}