using System;
using TestBench2025.Core.Board;
using TestBench2025.Core.Game.Save;
using UnityEngine;

namespace TestBench2025.Core.Game
{
    internal class ScoreManager : MonoBehaviour
    {
        public int Attempts { get; private set; }
        public int Matches { get; private set; }
        public int Coins { get; private set; }

        [Header("Points Settings")]
        [SerializeField] private int baseMatchPoints = 100;
        
        [Header("Combo Settings")]
        [SerializeField] private float comboDuration = 3f; // seconds before combo breaks
        
        public event Action<int> OnCoinsChanged;
        public event Action<int, int> OnProgressChanged;
        public event Action<int> OnComboChanged; 
        
        private float _comboTimer;
        private int _comboCount;
        private bool _comboActive;
        
        public void Initialize()
        {
            _comboActive = false;
            _comboCount = 0;
            _comboTimer = 0f;
            
            ResetScore();
        }
        
        private void OnEnable()
        {
            BoardController.OnPairEvaluated += HandlePairEvaluated;
        }

        private void OnDisable()
        {
            BoardController.OnPairEvaluated -= HandlePairEvaluated;
        }

        private void Update()
        {
            if (!_comboActive) return;
            _comboTimer -= Time.deltaTime;
            if (_comboTimer <= 0f)
            {
                ResetCombo();
            }
        }
        
        private void HandlePairEvaluated(bool isMatch)
        {
            Attempts++;

            if (isMatch)
            {
                Matches++;
                
                if (_comboActive && _comboTimer > 0f)
                {
                    _comboCount++;
                }
                else
                {
                    _comboCount = 1; // start new combo
                    _comboActive = true;
                }
                _comboTimer = comboDuration;
                
                var multiplier = Mathf.Max(1, _comboCount);
                var points = baseMatchPoints * multiplier;
                
                Coins += points;
                
                OnCoinsChanged?.Invoke(Coins);
                if(multiplier >= 2) OnComboChanged?.Invoke(multiplier);
            }
            else
            {
                ResetCombo();
            }
            
            OnProgressChanged?.Invoke(Matches, Attempts);
        }

        private void ResetCombo()
        {
            if (!_comboActive) return;

            _comboActive = false;
            _comboCount = 0;
            _comboTimer = 0f;
        }
        
        public void ResetScore()
        {
            Attempts = 0;
            Matches = 0;
            Coins = 0;
            _comboCount = 0;
            _comboActive = false;
            _comboTimer = 0f;

            OnCoinsChanged?.Invoke(Coins);
            OnProgressChanged?.Invoke(Matches, Attempts);
        }

        public void LoadState(SavedGame savedGame)
        {
            if (savedGame == null) return;
            
            Attempts = savedGame.attempts;
            Matches = savedGame.matches;
            Coins = savedGame.coins;
            _comboCount = 0;
            _comboActive = false;
            _comboTimer = 0f;

            OnCoinsChanged?.Invoke(Coins);
            OnProgressChanged?.Invoke(Matches, Attempts);
        }
    }
}