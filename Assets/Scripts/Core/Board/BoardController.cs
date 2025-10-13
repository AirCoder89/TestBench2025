
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
        [SerializeField] private GridBuilder builder;
        [SerializeField] private float cardFlipBackDelay = 0.5F;
        [SerializeField] private float pairCheckDelay = 0.1F;

        private CardController _tempCard;
        private readonly Queue<CardPair> _pendingPairs = new();
        private bool _isProcessing;

        public void Initialize()
        {
            CardController.OnCardRevealed += HandleCardRevealed;
            builder.Initialize();
        }
        
        private void OnDestroy()
        {
            CardController.OnCardRevealed -= HandleCardRevealed;
        }
        
        public void StartLevel(LevelDifficulty difficulty)
        {
            _tempCard = null;
            _pendingPairs.Clear();
            _isProcessing = false;
            
            builder.Build(difficulty);
        }

        private void HandleCardRevealed(CardController card)
        {
            if (card.State == CardState.Matched)
                return;

            // if we don't have a temp -> hold this one
            if (_tempCard == null)
            {
                _tempCard = card;
                return;
            }

            // if we already have a waiting card -> make a pair
            if (_tempCard != null)
            {
                _pendingPairs.Enqueue(new CardPair(_tempCard, card));
                _tempCard = null;

                // start comparison if not already running
                if (!_isProcessing)
                    StartCoroutine(ProcessPairs());
            }
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

                if (pair.first.Data.cardId == pair.second.Data.cardId)
                {
                    pair.first.SetMatched();
                    pair.second.SetMatched();
                }
                else
                {
                    pair.first.FlipBack();
                    pair.second.FlipBack();
                }

                // small delay before processing next pair
                yield return new WaitForSeconds(pairCheckDelay);
            }

            _isProcessing = false;
        }
    }
}
