using UnityEngine;
using UnityEngine.Events;

public class LockController : MonoBehaviour
{
    [Header("��������� �����")]
    public ItemID requiredKey;
    public bool isLocked = true;

    [Header("����������")]
    public Animator animator;
    public DialogueData lockedDialogue;
    public DialogueData unlockedDialogue;

    [Header("�������")]
    public UnityEvent onUnlock;

    private Inventory inventory;
    private DialogueManager dialogueManager;
    private bool hasKeyInInventory;
    
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    public void TryUnlock()
    {
        if (!isLocked) {
            dialogueManager.StartDialogue(unlockedDialogue, "stone");
            return; // ���� ��� ������������� - ������ �� ������
        } 
            

        // ��������� ������� �����
        hasKeyInInventory = CheckForKey();

        if (hasKeyInInventory)
        {
            Unlock();
        }
        else if (lockedDialogue != null)
        {
            dialogueManager.StartDialogue(lockedDialogue, "stone");
        }
    }

    private bool CheckForKey()
    {
        foreach (var slot in inventory.slots)
        {
            if (slot.transform.childCount > 0)
            {
                ItemData item = slot.transform.GetChild(0).GetComponent<ItemData>();
                if (item != null && item.itemID == requiredKey)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void Unlock()
    {
        // ������� ���� �� ���������
        RemoveKeyFromInventory();
        AudioController.Instance.RegisterSoundEffect(_audioSource);
        _audioSource.Play();
        // ������ ��������� �����
        isLocked = false;

        // ��������� ��������
        if (animator != null)
        {
            animator.SetTrigger("Unlock");
        }


        // �������� �������
        onUnlock.Invoke();
    }

    private void RemoveKeyFromInventory()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            if (inventory.isFull[i] &&
                inventory.slots[i].transform.GetChild(0).GetComponent<ItemData>().itemID == requiredKey)
            {
                Destroy(inventory.slots[i].transform.GetChild(0).gameObject);
                inventory.isFull[i] = false;
                inventory.SaveInventory();
                break;
            }
        }
    }
}