using System;
using System.Collections;
using TestBench2025.Utilities;
using UnityEngine;

namespace TestBench2025.Core.Cards
{
    internal class CardAnimator : MonoBehaviour
    {
        [SerializeField] private RectTransform cardContent;
        [SerializeField] private GameObject frontFace;
        [SerializeField] private GameObject backFace;
        [SerializeField] private float flipDuration = 0.4f;

        private Coroutine _currentRoutine;

        public void Initialize()
        {
            ResetCard();
        }
        
        public void FlipToFront(Action onComplete = null, float startDelay = 0f)
        {
            StartCoroutine(FlipRoutine(0, 90, () =>
            {
                frontFace.SetActive(true); 
                backFace.SetActive(false); 
                StartCoroutine(FlipRoutine(90, 0, onComplete));
            }, startDelay));
        }

        public void FlipToBack(Action onComplete = null, float startDelay = 0f)
        {
            StartCoroutine(FlipRoutine(0, 90, () =>
            {
                frontFace.SetActive(false); 
                backFace.SetActive(true); 
                StartCoroutine(FlipRoutine(90, 0, onComplete));
            }, startDelay));
        }
        
        public void Matched(Action onComplete = null, float startDelay = 0f)
        {
            cardContent.gameObject.SetActive(false);
        }

        public void KillAnimation()
        {
            KillCurrentInterpolation();
            ResetCard();
        }

        private void ResetCard()
        {
            frontFace.SetActive(false);
            backFace.SetActive(true);
            cardContent.localRotation = Quaternion.Euler(0, 0, 0);
        }
        
        private void KillCurrentInterpolation()
        {
            if (_currentRoutine != null)
            {
                StopCoroutine(_currentRoutine);
                _currentRoutine = null;
            }
        }

        private IEnumerator FlipRoutine(float from, float to, Action onComplete, float startDelay = 0f)
        {
            if (startDelay > 0) yield return new WaitForSeconds(startDelay); 
            var elapsed = 0F;
            while (elapsed < flipDuration)
            {
                elapsed += Time.deltaTime; var t = Mathf.Clamp01(elapsed / flipDuration); 
                var eased = MiniTween.BackEaseOut(t * flipDuration, 0, 1, flipDuration); 
                var rotation = Mathf.Lerp(from, to, eased); 
                cardContent.localRotation = Quaternion.Euler(0, rotation, 0); 
                yield return null;
            } 
            
            onComplete?.Invoke();
        } 

    }
}
