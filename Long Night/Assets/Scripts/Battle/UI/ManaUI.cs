using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ManaUI : MonoBehaviour
{
    [Header("Настройки")]
    public Transform manaContainer;  // Контейнер для иконок
    public GameObject manaPrefab;    // Префаб одной единицы маны

    void Update()
    {
        UpdateManaDisplay();
    }

    void UpdateManaDisplay()
    {
        // Удаляем старые иконки
        foreach (Transform child in manaContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаем новые иконки по текущему количеству маны
        for (int i = 0; i < PlayerStats.Instance.currentMana; i++)
        {
            Instantiate(manaPrefab, manaContainer);
        }
    }
}
