using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class ContinueButtonController : MonoBehaviour
{
    [SerializeField] private string saveFileName = "save.json";
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text buttonText; // Теперь работаем с Text

    [Header("Настройки визуала")]
    [SerializeField][Range(0, 1)] private float disabledAlpha = 0.5f;
    [SerializeField] private bool changeInteractable = true;
    [SerializeField] private bool changeTransparency = true;

    private void Start()
    {
        if (continueButton == null)
            continueButton = GetComponent<Button>();

        if (buttonText == null)
            buttonText = GetComponentInChildren<TMP_Text>(); // Ищем Text в дочерних объектах

        CheckSaveFile();
    }

    private void CheckSaveFile()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        bool saveExists = File.Exists(path);

        if (changeInteractable)
            continueButton.interactable = saveExists;

        if (changeTransparency && buttonText != null)
        {
            Color color = buttonText.color;
            color.a = saveExists ? 1f : disabledAlpha;
            buttonText.color = color;
        }

    }

    public void RefreshButtonState()
    {
        CheckSaveFile();
    }

#if UNITY_EDITOR
    [ContextMenu("Сымитировать наличие сохранения")]
    private void SimulateSaveExists()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (!File.Exists(path))
        {
            File.CreateText(path).Close();
        }
        RefreshButtonState();
    }

    [ContextMenu("Удалить тестовое сохранение")]
    private void DeleteTestSave()
    {
        string path = Path.Combine(Application.persistentDataPath, saveFileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        RefreshButtonState();
    }
#endif
}