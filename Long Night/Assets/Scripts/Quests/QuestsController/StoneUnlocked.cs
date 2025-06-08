using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static PlasticPipe.PlasticProtocol.Client.ConnectionCreator.PlasticProtoSocketConnection;

public class LockController : MonoBehaviour
{
    [Header("Настройки замка")]
    public ItemID requiredKey;
    public bool isLocked = true;
    public string portalOpenedAnimTrigger = "OpenPortal";
    public string goodSideAnimTrigger = "GoodSidePortal";
    public string evilSideAnimTrigger = "EvilSidePortal";

    [Header("Компоненты")]
    public Animator animator;
    public DialogueData lockedDialogue;
    public DialogueData unlockedDialogue;
    public DialogueData portalActivatedDialogue;
    public DialogueData portalReadyDialogue;

    [Header("События")]
    public UnityEvent onUnlock;
    public UnityEvent onPortalOpened;
    public UnityEvent onPortalReady;

    private Inventory inventory;
    private DialogueManager dialogueManager;
    private bool hasKeyInInventory;
    private bool isPortalActivated = false;
    private bool isPortalReady = false;
    private string chosenSide = ""; // "good" или "evil"

    private AudioSource _audioSource;

    private const string PORTAL_ACTIVATED_KEY = "PortalActivated";
    private const string PORTAL_READY_KEY = "PortalReady";
    private const string STONE_LOCK_KEY = "StoneLocked";
    private const string SIDE_SAVE_KEY = "ChoiceDialog_Side";

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        LoadPortalState();
    }

    private void Start()
    {
        LoadPortalState();

        if (isLocked == false)
        {
            animator.SetTrigger("Unlock");
        };

        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void LoadPortalState()
    {
        isLocked = PlayerPrefs.GetInt(STONE_LOCK_KEY, 1) == 1;
        chosenSide = PlayerPrefs.GetString(SIDE_SAVE_KEY, "");
        isPortalActivated = PlayerPrefs.GetInt(PORTAL_ACTIVATED_KEY, 0) == 1;
        isPortalReady = PlayerPrefs.GetInt(PORTAL_READY_KEY, 0) == 1;
    }

    private void SavePortalState()
    {
        PlayerPrefs.SetInt(PORTAL_ACTIVATED_KEY, isPortalActivated ? 1 : 0);
        PlayerPrefs.SetInt(PORTAL_READY_KEY, isPortalReady ? 1 : 0);
        PlayerPrefs.SetInt(STONE_LOCK_KEY, isLocked ? 1: 0);
        PlayerPrefs.Save();
    }

    public void TryUnlock()
    {
        if (isPortalReady)
        {
         
            SceneManager.LoadScene("EndScene");
            return;
        }

        if (isPortalActivated)
        {
            // Второе взаимодействие - активируем портал полностью
            ActivatePortalFinal();
            return;
        }

        if (!isLocked)
        {

            
            // Первое взаимодействие после открытия - начинаем активацию портала
            ActivatePortalInitial();
            return;
        }

        // Проверяем наличие ключа
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
        RemoveKeyFromInventory();
        AudioController.Instance.RegisterSoundEffect(_audioSource);
        _audioSource.Play();
        isLocked = false;
        SavePortalState();

        if (animator != null)
        {
            animator.SetTrigger("Unlock");
        }

        onUnlock.Invoke();
    }

    private void ActivatePortalInitial()
    {
        chosenSide = PlayerPrefs.GetString(SIDE_SAVE_KEY, "");

        if (chosenSide == "")
        {
            dialogueManager.StartDialogue(unlockedDialogue, "stone");
            return;
        }

        // Первая активация портала
        isPortalActivated = true;
        SavePortalState();

        // Проигрываем анимацию открытия портала
        if (animator != null)
        {
            if (chosenSide == "evil")
            {
                animator.SetTrigger(evilSideAnimTrigger);
            }
            if (chosenSide == "good")
            {
                animator.SetTrigger(goodSideAnimTrigger);
            }

        }

        // Показываем диалог активации
        if (portalActivatedDialogue != null)
        {
            dialogueManager.StartDialogue(portalActivatedDialogue, "stone");
        }

        onPortalOpened.Invoke();
    }

    private void ActivatePortalFinal()
    {
        chosenSide = PlayerPrefs.GetString(SIDE_SAVE_KEY, "");
        // Вторая активация - завершение портала
        isPortalReady = true;
        SavePortalState();
        
        // Проигрываем соответствующую анимацию
        if (animator != null)
        {
            animator.SetTrigger(portalOpenedAnimTrigger);
        }

        // Показываем диалог готовности портала
        if (portalReadyDialogue != null)
        {
            dialogueManager.StartDialogue(portalReadyDialogue, "stone");
        }

        onPortalReady.Invoke();
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

    [ContextMenu("Reset Portal State")]
    public void ResetPortalState()
    {
        isPortalActivated = false;
        isPortalReady = false;
        PlayerPrefs.DeleteKey(PORTAL_ACTIVATED_KEY);
        PlayerPrefs.DeleteKey(PORTAL_READY_KEY);
        if (animator != null)
        {
            animator.ResetTrigger(portalOpenedAnimTrigger);
            animator.ResetTrigger(goodSideAnimTrigger);
            animator.ResetTrigger(evilSideAnimTrigger);
        }
        Debug.Log("Состояние портала сброшено");
    }
}