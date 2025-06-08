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

        // ������������� �� ������
        equipButton.onClick.AddListener(OnEquipClicked);
        dropButton.onClick.AddListener(OnDropClicked);
    }

    public void ShowMenu(ItemData item, GameObject slot, Vector2 position)
    {
        currentItem = item;
        currentSlot = slot;

        // ������������� ����
        contextMenuPanel.transform.position = position + new Vector2(100, -100);

        // ����������� ����� ������ � ����������� �� ���� ��������
        bool isEquipped = IsInEquipmentSlot();
        equipButton.GetComponentInChildren<TMP_Text>().text = isEquipped ? "�����" : "�����������";

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

        // ���������� ������ ������� ��������
        Debug.Log($"�������� �������: {currentItem.itemName}");

       
        // ������� �������
        Destroy(currentItem.gameObject);
        Inventory.Instance.SaveInventory();

        HideMenu();
    }
}