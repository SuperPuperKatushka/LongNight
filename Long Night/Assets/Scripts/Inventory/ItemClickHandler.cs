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
        // �������� ������ ��� ����� �������
        TooltipManager.Instance.HideTooltip();

        // �������������: �������� ����������� ���� ���� ������ ���� � ��������
        if (!IsPointerOverContextMenu())
        {
            ContextMenuSystem.Instance.HideMenu();
        }
    }

    private bool IsPointerOverContextMenu()
    {
        // �������� RectTransform ����
        RectTransform menuRect = ContextMenuSystem.Instance.contextMenuPanel.GetComponent<RectTransform>();

        // ������������ ������� ������� � ��������� ���������� ����
        Vector2 localMousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            menuRect,
            Input.mousePosition,
            null, // ���������� ������� ������
            out localMousePosition);

        // ���������, ��������� �� ����� ������ �������������� ����
        return menuRect.rect.Contains(localMousePosition);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ItemData itemData = GetComponent<ItemData>();
        if (itemData == null) return;

        // �������� ������� ������������ ���� ��� ������ �����
        GameObject currentSlot = transform.parent.gameObject;

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // ���������, �� ������� �� ��� ����
            if (ContextMenuSystem.Instance.contextMenuPanel.activeSelf)
            {
                ContextMenuSystem.Instance.HideMenu();
                return;
            }

            // ���������� ����������� ����
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
                Debug.Log("���������");
                break;
            case ItemID.ManaPotion:
                //PlayerStats.Instance.RestoreMana(itemData.value);
                Debug.Log("�������");

                break;
        }

        Destroy(gameObject);
        inventory.SaveInventory();
    }
}