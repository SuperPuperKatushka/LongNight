using UnityEngine;

public class Pickup : MonoBehaviour
{

    public GameObject itemPrefab; // Теперь присваиваем префаб напрямую

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            if (inventory != null && inventory.AddItem(itemPrefab))
            {
                Destroy(gameObject);

            }
        }
    }
}