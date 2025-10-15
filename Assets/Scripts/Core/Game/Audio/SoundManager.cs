using System;
using System.Collections.Generic;
using TestBench2025.Utilities;
using UnityEngine;

namespace TestBench2025.Core.Game.Audio
{
    [Serializable]
    internal struct SoundEffect
    {
        public SFXName name;
        public AudioClip clip;
    }

    public enum SFXName
    {
        Combo, Match, Mismatch, CardFlip, LevelComplete, ButtonClick
    }
    
    internal class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] private AudioSource backgroundMusic;
        [SerializeField] private List<SoundEffect> sounds = new();

        [Header("Pool Settings")]
        [SerializeField] private AudioSourcePooled audioSourcePrefab;
        [SerializeField] private int initialPoolSize = 4;
        
        
        private float _sfxVolume;
        private float _musicVolume;

        private ObjectPool<AudioSourcePooled> _pool;
        private readonly Dictionary<SFXName, AudioClip> _soundMap = new();
        private float _musicPauseTime;
        private SettingsManager _settingsManager;
        
        public void Initialize(SettingsManager settings)
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (audioSourcePrefab == null)
            {
                Debug.LogError("SoundManager: Missing AudioSource prefab!");
                enabled = false;
                return;
            }
            
            _settingsManager = settings;
            _settingsManager.OnMusicVolumeChanged += UpdateMusicVolume;
            _settingsManager.OnSfxVolumeChanged += UpdateSfxVolume;
            _sfxVolume = _settingsManager.SfxVolume;
            _musicVolume = _settingsManager.MusicVolume;
            
            foreach (var s in sounds)
            {
                if (s.clip != null && !_soundMap.ContainsKey(s.name))
                    _soundMap.Add(s.name, s.clip);
            }
            _pool = new ObjectPool<AudioSourcePooled>(audioSourcePrefab, initialPoolSize, transform);
        }

        private void OnDestroy()
        {
            if (_settingsManager != null)
            {
                _settingsManager.OnMusicVolumeChanged -= UpdateMusicVolume;
                _settingsManager.OnSfxVolumeChanged -= UpdateSfxVolume;
            }
        }

        private void UpdateSfxVolume(float volume)
        {
            _sfxVolume = volume;
        }

        private void UpdateMusicVolume(float volume)
        {
            _musicVolume = volume;
            if(backgroundMusic != null)
                backgroundMusic.volume = volume;
        }

        public void StartMusic()
        {
            if(backgroundMusic != null && !backgroundMusic.isPlaying)
                backgroundMusic.Play();
        }
        
        public void StopMusic()
        {
            if(backgroundMusic != null && backgroundMusic.isPlaying)
                backgroundMusic.Stop();
        }
        
        public void PauseMusic()
        {
            if(backgroundMusic != null && backgroundMusic.isPlaying)
            {
                _musicPauseTime = backgroundMusic.time;
                backgroundMusic.Pause();
            }
        }
        
        public void ResumeMusic()
        {
            if(backgroundMusic != null && !backgroundMusic.isPlaying)
            {
                backgroundMusic.time = _musicPauseTime;
                backgroundMusic.Play();
            }
        }
        
        public void Play(SFXName soundName)
        {
            if (!_soundMap.TryGetValue(soundName, out var clip) || clip == null)
            {
                Debug.LogWarning($"SoundManager: Sound '{soundName}' not found!");
                return;
            }

            var source = _pool.Get();
            var vol = _sfxVolume;
            source.Play(clip, vol, ReturnToPool);
        }
        
        private void ReturnToPool(AudioSourcePooled source)
        {
            source.ResetSource();
            _pool.Return(source);
        }

        public void StopAll()
        {
            foreach (Transform child in transform)
            {
                var sfx = child.GetComponent<AudioSourcePooled>();
                if (sfx != null)
                {
                    sfx.ResetSource();
                    _pool.Return(sfx);
                }
            }
        }
    }
}