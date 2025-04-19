using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using TMPro;

public class BattleUI : MonoBehaviour
{
    public Slider playerHPBar;
    public TMP_Text playerHPText;

    public Slider enemyHPBar;
    public TMP_Text enemyHPText;

    //public Transform manaDotsContainer;
    //public GameObject manaDotPrefab;

    public TMP_Text messageText;
    public GameObject messageBox;

    private Coroutine messageCoroutine;

    //public ManaUI manaDisplay;

    void Start()
    {
        UpdateUI();
        messageBox.SetActive(false);
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
        enemyHPBar.maxValue = enemy.maxHP;
        enemyHPBar.value = enemy.currentHP;
        enemyHPText.text = $"{enemy.currentHP}/{enemy.maxHP}";

    }

    public void ShowMessage(string text)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageBox.SetActive(true);
        messageText.text = text;

        messageCoroutine = StartCoroutine(HideMessageAfterSeconds(5f));
    }

    private IEnumerator HideMessageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        messageBox.SetActive(false);
    }
}
        //UpdateManaDisplay(stats.currentMana);

    //void UpdateManaDisplay(int mana)
    //{
    //    foreach (Transform child in manaDotsContainer)
    //        Destroy(child.gameObject);

    //    for (int i = 0; i < mana; i++)
    //    {
    //        Instantiate(manaDotPrefab, manaDotsContainer);
    //    }
    //}
