using System.Collections.Generic;
using UnityEngine;

public class SkillsUIManager : MonoBehaviour
{
    public static SkillsUIManager Instance { get; private set; }

    [Header("��������� UI")]
    public Transform skillsContainer;

    [Header("������� ������")]
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
        // ������� ���������� ������
        foreach (Transform child in skillsContainer)
        {
            Destroy(child.gameObject);
        }
        Debug.Log(equippedSkills.Count + "������");
        // �������� ����� ������
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
                    Debug.LogError($"������ {prefab.name} �� �������� ��������� SkillButton");
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
            _ => throw new System.ArgumentException($"��� ������� ��� ItemID: {id}")
        };
    }
}