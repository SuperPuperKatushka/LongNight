using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        AudioController.Instance.RegisterSoundEffect(_audioSource);
    }

    private void OnDestroy()
    {
        AudioController.Instance.UnregisterSoundEffect(_audioSource);
    }
}