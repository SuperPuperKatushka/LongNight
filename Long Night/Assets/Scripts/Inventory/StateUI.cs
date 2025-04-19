using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("UI Elements")]

    public TMP_Text hpText;
    public TMP_Text manaText;
    public TMP_Text levelText;
    public TMP_Text attackText;
    public TMP_Text expText;


    void Start()
    {
        UpdateUI();
    }

    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (PlayerStats.Instance == null) return;
        levelText.text = $"LV : {PlayerStats.Instance.level}";
        hpText.text = $"HP: {PlayerStats.Instance.currentHP} / {PlayerStats.Instance.maxHP}";
        manaText.text = $"MP: {PlayerStats.Instance.currentMana} / {PlayerStats.Instance.maxMana}";
        attackText.text = $"ATK: {PlayerStats.Instance.attackPower}";
        expText.text = $"EXP: {PlayerStats.Instance.currentEXP}";
    }
}
