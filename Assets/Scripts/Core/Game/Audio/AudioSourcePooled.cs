using System;
using UnityEngine;

namespace TestBench2025.Core.Game.Audio
{
    [RequireComponent(typeof(AudioSource))]
    internal class AudioSourcePooled : MonoBehaviour
    {
        private AudioSource _audioSource;
        private AudioSource Source
        {
            get
            {
                if (_audioSource == null)
                    _audioSource = GetComponent<AudioSource>();
                return _audioSource;
            }
        }
        
        private Action<AudioSourcePooled> _onFinished;

        public void Play(AudioClip clip, float volume, Action<AudioSourcePooled> onFinished)
        {
            if (clip == null) return;

            _onFinished = onFinished;
            Source.clip = clip;
            Source.volume = volume;
            Source.Play();

            CancelInvoke(nameof(NotifyFinished));
            Invoke(nameof(NotifyFinished), clip.length);
        }

        private void NotifyFinished()
        {
            Source.Stop();
            _onFinished?.Invoke(this);
        }

        public void ResetSource()
        {
            Source.clip = null;
            Source.Stop();
        }
    }
}