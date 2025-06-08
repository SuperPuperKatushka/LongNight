using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ScreenModeButton : MonoBehaviour
{
    public enum ScreenMode { Fullscreen, Windowed }
    public ScreenMode buttonMode;

    [Header("Настройки кнопок")]
    public Color activeColor = Color.green;
    public Color inactiveColor = Color.white;

    [Header("Ссылки")]
    public ScreenModeButton otherButton;
    public TMP_Text buttonText;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClick);

        // Начальная настройка цвета
        UpdateButtonAppearance();
    }

    private void OnButtonClick()
    {
        UpdateButtonAppearance();
        otherButton.UpdateButtonAppearance();

        // Переключаем режим экрана
        if (buttonMode == ScreenMode.Fullscreen)
        {
            ScreenManager.Instance.SetFullscreen();
 
        }
        else
        {
            ScreenManager.Instance.SetWindowed();
            
        }
        UpdateButtonAppearance();
        otherButton.UpdateButtonAppearance();

    }

    public void UpdateButtonAppearance()
    {
        bool isActive = (buttonMode == ScreenMode.Fullscreen && ScreenManager.Instance.IsFullscreen()) ||
                       (buttonMode == ScreenMode.Windowed && !ScreenManager.Instance.IsFullscreen());
        buttonText.color = isActive ? activeColor : inactiveColor;
    }
}
