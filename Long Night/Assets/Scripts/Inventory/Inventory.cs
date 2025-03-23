using UnityEngine;

public class Inventory : MonoBehaviour
{
    public bool[] isFull;
    public GameObject[] slots;
    public GameObject inventory;
    private bool inventoryOpen;

    private void Start()
    {
        inventoryOpen = false;
        inventory.SetActive(false);

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) // Проверка нажатия клавиши E
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (inventoryOpen == false) 
        {
            inventoryOpen = true;
            inventory.SetActive(true);
        }
        else if (inventoryOpen == true)
        {
            inventoryOpen = false;
            inventory.SetActive(false);
        }

    }
}
