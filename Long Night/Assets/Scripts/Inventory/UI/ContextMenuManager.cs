using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextMenuSystem : MonoBehaviour
{
    public static ContextMenuSystem Instance;

    public GameObject contextMenuPanel;
    public Button equipButton;
    public Button dropButton;

    private ItemData currentItem;
    private GameObject currentSlot;

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

        HideMenu();

        // Подписываемся на кнопки
        equipButton.onClick.AddListener(OnEquipClicked);
        dropButton.onClick.AddListener(OnDropClicked);
    }

    public void ShowMenu(ItemData item, GameObject slot, Vector2 position)
    {
        currentItem = item;
        currentSlot = slot;

        // Позиционируем меню
        contextMenuPanel.transform.position = position + new Vector2(100, -100);

        // Настраиваем текст кнопок в зависимости от типа предмета
        bool isEquipped = IsInEquipmentSlot();
        equipButton.GetComponentInChildren<TMP_Text>().text = isEquipped ? "Снять" : "Экипировать";

        contextMenuPanel.SetActive(true);
    }

    public void HideMenu()
    {
        contextMenuPanel.SetActive(false);
    }

    private bool IsInEquipmentSlot()
    {
        if (currentSlot == null) return false;
        return System.Array.Exists(Inventory.Instance.equipmentSlots,
            slot => slot == currentSlot);
    }

    private void OnEquipClicked()
    {
        if (currentItem == null || currentSlot == null) return;

        bool isEquipped = IsInEquipmentSlot();

        if (isEquipped)
        {
            Inventory.Instance.UnequipItem(currentItem, currentSlot);
        }
        else
        {
            Inventory.Instance.EquipItem(currentItem, currentSlot);
        }

        HideMenu();
    }

    private void OnDropClicked()
    {
        if (currentItem == null || currentSlot == null) return;

        // Реализуйте логику выброса предмета
        Debug.Log($"Выброшен предмет: {currentItem.itemName}");

       
        // Удаляем предмет
        Destroy(currentItem.gameObject);
        Inventory.Instance.SaveInventory();

        HideMenu();
    }
}