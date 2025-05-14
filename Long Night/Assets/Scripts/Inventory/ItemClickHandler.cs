using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    private Inventory inventory;
    private GameObject parentSlot;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        parentSlot = transform.parent.gameObject;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
            Debug.Log("PointerEventData.InputButton.Left");
        ItemData itemData = GetComponent<ItemData>();
        if (itemData == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {

            if (itemData.itemType == ItemType.Skill || itemData.itemType == ItemType.Equipment)
            {
                if (!inventory.equipmentSlots.Contains(parentSlot))
                {
                    inventory.EquipItem(itemData, parentSlot);
                }
            }
            else
            {
                //UseItem(itemData);
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("PointerEventData.InputButton.Right");

            //if (inventory.equipmentSlots.Contains(parentSlot))
            //{
            //    inventory.UnequipItem(itemData, parentSlot);
            //}
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