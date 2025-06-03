using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class AudioController : MonoBehaviour
{
    public static AudioController Instance { get; private set; }

    [SerializeField] private AudioSource _backgroundMusicSource;
    [SerializeField] private List<AudioClip> _backgroundMusicTracks; // Можно задать треки для разных сцен

    private bool _isMuted = false;
    private List<AudioSource> _soundEffects = new List<AudioSource>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded; // Подписываемся на событие загрузки сцены
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        _isMuted = PlayerPrefs.GetInt("Muted", 0) == 1;
        UpdateAudioState();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded; // Отписываемся при уничтожении
        }
    }

    // Вызывается при загрузке новой сцены
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Можно задать разную музыку для разных сцен (например, по индексу или имени сцены)
        if (_backgroundMusicTracks.Count > 0)
        {
            int trackIndex = scene.buildIndex % _backgroundMusicTracks.Count; // Простой пример
            SetBackgroundMusic(_backgroundMusicTracks[trackIndex]);
        }
    }

    // Установка новой фоновой музыки (можно вызывать вручную из других скриптов)
    public void SetBackgroundMusic(AudioClip newTrack)
    {
        if (_backgroundMusicSource == null)
        {
            _backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            _backgroundMusicSource.loop = true;
        }

        if (_backgroundMusicSource.clip != newTrack)
        {
            _backgroundMusicSource.clip = newTrack;
            _backgroundMusicSource.mute = _isMuted;
            if (!_isMuted) _backgroundMusicSource.Play();
        }
    }

    public void ToggleSound()
    {
        _isMuted = !_isMuted;
        UpdateAudioState();
        PlayerPrefs.SetInt("Muted", _isMuted ? 1 : 0);
    }

    private void UpdateAudioState()
    {
        // Обновляем музыку
        if (_backgroundMusicSource != null)
        {
            _backgroundMusicSource.mute = _isMuted;
            if (!_isMuted && !_backgroundMusicSource.isPlaying)
                _backgroundMusicSource.Play();
        }

        // Обновляем звуковые эффекты
        foreach (var sound in _soundEffects)
        {
            if (sound != null) sound.mute = _isMuted;
        }

        // Обновляем все кнопки звука
        UpdateAllSoundButtons();
    }

    public void RegisterSoundEffect(AudioSource sound)
    {
        if (!_soundEffects.Contains(sound))
        {
            _soundEffects.Add(sound);
            sound.mute = _isMuted;
        }
    }

    public void UnregisterSoundEffect(AudioSource sound)
    {
        _soundEffects.Remove(sound);
    }

    private void UpdateAllSoundButtons()
    {
        var buttons = FindObjectsOfType<SoundButton>(true);
        foreach (var button in buttons)
        {
            button.UpdateButtonState(_isMuted);
        }
    }

    public bool IsMuted() => _isMuted;
}