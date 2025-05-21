using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    public static event System.Action<string> OnInteracted;

    [Header("Настройки объекта")]
    public string interactableId;
    public bool isCollectible = true;
    public GameObject promptUI;

    [Header("Параметры взаимодействия")]
    [Tooltip("Количество возможных взаимодействий. 0 или меньше — бесконечно.")]
    public int interactionLimit = 1;

    [Header("Событие при взаимодействии")]
    public UnityEvent onInteractEvent;

    private bool playerInRange;
    private int interactionCount = 0;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            TryInteract();
        }
    }

    private void TryInteract()
    {
        if (interactionLimit <= 0 || interactionCount < interactionLimit)
        {
            Interact();
            interactionCount++;
        }
    }
    //Метод взаимодействия с предметом
    private void Interact()
    {
        onInteractEvent?.Invoke();
        OnInteracted?.Invoke(interactableId);

        if (isCollectible && (interactionLimit <= 0 || interactionCount >= interactionLimit))
        {
            Destroy(gameObject);
        }

        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (promptUI != null)
                promptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (promptUI != null)
                promptUI.SetActive(false);
        }
    }
}
