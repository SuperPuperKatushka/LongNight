// RuntimeChainQuest.cs
using System.Collections.Generic;

public class RuntimeChainQuest
{
    public string chainID;
    public List<Quest> questsInChain = new List<Quest>();
    public bool restartFromBeginning;
    public int currentQuestIndex;

    public Quest GetCurrentQuest() =>
        currentQuestIndex < questsInChain.Count ? questsInChain[currentQuestIndex] : null;

    public void AdvanceToNextQuest()
    {
        currentQuestIndex++;
        if (currentQuestIndex >= questsInChain.Count && restartFromBeginning)
            currentQuestIndex = 0;
    }

    public bool IsChainComplete() => currentQuestIndex >= questsInChain.Count;
}