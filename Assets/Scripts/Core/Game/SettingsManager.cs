using System;
using System.Collections.Generic;
using UnityEngine;

namespace TestBench2025.Core.Game
{
    [Serializable]
    internal struct CardDesignData
    {
        public int id;
        public Sprite pattern;
        public float pixelPerUnit;
    }
    
    internal class SettingsManager : MonoBehaviour
    {
        [SerializeField] private List<CardDesignData> cardDesigns;
        
        [Header("Default Settings")]
        [SerializeField] [Range(0,1F)] private float defaultMusicVolume;
        [SerializeField] [Range(0,1F)] private float defaultSfxVolume;
        [SerializeField] private int defaultCardBackID = 6;
        
        public event Action<float> OnMusicVolumeChanged;
        public event Action<float> OnSfxVolumeChanged;
        public event Action<CardDesignData> OnCardBackChanged;
        
        private const string MusicKey = "MusicVolume";
        private const string SfxKey = "SFXVolume";
        private const string CardKey = "CardBackID";
        
        public List<CardDesignData> CardDesigns => cardDesigns;
        public float MusicVolume { get; private set; }
        public float SfxVolume { get; private set; }
        public int CardBackID { get; private set; }
        public CardDesignData CardDesign { get; private set; }
        
        public void Initialize()
        {
            MusicVolume = PlayerPrefs.GetFloat(MusicKey, defaultMusicVolume);
            SfxVolume = PlayerPrefs.GetFloat(SfxKey, defaultSfxVolume);
            CardBackID = PlayerPrefs.GetInt(CardKey, defaultCardBackID);
            CardDesign = GetCardDesignByID(CardBackID);
        }
        
        public void SetMusicVolume(float value)
        {
            MusicVolume = value;
            PlayerPrefs.SetFloat(MusicKey, value);
            OnMusicVolumeChanged?.Invoke(value);
        }

        public void SetSfxVolume(float value)
        {
            SfxVolume = value;
            PlayerPrefs.SetFloat(SfxKey, value);
            OnSfxVolumeChanged?.Invoke(value);
        }

        public void SetCardBack(int id)
        {
            CardBackID = id;
            PlayerPrefs.SetInt(CardKey, id);
            var design = GetCardDesignByID(id);
            CardDesign = design;
            OnCardBackChanged?.Invoke(CardDesign);
        }
        
        public void ResetSettings()
        {
            SetMusicVolume(defaultMusicVolume);
            SetSfxVolume(defaultSfxVolume);
            SetCardBack(defaultCardBackID);
            SaveAll();
        }
        
        private CardDesignData GetCardDesignByID(int id)
        {
            return cardDesigns.Find(c => c.id == id);
        }

        public void SaveAll() => PlayerPrefs.Save();
    }
}