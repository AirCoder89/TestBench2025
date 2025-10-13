using System.Collections.Generic;
using System.Linq;
using TestBench2025.Core.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.Board
{
    [RequireComponent(typeof(GridLayoutGroup))]
    internal class GridBuilder : MonoBehaviour
    {
        [SerializeField] private CardController cardPrefab;
        [SerializeField] private List<CardData> cardPool;
        
        [Header("Cards Appearance")]
        [SerializeField] private Color backgroundColor;
        [SerializeField] private Color frontColor;
        [SerializeField] private Color backColor;
        [SerializeField] private List<LayoutData> layouts;
        
        
        private GridLayoutGroup _gridLayout;
        private GridLayoutGroup GridLayout => _gridLayout ? _gridLayout : _gridLayout = GetComponent<GridLayoutGroup>();

        public void Initialize()
        {
            
        }

        public void Build(LevelDifficulty difficulty)
        {
            var layout = GetLayoutData(difficulty);
            if (layout == null)
            {
                Debug.LogError($"No layout found for difficulty {difficulty}");
                return;
            }

            SetupLayout(layout);
            
            var totalCards = layout.rows * layout.columns;
            var pairs = totalCards / 2;

            var selectedCards = cardPool.OrderBy(x => Random.value).Take(pairs).ToList();
            var cardsToUse = new List<CardData>(selectedCards);
            cardsToUse.AddRange(selectedCards);
            cardsToUse = cardsToUse.OrderBy(x => Random.value).ToList();

            // Instantiate cards
            ClearGrid();
            for (var i = 0; i < totalCards; i++)
            {
                var cardData = cardsToUse[i];
                var card = Instantiate(cardPrefab, transform);
                card.UpdateCardColors(layout.backgroundColor, layout.frontColor, layout.backColor);
                card.Initialize(cardData);
            }
        }
        
        private void ClearGrid()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
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
        
        private LayoutData GetLayoutData(LevelDifficulty difficulty)
        {
            foreach (var layout in layouts)
            {
                if (layout.difficulty == difficulty)
                {
                    return layout;
                }
            }

            return layouts.FirstOrDefault();
        }
    }
}
