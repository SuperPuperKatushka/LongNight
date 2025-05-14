using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Все данные игры, которые нужно сохранять
    [System.Serializable]
    public class GameData
    {
        //public QuestSystem.QuestSaveData questData;
        // Другие данные (инвентарь, статистика игрока и т.д.)
    }

    private GameData _currentGameData;

    void Update()
    {
        QuestSystem.Instance.UpdateQuests(); // Запускает всю цепочку проверок
    }

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

    // Загрузка при старте
    private void Start()
    {
        LoadGame();
    }

    // Сохранение игры
    public void SaveGame()
    {
        _currentGameData = new GameData
        {
            //questData = QuestSystem.Instance.GetSaveData(),
            // Сохраняем другие системы...
        };

        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/save.dat";
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, _currentGameData);
        stream.Close();

        Debug.Log("Игра сохранена в " + path);
    }

    // Загрузка игры
    public void LoadGame()
    {
        string path = Application.persistentDataPath + "/save.dat";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            _currentGameData = formatter.Deserialize(stream) as GameData;
            stream.Close();

            // Восстанавливаем данные квестов
            //QuestSystem.Instance.LoadSaveData(_currentGameData.questData);

            Debug.Log("Игра загружена из " + path);
        }
        else
        {
            Debug.Log("Файл сохранения не найден. Создаём новую игру.");
            _currentGameData = new GameData();
        }
    }

    // Для автосохранения при выходе
    private void OnApplicationQuit()
    {
        SaveGame();
    }
}