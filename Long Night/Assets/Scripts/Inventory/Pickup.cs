using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] private Inventory inventory; // Позволяет задавать в инспекторе
    public GameObject slotButton;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = GameObject.FindGameObjectWithTag("Player")?.GetComponent<Inventory>();
        }
    }

    public bool TryPickup()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (!inventory.isFull[i])
            {
                inventory.isFull[i] = true;
                Instantiate(slotButton, inventory.slots[i].transform);
                Destroy(gameObject);
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TryPickup();
        }
    }
}
