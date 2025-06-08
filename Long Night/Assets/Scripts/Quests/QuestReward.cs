using UnityEngine;

// ����������� ����� ��� �������� ������ 
[System.Serializable]
public abstract class QuestReward
{
    public abstract void GiveReward();
}

[System.Serializable]
public class ItemReward : QuestReward
{
    public string itemID;
    public int amount;

    public override void GiveReward()
    {
        ItemGiver.Instance.GiveItem(itemID);
    }
}

[System.Serializable]
public class ExperienceReward : QuestReward
{
    public int amount;

    public override void GiveReward()
    {
        PlayerStats.Instance.GainEXP(amount);
    }
}