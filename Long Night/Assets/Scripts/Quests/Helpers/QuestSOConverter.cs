using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class QuestSOConverter
{
    public static RuntimeChainQuest ConvertToRuntimeChain(ChainQuest chainData)
    {
        return new RuntimeChainQuest
        {
            chainID = chainData.chainID,
            restartFromBeginning = chainData.restartFromBeginning,
            questsInChain = chainData.questsInChain
                .Select(ConvertToRuntimeQuest)
                .ToList()
        };
    }

    public static Quest ConvertToRuntimeQuest(QuestData data)
    {
        return new Quest
        {
            questID = data.questID,
            title = data.title,
            description = data.description,
            state = Quest.QuestState.NotAvailable,
            objectives = ConvertObjectives(data.objectives).ToList(),
            rewards = ConvertRewards(data.rewards).ToList(),
            requirements = ConvertRequirements(data.requirements).ToList()
        };
    }

    private static IEnumerable<QuestObjective> ConvertObjectives(
        IEnumerable<QuestObjectiveData> objectives)
    {
        foreach (var objData in objectives)
        {
            QuestObjective objective = objData.type switch
            {
                QuestObjectiveData.ObjectiveType.Kill => new KillObjective
                {
                    description = objData.description,
                    enemyID = objData.targetID,
                    requiredKills = objData.requiredAmount
                },

                QuestObjectiveData.ObjectiveType.Collect => new CollectObjective
                {
                    description = objData.description,
                    itemID = objData.targetID,
                    requiredAmount = objData.requiredAmount
                },

                QuestObjectiveData.ObjectiveType.Interact => new InteractObjective
                {
                    description = objData.description,
                    interactableId = objData.targetID
                },

                QuestObjectiveData.ObjectiveType.Talk => new TalkObjective
                {
                    description = objData.description,
                    npcID = objData.targetID,
                    requiredAmount = objData.requiredAmount,
                },

                _ => ThrowUnknownObjectiveType(objData.type)
            };

            yield return objective;
        }
    }

    private static IEnumerable<QuestReward> ConvertRewards(
        IEnumerable<QuestRewardData> rewards)
    {
        foreach (var rewardData in rewards)
        {
            QuestReward reward = rewardData.type switch
            {
                QuestRewardData.RewardType.Item => new ItemReward
                {
                    itemID = rewardData.itemID,
                    amount = rewardData.amount
                },

                QuestRewardData.RewardType.Experience => new ExperienceReward
                {
                    amount = rewardData.amount
                },

                //QuestRewardData.RewardType.Currency => new CurrencyReward
                //{
                //    amount = rewardData.amount
                //},

                _ => ThrowUnknownRewardType(rewardData.type)
            };

            yield return reward;
        }
    }

    private static IEnumerable<QuestRequirement> ConvertRequirements(
        IEnumerable<QuestRequirementData> requirements)
    {
        foreach (var reqData in requirements)
        {
            QuestRequirement requirement = reqData.type switch
            {
                QuestRequirementData.RequirementType.Level => new LevelRequirement
                {
                    requiredLevel = reqData.requiredLevel
                },

                QuestRequirementData.RequirementType.Quest => new QuestRequirement
                {
                    requiredQuestID = reqData.requiredQuestID,
                    requiredState = reqData.requiredQuestState
                },

                _ => ThrowUnknownRequirementType(reqData.type)
            };

            yield return requirement;
        }
    }

    private static QuestObjective ThrowUnknownObjectiveType(
        QuestObjectiveData.ObjectiveType type)
    {
        Debug.LogError($"Unknown objective type: {type}");
        return null;
    }

    private static QuestReward ThrowUnknownRewardType(
        QuestRewardData.RewardType type)
    {
        Debug.LogError($"Unknown reward type: {type}");
        return null;
    }

    private static QuestRequirement ThrowUnknownRequirementType(
        QuestRequirementData.RequirementType type)
    {
        Debug.LogError($"Unknown requirement type: {type}");
        return null;
    }
}