using System;
using TestBench2025.Core.Game;
using TestBench2025.Core.Game.Audio;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.Cards
{
    internal class CardController : MonoBehaviour
    {
        public static event Action<CardController> OnCardRevealed;

        public RectTransform holder;
        [SerializeField] private CardAnimator animator;
        [SerializeField] private Button cardBtn;
        [SerializeField] private Image cardSymbol;
        [SerializeField] private Image cardBackground;
        [SerializeField] private Image cardFront;
        [SerializeField] private Image cardBack;
        [SerializeField] private Image cardDesign;
        
        private bool CanReveal => GameManager.Instance.LevelStarted && State == CardState.Hidden;
        public CardState State { get; private set; }
        public CardData Data { get; private set; }
        private CardDesignData _currentDesign;

        public void Initialize(CardData data, CardDesignData design, Color background, Color front, Color back)
        {
            _currentDesign = design;
            UpdateDesign(design);
            
            Data = data;
            cardSymbol.sprite = Data.symbol;
            UpdateCardColors(background, front, back);
            animator.Initialize();   
            cardBtn.onClick.AddListener(OnTap);
            ResetCard();
        }
        
        private void OnDestroy()
        {
            cardBtn.onClick.RemoveListener(OnTap);
        }
        
        public void UpdateDesign(CardDesignData design)
        {
            _currentDesign = design;
            cardDesign.sprite = _currentDesign.pattern;
            cardDesign.pixelsPerUnitMultiplier = design.pixelPerUnit;
        }
        
        public void ResetCard()
        {
            State = CardState.Hidden;
            cardBtn.interactable = true;
            animator.ResetCard();
        }

        public void SetState(CardState state)
        {
            State = state;
            if (state == CardState.Hidden)
            {
                cardBtn.interactable = true;
                animator.FlipToBackImmediate();
            }
            else if (state == CardState.Revealed)
            {
                cardBtn.interactable = false;
                animator.FlipToFrontImmediate();
            }
            else if (state == CardState.Matched)
            {
                cardBtn.interactable = false;
                animator.MatchedImmediate();
            }
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

        private void UpdateCardColors(Color background, Color front, Color back)
        {
            cardBackground.color = background;
            cardFront.color = front;
            cardBack.color = back;
        }
        
        private void OnTap()
        {
            if (!CanReveal) return;
            SoundManager.Instance.Play(SFXName.CardFlip);
            State = CardState.Flipping;
            animator.FlipToFront(HandleRevealedCard);
        }

        private void HandleRevealedCard()
        {
            State = CardState.Revealed;
            OnCardRevealed?.Invoke(this);
        }

        public void SetMatched()
        {
            State = CardState.Matched;
            cardBtn.interactable = false;
            animator.Matched();
        }

        public void FlipBack()
        {
            if(State is CardState.Flipping or not CardState.Revealed) return; 
            State = CardState.Flipping;
            animator.FlipToBack(() => State = CardState.Hidden);
        }

    }
}
