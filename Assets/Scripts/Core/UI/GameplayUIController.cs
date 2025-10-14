using TestBench2025.Core.Game;
using UnityEngine;

namespace TestBench2025.Core.UI
{
    internal class GameplayUIController : UICanvasView
    {
        [SerializeField] private GameplayHUD hud;
        [SerializeField] private ComboEffectUI comboEffect;
        
        private ScoreManager _scoreManager;
        
        public void Initialize(ScoreManager manager)
        {
            _scoreManager = manager;
            _scoreManager.OnCoinsChanged += hud.UpdateCoins;
            _scoreManager.OnProgressChanged += hud.UpdateProgress;
            _scoreManager.OnComboChanged += comboEffect.PlayComboFeedback;
            
            hud.Initialize();
            comboEffect.Initialize();
        }

        private void OnDestroy()
        {
            if (_scoreManager == null) return;
            _scoreManager.OnCoinsChanged -= hud.UpdateCoins;
            _scoreManager.OnProgressChanged -= hud.UpdateProgress;
            _scoreManager.OnComboChanged -= comboEffect.PlayComboFeedback;
        }
    }
}