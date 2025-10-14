using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestBench2025.Core.Cards;
using TestBench2025.Core.Game;
using TestBench2025.Core.Game.Save;
using TestBench2025.Utilities;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace TestBench2025.Core.Board
{
    [RequireComponent(typeof(GridLayoutGroup))]
    internal class GridBuilder : MonoBehaviour
    {
        public event Action OnLevelReady;
        
        [SerializeField] private CardController cardPrefab;
        [SerializeField] private List<CardData> cardPool;
        [SerializeField] private int preloadCount = 36;
        
        [Header("Animation")]
        [SerializeField] private RectTransform animationPosition;

        [SerializeField] private float entryAnimationSpeed = 2000F;
        [SerializeField] private float entryAnimationDelay = 0.25F;
        
        private ObjectPool<CardController> _cardPool;
        private List<CardController> _activeCards = new();
        
        private GridLayoutGroup _gridLayout;
        private GridLayoutGroup GridLayout => _gridLayout ? _gridLayout : _gridLayout = GetComponent<GridLayoutGroup>();

        private LevelData _currentLevelData;
        
        public IReadOnlyList<CardController> ActiveCards => _activeCards;

        public void Initialize()
        {
            _cardPool = new ObjectPool<CardController>(cardPrefab, preloadCount, transform);
        }

        public void Build(LevelData levelData)
        {
            ClearGrid();
            
            _currentLevelData = levelData;
            SetupLayout(levelData.layout);
            var pairs = levelData.layout.TotalCards / 2;

            var selectedCards = cardPool.OrderBy(x => Random.value).Take(pairs).ToList();
            var cardsToUse = new List<CardData>(selectedCards);
            cardsToUse.AddRange(selectedCards);
            cardsToUse = cardsToUse.OrderBy(x => Random.value).ToList();

            // Instantiate cards
            var generatedCards = new List<CardController>();
            for (var i = 0; i < levelData.layout.TotalCards; i++)
            {
                var cardData = cardsToUse[i];
                var card = _cardPool.Get();
                card.Initialize(cardData, levelData.appearance.backgroundColor, levelData.appearance.frontColor, levelData.appearance.backColor);
                generatedCards.Add(card);
            }

            _activeCards = generatedCards.OrderBy(e => e.transform.GetSiblingIndex()).ToList();

            StartCoroutine(PlayEntrySequence());
        }
        
        public void BuildFromSave(LevelData levelData, SavedGame save)
        {
            ClearGrid();
            
            _currentLevelData = levelData;
            SetupLayout(levelData.layout);
            
            var generatedCards = new List<CardController>();
            for (var i = 0; i < save.cards.Count; i++)
            {
                var saveCard = save.cards[i];
                var cardData = GetCardDataById(saveCard.cardId);
                var card = _cardPool.Get();
                card.Initialize(cardData, levelData.appearance.backgroundColor, levelData.appearance.frontColor, levelData.appearance.backColor);
                card.SetState((CardState)saveCard.state);
                generatedCards.Add(card);
            }

            _activeCards = generatedCards.OrderBy(e => e.transform.GetSiblingIndex()).ToList();

            // grid already built, so we skip animations and just mark ready
            OnLevelReady?.Invoke();
        }
        
        private CardData GetCardDataById(string id)
        {
            return cardPool.FirstOrDefault(c => c.cardId == id);
        }

        private IEnumerator PlayEntrySequence()
        {
            // Ensure grid layout is updated before we start
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

            for (var i = 0; i < _activeCards.Count; i++)
            { 
                var card = _activeCards[i];
                var delay = i * entryAnimationDelay;

                var localPos = animationPosition.position.CalculateRelativeAnchoredPos(card.holder.parent as RectTransform);
                card.holder.anchoredPosition = localPos;

                card.PlayEntryAnimation(localPos, entryAnimationSpeed, delay);
            }

            // Wait until all cards finished moving before reveal
            yield return new WaitForSeconds(entryAnimationDelay * _activeCards.Count + 0.4f);
           PlayEntryReveal();
        }

        private void PlayEntryReveal()
        { 
            for (var i = 0; i < _activeCards.Count; i++) 
            { 
                var delay = i * entryAnimationDelay; 
                var card = _activeCards[i];
                StartCoroutine(PlayCardReveal(card, delay));
            }
        }

        private IEnumerator PlayCardReveal(CardController card, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (card == null || !card.gameObject.activeInHierarchy) yield break;

            card.PlayEntryReveal(_currentLevelData.revealPreviewDuration, () =>
            {
                if (card == _activeCards[^1]) 
                    OnLevelReady?.Invoke();
            });
        }


        private void ClearGrid()
        {
            foreach (var card in _activeCards)
            {
                card.StopAnimations();
                _cardPool.Return(card);
            }

            _activeCards.Clear();
        }


        private void SetupLayout(LayoutData layout)
        {
            if (layout == null)
            {
                Debug.LogError("No layout data found!");
                return;
            }
            
            GridLayout.cellSize = layout.cellSize;
            GridLayout.spacing = layout.spacing;
            GridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            GridLayout.constraintCount = layout.columns;
        }
    }
}
