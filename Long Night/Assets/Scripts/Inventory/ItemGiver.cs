using UnityEngine;

public class ItemGiver : MonoBehaviour
{

    private static ItemGiver instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public static ItemGiver Instance => instance;

    // Вариант 1: Даем предмет по ID
    public void GiveItem(ItemID itemID)
    {
        // Находим префаб предмета в Resources
        GameObject itemPrefab = Resources.Load<GameObject>($"Items/{itemID}");
        if (itemPrefab != null)
        {
            Inventory.Instance.AddItem(itemPrefab);
        }
        else
        {
        }
    }

    public void GiveItem(string itemID)
    {
        // Находим префаб предмета в Resources
        GameObject itemPrefab = Resources.Load<GameObject>($"Items/{itemID}");
        Debug.Log(itemPrefab);

;        if (itemPrefab != null)
        {
            Inventory.Instance.AddItem(itemPrefab);
        }
        else
        {
        }
    }


    // Вариант 2: Даем предмет по префабу
    public void GiveItem(GameObject itemPrefab)
    {
        if (itemPrefab != null)
        {
            Inventory.Instance.AddItem(itemPrefab);
        }
    }

    // Вариант 3: Даем случайный предмет из массива
    public void GiveRandomItem(ItemID[] possibleItems)
    {
        if (possibleItems.Length > 0)
        {
            ItemID randomItem = possibleItems[Random.Range(0, possibleItems.Length)];
            GiveItem(randomItem);
        }
    }
}