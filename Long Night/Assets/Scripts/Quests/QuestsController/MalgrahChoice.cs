using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class ChoiceDialogController : MonoBehaviour
{
    public EnemyTemplate enemyTemplate;
    [Header("Диалоги")]
    public DialogueData firstDialogue;
    public DialogueData evilSideDialogue;

    [Header("Награды")]
    public string goodSideItemID;
    public string evilSideItemID;

    [Header("UI Elements")]
    public GameObject choicePanel;
    public Button goodSideButton;
    public Button evilSideButton;

    private DialogueManager dialogueManager;
    private bool choiceMade = false;
    private bool firstDialogueMake = false;
    private string chosenSide = ""; // "good" или "evil"

    // Ключи для сохранения
    private const string FIRST_DIALOGUE_KEY = "ChoiceDialog_FirstDialogue";
    private const string CHOICE_MADE_KEY = "ChoiceDialog_ChoiceMade";
    private const string CHOSEN_SIDE_KEY = "ChoiceDialog_Side";

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        // Настройка кнопок
        goodSideButton.onClick.AddListener(ChooseGoodSide);
        evilSideButton.onClick.AddListener(ChooseEvilSide);

        // Скрываем панель выбора
        choicePanel.SetActive(false);

        // Загрузка сохраненных статусов
        LoadStates();
    }

    private void LoadStates()
    {
        firstDialogueMake = PlayerPrefs.GetInt(FIRST_DIALOGUE_KEY, 0) == 1;
        choiceMade = PlayerPrefs.GetInt(CHOICE_MADE_KEY, 0) == 1;
        chosenSide = PlayerPrefs.GetString(CHOSEN_SIDE_KEY, "");
    }

    private void SaveStates()
    {
        PlayerPrefs.SetInt(FIRST_DIALOGUE_KEY, firstDialogueMake ? 1 : 0);
        PlayerPrefs.SetInt(CHOICE_MADE_KEY, choiceMade ? 1 : 0);
        PlayerPrefs.SetString(CHOSEN_SIDE_KEY, chosenSide);
        PlayerPrefs.Save();
    }

    public void StartInteraction()
    {
        if (!choiceMade && !firstDialogueMake)
        {
            // Первый диалог
            dialogueManager.StartDialogue(firstDialogue, "malgrah");
            firstDialogueMake = true;
            SaveStates();
        }
        else if (firstDialogueMake && !choiceMade)
        {
            ShowChoicePanel();
        }
        else if (choiceMade)
        {
            // Если выбор уже сделан, можно добавить дополнительную логику
            Debug.Log($"Игрок уже выбрал сторону: {chosenSide}");
        }
    }

    private void ShowChoicePanel()
    {
        choicePanel.SetActive(true);
    }

    private void ChooseGoodSide()
    {
        chosenSide = "good";
        choiceMade = true;
        choicePanel.SetActive(false);
        SaveStates();

        // Выдача предмета
        if (!string.IsNullOrEmpty(goodSideItemID))
        {
            ItemGiver.Instance.GiveItem(goodSideItemID);
        }

        StartBattle();
    }

    private void ChooseEvilSide()
    {
        chosenSide = "evil";
        choiceMade = true;
        choicePanel.SetActive(false);
        SaveStates();

        // Выдача предмета
        if (!string.IsNullOrEmpty(evilSideItemID))
        {
            ItemGiver.Instance.GiveItem(evilSideItemID);
        }

        dialogueManager.StartDialogue(evilSideDialogue, "malgrah");
    }

    [ContextMenu("Reset Choice")]
    public void ResetChoice()
    {
        choiceMade = false;
        chosenSide = "";
        firstDialogueMake = false;
        choicePanel.SetActive(false);

        // Сбрасываем сохранения
        PlayerPrefs.DeleteKey(FIRST_DIALOGUE_KEY);
        PlayerPrefs.DeleteKey(CHOICE_MADE_KEY);
        PlayerPrefs.DeleteKey(CHOSEN_SIDE_KEY);

        Debug.Log("Выбор сброшен");
    }

    private void StartBattle()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            PlayerPositionData.Instance.SavePosition(player.transform.position);
            EnemyDataTransfer.Instance.SetEnemyTemplate(enemyTemplate);
        }
        SceneManager.LoadScene("BattleBossScene");
    }

    // Метод для проверки выбранной стороны
    public bool IsGoodSide()
    {
        return chosenSide == "good";
    }

    public bool IsEvilSide()
    {
        return chosenSide == "evil";
    }
}