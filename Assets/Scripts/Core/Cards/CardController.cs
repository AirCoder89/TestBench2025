using System;
using TestBench2025.Core.Game;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.Cards
{
    internal class CardController : MonoBehaviour
    {
        public static event Action<CardController> OnCardFlipped;
        public static event Action<CardController> OnCardRevealed;

        public RectTransform holder;
        [SerializeField] private CardAnimator animator;
        [SerializeField] private Button cardBtn;
        [SerializeField] private Image cardSymbol;
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardFront;
        [SerializeField] private Image cardBack;
        
        
        private bool CanReveal => GameManager.Instance.LevelStarted && State == CardState.Hidden;
        public CardState State { get; private set; }
        public CardData Data { get; private set; }

        public void Initialize(CardData data, Color background, Color front, Color back)
        {
            Data = data;
            cardSymbol.sprite = Data.symbol;
            UpdateCardColors(background, front, back);
            animator.Initialize();   
            cardBtn.onClick.AddListener(OnTap);
            ResetCard();
        }
        
        public void ResetCard()
        {
            State = CardState.Hidden;
            cardBtn.interactable = true;
            animator.ResetCard();
        }

        public void PlayEntryAnimation(Vector2 relativeOrigin, float speed, float delay, Action onComplete = null)
        {
            animator.PlayEntryAnimation(relativeOrigin, speed, delay, onComplete);
        }

        public void PlayEntryReveal(float previewDuration, Action onComplete = null)
        {
            animator.PlayEntryReveal(previewDuration, onComplete);
        }
        
        public void StopAnimations()
        {
            animator.KillAnimation();
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
            if (!CanReveal) return;
            
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
