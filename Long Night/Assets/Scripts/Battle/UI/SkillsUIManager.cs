using System.Collections.Generic;
using UnityEngine;

public class SkillsUIManager : MonoBehaviour
{
    public static SkillsUIManager Instance { get; private set; }

    [Header("Настройки UI")]
    public Transform skillsContainer;

    [Header("Префабы кнопок")]
    public GameObject blueRuneButtonPrefab;
    public GameObject redRuneButtonPrefab;
    public GameObject greenRuneButtonPrefab;
    public GameObject purpleSkillButtonPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetupSkillButtons(List<ItemData> equippedSkills)
    {
        // Очистка предыдущих кнопок
        foreach (Transform child in skillsContainer)
        {
            Destroy(child.gameObject);
        }
        Debug.Log(equippedSkills.Count + "Скиллы");
        // Создание новых кнопок
        foreach (var skill in equippedSkills)
        {
            GameObject prefab = GetButtonPrefab(skill.itemID);
            if (prefab != null)
            {
                GameObject button = Instantiate(prefab, skillsContainer);
                if (button.TryGetComponent(out SkillButton skillButton))
                {
                    skillButton.Setup(skill);
                }
                else
                {
                    Debug.LogError($"Префаб {prefab.name} не содержит компонент SkillButton");
                }
            }
        }
    }

    private GameObject GetButtonPrefab(ItemID id)
    {
        return id switch
        {
            ItemID.GuardianShieldRuna => blueRuneButtonPrefab,
            ItemID.FlashFury => redRuneButtonPrefab,
            ItemID.LifeImpulse => greenRuneButtonPrefab,
            ItemID.RealityRift => purpleSkillButtonPrefab,
            _ => throw new System.ArgumentException($"Нет префаба для ItemID: {id}")
        };
    }
}