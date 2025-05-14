using UnityEngine;

public class Slot : MonoBehaviour
{
    private Inventory inventory;
    public int index;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
    }

    private void Update()
    {
        if (transform.childCount <= 0)
        {
            inventory.isFull[index] = false;
        }
    }

    public void DropItem()
    {
        foreach (Transform child in transform)
        {
            Spawn spawn = child.GetComponent<Spawn>();
            if (spawn != null)
            {
                spawn.SpawnDroppedItem();
            }
            Destroy(child.gameObject);
            inventory.SaveInventory();
        }
    }
}