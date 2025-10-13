using System;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.Cards
{
    internal class CardController : MonoBehaviour
    {
        public static event Action<CardController> OnCardFlipped;
        public static event Action<CardController> OnCardRevealed;

        
        [SerializeField] private CardAnimator animator;
        [SerializeField] private Button cardBtn;
        [SerializeField] private Image cardSymbol;
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardFront;
        [SerializeField] private Image cardBack;
        
        public CardState State { get; private set; }
        public CardData Data { get; private set; }

        public void Initialize(CardData data, Color background, Color front, Color back)
        {
            Data = data;
            cardSymbol.sprite = Data.symbol;
            animator.Initialize();    
            UpdateCardColors(background, front, back);
            cardBtn.onClick.AddListener(OnTap);
            State = CardState.Hidden;
            cardBtn.interactable = true;
        }

        private void OnDestroy()
        {
            cardBtn.onClick.RemoveListener(OnTap);
        }

        private void UpdateCardColors(Color background, Color front, Color back)
        {
            cardBackground.color = background;
            cardFront.color = front;
            cardBack.color = back;
        }
        
        private void OnTap()
        {
            if(State != CardState.Hidden) return; // Ignore if not hidden
            
            State = CardState.Flipping;
            OnCardFlipped?.Invoke(this);
            animator.FlipToFront(HandleRevealedCard);
        }

        private void HandleRevealedCard()
        {
            Debug.Log("Revealed");
            State = CardState.Revealed;
            OnCardRevealed?.Invoke(this);
        }

        public void SetMatched()
        {
            State = CardState.Matched;
            cardBtn.interactable = false;
            
            Debug.Log("Matched");
            
            animator.Matched();
        }

        public void FlipBack()
        {
            if(State is CardState.Flipping or not CardState.Revealed) return; 
            State = CardState.Flipping;
            animator.FlipToBack(() => State = CardState.Hidden);
            
            Debug.Log("Flipped Back");
        }
    }
}
