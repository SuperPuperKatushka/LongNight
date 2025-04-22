using UnityEngine;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private Image buttonImage;

    private bool isMuted = false;

    void Start()
    {
        // Автоматическое назначение AudioSource, если не задано
        if (backgroundMusic == null)
            backgroundMusic = GetComponent<AudioSource>();

        // Восстановление состояния звука (если нужно)
        isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        UpdateAudioState();
    }

    public void ToggleSound()
    {
        isMuted = !isMuted;
        UpdateAudioState();

        // Сохранение состояния
        PlayerPrefs.SetInt("Muted", isMuted ? 1 : 0);
    }

    private void UpdateAudioState()
    {
        backgroundMusic.mute = isMuted;
        buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
    }
}
