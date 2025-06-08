using UnityEngine.EventSystems;
using UnityEngine;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;

public class ItemClickHandler : MonoBehaviour, IPointerClickHandler,
    IPointerEnterHandler, IPointerExitHandler
{
    private Inventory inventory;
    private ItemData itemData;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        itemData = GetComponent<ItemData>();
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (itemData != null)
        {
            string tooltipText = $"<b>{itemData.itemName}</b>\n{itemData.description}";
            TooltipManager.Instance.ShowTooltip(tooltipText);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Скрываем тултип при уходе курсора
        TooltipManager.Instance.HideTooltip();

        // Дополнительно: скрываем контекстное меню если курсор ушел с предмета
        if (!IsPointerOverContextMenu())
        {
            ContextMenuSystem.Instance.HideMenu();
        }
    }

    private bool IsPointerOverContextMenu()
    {
        // Получаем RectTransform меню
        RectTransform menuRect = ContextMenuSystem.Instance.contextMenuPanel.GetComponent<RectTransform>();

        // Конвертируем позицию курсора в локальные координаты меню
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            menuRect,
            Input.mousePosition,
            null, // Используем текущую камеру
            out localMousePosition);

        // Проверяем, находится ли точка внутри прямоугольника меню
        return menuRect.rect.Contains(localMousePosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemData itemData = GetComponent<ItemData>();
        if (itemData == null) return;

        // Получаем текущий родительский слот при каждом клике
        GameObject currentSlot = transform.parent.gameObject;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Проверяем, не открыто ли уже меню
            if (ContextMenuSystem.Instance.contextMenuPanel.activeSelf)
            {
                ContextMenuSystem.Instance.HideMenu();
                return;
            }

            // Показываем контекстное меню
            ContextMenuSystem.Instance.ShowMenu(
                itemData,
                transform.parent.gameObject,
                eventData.position
            );
        }
    }

    private void UseItem(ItemData itemData)
    {
        switch (itemData.itemID)
        {
            case ItemID.HealthPotion:
                //PlayerStats.Instance.Heal(itemData.value);
                Debug.Log("Пимпирпим");
                break;
            case ItemID.ManaPotion:
                //PlayerStats.Instance.RestoreMana(itemData.value);
                Debug.Log("Пумупум");

                break;
        }

        Destroy(gameObject);
        inventory.SaveInventory();
    }
}