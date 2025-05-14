// QuestInitializer.cs
using System.IO;
using UnityEngine;

public class QuestInitializer : MonoBehaviour
{
    [SerializeField] private QuestData[] singleQuestDataAssets;
    [SerializeField] private ChainQuest[] chainQuestDataAssets;

    private void Start()
    {
        InitializeSingleQuests();
        InitializeChainQuests();
    }

    private void InitializeSingleQuests()
    {
        foreach (var questData in singleQuestDataAssets)
        {
            var quest = QuestSOConverter.ConvertToRuntimeQuest(questData);
            QuestSystem.Instance.RegisterQuest(quest);
            Debug.Log("��������������� �����  " + quest);

        }
    }

    private void InitializeChainQuests()
    {
        foreach (var chainData in chainQuestDataAssets)
        {
            Debug.Log("�������� �����������  " );
            var runtimeChain = QuestSOConverter.ConvertToRuntimeChain(chainData);
            QuestSystem.Instance.RegisterChainQuest(runtimeChain);
            Debug.Log("���������������� ������� �������  " + runtimeChain);

        }
    }
}