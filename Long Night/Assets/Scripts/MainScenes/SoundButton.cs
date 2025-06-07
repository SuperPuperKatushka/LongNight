using UnityEngine;
using UnityEngine.UI;

public class SoundButton : MonoBehaviour
{
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;
    [SerializeField] private Image buttonImage;

    void Start()
    {
        UpdateButtonState(AudioController.Instance.IsMuted());
    }

    public void ToggleSound()
    {
        AudioController.Instance.ToggleSound();
    }

    public void UpdateButtonState(bool isMuted)
    {
        if (buttonImage != null)
            buttonImage.sprite = isMuted ? soundOffSprite : soundOnSprite;
    }
}