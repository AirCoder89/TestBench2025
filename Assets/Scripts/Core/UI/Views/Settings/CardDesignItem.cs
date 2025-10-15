using System;
using TestBench2025.Core.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.UI.Views.Settings
{
    [RequireComponent(typeof(Button))]
    internal class CardDesignItem : MonoBehaviour
    {
        [SerializeField] private Image patternImage;
        
        public Action<CardDesignItem> OnSelected;
        private Button _button;
        private Button Button
        {
            get
            {
                if (_button == null)
                    _button = GetComponent<Button>();
                return _button;
            }
        }
        
        public CardDesignData CurrentData { get; private set; }
        private bool _isSelected;

        public void Initialize(CardDesignData data)
        {
            _isSelected = false;
            CurrentData = data;
            Button.onClick.AddListener(OnTap);
            
            patternImage.sprite = data.pattern;
            patternImage.pixelsPerUnitMultiplier = data.pixelPerUnit;
            UpdateVisual();
        }

        public void Select()
        {
            _isSelected = true;
            UpdateVisual();
        }
        
        public void Deselect()
        {
            _isSelected = false;
            UpdateVisual();
        }

        private void OnDestroy()
        {
            OnSelected = null;
            Button.onClick.RemoveListener(OnTap);
        }

        private void OnTap()
        {
            _isSelected = true;
            UpdateVisual();
            OnSelected?.Invoke(this);
        }

        private void UpdateVisual()
        {
            Button.interactable = !_isSelected;
        }
    }
}