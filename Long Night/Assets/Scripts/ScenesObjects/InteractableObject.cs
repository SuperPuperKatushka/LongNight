using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour
{
    [Header("��������� �������")]
    public string interactableId;
    public bool isCollectible = true;
    public GameObject promptUI;

    [Header("������� ��� ��������������")]
    public UnityEvent onInteract;

    private bool playerInRange;

    private void Start()
    {
        if (promptUI != null)
            promptUI.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Q))
        {
            Interact();
        }
    }

    private void Interact()
    {
        // �������� �������
        onInteract?.Invoke();

        // ����� ����� �������� ���, ���� ����������� ������� �������
         QuestEvents.Instance.ItemCollected(interactableId);

        if (isCollectible)
            Destroy(gameObject);

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
