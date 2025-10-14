
using System;
using System.Collections;
using System.Collections.Generic;
using TestBench2025.Core.Cards;
using TestBench2025.Core.Game;
using UnityEngine;

namespace TestBench2025.Core.Board
{
   internal struct CardPair
    {
        public CardController first;
        public CardController second;

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
        
        [SerializeField] private GridBuilder builder;
        [SerializeField] private float cardFlipBackDelay = 0.5F;
        [SerializeField] private float pairCheckDelay = 0.1F;

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
            
            builder.Build(levelData);
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

                // skip null or matched
                if (pair.first == null || pair.second == null || pair.first.State == CardState.Matched ||  pair.second.State == CardState.Matched)
                {
                    continue;
                }

                // wait a moment before flipping back
                yield return new WaitForSeconds(cardFlipBackDelay);

                bool isMatch = pair.first.Data.cardId == pair.second.Data.cardId;
                if (isMatch)
                {
                    pair.first.SetMatched();
                    pair.second.SetMatched();
                }
                else
                {
                    pair.first.FlipBack();
                    pair.second.FlipBack();
                }

                OnPairEvaluated?.Invoke(isMatch);
                
                // small delay before processing next pair
                yield return new WaitForSeconds(pairCheckDelay);
            }

            _isProcessing = false;
        }
    }
}
