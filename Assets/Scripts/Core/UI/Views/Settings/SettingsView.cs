using System.Collections.Generic;
using TestBench2025.Core.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.UI.Views.Settings
{
    internal class SettingsView : UICanvasView
    {
        [SerializeField] private CardDesignItem designItemPrefab;
        [SerializeField] private RectTransform itemsHolder;
        [SerializeField] private Slider musicVolume;
        [SerializeField] private Slider sfxVolume;
        [SerializeField] private Button clearSaveButton;

        private CardDesignItem _selectedDesignItem;
        private SettingsManager _settings;
        private List<CardDesignItem> _items;

        private ContentSizeFitter _sizeFitter;
        private ContentSizeFitter SizeFitter
        {
            get
            {
                if (_sizeFitter == null)
                    _sizeFitter = itemsHolder.GetComponent<ContentSizeFitter>();
                return _sizeFitter;
            }
        }
        
        private bool _isInitialized;
        
        public void Initialize(SettingsManager settings)
        {
            _settings = settings;
            
            musicVolume.SetValueWithoutNotify(_settings.MusicVolume);
            sfxVolume.SetValueWithoutNotify(_settings.SfxVolume);

            musicVolume.onValueChanged.AddListener(_settings.SetMusicVolume);
            sfxVolume.onValueChanged.AddListener(_settings.SetSfxVolume);

            GenerateDesignItems();
            
            _isInitialized = true;
        }

        private void GenerateDesignItems()
        {
            if(_isInitialized || _settings.CardDesigns == null || _settings.CardDesigns.Count == 0)  return;
            
            _items = new List<CardDesignItem>();
            SizeFitter.enabled = false;
            
            foreach (var design in _settings.CardDesigns)
            {
                var item = Instantiate(designItemPrefab, itemsHolder);
                var isSelected = design.id == _settings.CardBackID;
                item.Initialize(design);
                item.OnSelected += OnDesignItemSelected;
                
                if (isSelected)
                {
                    _selectedDesignItem = item;
                    _selectedDesignItem.Select();
                }
                
                _items.Add(item);
                
            }

            if (_selectedDesignItem == null)
            {
                //select first item by default
                _selectedDesignItem = _items[0];
                _selectedDesignItem.Select();
                _settings.SetCardBack(_selectedDesignItem.CurrentData.id);
            }
            
            SizeFitter.enabled = true;
        }

        private void OnDesignItemSelected(CardDesignItem selectedItem)
        {
            if (_selectedDesignItem != null && _selectedDesignItem != selectedItem)
            {
                _selectedDesignItem.Deselect();
            }

            _selectedDesignItem = selectedItem;
            _settings.SetCardBack(_selectedDesignItem.CurrentData.id);
        }
        
        public void ResetSettings()
        {
            _settings.ResetSettings();
            musicVolume.SetValueWithoutNotify(_settings.MusicVolume);
            sfxVolume.SetValueWithoutNotify(_settings.SfxVolume);
            
            if(_items == null || _items.Count == 0) return;
            foreach (var item in _items)
            {
                item.Deselect();
            }
            _selectedDesignItem = _items[0];
            _selectedDesignItem.Select();
        }
        
        private void UpdateClearSaveButton()
        {
            var hasSave = GameManager.Instance.HasSaveGame;
            clearSaveButton.interactable = hasSave;
        }

        public void ClearSaveGame()
        {
            GameManager.Instance.ClearSaveGame();
            UpdateClearSaveButton();
        }
    }
}