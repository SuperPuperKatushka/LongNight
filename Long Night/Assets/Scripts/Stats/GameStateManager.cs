using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [System.Serializable]
    public class GameData
    {
        public PlayerStats.PlayerStatsData playerStats;
        public List<SlotItemData> inventoryItems;
        public List<SlotItemData> equippedItems;

        [System.Serializable]
        public class QuestSave
        {
            public string questID;
            public Quest.QuestState state;
            public List<ObjectiveSave> objectives = new List<ObjectiveSave>();
        }

        [System.Serializable]
        public class ObjectiveSave
        {
            public string type; // "Collect", "Kill", "Interact", "Talk"
            public int currentProgress;
            public bool isComplete;
        }

        [System.Serializable]
        public class ChainQuestSave
        {
            public string chainID;
            public int currentQuestIndex;
        }

        public List<QuestSave> questSaves = new List<QuestSave>();
        public List<ChainQuestSave> chainQuestSaves = new List<ChainQuestSave>();
    }



    private GameData _currentGameData;

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

 
    void Update()
    {
        QuestSystem.Instance?.UpdateQuests(); // Запускает всю цепочку проверок
    }
    public void SaveGame()
    {
        _currentGameData = new GameData
        {
            playerStats = PlayerStats.Instance.GetSaveData(),
            inventoryItems = PlayerStats.Instance.inventoryData.slotItems,
            equippedItems = PlayerStats.Instance.equipmentData.equippedItems,
            questSaves = QuestSystem.Instance.GetQuestsSaveData(),
            chainQuestSaves = QuestSystem.Instance.GetChainQuestsSaveData(),

        };

        string json = JsonUtility.ToJson(_currentGameData, true);
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        File.WriteAllText(path, json);
        PlayerPrefs.Save();
        Debug.Log($"Game saved to {path}\n{json}");
    }

    public void LoadGame()
    {
        string path = Path.Combine(Application.persistentDataPath, "save.json");
        //Player.Instance.UnblockMovement();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            _currentGameData = JsonUtility.FromJson<GameData>(json);

            // Загружаем данные
            PlayerStats.Instance.LoadSaveData(_currentGameData.playerStats);
            PlayerStats.Instance.inventoryData.slotItems = _currentGameData.inventoryItems ?? new List<SlotItemData>();
            PlayerStats.Instance.equipmentData.equippedItems = _currentGameData.equippedItems ?? new List<SlotItemData>();
            QuestSystem.Instance.LoadQuests(_currentGameData.questSaves ?? new List<GameData.QuestSave>());
            QuestSystem.Instance.LoadChainQuests(_currentGameData.chainQuestSaves ?? new List<GameData.ChainQuestSave>());

        }
        else
        {
            _currentGameData = new GameData();
            NewGame();
        }
    }


    public void NewGame()
    {
        string savePath = Path.Combine(Application.persistentDataPath, "save.json");
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("Save file deleted: " + savePath);
        }
        else
        {
            Debug.Log("No save file found to delete");
        }

        _currentGameData = new GameData();
        ResetAllGameSystems();

    }
    private void ResetAllGameSystems()
    {
        PlayerPositionData.Instance.savedPosition = new Vector2();
        PlayerStats.Instance.inventoryData.slotItems.Clear();
        PlayerStats.Instance.equipmentData.equippedItems.Clear();
        PlayerStats.Instance.ResetToDefault();

    }


}