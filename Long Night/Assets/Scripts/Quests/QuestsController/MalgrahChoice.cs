using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using static UnityEngine.EventSystems.EventTrigger;

public class ChoiceDialogController : MonoBehaviour
{
    public EnemyTemplate enemyTemplate;
    [Header("�������")]
    public DialogueData firstDialogue;
    public DialogueData evilSideDialogue;

    [Header("�������")]
    public string goodSideItemID;
    public string evilSideItemID;

    [Header("UI Elements")]
    public GameObject choicePanel;
    public Button goodSideButton;
    public Button evilSideButton;

    private DialogueManager dialogueManager;
    private bool choiceMade = false;
    private bool firstDialogueMake = false;
    private string chosenSide = ""; // "good" ��� "evil"

    // ����� ��� ����������
    private const string FIRST_DIALOGUE_KEY = "ChoiceDialog_FirstDialogue";
    private const string CHOICE_MADE_KEY = "ChoiceDialog_ChoiceMade";
    private const string CHOSEN_SIDE_KEY = "ChoiceDialog_Side";

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        // ��������� ������
        goodSideButton.onClick.AddListener(ChooseGoodSide);
        evilSideButton.onClick.AddListener(ChooseEvilSide);

        // �������� ������ ������
        choicePanel.SetActive(false);

        // �������� ����������� ��������
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
            // ������ ������
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
            // ���� ����� ��� ������, ����� �������� �������������� ������
            Debug.Log($"����� ��� ������ �������: {chosenSide}");
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

        // ������ ��������
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

        // ������ ��������
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

        // ���������� ����������
        PlayerPrefs.DeleteKey(FIRST_DIALOGUE_KEY);
        PlayerPrefs.DeleteKey(CHOICE_MADE_KEY);
        PlayerPrefs.DeleteKey(CHOSEN_SIDE_KEY);

        Debug.Log("����� �������");
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

    // ����� ��� �������� ��������� �������
    public bool IsGoodSide()
    {
        return chosenSide == "good";
    }

    public bool IsEvilSide()
    {
        return chosenSide == "evil";
    }
}