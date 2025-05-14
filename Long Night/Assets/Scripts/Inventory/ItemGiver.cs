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

    // ������� 1: ���� ������� �� ID
    public void GiveItem(ItemID itemID)
    {
        // ������� ������ �������� � Resources
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
        // ������� ������ �������� � Resources
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


    // ������� 2: ���� ������� �� �������
    public void GiveItem(GameObject itemPrefab)
    {
        if (itemPrefab != null)
        {
            Inventory.Instance.AddItem(itemPrefab);
        }
    }

    // ������� 3: ���� ��������� ������� �� �������
    public void GiveRandomItem(ItemID[] possibleItems)
    {
        if (possibleItems.Length > 0)
        {
            ItemID randomItem = possibleItems[Random.Range(0, possibleItems.Length)];
            GiveItem(randomItem);
        }
    }
}