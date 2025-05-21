// QuestInitializer.cs
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

public class QuestInitializer : MonoBehaviour
{
    [SerializeField] private QuestData[] singleQuestDataAssets;
    [SerializeField] private ChainQuest[] chainQuestDataAssets;

    public static QuestInitializer Instance { get; private set; }
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
    private void Start()
    {
        string startOrContinue = PlayerPrefs.GetString("StartOrContinue");

        InitializeSingleQuests();
        InitializeChainQuests();

        if (startOrContinue == "Continue")
        {
            GameManager.Instance.LoadGame();
        }
    }

    private void InitializeSingleQuests()
    {
        foreach (var questData in singleQuestDataAssets)
        {
            var quest = QuestSOConverter.ConvertToRuntimeQuest(questData);
            QuestSystem.Instance.RegisterQuest(quest);
            Debug.Log("Зарегистрирован квест  " + quest);

        }
    }

    private void InitializeChainQuests()
    {
        foreach (var chainData in chainQuestDataAssets)
        {
            var runtimeChain = QuestSOConverter.ConvertToRuntimeChain(chainData);
            QuestSystem.Instance.RegisterChainQuest(runtimeChain);
        }
    }
    public void ForceInitialize()
    {
        InitializeChainQuests();
        InitializeSingleQuests();
    }
}