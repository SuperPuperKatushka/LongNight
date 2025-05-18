using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ManaUI : MonoBehaviour
{
    [Header("���������")]
    public Transform manaContainer;  // ��������� ��� ������
    public GameObject manaPrefab;    // ������ ����� ������� ����

    void Update()
    {
        UpdateManaDisplay();
    }

    void UpdateManaDisplay()
    {
        // ������� ������ ������
        foreach (Transform child in manaContainer)
        {
            Destroy(child.gameObject);
        }

        // ������� ����� ������ �� �������� ���������� ����
        for (int i = 0; i < PlayerStats.Instance.currentMana; i++)
        {
            Instantiate(manaPrefab, manaContainer);
        }
    }
}
