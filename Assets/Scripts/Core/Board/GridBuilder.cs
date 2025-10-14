using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TestBench2025.Core.Cards;
using TestBench2025.Core.Game;
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
        
        [Header("Animation")]
        [SerializeField] private RectTransform animationPosition;

        [SerializeField] private float entryAnimationSpeed = 2000F;
        [SerializeField] private float entryAnimationDelay = 0.25F;
        
        
        public List<CardController> Cards { get; private set; } = new List<CardController>();
        
        private GridLayoutGroup _gridLayout;
        private GridLayoutGroup GridLayout => _gridLayout ? _gridLayout : _gridLayout = GetComponent<GridLayoutGroup>();

        private LevelData _currentLevelData;
        public void Initialize()
        {
            
        }

        public void Build(LevelData levelData)
        {
            _currentLevelData = levelData;
            SetupLayout(levelData.layout);
            var pairs = levelData.layout.TotalCards / 2;

            var selectedCards = cardPool.OrderBy(x => Random.value).Take(pairs).ToList();
            var cardsToUse = new List<CardData>(selectedCards);
            cardsToUse.AddRange(selectedCards);
            cardsToUse = cardsToUse.OrderBy(x => Random.value).ToList();

            // Instantiate cards
            ClearGrid();
            for (var i = 0; i < levelData.layout.TotalCards; i++)
            {
                var cardData = cardsToUse[i];
                var card = Instantiate(cardPrefab, transform);
                card.Initialize(cardData, levelData.appearance.backgroundColor, levelData.appearance.frontColor, levelData.appearance.backColor);
                Cards.Add(card);
            }

            PlayEntryAnimation();
        }

        private void PlayEntryAnimation()
        {
            StartCoroutine(PlayEntrySequence());
        }

        private IEnumerator PlayEntrySequence()
        {
            // Ensure grid layout is updated before we start
            LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)transform);

            for (var i = 0; i < Cards.Count; i++)
            { 
                var card = Cards[i];
                var delay = i * entryAnimationDelay;

                var localPos = animationPosition.position.CalculateRelativeAnchoredPos(card.holder.parent as RectTransform);
                card.holder.anchoredPosition = localPos;

                card.PlayEntryAnimation(localPos, entryAnimationSpeed, delay);
            }

            // Wait until all cards finished moving before reveal
            yield return new WaitForSeconds(entryAnimationDelay * Cards.Count + 0.4f);

            PlayEntryReveal();
        }

        private void PlayEntryReveal()
        { 
            for (var i = 0; i < Cards.Count; i++) 
            { 
                var delay = i * entryAnimationDelay; 
                var card = Cards[i];
                StartCoroutine(PlayCardReveal(card, delay));
            }
        }

        private IEnumerator PlayCardReveal(CardController card, float delay)
        { 
            yield return new WaitForSeconds(delay);
            card.PlayEntryReveal(_currentLevelData.revealPreviewDuration, () =>
            {
                if (card == Cards[^1]) // last card triggers level ready
                    OnLevelReady?.Invoke();
            }); 
        }

        private void ClearGrid()
        {
            foreach (Transform child in transform)
            {
                var card = child.GetComponent<CardController>();
                if (card != null)
                {
                    card.StopAnimations(); // new helper
                }
                Destroy(child.gameObject);
            }
            Cards.Clear();
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
