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
        Combo, Match, Mismatch, CardFlip, LevelComplete, ButtonClick, CardMove
    }
    
    internal class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance { get; private set; }

        [SerializeField] private AudioSource backgroundMusic;
        [SerializeField] private List<SoundEffect> sounds = new();

        [Header("Pool Settings")]
        [SerializeField] private AudioSourcePooled audioSourcePrefab;
        [SerializeField] private int initialPoolSize = 4;
        [SerializeField] private float defaultVolume = 0.8f;

        private ObjectPool<AudioSourcePooled> _pool;
        private readonly Dictionary<SFXName, AudioClip> _soundMap = new();
        private float _musicPauseTime;
        
        public void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (audioSourcePrefab == null)
            {
                Debug.LogError("SoundManager: Missing AudioSource prefab!");
                enabled = false;
                return;
            }
            
            foreach (var s in sounds)
            {
                if (s.clip != null && !_soundMap.ContainsKey(s.name))
                    _soundMap.Add(s.name, s.clip);
            }
            _pool = new ObjectPool<AudioSourcePooled>(audioSourcePrefab, initialPoolSize, transform);
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
        
        public void Play(SFXName soundName, float volume = -1f)
        {
            if (!_soundMap.TryGetValue(soundName, out var clip) || clip == null)
            {
                Debug.LogWarning($"SoundManager: Sound '{soundName}' not found!");
                return;
            }

            var source = _pool.Get();
            var vol = volume < 0 ? defaultVolume : volume;
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