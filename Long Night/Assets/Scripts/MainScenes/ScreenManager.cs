using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public static ScreenManager Instance { get; private set; }

    private const string FULLSCREEN_PREF_KEY = "FullscreenMode";
    private const string WINDOWED_PREF_KEY = "WindowedResolution";

    [SerializeField]
    private Vector2[] availableResolutions = {
        new Vector2(1280, 720),
        new Vector2(1920, 1080),
        new Vector2(2560, 1440)
    };

    private int currentResolutionIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeScreenSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeScreenSettings()
    {
        // Загрузка сохраненных настроек
        bool fullscreen = PlayerPrefs.GetInt(FULLSCREEN_PREF_KEY, 1) == 1;
        currentResolutionIndex = PlayerPrefs.GetInt(WINDOWED_PREF_KEY, 0);

        // Применение настроек
        if (fullscreen)
        {
            SetFullscreen();
        }
        else
        {
            SetWindowed(currentResolutionIndex);
        }
    }

    public void SetFullscreen()
    {
        // Полноэкранный режим с текущим разрешением
        Resolution maxResolution = Screen.resolutions[Screen.resolutions.Length - 1];
        Screen.SetResolution(maxResolution.width, maxResolution.height, FullScreenMode.FullScreenWindow);

        // Сохранение в PlayerPrefs
        PlayerPrefs.SetInt(FULLSCREEN_PREF_KEY, 1);
        PlayerPrefs.Save();

        Debug.Log("Fullscreen mode activated");
    }

    public void SetWindowed(int resolutionIndex = -1)
    {
        // Проверка и корректировка индекса разрешения
        if (resolutionIndex == -1) resolutionIndex = currentResolutionIndex;
        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, availableResolutions.Length - 1);

        Vector2 resolution = availableResolutions[resolutionIndex];
        Screen.SetResolution((int)resolution.x, (int)resolution.y, FullScreenMode.Windowed);
        currentResolutionIndex = resolutionIndex;

        // Сохранение в PlayerPrefs
        PlayerPrefs.SetInt(FULLSCREEN_PREF_KEY, 0);
        PlayerPrefs.SetInt(WINDOWED_PREF_KEY, currentResolutionIndex);
        PlayerPrefs.Save();

        Debug.Log($"Windowed mode activated: {resolution.x}x{resolution.y}");
    }

    public void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            SetWindowed();
        }
        else
        {
            SetFullscreen();
        }
    }

    public bool IsFullscreen()
    {
        return Screen.fullScreen;
    }

    public Vector2 GetCurrentWindowedResolution()
    {
        return availableResolutions[currentResolutionIndex];
    }

    public Vector2[] GetAvailableResolutions()
    {
        return availableResolutions;
    }
}