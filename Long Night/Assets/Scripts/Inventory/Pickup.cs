using UnityEngine;
using UnityEngine.Events;

public class Pickup : MonoBehaviour
{

    public GameObject itemPrefab; // Теперь присваиваем префаб напрямую
    public UnityEvent onPickUp;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Inventory inventory = other.GetComponent<Inventory>();
            onPickUp?.Invoke();
            if (inventory != null && inventory.AddItem(itemPrefab))
            {
                Destroy(gameObject);

            }
        }
    }
}