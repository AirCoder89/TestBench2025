using System;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.Cards
{
    internal class CardController : MonoBehaviour
    {
        [SerializeField] private CardAnimator animator;
        [SerializeField] private Button cardBtn;
        [SerializeField] private Image cardSymbol;
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardFront;
        [SerializeField] private Image cardBack;
        
        
        private CardData _cardData;

        public void Initialize(CardData data)
        {
            _cardData = data;
            cardSymbol.sprite = _cardData.symbol;
            animator.Initialize();    
            cardBtn.onClick.AddListener(OnTap);
        }
        
        public void UpdateCardColors(Color background, Color front, Color back)
        {
            cardBackground.color = background;
            cardFront.color = front;
            cardBack.color = back;
        }
        
        private bool _isFlipped;
        private void OnTap()
        {
            _isFlipped = !_isFlipped;
            if (_isFlipped)
            {
                animator.FlipToFront();
            }
            else
            {
                animator.FlipToBack();
            }
        }
        
        
    }
}
