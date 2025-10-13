using System;
using UnityEngine;
using UnityEngine.UI;

namespace TestBench2025.Core.Cards
{
    internal class CardController : MonoBehaviour
    {
        [SerializeField] private CardAnimator animator;
        [SerializeField] private Button cardBtn;
        [SerializeField] private CardData cardData;

        private void Start()
        {
            Initialize();
            cardBtn.onClick.AddListener(OnTap);
        }

        public void Initialize()
        {
            animator.Initialize();    
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
