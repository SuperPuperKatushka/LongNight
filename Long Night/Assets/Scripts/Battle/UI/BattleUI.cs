using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TMPro;
//using Codice.CM.Client.Differences.Graphic;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour
{
    public Slider playerHPBar;
    public TMP_Text playerHPText;

    public Slider enemyHPBar;
    public TMP_Text enemyHPText;

    public TMP_Text messageText;
    public GameObject messageBox;

    public GameObject messageTitleBox;
    public TMP_Text messageTitleText;

    public GameObject defeatScreen;
    public Button BackToMainScreen;


    private Coroutine messageCoroutine;


    void Start()
    {
        UpdateUI();
        messageBox.SetActive(false);
        ChangeTitle("“вой ход");
        BackToMainScreen.onClick.AddListener(OnBackToMainScreenClicked);
    }

    void Update()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {

        var stats = PlayerStats.Instance;

        playerHPBar.maxValue = stats.maxHP;
        playerHPBar.value = stats.currentHP;
        playerHPText.text = $"{stats.currentHP}/{stats.maxHP}";

        var enemy = BattleManager.Instance.enemy;
        enemyHPBar.maxValue = enemy.data.maxHP;
        enemyHPBar.value = enemy.currentHP;
        enemyHPText.text = $"{enemy.currentHP}/{enemy.data.maxHP}";

    }

    public void ShowMessage(string text)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageBox.SetActive(true);
        messageText.text = text;

        messageCoroutine = StartCoroutine(HideMessageAfterSeconds(5f));
    }

    public void ChangeTitle(string title)
    {
        messageTitleText.text = title;
    }

    private IEnumerator HideMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        messageBox.SetActive(false);
    }

    public void ShowDefeatScreen()
    {
        defeatScreen.SetActive(true);
    }



    private void OnBackToMainScreenClicked()
    {
        SceneManager.LoadScene("StartScene");
    }

}

