
using System;
using System.Collections;
using System.Collections.Generic;
using TestBench2025.Core.Cards;
using TestBench2025.Core.Game;
using TestBench2025.Core.Game.Audio;
using TestBench2025.Core.Game.Save;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.Board
{
   internal struct CardPair
    {
        public CardController first;
        public CardController second;

        private bool IsValid => first != null && second != null;
        public bool CanProcess => IsValid && first.State != CardState.Matched && second.State != CardState.Matched;
        public CardPair(CardController first, CardController second)
        {
            this.first = first;
            this.second = second;
        }
    }
   
    internal class BoardController : MonoBehaviour
    {
        public static event Action<bool> OnPairEvaluated; 
        public event Action OnLevelReady;
        public event Action OnLevelCompleted;

        public GridBuilder Builder => builder;
        
        [SerializeField] private GridBuilder builder;
        [SerializeField] private float cardFlipBackDelay = 0.5F;
        [SerializeField] private float pairCheckDelay = 0.1F;
        [SerializeField] private Image boardBackground;
        [SerializeField] private Image topBarImage;
        [SerializeField] private Image backButtonImage;
        [SerializeField] private Image pauseButtonImage;

        private CardController _pendingCard;
        private readonly Queue<CardPair> _pendingPairs = new();
        private bool _isProcessing;

        public void Initialize()
        {
            CardController.OnCardRevealed += HandleCardRevealed;
            builder.OnLevelReady += OnLevelReady;
            builder.Initialize();
        }
        
        private void OnDestroy()
        {
            CardController.OnCardRevealed -= HandleCardRevealed;
            builder.OnLevelReady -= OnLevelReady;
        }
        
        public void StartLevel(LevelData levelData)
        {
            _pendingCard = null;
            _pendingPairs.Clear();
            _isProcessing = false;

            boardBackground.color = levelData.appearance.boardColor;
            topBarImage.color = levelData.appearance.topBarColor;
            backButtonImage.color = levelData.appearance.buttonColor;
            pauseButtonImage.color = levelData.appearance.buttonColor;
            builder.Build(levelData);
        }
        
        public void StartSavedLevel(LevelData levelData, SavedGame savedGame)
        {
            _pendingCard = null;
            _pendingPairs.Clear();
            _isProcessing = false;
            
            boardBackground.color = levelData.appearance.boardColor;
            topBarImage.color = levelData.appearance.topBarColor;
            backButtonImage.color = levelData.appearance.buttonColor;
            pauseButtonImage.color = levelData.appearance.buttonColor;
            builder.BuildFromSave(levelData, savedGame);
        }

        private void HandleCardRevealed(CardController card)
        {
            if (card.State == CardState.Matched) return;
            
            // prevent too many pending pairs
            if (_isProcessing && _pendingPairs.Count > 3) return; 

            if (_pendingCard == null)
            {
                _pendingCard = card;
                return;
            }

            _pendingPairs.Enqueue(new CardPair(_pendingCard, card));
            _pendingCard = null;

            if (!_isProcessing)
                StartCoroutine(ProcessPairs());
        }

        private IEnumerator ProcessPairs()
        {
            _isProcessing = true;

            while (_pendingPairs.Count > 0)
            {
                var pair = _pendingPairs.Dequeue();

                if (!pair.CanProcess) continue;

                // wait a moment before flipping back
                yield return new WaitForSeconds(cardFlipBackDelay);

                var isMatch = pair.first.Data.cardId == pair.second.Data.cardId;
                if (isMatch)
                {
                    pair.first.SetMatched();
                    pair.second.SetMatched();
                    SoundManager.Instance.Play(SFXName.Match);
                }
                else
                {
                    pair.first.FlipBack();
                    pair.second.FlipBack();
                    SoundManager.Instance.Play(SFXName.Mismatch);
                }

                OnPairEvaluated?.Invoke(isMatch);
                
                // small delay before processing next pair
                yield return new WaitForSeconds(pairCheckDelay);
            }

            _isProcessing = false;
            
            CheckLevelCompletion();
        }
        
        private void CheckLevelCompletion()
        {
            var allMatched = true;
            foreach (var card in builder.ActiveCards)
            {
                if (card.State != CardState.Matched)
                {
                    allMatched = false;
                    break;
                }
            }

            if (allMatched)
            {
                OnLevelCompleted?.Invoke();
            }
        }

        
    }
}
